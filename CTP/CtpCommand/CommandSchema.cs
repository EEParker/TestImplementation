﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTP
{
    internal class CommandSchemaWriter
    {
        private ByteWriter m_stream;

        public CommandSchemaWriter()
        {
            m_stream = new ByteWriter();
        }

        public void DefineArray(string name)
        {
            m_stream.WriteBits8(0);
            m_stream.Write(name);
        }

        public void DefineElement(string name, int count)
        {
            m_stream.WriteBits8(1);
            m_stream.Write(name);
            m_stream.Write7BitInt((uint)count);
        }

        public void DefineValue(string name)
        {
            m_stream.WriteBits8(2);
            m_stream.Write(name);
        }

        public CommandSchema ToSchema()
        {
            return new CommandSchema(m_stream.ToArray());
        }
    }

    public enum CommandSchemaSymbol
    {
        DefineArray,
        DefineElement,
        DefineValue,
        EndOFStream
    }

    internal class CommandSchemaReader
    {
        private ByteReader m_stream;

        public CommandSchemaSymbol Symbol;

        public string Name;

        public int ElementCount;

        public CommandSchemaReader(byte[] data)
        {
            m_stream = new ByteReader();
            m_stream.SetBuffer(data, 0, data.Length);
        }

        public int Position
        {
            get => m_stream.Position;
            set => m_stream.Position = value;
        }

        public bool Next()
        {
            if (m_stream.IsEmpty)
            {
                Symbol = CommandSchemaSymbol.EndOFStream;
                Name = null;
                ElementCount = 0;
                return false;
            }

            switch (m_stream.ReadBits8())
            {
                case 0:
                    Symbol = CommandSchemaSymbol.DefineArray;
                    Name = m_stream.ReadString();
                    ElementCount = 0;
                    return true;
                case 1:
                    Symbol = CommandSchemaSymbol.DefineElement;
                    Name = m_stream.ReadString();
                    ElementCount = (int)m_stream.Read7BitInt();
                    return true;
                case 2:
                    Symbol = CommandSchemaSymbol.DefineValue;
                    Name = m_stream.ReadString();
                    ElementCount = 0;
                    return true;
                default:
                    throw new Exception("Wrong version number");
            }
        }
    }

    internal class CommandSchemaCompiled
    {
        public class Node
        {
            public readonly CommandSchemaSymbol Symbol;
            public readonly CtpCommandKeyword NodeName;
            public readonly int ElementCount;
            public readonly int PositionIndex;

            public Node(CommandSchemaReader reader, int positionIndex)
            {
                PositionIndex = positionIndex;
                NodeName = CtpCommandKeyword.Create(reader.Name);
                Symbol = reader.Symbol;
                ElementCount = reader.ElementCount;
            }

            public override string ToString()
            {
                return $"{Symbol} {NodeName} {ElementCount}";
            }
        }

        private List<Node> m_nodes = new List<Node>();

        public CommandSchemaCompiled(CommandSchema schema)
        {
            var reader = schema.CreateReader();
            while (reader.Next())
            {
                m_nodes.Add(new Node(reader, m_nodes.Count));
            }
        }

        public Node this[int index]
        {
            get
            {
                if (index >= m_nodes.Count)
                    return null;
                return m_nodes[index];
            }
        }
    }

    public class CommandSchema
    {
        private byte[] m_data;

        public CommandSchema(byte[] data)
        {
            m_data = data;
        }

        internal CommandSchemaReader CreateReader()
        {
            return new CommandSchemaReader(m_data);
        }

        internal CommandSchemaCompiled CompiledReader()
        {
            return new CommandSchemaCompiled(this);
        }
    }


}
