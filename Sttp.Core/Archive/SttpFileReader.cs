﻿using System;
using System.Collections.Generic;
using System.IO;
using CTP;
using CTP.IO;
using Sttp.Codec;
using Sttp.DataPointEncoding;

namespace Sttp
{
    public enum FileReaderItem
    {
        ProducerMetadata,
        DataPoint,
        EndOfStream
    }

    public class SttpFileReader : IDisposable
    {
        private CtpFileStream m_stream;
        private DecoderBase m_decoder;
        private CtpCommand m_nextCommand;

        private CommandBeginCompressionStream m_compressionStream;
        private CommandBeginDataStream m_dataStream;
        private SttpProducerMetadata m_metadata;
        private DefalteHelper m_comp;

        private Dictionary<CtpObject, SttpDataPointMetadata> m_metadataLookup;

        public SttpFileReader(Stream stream, bool ownsStream)
        {
            m_stream = new CtpFileStream(stream, ownsStream);
            m_metadataLookup = new Dictionary<CtpObject, SttpDataPointMetadata>();
        }

        private SttpDataPointMetadata Lookup(CtpObject dataPointID)
        {
            if (m_metadataLookup.TryGetValue(dataPointID, out var value))
            {
                return value;
            }
            var dp = new SttpDataPointMetadata(null);
            dp.DataPointID = dataPointID;
            return dp;
        }

        public FileReaderItem Next()
        {
            TryAgain:
            m_nextCommand = m_stream.Read();
            if ((object)m_nextCommand == null)
                return FileReaderItem.EndOfStream;

            TryAfterDecompress:

            if (m_nextCommand.IsRaw)
            {
                var raw = (CtpRaw)m_nextCommand;
                if (m_dataStream != null && raw.Channel == m_dataStream.ChannelCode)
                {
                    m_decoder.Load(raw.Payload);
                    return FileReaderItem.DataPoint;
                }

                if (m_compressionStream != null && raw.Channel == m_compressionStream.ChannelCode)
                {
                    m_nextCommand = m_comp.Inflate(raw);
                    goto TryAfterDecompress;

                }
                throw new Exception("Raw stream is not defined for the specified channel.");
            }
            else if (m_nextCommand.RootElement == "BeginDataStream")
            {
                m_dataStream = (CommandBeginDataStream)m_nextCommand;
                if (m_dataStream.EncodingMechanism == "Basic")
                {
                    m_decoder = new BasicDecoder(Lookup);
                }
                if (m_dataStream.EncodingMechanism == "Advanced")
                {
                    m_decoder = new AdvancedDecoder(Lookup);
                }
                if (m_dataStream.EncodingMechanism == "Raw")
                {
                    m_decoder = new RawDecoder(Lookup);
                }
            }
            else if (m_nextCommand.RootElement == "BeginCompressionStream")
            {
                m_compressionStream = (CommandBeginCompressionStream)m_nextCommand;
                m_comp = new DefalteHelper(m_compressionStream);

            }
            else if (m_nextCommand.RootElement == "ProducerMetadata")
            {
                m_metadata = (SttpProducerMetadata)m_nextCommand;
                foreach (var item in m_metadata.DataPoints)
                {
                    m_metadataLookup[item.DataPointID] = item;
                }
                return FileReaderItem.ProducerMetadata;
            }
            goto TryAgain;
        }

        public bool ReadDataPoint(SttpDataPoint dataPoint)
        {
            return m_decoder.Read(dataPoint);
        }

        public SttpProducerMetadata GetMetadata()
        {
            return m_metadata;
        }

        public void Dispose()
        {
            m_stream?.Dispose();
        }
    }
}
