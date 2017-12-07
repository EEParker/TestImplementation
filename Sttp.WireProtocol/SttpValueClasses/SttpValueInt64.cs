using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sttp.Codec;

namespace Sttp.SttpValueClasses
{
    public class SttpValueInt64 : SttpValue
    {
        public readonly long Value;

        public SttpValueInt64(long value)
        {
            Value = value;
        }

        public override SttpValueTypeCode ValueTypeCode => SttpValueInt64Methods.ValueTypeCode;
        public override string ToTypeString => SttpValueInt64Methods.ToTypeString(Value);
        public override object ToNativeType => SttpValueInt64Methods.ToNativeType(Value);
        public override sbyte AsSByte => SttpValueInt64Methods.AsSByte(Value);
        public override short AsInt16 => SttpValueInt64Methods.AsInt16(Value);
        public override int AsInt32 => SttpValueInt64Methods.AsInt32(Value);
        public override long AsInt64 => SttpValueInt64Methods.AsInt64(Value);
        public override byte AsByte => SttpValueInt64Methods.AsByte(Value);
        public override ushort AsUInt16 => SttpValueInt64Methods.AsUInt16(Value);
        public override uint AsUInt32 => SttpValueInt64Methods.AsUInt32(Value);
        public override ulong AsUInt64 => SttpValueInt64Methods.AsUInt64(Value);
        public override float AsSingle => SttpValueInt64Methods.AsSingle(Value);
        public override double AsDouble => SttpValueInt64Methods.AsDouble(Value);
        public override decimal AsDecimal => SttpValueInt64Methods.AsDecimal(Value);
        public override DateTime AsDateTime => SttpValueInt64Methods.AsDateTime(Value);
        public override DateTimeOffset AsDateTimeOffset => SttpValueInt64Methods.AsDateTimeOffset(Value);
        public override SttpTime AsSttpTime => SttpValueInt64Methods.AsSttpTime(Value);
        public override SttpTimeOffset AsSttpTimeOffset => SttpValueInt64Methods.AsSttpTimeOffset(Value);
        public override TimeSpan AsTimeSpan => SttpValueInt64Methods.AsTimeSpan(Value);
        public override bool AsBoolean => SttpValueInt64Methods.AsBoolean(Value);
        public override char AsChar => SttpValueInt64Methods.AsChar(Value);
        public override Guid AsGuid => SttpValueInt64Methods.AsGuid(Value);
        public override string AsString => SttpValueInt64Methods.AsString(Value);
        public override SttpBuffer AsSttpBuffer => SttpValueInt64Methods.AsSttpBuffer(Value);
        public override SttpValueSet AsSttpValueSet => SttpValueInt64Methods.AsSttpValueSet(Value);
        public override SttpNamedSet AsSttpNamedSet => SttpValueInt64Methods.AsSttpNamedSet(Value);
        public override SttpMarkup AsSttpMarkup => SttpValueInt64Methods.AsSttpMarkup(Value);
        public override Guid AsBulkTransportGuid => SttpValueInt64Methods.AsBulkTransportGuid(Value);
    }

    internal static class SttpValueInt64Methods
    {
        public static SttpValueTypeCode ValueTypeCode => SttpValueTypeCode.Int64;
       
        public static string ToTypeString(long value)
        {
            return $"(long){value.ToString()}";
        }

        public static object ToNativeType(long value)
        {
            return value;
        }

        #region [ Type Casting ]

        public static sbyte AsSByte(long value)
        {
            checked
            {
                return (sbyte)value;
            }
        }
        public static short AsInt16(long value)
        {
            checked
            {
                return (short)value;
            }
        }

        public static int AsInt32(long value)
        {
            checked
            {
                return (int)value;
            }
        }

        public static long AsInt64(long value)
        {
            checked
            {
                return (long)value;
            }
        }

        public static byte AsByte(long value)
        {
            checked
            {
                return (byte)value;
            }
        }

        public static ushort AsUInt16(long value)
        {
            checked
            {
                return (ushort)value;
            }
        }

        public static uint AsUInt32(long value)
        {
            checked
            {
                return (uint)value;
            }
        }

        public static ulong AsUInt64(long value)
        {
            checked
            {
                return (ulong)value;
            }
        }

        public static float AsSingle(long value)
        {
            checked
            {
                return (float)value;
            }
        }

        public static double AsDouble(long value)
        {
            checked
            {
                return (double)value;
            }
        }

        public static decimal AsDecimal(long value)
        {
            checked
            {
                return (decimal)value;
            }
        }

        public static DateTime AsDateTime(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to DateTime");
        }

        public static DateTimeOffset AsDateTimeOffset(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to DateTimeOffset");
        }

        public static SttpTime AsSttpTime(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to SttpTime");
        }

        public static SttpTimeOffset AsSttpTimeOffset(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to SttpTimeOffset");
        }

        public static TimeSpan AsTimeSpan(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to TimeSpan");
        }

        public static bool AsBoolean(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to Boolean");
        }

        public static char AsChar(long value)
        {
            checked
            {
                return (char)value;
            }
        }

        public static Guid AsGuid(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to Guid");
        }

        public static string AsString(long value)
        {
            return value.ToString();
        }

        public static SttpBuffer AsSttpBuffer(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to SttpBuffer");
        }

        public static SttpValueSet AsSttpValueSet(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to SttpValueSet");
        }

        public static SttpNamedSet AsSttpNamedSet(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to SttpNamedSet");
        }

        public static SttpMarkup AsSttpMarkup(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to SttpMarkup");
        }

        public static Guid AsBulkTransportGuid(long value)
        {
            throw new InvalidCastException($"Cannot cast from {ToTypeString(value)} to BulkTransportGuid");
        }

        #endregion
    }
}
