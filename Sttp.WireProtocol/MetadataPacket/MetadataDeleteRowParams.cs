﻿namespace Sttp.WireProtocol.Data
{
    public class MetadataDeleteRowParams : IMetadataParams
    {
        public MetadataCommand Command => MetadataCommand.DeleteRow;
        public int RowIndex;

    }
}