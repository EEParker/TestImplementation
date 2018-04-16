﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sttp.Codec
{
    public class CommandSubscriptionRequestFailed : CommandBase
    {
        public readonly string Reason;
        public readonly string Details;

        public CommandSubscriptionRequestFailed(string reason, string details)
            : base("SubscriptionRequestFailed")
        {
            Reason = reason;
            Details = details;
        }

        public CommandSubscriptionRequestFailed(SttpMarkupReader reader)
            : base("SubscriptionRequestFailed")
        {
            var element = reader.ReadEntireElement();

            Reason = (string)element.GetValue("Reason");
            Details = (string)element.GetValue("Details");


            element.ErrorIfNotHandled();
        }

        public override void Save(SttpMarkupWriter writer)
        {
            writer.WriteValue("Reason", Reason);
            writer.WriteValue("Details", Details);
        }
    }
}