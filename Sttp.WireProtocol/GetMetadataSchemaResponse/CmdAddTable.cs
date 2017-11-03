﻿using System;

namespace Sttp.WireProtocol.GetMetadataSchemaResponse
{
    public class CmdAddTable : ICmd
    {
        public SubCommand SubCommand => SubCommand.AddTable;
        public short TableIndex;
        public string TableName;
        public TableFlags TableFlags;

        public void Load(PacketReader reader)
        {
            TableIndex = reader.ReadInt16();
            TableName = reader.ReadString();
            TableFlags = reader.Read<TableFlags>();
        }

    }
}