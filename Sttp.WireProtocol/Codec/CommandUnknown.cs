﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTP
{
    /// <summary>
    /// If a command code is not registered by this API layer, it is therefore reported in it's raw format. 
    /// To register a command, call <see cref="DocumentCommandBase.Register"/>
    /// </summary>
    public class CommandUnknown : DocumentCommandBase
    {
        /// <summary>
        /// The Markup data for this command.
        /// </summary>
        public readonly CtpDocument Document;


        public CommandUnknown(string commandName, CtpDocument document)
            : base(commandName)
        {
            Document = document;
        }

        /// <summary>
        /// Saves this command object to a <see cref="CtpDocument"/>.
        /// </summary>
        /// <param name="writer">The writer to save the command to.</param>
        public override void Save(CtpDocumentWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
