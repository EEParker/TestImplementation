﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTP;
using Sttp.Codec;

namespace Sttp
{
    public class SttpQueryRaw
    {
        public string QueryText;
        public string SyntaxLanguage;
        public List<Tuple<string, CtpObject>> Literals = new List<Tuple<string, CtpObject>>();

        public void GetFullOutputString(string linePrefix, StringBuilder builder)
        {
            builder.Append(linePrefix); builder.AppendLine("(" + nameof(SttpQueryRaw) + ")");
            builder.Append(linePrefix); builder.AppendLine($"SyntaxLanguage: {SyntaxLanguage} ");
            builder.Append(linePrefix); builder.AppendLine($"QueryText: {QueryText} ");
            builder.Append(linePrefix); builder.AppendLine($"Literals Count {Literals.Count} ");
            foreach (var table in Literals)
            {
                builder.Append(linePrefix); builder.AppendLine($" {table.Item1} {table.Item2.AsString} ");
            }
        }
    }
}
