using System;
using System.Collections.Generic;
using System.Linq;

namespace Sttp.Data
{
    public class MetadataRow
    {
        public readonly SttpValue Key;
        public readonly SttpValueSet Fields;
        /// <summary>
        /// The index position in he table that has this foreign key. 
        /// -1 means the row does not exist.
        /// </summary>
        public readonly int[] ForeignKeys;
        public readonly long Revision;

        public MetadataRow(SttpValue key, SttpValueSet fields, int foreignKeyCount, long revision)
        {
            Revision = revision;
            Key = key;
            Fields = fields;
            ForeignKeys = new int[foreignKeyCount];
            for (int x = 0; x < foreignKeyCount; x++)
            {
                ForeignKeys[x] = -1;
            }
        }

    }
}
