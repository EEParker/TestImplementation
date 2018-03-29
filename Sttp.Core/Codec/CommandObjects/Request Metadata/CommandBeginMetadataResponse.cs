﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sttp.Codec
{
    public class CommandBeginMetadataResponse : CommandBase
    {
        public readonly byte RawChannelID;
        public readonly Guid EncodingMethod;
        public Guid RuntimeID;
        public long VersionNumber;
        public string TableName;
        public List<MetadataColumn> Columns;

        public CommandBeginMetadataResponse(byte rawChannelID, Guid encodingMethod, Guid runtimeID, long versionNumber, string tableName, List<MetadataColumn> columns)
            : base("BeginMetadataResponse")
        {
            RawChannelID = rawChannelID;
            EncodingMethod = encodingMethod;
            RuntimeID = runtimeID;
            VersionNumber = versionNumber;
            TableName = tableName;
            Columns = columns;
        }

        public CommandBeginMetadataResponse(SttpMarkupReader reader)
            : base("BeginMetadataResponse")
        {
            var element = reader.ReadEntireElement();

            RawChannelID = (byte)element.GetValue("RawChannelID");
            EncodingMethod = (Guid)element.GetValue("EncodingMethod");
            RuntimeID = (Guid)element.GetValue("RuntimeID");
            VersionNumber = (long)element.GetValue("VersionNumber");
            TableName = (string)element.GetValue("TableName");
            Columns = new List<MetadataColumn>();
            foreach (var e in element.GetElement("Columns").ChildElements)
            {
                Columns.Add(new MetadataColumn(e));
            }
            element.ErrorIfNotHandled();
        }

        public override void Save(SttpMarkupWriter writer)
        {
            writer.WriteValue("RawChannelID", RawChannelID);
            writer.WriteValue("EncodingMethod", EncodingMethod);
            writer.WriteValue("RuntimeID", RuntimeID);
            writer.WriteValue("VersionNumber", VersionNumber);
            writer.WriteValue("TableName", TableName);
            using (writer.StartElement("Columns"))
            {
                foreach (var c in Columns)
                {
                    using (writer.StartElement("Column"))
                    {
                        c.Save(writer);
                    }
                }
            }
        }
    }
}