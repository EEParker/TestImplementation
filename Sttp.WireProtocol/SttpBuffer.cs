﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sttp.Codec;

namespace Sttp
{
    public class SttpBuffer
    {
        public byte[] Data;

        public SttpBuffer(byte[] data)
        {
            Data = (byte[])data.Clone();
        }

        public SttpBuffer(ByteReader data)
        {
            throw new NotImplementedException();
        }
    }
}
