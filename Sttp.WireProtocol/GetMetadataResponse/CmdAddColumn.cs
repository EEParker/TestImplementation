﻿namespace Sttp.WireProtocol.GetMetadataResponse
{
    public class CmdAddColumn : ICmd
    {
        public SubCommand SubCommand => SubCommand.AddColumn;
        public short TableIndex;
        public short ColumnIndex;
        public string ColumnName;
        public byte ColumnTypeCode;

        public void Load(PacketReader reader)
        {
            TableIndex = reader.ReadInt16();
            ColumnIndex = reader.ReadInt16();
            ColumnName = reader.ReadString();
            ColumnTypeCode = reader.ReadByte();
        }
       
    }
}