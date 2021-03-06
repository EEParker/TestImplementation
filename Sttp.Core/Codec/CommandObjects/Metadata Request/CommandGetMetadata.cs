﻿using System.Collections.Generic;
using CTP;

namespace Sttp.Codec
{
    /// <summary>
    /// Requests metadata. Specifies the table and optional columns to retrieve. 
    /// If the columns are empty, all columns will be returned for the specified table.
    /// 
    /// If there's an error, <see cref="CommandMetadataRequestFailed"/> will be returned.
    /// If successful, the following series of commands will occur:
    ///     <see cref="CommandBeginMetadataResponse"/> - Opening the raw channel to send the rows, and defining the response.
    ///     <see cref="CommandEndMetadataResponse"/> - Closing the raw channel.
    /// </summary>
    [CommandName("GetMetadata")]
    public class CommandGetMetadata
        : CommandObject<CommandGetMetadata>
    {
        [CommandField()]
        public string Table { get; private set; }
        [CommandField()]
        public List<string> Columns { get; private set; } = new List<string>();

        public CommandGetMetadata(string table, IEnumerable<string> columns)
        {
            Table = table;
            Columns.AddRange(columns);
        }

        //Exists to support CtpSerializable
        private CommandGetMetadata()
        { }

        public static explicit operator CommandGetMetadata(CtpCommand obj)
        {
            return FromCommand(obj);
        }

    }
}