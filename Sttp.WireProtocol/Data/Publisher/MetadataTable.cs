﻿using System;
using System.Collections.Generic;
using ValueType = Sttp.WireProtocol.ValueType;

namespace Sttp.Data.Publisher
{
    public class MetadataTable
    {
        public readonly int TableId;

        /// <summary>
        /// All possible columns that are defined for the table.
        /// </summary>
        public Dictionary<int, MetadataColumn> Columns;

        /// <summary>
        /// All possible rows.
        /// </summary>
        public Dictionary<int, MetadataRow> Rows;

        public MetadataTable(int tableId)
        {
            TableId = tableId;
            Columns = new Dictionary<int, MetadataColumn>();
            Rows = new Dictionary<int, MetadataRow>();
        }

        public void FillSchema(MetadataChangeLog changeLog, int columnId, ValueType columnType)
        {
            MetadataColumn column;
            if (!Columns.TryGetValue(columnId, out column))
            {
                column = new MetadataColumn(columnId, columnType);
                Columns[columnId] = column;
                changeLog.AddColumn(TableId, column);
            }
        }

        public void FillData(MetadataChangeLog changeLog, int columnId, int recordID, object fieldValue)
        {
            MetadataRow row;
            if (!Rows.TryGetValue(recordID, out row))
            {
                row = new MetadataRow(recordID);
                Rows[recordID] = row;
                changeLog.AddRow(TableId, row);
            }
            row.FillData(TableId, changeLog, Columns[columnId], fieldValue);

        }
    }
}
