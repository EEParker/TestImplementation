﻿using System;

namespace Sttp.WireProtocol
{
    /// <summary>
    /// Responsible for encoding each command into bytes
    /// </summary>
    public class SubscriptionDecoder 
    {
        public CommandCode CommandCode => CommandCode.Subscribe;

        public void Fill(PacketReader buffer)
        {
            throw new System.NotImplementedException();
        }

        
    }
}
