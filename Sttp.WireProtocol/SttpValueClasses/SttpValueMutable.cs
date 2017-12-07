﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sttp.Codec;
using Sttp.SttpValueClasses;

namespace Sttp
{
    /// <summary>
    /// This class contains the fundamental value for STTP.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial class SttpValueMutable : SttpValue
    {
        #region [ Members ]

        [FieldOffset(0)]
        private long m_rawBytes0_7;
        [FieldOffset(8)]
        private long m_rawBytes8_15;

        [FieldOffset(0)]
        private sbyte m_valueSByte;
        [FieldOffset(0)]
        private byte m_valueByte;
        [FieldOffset(0)]
        private short m_valueInt16;
        [FieldOffset(0)]
        private int m_valueInt32;
        [FieldOffset(0)]
        private long m_valueInt64;
        [FieldOffset(0)]
        private ushort m_valueUInt16;
        [FieldOffset(0)]
        private uint m_valueUInt32;
        [FieldOffset(0)]
        private ulong m_valueUInt64;
        [FieldOffset(0)]
        private double m_valueDouble;

        [FieldOffset(0)]
        private float m_valueSingle;
        [FieldOffset(0)]
        private TimeSpan m_valueTimeSpan;
        [FieldOffset(0)]
        private char m_valueChar;
        [FieldOffset(0)]
        private bool m_valueBoolean;
        [FieldOffset(0)]
        private SttpTime m_valueSttpTime;
        [FieldOffset(0)]
        private SttpTimeOffset m_valueSttpTimeOffset;
        [FieldOffset(0)]
        private DateTime m_valueDateTime;
        [FieldOffset(0)]
        private DateTimeOffset m_valueDateTimeOffset;
        [FieldOffset(0)]
        private decimal m_valueDecimal;
        [FieldOffset(0)]
        private Guid m_valueGuid;

        [FieldOffset(16)]
        private object m_valueObject;

        [FieldOffset(33)]
        private SttpValueTypeCode m_valueTypeCode;

        #endregion

        #region [ Constructors ]

        public SttpValueMutable()
        {
            SetNull();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The type code of the raw value.
        /// </summary>
        public override SttpValueTypeCode ValueTypeCode
        {
            get
            {
                return m_valueTypeCode;
            }
        }

        #endregion

        #region [ Methods ] 

        public SttpValue CloneAsImmutable()
        {
            switch (ValueTypeCode)
            {
                case SttpValueTypeCode.Null:
                    return SttpValue.Null;
                case SttpValueTypeCode.SByte:
                    return (SttpValue)AsSByte;
                case SttpValueTypeCode.Int16:
                    return (SttpValue)AsInt16;
                case SttpValueTypeCode.Int32:
                    return (SttpValue)AsInt32;
                case SttpValueTypeCode.Int64:
                    return (SttpValue)AsInt64;
                case SttpValueTypeCode.Byte:
                    return (SttpValue)AsByte;
                case SttpValueTypeCode.UInt16:
                    return (SttpValue)AsUInt16;
                case SttpValueTypeCode.UInt32:
                    return (SttpValue)AsUInt32;
                case SttpValueTypeCode.UInt64:
                    return (SttpValue)AsUInt64;
                case SttpValueTypeCode.Single:
                    return (SttpValue)AsSingle;
                case SttpValueTypeCode.Double:
                    return (SttpValue)AsDouble;
                case SttpValueTypeCode.Decimal:
                    return (SttpValue)AsDecimal;
                case SttpValueTypeCode.DateTime:
                    return (SttpValue)AsDateTime;
                case SttpValueTypeCode.DateTimeOffset:
                    return (SttpValue)AsDateTimeOffset;
                case SttpValueTypeCode.SttpTime:
                    return (SttpValue)AsSttpTime;
                case SttpValueTypeCode.SttpTimeOffset:
                    return (SttpValue)AsSttpTimeOffset;
                case SttpValueTypeCode.TimeSpan:
                    return (SttpValue)AsTimeSpan;
                case SttpValueTypeCode.Boolean:
                    return (SttpValue)AsBoolean;
                case SttpValueTypeCode.Char:
                    return (SttpValue)AsChar;
                case SttpValueTypeCode.Guid:
                    return (SttpValue)AsGuid;
                case SttpValueTypeCode.String:
                    return (SttpValue)AsString;
                case SttpValueTypeCode.SttpBuffer:
                    return (SttpValue)AsSttpBuffer;
                case SttpValueTypeCode.SttpValueSet:
                    return (SttpValue)AsSttpValueSet;
                case SttpValueTypeCode.SttpNamedSet:
                    return (SttpValue)AsSttpNamedSet;
                case SttpValueTypeCode.SttpMarkup:
                    return (SttpValue)AsSttpMarkup;
                case SttpValueTypeCode.BulkTransportGuid:
                    return SttpValue.CreateBulkTransportGuid(AsBulkTransportGuid);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool operator ==(SttpValueMutable a, SttpValueMutable b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;
            if (a.m_valueTypeCode != b.m_valueTypeCode)
                return false;
            switch (a.m_valueTypeCode)
            {
                //ToDo: Finish.
            }
            return true;
        }

        public static bool operator !=(SttpValueMutable a, SttpValueMutable b)
        {
            return !(a == b);
        }

        #endregion

        public void Load(PayloadReader payloadReader)
        {
            throw new NotImplementedException();
        }

        public void Save(PayloadWriter payloadWriter, bool includeTypeCode)
        {
            throw new NotImplementedException();
        }

        public static explicit operator SttpValueMutable(double v)
        {
            var rv = new SttpValueMutable();
            rv.SetValue(v);
            return rv;
        }
        public static explicit operator SttpValueMutable(string v)
        {
            var rv = new SttpValueMutable();
            rv.SetValue(v);
            return rv;
        }

    }
}
