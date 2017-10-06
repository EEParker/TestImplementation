﻿using System;
using Sttp.WireProtocol.MetadataPacket;

namespace Sttp.WireProtocol.Data
{
    public interface IMetadataEncoder
    {
        void BeginCommand();
        void EndCommand();

        #region [ Response Publisher to Subscriber ]

        void UseTable(int tableIndex);
        void AddTable(Guid majorVersion, long minorVersion, string tableName, TableFlags tableFlags);
        void AddColumn(int columnIndex, string columnName, ValueType columnType);
        void AddValue(int columnIndex, int rowIndex, byte[] value);
        void DeleteRow(int rowIndex);
        void TableVersion(int tableIndex, Guid majorVersion, long minorVersion);
        void AddRelationship(int tableIndex, int columnIndex, int foreignTableIndex);

        #endregion

        #region [ Request Subscriber to Publisher ]

        void GetTable(int tableIndex, int[] columnList, string[] filterExpression);
        void SyncTable(int tableIndex, Guid majorVersion, long minorVersion, int[] columnList);
        void SelectAllTablesWithSchema();
        void GetAllTableVersions();


        #endregion

    }
}