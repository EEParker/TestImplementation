﻿using System;

namespace Sttp
{
    /// <summary>
    /// The default timestamp field for STTP.
    /// </summary>
    public struct SttpTimeOffset
    {
        private const long TicksMask = LeapsecondFlag - 1;
        private const long LeapsecondFlag = 1L << 62;

        public readonly long RawValue;           // Bits 0-62 Same as DateTime.Ticks  Bit 63 LeapSecondPending. Bit64 Sign for time BC.

        public SttpTimeOffset(DateTime time, bool leapSecondInProgress = false)
        {
            RawValue = time.Ticks;
            if (leapSecondInProgress)
                RawValue |= LeapsecondFlag;
        }

        public SttpTimeOffset(long rawValue)
        {
            RawValue = rawValue;
        }

        public SttpTimeOffset(DateTimeOffset rawValue)
        {
            throw new NotImplementedException();
        }

        public DateTime Ticks => new DateTime(RawValue & TicksMask);

        public bool LeapsecondInProgress => (RawValue & LeapsecondFlag) > 0;

        public static SttpTimeOffset Parse(string isString)
        {
            throw new NotImplementedException();
        }

        public SttpTime ToSttpTimestamp()
        {
            throw new NotImplementedException();
        }
    } // 8-bytes
}
