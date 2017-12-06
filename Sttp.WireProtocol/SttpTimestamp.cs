﻿using System;

namespace Sttp
{
    /// <summary>
    /// The default timestamp field for STTP.
    /// </summary>
    public struct SttpTime
    {
        private const long TicksMask = LeapsecondFlag - 1;
        private const long LeapsecondFlag = 1L << 62;

        public readonly long RawValue;           // Bits 0-62 Same as DateTime.Ticks Bit 63 LeapSecondPending. Bit64 is reserved for RepeatedHour if time is specified in local time.
        //ToDo: Consider using all 4 values to maintain support with DateTime. 

        public SttpTime(DateTime time, bool leapSecondInProgress = false)
        {
            RawValue = time.Ticks;
            if (leapSecondInProgress)
                RawValue |= LeapsecondFlag;
        }

        public SttpTime(long rawValue)
        {
            RawValue = rawValue;
        }

        public DateTime Ticks => new DateTime(RawValue & TicksMask);

        public bool LeapsecondInProgress => (RawValue & LeapsecondFlag) > 0;

        public SttpTimeOffset ToSttpTimestampOffset()
        {
            throw new NotImplementedException();
        }

        public static SttpTime Parse(string isString)
        {
            throw new NotImplementedException();
        }
    } // 8-bytes
}
