﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Sttp.Codec;
using Sttp.Core.BulkTransport;
using Sttp.Core.Data;

namespace Sttp.Core
{
    public class SttpServer
    {
        private Stream m_stream;
        private WireEncoder m_encoder;
        private WireDecoder m_decoder;
        private Thread m_processing;
        private Dictionary<string, ISttpCommandHandler> m_handler;

        public SttpServer(Stream networkStream)
        {
            m_handler = new Dictionary<string, ISttpCommandHandler>();
            m_stream = networkStream;
            m_encoder = new WireEncoder();
            m_decoder = new WireDecoder();

            RegisterCommandHandler(new SttpBulkTransportServer());
            RegisterCommandHandler(new SttpMetadataServer());
        }

        public void RegisterCommandHandler(ISttpCommandHandler handler)
        {
            foreach (var name in handler.CommandsHandled())
            {
                m_handler[name] = handler;
            }
        }

        private void ProcessRequest()
        {
            byte[] buffer = new byte[4096];
            int length = 0;
            while ((length = m_stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                m_decoder.FillBuffer(buffer, 0, length);
                CommandObjects obj;
                while ((obj = m_decoder.NextCommand()) != null)
                {
                    if (m_handler.TryGetValue(obj.CommandName, out ISttpCommandHandler handler))
                    {
                        handler.HandleCommand(obj, m_encoder);
                    }
                    else
                    {
                        m_encoder.RequestFailed(obj.CommandName, false, "Command Handler does not exist", "");
                    }
                }
            }
        }

        public void Start()
        {
            m_processing = new Thread(ProcessRequest);
            m_processing.Start();
        }





    }
}
