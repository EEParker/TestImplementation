﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sttp.Codec;

namespace Sttp
{
    public class SttpQueryColumn
    {
        public int TableIndex;
        public string ColumnName;
        public int Variable;

        public SttpQueryColumn(int tableIndex, string columnName, int variable)
        {
            TableIndex = tableIndex;
            ColumnName = columnName;
            Variable = variable;
        }

        public SttpQueryColumn(PayloadReader rd)
        {
            TableIndex = rd.ReadInt32();
            ColumnName = rd.ReadString();
            Variable = rd.ReadInt32();
        }

        public void Save(PayloadWriter wr)
        {
            wr.Write(TableIndex);
            wr.Write(ColumnName);
            wr.Write(Variable);
        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine($"({nameof(SttpQueryColumn)}) {Variable} = {TableIndex}.{ColumnName} ");
        }

    }

    public class SttpQueryJoinedTable
    {
        public int TableIndex;
        public List<SttpQueryJoinPath> JoinPath;

        public SttpQueryJoinedTable(int tableIndex, List<SttpQueryJoinPath> joinPath)
        {
            TableIndex = tableIndex;
            JoinPath = joinPath;
        }

        public SttpQueryJoinedTable(PayloadReader rd)
        {
            throw new NotImplementedException();
        }


        public void Save(PayloadWriter wr)
        {
            wr.Write(TableIndex);
            wr.Write(JoinPath);
        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine("(" + nameof(SttpQueryJoinedTable) + ")");
            builder.Append(linePrefix); builder.AppendLine($"TableIndex: {TableIndex} ");

            builder.Append(linePrefix); builder.AppendLine($"JoinPath Count {JoinPath.Count} ");
            foreach (var table in JoinPath)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }

        }
    }

    public class SttpQueryJoinPath
    {
        public string JoinedColumn;
        public string JoinedTable;
        public JoinType JoinType;

        public SttpQueryJoinPath(string column, string table, JoinType type = JoinType.Inner)
        {
            JoinedColumn = column;
            JoinedTable = table;
            JoinType = type;
        }

        public SttpQueryJoinPath(PayloadReader rd)
        {
            throw new NotImplementedException();
        }

        public void Save(PayloadWriter wr)
        {
            wr.Write(JoinedColumn);
            wr.Write(JoinedTable);
            wr.Write((byte)JoinType);
        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine($"({nameof(SttpQueryJoinPath)}) {JoinedColumn} {JoinType} {JoinedTable} ");
        }
    }

    public enum JoinType
    {
        Left,
        Inner,
    }

    public class SttpQueryLiterals
    {
        public SttpValue Value;
        public int Variable;

        public SttpQueryLiterals(SttpValue value, int variable)
        {
            Value = value;
            Variable = variable;
        }
        public SttpQueryLiterals(PayloadReader rd)
        {
            throw new NotImplementedException();
        }
        public void Save(PayloadWriter wr)
        {
            wr.Write(Variable);
            wr.Write(Value);
        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine($"({nameof(SttpQueryLiterals)}) {Variable} = {Value.AsString} ");
        }
    }

    public class SttpProcedureStep
    {
        public string Function;
        public List<int> InputVariables;
        public int OutputVariable;

        public SttpProcedureStep(string function, List<int> inputVariables, int outputVariable)
        {
            Function = function;
            InputVariables = inputVariables;
            OutputVariable = outputVariable;
        }

        public SttpProcedureStep(PayloadReader rd)
        {
            throw new NotImplementedException();
        }
        public void Save(PayloadWriter wr)
        {
            wr.Write(Function);
            wr.Write(InputVariables);
            wr.Write(OutputVariable);
        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine($"({nameof(SttpProcedureStep)}) {OutputVariable} = {Function}({string.Join(",", InputVariables)}) ");
        }
    }

    public class SttpOutputColumns
    {
        public int Variable;
        public string ColumnName;

        public SttpOutputColumns(int variable, string columnName)
        {
            Variable = variable;
            ColumnName = columnName;
        }

        public SttpOutputColumns(PayloadReader rd)
        {
            Variable = rd.ReadInt32();
            ColumnName = rd.ReadString();
        }

        public void Save(PayloadWriter wr)
        {
            wr.Write(Variable);
            wr.Write(ColumnName);
        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine($"({nameof(SttpOutputColumns)}) {Variable} = {ColumnName} ");
        }


    }

    /// <summary>
    /// The STTP based query expression object
    /// </summary>
    public class SttpQueryStatement
    {
        public string DirectTable;
        public List<SttpQueryJoinedTable> JoinedTables = new List<SttpQueryJoinedTable>();
        public List<SttpQueryLiterals> Literals = new List<SttpQueryLiterals>();
        public List<SttpQueryColumn> ColumnInputs = new List<SttpQueryColumn>();
        public List<SttpProcedureStep> Procedure = new List<SttpProcedureStep>();
        public List<SttpOutputColumns> Outputs = new List<SttpOutputColumns>();
        public List<int> GroupByVariables = new List<int>();
        public int WhereBooleanVariable = -1;
        public List<SttpProcedureStep> HavingProcedure = new List<SttpProcedureStep>();
        public int HavingBooleanVariable = -1;

        public int Limit = -1;

        public SttpQueryStatement()
        {
        }

        public SttpQueryStatement(PayloadReader rd)
        {
            bool quit = false;
            while (!quit)
            {
                switch (rd.ReadByte())
                {
                    case 0:
                        quit = true;
                        break;
                    case 1:
                        DirectTable = rd.ReadString();
                        break;
                    case 2:
                        JoinedTables = rd.ReadListSttpQueryJoinedTable();
                        break;
                    case 3:
                        Literals = rd.ReadListSttpQueryLiterals();
                        break;
                    case 4:
                        ColumnInputs = rd.ReadListSttpQueryColumn();
                        break;
                    case 5:
                        Procedure = rd.ReadListSttpSttpProcedureStep();
                        break;
                    case 6:
                        Outputs = rd.ReadListSttpOutputColumns();
                        break;
                    case 7:
                        GroupByVariables = rd.ReadListInt();
                        break;
                    case 8:
                        WhereBooleanVariable = rd.ReadInt32();
                        break;
                    case 9:
                        HavingProcedure = rd.ReadListSttpSttpProcedureStep();
                        break;
                    case 10:
                        HavingBooleanVariable = rd.ReadInt32();
                        break;
                    case 11:
                        Limit = rd.ReadInt32();
                        break;
                    default:
                        throw new VersionNotFoundException();
                }

            }
        }

        public void Save(PayloadWriter wr)
        {
            wr.Write((byte)1); wr.Write(DirectTable);
            wr.Write((byte)2); wr.Write(JoinedTables);
            wr.Write((byte)3); wr.Write(Literals);
            wr.Write((byte)4); wr.Write(ColumnInputs);
            wr.Write((byte)5); wr.Write(Procedure);
            wr.Write((byte)6); wr.Write(Outputs);
            wr.Write((byte)7); wr.Write(GroupByVariables);
            wr.Write((byte)8); wr.Write(WhereBooleanVariable);
            wr.Write((byte)9); wr.Write(HavingProcedure);
            wr.Write((byte)10); wr.Write(HavingBooleanVariable);
            wr.Write((byte)11); wr.Write(Limit);
            wr.Write((byte)0);

        }

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine("(" + nameof(SttpQueryStatement) + ")");
            builder.Append(linePrefix); builder.AppendLine($"DirectTable: {DirectTable} ");
            builder.Append(linePrefix); builder.AppendLine($"WhereBooleanVariable: {WhereBooleanVariable} ");
            builder.Append(linePrefix); builder.AppendLine($"HavingBooleanVariable: {HavingBooleanVariable} ");
            builder.Append(linePrefix); builder.AppendLine($"Limit: {Limit} ");

            builder.Append(linePrefix); builder.AppendLine($"JoinedTables Count {JoinedTables.Count} ");
            foreach (var table in JoinedTables)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }
            builder.Append(linePrefix); builder.AppendLine($"Literals Count {Literals.Count} ");
            foreach (var table in Literals)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }
            builder.Append(linePrefix); builder.AppendLine($"ColumnInputs Count {ColumnInputs.Count} ");
            foreach (var table in ColumnInputs)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }
            builder.Append(linePrefix); builder.AppendLine($"Procedure Count {Procedure.Count} ");
            foreach (var table in Procedure)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }
            builder.Append(linePrefix); builder.AppendLine($"Outputs Count {Outputs.Count} ");
            foreach (var table in Outputs)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }
            builder.Append(linePrefix); builder.AppendLine($"GroupByVariables Count {GroupByVariables.Count} ");
            builder.Append(linePrefix); builder.AppendLine($"Group By: ({string.Join(",", GroupByVariables)}) ");
            builder.Append(linePrefix); builder.AppendLine($"HavingProcedure Count {HavingProcedure.Count} ");
            foreach (var table in HavingProcedure)
            {
                table.GetFullOutputString(linePrefix + " ", builder);
            }
        }
    }
}
