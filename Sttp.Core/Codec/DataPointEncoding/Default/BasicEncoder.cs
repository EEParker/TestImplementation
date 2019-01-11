﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CTP;
using Sttp.Codec;

namespace Sttp.Codec.DataPoint
{
    public class BasicEncoder
    {
        private ByteWriter m_stream;
        private int m_lastChannelID = 0;
        private readonly CtpObject m_lastTimestamp = new CtpObject();
        private long m_lastQuality = 0;
        private CtpTypeCode m_lastValueCode;
        private MetadataChannelMapEncoder m_channelMap;

        public BasicEncoder()
        {
            m_stream = new ByteWriter();
            m_channelMap = new MetadataChannelMapEncoder();
        }

        public int Length => m_stream.Length;

        public void Clear(bool clearMapping)
        {
            m_lastChannelID = 0;
            m_lastTimestamp.SetNull();
            m_lastQuality = 0;
            m_lastValueCode = CtpTypeCode.Null;
            m_stream.Clear();
            if (clearMapping)
            {
                m_channelMap.Clear();
            }
        }

        public void AddDataPoint(SttpDataPoint point)
        {
            bool hasExtendedData = !point.ExtendedData.IsNull;
            bool qualityChanged = point.Quality != m_lastQuality;
            bool timeChanged = point.Time != m_lastTimestamp;
            bool typeChanged = point.Value.ValueTypeCode != m_lastValueCode;

            if (!hasExtendedData && !qualityChanged && !timeChanged && !typeChanged)
            {
                m_stream.WriteBits1(true); //Is the common header.
            }
            else
            {
                m_stream.WriteBits1(false); //Is not the common header.
                m_stream.WriteBits1(hasExtendedData);
                m_stream.WriteBits1(qualityChanged);
                m_stream.WriteBits1(timeChanged);
                m_stream.WriteBits1(typeChanged);
            }

            int channelID = m_channelMap.GetChannelID(point.Metadata, out bool isNew);
            int pointIDDelta = channelID ^ m_lastChannelID;
            m_stream.Write4BitSegments((uint)pointIDDelta);
            m_lastChannelID = channelID;

            if (hasExtendedData)
            {
                CtpValueEncodingNative.Save(m_stream, point.ExtendedData);
            }

            if (qualityChanged)
            {
                m_stream.Write(point.Quality);
            }

            if (timeChanged)
            {
                CtpValueEncodingNative.Save(m_stream, point.Time);
                m_lastTimestamp.SetValue(point.Time);
            }

            if (typeChanged)
            {
                m_stream.WriteBits4((byte)point.Value.ValueTypeCode);
                m_lastValueCode = point.Value.ValueTypeCode;
            }

            CtpValueEncodingWithoutType.Save(m_stream, point.Value);

            if (isNew)
            {
                CtpValueEncodingNative.Save(m_stream, point.Metadata.DataPointID);
            }
        }

        public byte[] ToArray()
        {
            return m_stream.ToArray();
        }
    }
}
