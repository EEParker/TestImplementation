﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using Sttp.Codec;

namespace Sttp
{
    /// <summary>
    /// This class contains the fundamental unit of transmission for STTP.
    /// The intention of this class is to be reusable. 
    /// </summary>
    public class SttpDataPoint
    {
        /// <summary>
        /// A token was defined by the API layer when the runtime ID was defined at the protocol level.
        /// This is only present when <see cref="DataPointID"/> is a RuntimeID type. This field is only 
        /// used on the receiving end. The source of the data point does not need to assign this field.
        /// 
        /// Suggestions for this token include the properly mapped point identifier and possibly routing information.
        /// </summary>
        public object DataPointAPIToken;

        /// <summary>
        /// A runtime ID for the data point. A negative value designates that this runtime ID is not valid. 
        /// 
        /// Ideally, all measurements will be mapped to a runtime ID, however, for systems that 
        /// contains millions or billions of measurements, this is not a practical expectation.
        /// </summary>
        public int DataPointRuntimeID = -1;

        /// <summary>
        /// The unique identifier for this PointID.
        /// </summary>
        public readonly SttpValueMutable DataPointID = new SttpValueMutable();

        /// <summary>
        /// A timestamp field. Virtually all instances will use SttpTime as the means of storing time, but it's not required.
        /// </summary>
        public readonly SttpValueMutable Time = new SttpValueMutable();

        /// <summary>
        /// The value for the data point.
        /// </summary>
        public readonly SttpValueMutable Value = new SttpValueMutable();

        /// <summary>
        /// 8-bits for identifying the quality of the time.
        /// </summary>
        public TimeQualityFlags TimestampQuality;

        /// <summary>
        /// 8-bits for identifying the quality of the value.
        /// </summary>
        public ValueQualityFlags ValueQuality;

        /// <summary>
        /// An extra field in the event that one is needed. It's not recommended to use this field for complex value types, but rather
        /// if data must be transported with this value that does not fit in one of the other categories (PointID, Time, Value, Quality)
        /// Examples include:
        ///   Some kind of sequence identifier.
        ///   Some kind of pivot field. 
        ///   Extra quality data or analysis.
        ///   Metadata that must be transported with every measurement. (Caution, this will introduce substatital overhead)
        /// 
        /// Note: Since this field is considered ancillary assigning something other than null will introduce a penalty that may be considerable in some circumstances.
        /// </summary>
        public readonly SttpValueMutable ExtendedData = new SttpValueMutable();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DataPointRuntimeID.ToString());
            sb.Append('\t');
            sb.Append(DataPointID.ToString());
            sb.Append('\t');
            sb.Append(Time.ToString());
            sb.Append('\t');
            sb.Append(Value.ToString());
            sb.Append('\t');
            sb.Append(TimestampQuality.ToString());
            sb.Append('\t');
            sb.Append(ValueQuality.ToString());
            return sb.ToString();
        }
    }
}
