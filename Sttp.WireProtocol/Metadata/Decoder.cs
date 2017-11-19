﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sttp.WireProtocol.Metadata
{
    public class Decoder
    {
        private ICmd[] m_commands;
        private Cmd m_cmd;

        private PacketReader m_packet = new PacketReader(new SessionDetails());

        public Decoder()
        {
            m_cmd = new Cmd();
            m_commands = new ICmd[10];
            m_commands[(byte)SubCommand.Finished] = new CmdFinished();
            m_commands[(byte)SubCommand.DefineRow] = new CmdDefineRow();
            m_commands[(byte)SubCommand.DefineResponse] = new CmdDefineResponse();
            m_commands[(byte)SubCommand.VersionNotCompatible] = new CmdVersionNotCompatible();
            m_commands[(byte)SubCommand.UndefineRow] = new CmdUndefineRow();
        }

        public CommandCode CommandCode => CommandCode.Metadata;

        public void Fill(PacketReader buffer)
        {
            m_packet = buffer;
        }

        public Cmd NextCommand()
        {
            if (m_packet.Position == m_packet.Length)
                return null;

            SubCommand subCommand = m_packet.Read<SubCommand>();
            m_commands[(byte)subCommand].Load(m_packet);
            m_cmd.Load(m_commands[(byte)subCommand]);
            return m_cmd;
        }
    }
}