﻿using System;
using System.Collections.Generic;
using System.Text;
using Sttp.Codec;

namespace Sttp.Codec
{
    public class MetadataSchemaTable
    {
        public string TableName;
        public long LastModifiedVersion;
        public List<MetadataColumn> Columns = new List<MetadataColumn>();
        public List<MetadataForeignKey> ForeignKeys = new List<MetadataForeignKey>();

        public MetadataSchemaTable()
        {

        }

        public MetadataSchemaTable(SttpMarkupElement element)
        {
            TableName = (string)element.GetValue("TableName");
            LastModifiedVersion = (long)element.GetValue("LastModifiedVersion");

            foreach (var query in element.GetElement("Columns").ChildElements)
            {
                Columns.Add(new MetadataColumn(query));
            }
            foreach (var query in element.GetElement("ForeignKeys").ChildElements)
            {
                ForeignKeys.Add(new MetadataForeignKey(query));
            }
            element.ErrorIfNotHandled();

        }

        public void Save(SttpMarkupWriter sml)
        {
            sml.WriteValue("TableName", TableName);
            sml.WriteValue("LastModifiedVersion", LastModifiedVersion);
            using (sml.StartElement("Columns"))
            {
                foreach (var item in Columns)
                {
                    using (sml.StartElement("Column"))
                    {
                        item.Save(sml);
                    }
                }
            }
            using (sml.StartElement("ForeignKeys"))
            {
                foreach (var item in ForeignKeys)
                {
                    using (sml.StartElement("Key"))
                    {
                        item.Save(sml);
                    }
                }
            }
        }
    }
}