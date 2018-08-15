﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CTP.SRP;

namespace CTP.Net
{
    public class ServerAuthentication
    {
        //Must be sorted because longest match is used to match an IP address
        private SortedList<IpMatchDefinition, TrustedIPUserMapping> m_ipUsers = new SortedList<IpMatchDefinition, TrustedIPUserMapping>();
        private Dictionary<string, string> m_pairingAccounts = new Dictionary<string, string>();

        public ServerAuthentication()
        {

        }

        public void AuthenticateSessionByIP(CtpSession session)
        {
            var ipBytes = session.RemoteEndpoint.Address.GetAddressBytes();

            foreach (var item in m_ipUsers.Values)
            {
                if (item.IP.IsMatch(ipBytes))
                {
                    session.LoginName = item.LoginName;
                    session.GrantedRoles.UnionWith(item.Roles);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds user information for a connection that occurs when an IP user.
        /// </summary>
        /// <param name="ip">The network IP address.</param>
        /// <param name="bitmask">The bitmask of the address.</param>
        /// <param name="loginName">The login name to associate with this connection.</param>
        /// <param name="roles">Roles to assign to this session.</param>
        public void AddIPUser(IPAddress ip, int bitmask, string loginName, params string[] roles)
        {
            var mask = new IpMatchDefinition(ip, bitmask);
            m_ipUsers[mask] = new TrustedIPUserMapping(mask, loginName, roles);
        }

        public void AddPairingPin(string accountName, string password)
        {
            m_pairingAccounts[accountName.Normalize(NormalizationForm.FormKC).Trim().ToLower()] = password.Normalize(NormalizationForm.FormKC);
        }

        public void RemoveCredential(string credentialName)
        {
            m_pairingAccounts.Remove(credentialName.Normalize(NormalizationForm.FormKC).Trim().ToLower());
        }

        public string LookupCredential(CertExchange command)
        {
            var identity = command;
            string userName = identity.AccountName.Normalize(NormalizationForm.FormKC).Trim().ToLower();

            if (!m_pairingAccounts.TryGetValue(userName, out var user))
            {
                throw new Exception("Account Not Found");
            }
            return user;
        }
    }
}
