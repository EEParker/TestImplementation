﻿using CTP;

namespace Sttp.Codec
{
    public class CommandMetadataRequestFailed : DocumentCommandBase
    {
        public readonly string Reason;
        public readonly string Details;

        public CommandMetadataRequestFailed(string reason, string details)
            : base("MetadataRequestFailed")
        {
            Reason = reason;
            Details = details;
        }

        public CommandMetadataRequestFailed(CtpDocumentReader reader)
            : base("MetadataRequestFailed")
        {
            var element = reader.ReadEntireElement();

            Reason = (string)element.GetValue("Reason");
            Details = (string)element.GetValue("Details");


            element.ErrorIfNotHandled();
        }

        public override void Save(CtpDocumentWriter writer)
        {
            writer.WriteValue("Reason", Reason);
            writer.WriteValue("Details", Details);
        }
    }
}