﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sttp.IO.Checksums;

namespace Sttp.Codec
{
    /// <summary>
    /// This class assists in the reassembly of fragmented data.
    /// </summary>
    public class FragmentReassembly
    {
        /// <summary>
        /// A buffer for building fragmented packets.
        /// </summary>
        public byte[] Buffer;
        /// <summary>
        /// The compressed size of a fragmented packet.
        /// </summary>
        public int TotalSize;

        /// <summary>
        /// The number of bytes of the fragment that have been received thus far.
        /// </summary>
        public int ReceivedSize;

        private uint m_checksum;

        private int m_totalFragments;
        private int m_prevFragment;

        /// <summary>
        /// The header that was included in this packet.
        /// </summary>
        public DataPacketHeader Header;

        public FragmentReassembly()
        {
            Buffer = new byte[0];
        }

        public bool IsComplete => TotalSize == ReceivedSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public void BeginFragment(byte[] buffer, int offset)
        {
            Header = (DataPacketHeader)buffer[offset];
            int packetLength = ToInt16(buffer, offset + 1);

            m_prevFragment = (int)(ushort)ToInt16(buffer, offset + 3);
            m_totalFragments = (int)(ushort)ToInt16(buffer, offset + 5);

            TotalSize = ToInt32(buffer, offset + 7);
            m_checksum = (uint)ToInt32(buffer, offset + 11);

           

            if (m_prevFragment != 1)
                throw new Exception("Wrong Current Fragment Number");

            if (packetLength - 15 > TotalSize)
                throw new Exception("Fragment overflow. Too many bytes were received for a fragment.");

            if (Buffer.Length < TotalSize)
            {
                Buffer = new byte[TotalSize];
            }
            ReceivedSize = packetLength - 15;
            Array.Copy(buffer, offset + 15, Buffer, 0, packetLength - 15);

            if (IsComplete)
            {
                ValidateChecksum();
            }
        }

        private void ValidateChecksum()
        {
            if (m_checksum != Crc32.Compute(Buffer, 0, TotalSize))
            {
                throw new InvalidOperationException("Fragment Checksum is not valid.");
            }
        }

        public void NextPacket(byte[] buffer, int offset)
        {
            if (IsComplete)
                throw new Exception("Fragment has already been assembled");
            int packetLength = ToInt16(buffer, offset + 1);

            int curFragment = (int)(ushort)ToInt16(buffer, offset + 3);
            int totalFragments = (int)(ushort)ToInt16(buffer, offset + 5);

            if (totalFragments != m_totalFragments)
                throw new Exception("Total fragment mismatched");

            if (m_prevFragment + 1 != curFragment)
                throw new Exception("Wrong Current Fragment Number");

            m_prevFragment++;

            if (ReceivedSize + packetLength - 7 > TotalSize)
                throw new Exception("Fragment overflow. Too many bytes were received by the packet.");

            Array.Copy(buffer, offset + 7, Buffer, ReceivedSize, packetLength - 7);
            ReceivedSize += packetLength - 7;

            if (IsComplete)
            {
                ValidateChecksum();
            }
        }

        private static short ToInt16(byte[] buffer, int startIndex)
        {
            return (short)((int)buffer[startIndex] << 8 | (int)buffer[startIndex + 1]);
        }

        private static int ToInt32(byte[] buffer, int startIndex)
        {
            return (int)buffer[startIndex + 0] << 24 |
                   (int)buffer[startIndex + 1] << 16 |
                   (int)buffer[startIndex + 2] << 8 |
                   (int)buffer[startIndex + 3];
        }

    }
}
