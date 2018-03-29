﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sttp.Codec
{
    public class CommandMetadataSchemaUpdate : CommandBase
    {
        public readonly Guid RuntimeID;
        public readonly long VersionNumber;
        public readonly List<MetadataSchemaTableUpdate> Tables;

        public CommandMetadataSchemaUpdate(Guid runtimeID, long versionNumber, List<MetadataSchemaTableUpdate> tables)
            : base("MetadataSchemaUpdate")
        {
            RuntimeID = runtimeID;
            VersionNumber = versionNumber;
            Tables = new List<MetadataSchemaTableUpdate>(tables);
        }

        public CommandMetadataSchemaUpdate(SttpMarkupReader reader)
            : base("MetadataSchemaUpdate")
        {
            var element = reader.ReadEntireElement();

            RuntimeID = (Guid)element.GetValue("RuntimeID");
            VersionNumber = (long)element.GetValue("VersionNumber");

            foreach (var query in element.GetElement("Tables").ChildElements)
            {
                Tables.Add(new MetadataSchemaTableUpdate(query));
            }
            element.ErrorIfNotHandled();
        }

        public override void Save(SttpMarkupWriter writer)
        {
            writer.WriteValue("RuntimeID", RuntimeID);
            writer.WriteValue("VersionNumber", VersionNumber);
            using (writer.StartElement("Tables"))
            {
                foreach (var q in Tables)
                {
                    using (writer.StartElement("Table"))
                    {
                        q.Save(writer);
                    }
                }
            }
        }
    }
}