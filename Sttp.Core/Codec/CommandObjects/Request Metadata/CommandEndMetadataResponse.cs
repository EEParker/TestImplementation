﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sttp.Codec
{
    public class CommandEndMetadataResponse : CommandBase
    {
        public readonly byte RawChannelID;
        public readonly int RowCount;

        public CommandEndMetadataResponse(byte rawChannelID, int rowCount)
            : base("EndMetadataResponse")
        {
            RawChannelID = rawChannelID;
            RowCount = rowCount;
        }

        public CommandEndMetadataResponse(SttpMarkupReader reader)
            : base("EndMetadataResponse")
        {
            var element = reader.ReadEntireElement();
            RawChannelID = (byte)element.GetValue("RawChannelID");
            RowCount = (int)element.GetValue("RowCount");
            element.ErrorIfNotHandled();
        }

        public override void Save(SttpMarkupWriter writer)
        {
            writer.WriteValue("RawChannelID", RawChannelID);
            writer.WriteValue("RowCount", RowCount);
        }
    }
}
