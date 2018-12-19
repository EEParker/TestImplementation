﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using GSF;

namespace CTP
{
    /// <summary>
    /// A packet that can be serialized.
    /// </summary>
    public class CtpCommand : IEquatable<CtpCommand>
    {
        private readonly byte[] m_data;
        private bool m_isRaw;
        private string m_commandName;
        private int m_length;
        private int m_channelCode;
        private int m_headerLength;

        /// <summary>
        /// Creates an CtpDocument from a byte array.
        /// </summary>
        /// <param name="data"></param>
        public CtpCommand(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            m_data = (byte[])data.Clone();
            ValidateData();
        }

        internal CtpCommand(byte[] data, bool unsafeShouldClone)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (unsafeShouldClone)
            {
                m_data = (byte[])data.Clone();
            }
            else
            {
                m_data = data;
            }
            ValidateData();
        }

        private void ValidateData()
        {
            if (m_data.Length < 2)
                throw new Exception("Packet wrong size");
            if (m_data[0] >= 64)
                throw new Exception("Wrong Version");

            m_isRaw = (m_data[0] & 32) > 0;
            bool longPayload = (m_data[0] & 16) > 0;
            if (!longPayload)
            {
                m_headerLength = 2;
                m_length = BigEndian.ToInt16(m_data, 0) & ((1 << 12) - 1);
            }
            else
            {
                m_headerLength = 4;
                if (m_data.Length < 4)
                    throw new Exception("Packet wrong size");
                m_length = BigEndian.ToInt32(m_data, 0) & ((1 << 28) - 1);

            }
            if (m_data.Length < m_length)
                throw new Exception("Packet wrong size");

            if (m_isRaw)
            {
                m_channelCode = m_data[m_headerLength];
                m_headerLength++;
            }
            else
            {
                m_channelCode = 0;
            }
        }

        public string RootElement
        {
            get
            {
                if (m_commandName == null)
                {
                    if (m_isRaw)
                        m_commandName = "Raw";
                    else
                        m_commandName = Encoding.ASCII.GetString(m_data, m_headerLength + 1, m_data[m_headerLength]);
                }
                return m_commandName;
            }
        }

        public bool IsRaw => m_isRaw;

        public int Length => m_length;

        /// <summary>
        /// Create a means for reading the data from the CtpDocument.
        /// </summary>
        /// <returns></returns>
        internal CtpCommandReader MakeReader()
        {
            return new CtpCommandReader(m_data, m_headerLength);
        }

        public byte[] ToArray()
        {
            return (byte[])m_data.Clone();
        }

        /// <summary>
        /// Copies the internal buffer to the provided byte array.
        /// Be sure to call <see cref="Length"/> to ensure that the destination buffer
        /// has enough space to receive the copy.
        /// </summary>
        /// <param name="buffer">the buffer to copy to.</param>
        /// <param name="offset">the offset position of <see pref="buffer"/></param>
        public void CopyTo(byte[] buffer, int offset)
        {
            Array.Copy(m_data, 0, buffer, offset, m_data.Length); // write data
        }

        /// <summary>
        /// Creates an XML string representation of this CtpDocument file.
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            var reader = MakeReader();

            var sb = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            var xml = XmlWriter.Create(sb, settings);

            xml.WriteStartElement(reader.RootElement.Value);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case CtpCommandNodeType.StartElement:
                        xml.WriteStartElement(reader.ElementName.Value);
                        break;
                    case CtpCommandNodeType.Value:
                        xml.WriteStartElement(reader.ValueName.Value);
                        xml.WriteAttributeString("ValueType", reader.Value.ValueTypeCode.ToString());
                        xml.WriteValue(reader.Value.AsString ?? string.Empty);
                        xml.WriteEndElement();
                        break;
                    case CtpCommandNodeType.EndElement:
                        xml.WriteEndElement();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            xml.WriteEndElement();
            xml.Flush();
            return sb.ToString();
        }

        /// <summary>
        /// Creates a JSON string representation of this CtpDocument file.
        /// </summary>
        /// <returns></returns>
        public string ToJSON()
        {
            var reader = MakeReader();

            var sb = new StringBuilder();

            Stack<string> prefix = new Stack<string>();
            prefix.Push("  ");

            sb.Append('"');
            sb.Append(reader.RootElement);
            sb.AppendLine("\": {");

            //Note: There's an issue with a trailing commas.

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case CtpCommandNodeType.StartElement:
                        sb.Append(prefix.Peek());
                        sb.Append('"');
                        sb.Append(reader.ElementName);
                        sb.AppendLine("\": {");
                        prefix.Push(prefix.Peek() + "  ");
                        break;
                    case CtpCommandNodeType.Value:
                        sb.Append(prefix.Peek());
                        sb.Append('"');
                        sb.Append(reader.ValueName);
                        sb.Append("\": \"");
                        sb.Append(reader.Value.ToTypeString);
                        sb.AppendLine("\",");
                        break;
                    case CtpCommandNodeType.EndElement:
                        prefix.Pop();
                        sb.Append(prefix.Peek());
                        sb.AppendLine("},");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Creates a YAML string representation of this CtpDocument file.
        /// </summary>
        /// <returns></returns>
        public string ToYAML()
        {
            var reader = MakeReader();

            var sb = new StringBuilder();

            Stack<string> prefix = new Stack<string>();
            prefix.Push(" ");

            sb.Append("---");
            sb.Append(reader.RootElement);
            sb.AppendLine();

            //Note: There's an issue with a trailing commas.

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case CtpCommandNodeType.StartElement:
                        sb.Append(prefix.Peek());
                        sb.Append(reader.ElementName);
                        sb.AppendLine(":");
                        prefix.Push(prefix.Peek() + " ");
                        break;
                    case CtpCommandNodeType.Value:
                        sb.Append(prefix.Peek());
                        sb.Append(reader.ValueName);
                        sb.Append(": ");
                        if (reader.Value.ValueTypeCode == CtpTypeCode.CtpDocument)
                        {
                            sb.AppendLine("(CtpCommand)");
                            string str = Environment.NewLine + prefix.Peek() + " ";
                            sb.Append(prefix.Peek() + " " + reader.Value.AsString.Replace(Environment.NewLine, str));
                        }
                        else
                        {
                            sb.Append(reader.Value.ToTypeString);
                        }
                        sb.AppendLine();
                        break;
                    case CtpCommandNodeType.EndElement:
                        prefix.Pop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            sb.Length -= Environment.NewLine.Length;
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToYAML();
        }

        /// <summary>
        /// Checks if the byte representation of two separate CtpDocument files are the same. 
        /// Note: Due to reordering and encoding mechanics, it's possible for two records to be 
        /// externally the same, while internally they are not.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CtpCommand other)
        {
            return this == other;
        }

        /// <summary>
        /// Checks if two objects are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CtpCommand)obj);
        }

        /// <summary>
        /// Computes a hashcode for this data.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 27;
            for (int x = 0; x < m_data.Length; x++)
            {
                hashCode = hashCode * 13 + m_data[x];
            }

            return hashCode;
        }

        /// <summary>
        /// Compares two object for equality.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(CtpCommand a, CtpCommand b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if ((object)a == null || (object)b == null)
                return false;
            if (a.m_data.Length != b.m_data.Length)
                return false;

            for (int x = 0; x < a.m_data.Length; x++)
            {
                if (a.m_data[x] != b.m_data[x])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares two object for inequality.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static bool operator !=(CtpCommand a, CtpCommand b)
        {
            return !Equals(a, b);
        }
    }
}