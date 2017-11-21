﻿namespace Sttp.Codec.Metadata
{
    public class CmdUndefineRow : ICmd
    {
        public SubCommand SubCommand => SubCommand.UndefineRow;
        public SttpValue PrimaryKey;

        public void Load(PayloadReader reader)
        {
            PrimaryKey = reader.Read<SttpValue>();
        }

    }
}