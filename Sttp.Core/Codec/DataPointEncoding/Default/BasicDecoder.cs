﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTP;

namespace Sttp.Codec.DataPoint
{
    public delegate SttpDataPointMetadata LookupMetadata(CtpObject dataPointID);

    public class BasicDecoder
    {
        private LookupMetadata m_lookup;
        private MetadataChannelMapDecoder m_channelMap;
        private ByteReader m_stream;
        private int m_lastChannelID = 0;
        private CtpObject m_lastTimestamp = new CtpObject();
        private long m_lastQuality = 0;
        private CtpTypeCode m_lastValueCode;

        public BasicDecoder(LookupMetadata lookup)
        {
            m_lookup = lookup;
            m_stream = new ByteReader();
        }

        public void Load(byte[] data, bool clearMapping)
        {
            m_lastChannelID = 0;
            m_lastTimestamp.SetNull();
            m_lastQuality = 0;
            m_lastValueCode = CtpTypeCode.Null;
            m_stream.SetBuffer(data, 0, data.Length);
            if (clearMapping)
            {
                m_channelMap.Clear();
            }
        }

        public bool Read(SttpDataPoint dataPoint)
        {
            if (m_stream.IsEmpty)
            {
                //It's possible that this is not enough since items might eventually be stored with a few bits, so I need some kind of extra escape sequence.
                return false;
            }

            bool hasExtendedData = false;
            bool qualityChanged = false;
            bool timeChanged = false;
            bool typeChanged = false;

            if (m_stream.ReadBits1() == 0)
            {
                hasExtendedData = m_stream.ReadBits1() == 1;
                qualityChanged = m_stream.ReadBits1() == 1;
                timeChanged = m_stream.ReadBits1() == 1;
                typeChanged = m_stream.ReadBits1() == 1;
            }


            m_lastChannelID ^= (int)(uint)m_stream.Read4BitSegments();
            dataPoint.Metadata = m_channelMap.GetMetadata(m_lastChannelID);

            if (hasExtendedData)
            {
                CtpValueEncodingNative.Load(m_stream, dataPoint.ExtendedData);
            }
            else
            {
                dataPoint.ExtendedData.SetNull();
            }

            if (qualityChanged)
            {
                m_lastQuality = m_stream.ReadInt64();
            }
            dataPoint.Quality = m_lastQuality;

            if (timeChanged)
            {
                CtpValueEncodingNative.Load(m_stream, m_lastTimestamp);
            }

            dataPoint.Time.SetValue(m_lastTimestamp);

            if (typeChanged)
            {
                m_lastValueCode = (CtpTypeCode)m_stream.ReadBits4();
            }

            CtpValueEncodingWithoutType.Load(m_stream, m_lastValueCode, dataPoint.Value);

            if (dataPoint.Metadata == null)
            {
                var obj = new CtpObject();
                CtpValueEncodingNative.Load(m_stream, obj);
                dataPoint.Metadata = m_lookup(obj);
                m_channelMap.Assign(dataPoint.Metadata, m_lastChannelID);
            }
            return true;
        }



    }
}
