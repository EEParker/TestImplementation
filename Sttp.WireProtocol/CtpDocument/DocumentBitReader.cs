﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTP
{
    internal unsafe class DocumentBitReader
    {
        private static readonly byte[] Empty = new byte[0];

        private byte[] m_buffer;
        private int m_startingPosition;
        private int m_lastPosition;

        private int m_currentBytePosition;
        private int m_currentBitPosition;

        private int m_bitStreamCacheBitCount;
        private uint m_bitStreamCache;
        private byte m_usedBitsForLastBitWord;

        public DocumentBitReader(byte[] data, int position, int length)
        {
            SetBuffer(data, position, length);
        }

        public void SetBuffer(byte[] data, int position, int length)
        {
            m_buffer = data;
            m_startingPosition = position;
            m_lastPosition = length + position;

            m_currentBytePosition = position;
            m_currentBitPosition = m_lastPosition;

            m_bitStreamCacheBitCount = 0;
            m_bitStreamCache = 0;
            m_usedBitsForLastBitWord = 0;
        }

        private void ThrowEndOfStreamException()
        {
            throw new EndOfStreamException();
        }

        private void EnsureCapacity(int length)
        {
            if (m_currentBytePosition + length > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
        }

        public float ReadSingle()
        {
            var value = ReadBits32();
            return *(float*)&value;
        }

        public long ReadInt64()
        {
            return (long)ReadBits64();
        }

        public double ReadDouble()
        {
            var value = ReadBits64();
            return *(double*)&value;
        }

        public Guid ReadGuid()
        {
            if (m_currentBytePosition + 16 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            Guid rv = m_buffer.ToRfcGuid(m_currentBytePosition);
            m_currentBytePosition += 16;
            return rv;
        }

        public byte[] ReadBytes()
        {
            int length = (int)Read4BitSegments();
            if (length == 0)
            {
                return Empty;
            }

            EnsureCapacity(length);

            byte[] rv = new byte[length];
            Array.Copy(m_buffer, m_currentBytePosition, rv, 0, length);
            m_currentBytePosition += length;
            return rv;
        }

        public string ReadString()
        {
            byte[] rv = ReadBytes();
            if (rv.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(rv);
        }

        public string ReadAscii()
        {
            if (m_currentBytePosition + 1 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            if (m_currentBytePosition + 1 + m_buffer[m_currentBytePosition] > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            char[] data = new char[m_buffer[m_currentBytePosition]];
            for (int x = 0; x < data.Length; x++)
            {
                data[x] = (char)m_buffer[m_currentBytePosition + 1 + x];
                if (data[x] > 127)
                    throw new Exception("Not an ASCII string");
            }
            m_currentBytePosition += 1 + data.Length;
            return new string(data);
        }

        public CtpDocument ReadCtpDocument()
        {
            return new CtpDocument(ReadBytes());
        }

        public CtpBuffer ReadCtpBufffer()
        {
            return new CtpBuffer(ReadBytes());
        }

        public CtpTime ReadSttpTime()
        {
            return new CtpTime(ReadInt64());
        }

        #region [ Read Bits ]

        public uint ReadBits1()
        {
            const int bits = 1;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }
        public uint ReadBits2()
        {
            const int bits = 2;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }

        public uint ReadBits3()
        {
            const int bits = 3;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }
        public uint ReadBits4()
        {
            const int bits = 4;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }
        public uint ReadBits5()
        {
            const int bits = 5;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }
        public uint ReadBits6()
        {
            const int bits = 6;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }
        public uint ReadBits7()
        {
            const int bits = 7;
            if (m_bitStreamCacheBitCount < bits)
                ReadMoreBits(bits);
            m_bitStreamCacheBitCount -= bits;
            return (uint)((m_bitStreamCache >> m_bitStreamCacheBitCount) & ((1 << bits) - 1));
        }
        public uint ReadBits8()
        {
            if (m_currentBytePosition + 1 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            byte rv = m_buffer[m_currentBytePosition];
            m_currentBytePosition++;
            return rv;
        }

        public uint ReadBits16()
        {
            if (m_currentBytePosition + 2 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            uint rv = (uint)m_buffer[m_currentBytePosition] << 8
                    | (uint)m_buffer[m_currentBytePosition + 1];
            m_currentBytePosition += 2;
            return rv;
        }

        public uint ReadBits24()
        {
            if (m_currentBytePosition + 3 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            uint rv = (uint)m_buffer[m_currentBytePosition] << 16
                      | (uint)m_buffer[m_currentBytePosition + 1] << 8
                      | (uint)m_buffer[m_currentBytePosition + 2];
            m_currentBytePosition += 3;
            return rv;
        }

        public uint ReadBits32()
        {
            if (m_currentBytePosition + 4 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            uint rv = (uint)m_buffer[m_currentBytePosition] << 24
                      | (uint)m_buffer[m_currentBytePosition + 1] << 16
                      | (uint)m_buffer[m_currentBytePosition + 2] << 8
                      | (uint)m_buffer[m_currentBytePosition + 3];
            m_currentBytePosition += 4;
            return rv;
        }

        public ulong ReadBits40()
        {
            if (m_currentBytePosition + 5 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            ulong rv = (ulong)m_buffer[m_currentBytePosition + 0] << 32 |
                       (ulong)m_buffer[m_currentBytePosition + 1] << 24 |
                       (ulong)m_buffer[m_currentBytePosition + 2] << 16 |
                       (ulong)m_buffer[m_currentBytePosition + 3] << 8 |
                       (ulong)m_buffer[m_currentBytePosition + 4];
            m_currentBytePosition += 5;
            return rv;
        }

        public ulong ReadBits48()
        {
            if (m_currentBytePosition + 6 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            ulong rv = (ulong)m_buffer[m_currentBytePosition + 0] << 40 |
                       (ulong)m_buffer[m_currentBytePosition + 1] << 32 |
                       (ulong)m_buffer[m_currentBytePosition + 2] << 24 |
                       (ulong)m_buffer[m_currentBytePosition + 3] << 16 |
                       (ulong)m_buffer[m_currentBytePosition + 4] << 8 |
                       (ulong)m_buffer[m_currentBytePosition + 5];
            m_currentBytePosition += 6;
            return rv;
        }

        public ulong ReadBits56()
        {
            if (m_currentBytePosition + 7 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            ulong rv = (ulong)m_buffer[m_currentBytePosition + 0] << 48 |
                       (ulong)m_buffer[m_currentBytePosition + 1] << 40 |
                       (ulong)m_buffer[m_currentBytePosition + 2] << 32 |
                       (ulong)m_buffer[m_currentBytePosition + 3] << 24 |
                       (ulong)m_buffer[m_currentBytePosition + 4] << 16 |
                       (ulong)m_buffer[m_currentBytePosition + 5] << 8 |
                       (ulong)m_buffer[m_currentBytePosition + 6];
            m_currentBytePosition += 7;
            return rv;
        }

        public ulong ReadBits64()
        {
            if (m_currentBytePosition + 8 > m_currentBitPosition)
            {
                ThrowEndOfStreamException();
            }
            ulong rv = (ulong)m_buffer[m_currentBytePosition + 0] << 56 |
                      (ulong)m_buffer[m_currentBytePosition + 1] << 48 |
                      (ulong)m_buffer[m_currentBytePosition + 2] << 40 |
                      (ulong)m_buffer[m_currentBytePosition + 3] << 32 |
                      (ulong)m_buffer[m_currentBytePosition + 4] << 24 |
                      (ulong)m_buffer[m_currentBytePosition + 5] << 16 |
                      (ulong)m_buffer[m_currentBytePosition + 6] << 8 |
                      (ulong)m_buffer[m_currentBytePosition + 7];
            m_currentBytePosition += 8;
            return rv;
        }

        private void ReadMoreBits(int len)
        {
            if (m_currentBitPosition - m_currentBytePosition < 1)
            {
                ThrowEndOfStreamException();
            }
            if (m_currentBitPosition == m_lastPosition)
            {
                m_currentBitPosition--;
                m_bitStreamCacheBitCount = 5;
                m_bitStreamCache = m_buffer[m_currentBitPosition];
                m_usedBitsForLastBitWord = (byte)(m_bitStreamCache >> 5);
                if (len > 5)
                {
                    ReadMoreBits(len - 5);
                }
            }
            else
            {
                m_currentBitPosition--;
                m_bitStreamCacheBitCount += 8;
                m_bitStreamCache = (m_bitStreamCache << 8) | m_buffer[m_currentBitPosition];
            }

        }

        public uint Read4BitSegments()
        {
            int bits = 0;
            uint value = 0;
            while (ReadBits1() == 1)
            {
                if (bits == 32)
                    throw new InvalidOperationException("Improperly encoded 4-bit segment");
                value |= (uint)ReadBits8() << bits;
                bits += 8;
            }
            return value;
        }

        public ulong Read8BitSegments()
        {
            int bits = 0;
            ulong value = 0;
            while (ReadBits1() == 1)
            {
                if (bits == 64)
                    throw new InvalidOperationException("Improperly encoded 8-bit segment");
                value |= (ulong)ReadBits8() << bits;
                bits += 8;
            }

            return value;
        }

        #endregion
    }
}
