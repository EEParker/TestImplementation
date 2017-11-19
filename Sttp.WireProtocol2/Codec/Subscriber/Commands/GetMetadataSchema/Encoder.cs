﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sttp.WireProtocol.GetMetadataSchema
{
    public class Encoder : BaseEncoder
    {
        protected override CommandCode Code => CommandCode.GetMetadataSchema;

        public Encoder(Action<byte[], int, int> sendPacket, SessionDetails sessionDetails)
            : base(sendPacket, sessionDetails)
        {

        }

        public void GetMetadataSchema(Guid schemaVersion, long revision)
        {
            BeginCommand();
            Stream.Write(schemaVersion);
            Stream.Write(revision);
            EndCommand();
        }


    }
}