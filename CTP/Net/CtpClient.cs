﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using CTP.SRP;

namespace CTP.Net
{
    public class CtpClient
    {
        private IPEndPoint m_remoteEndpoint;
        private string m_srpUsername;
        private string m_srpPassword;
        private string m_ldapDomain;
        private string m_ldapUsername;
        private string m_ldapPassword;

        private X509Certificate m_userCertificate;
        private NetworkCredential m_credential;
        private X509CertificateCollection m_trustedCertificates;
        private string[] m_names;
        private X509CertificateCollection m_trustedRootCertificates;

        private TcpClient m_client;
        private NetworkStream m_networkStream;
        private SslStream m_sslStream;
        private NegotiateStream m_negotiateStream;
        private bool m_useSSL = true;
        private string m_hostName;

        public CtpClient()
        {

        }

        public void TurnOffSSL()
        {
            m_useSSL = false;
        }

        public void SetUserCredentials(X509Certificate certificate)
        {
            m_userCertificate = certificate;
        }

        public void SetLDAPCredentials(string domain, string username, string password)
        {
            m_ldapDomain = domain;
            m_ldapUsername = username;
            m_ldapPassword = password;
        }

        public void SetUserCredentials(string username, string password)
        {
            m_srpUsername = username;
            m_srpPassword = password;
        }

        public void SetUserCredentials(NetworkCredential credentials)
        {
            m_credential = credentials;
        }

        public void AddTrustedCertificates(X509CertificateCollection trustedCertificates)
        {
            m_trustedCertificates = trustedCertificates;
        }

        public void AddTrustedNames(string[] names, X509CertificateCollection trustedRootCertificates)
        {
            m_names = names;
            m_trustedRootCertificates = trustedRootCertificates;
        }

        public void SetHost(IPAddress address, int port)
        {
            m_remoteEndpoint = new IPEndPoint(address, port);
        }

        public void SetHost(IPEndPoint host)
        {
            m_remoteEndpoint = host;
        }

        public void SetHost(string hostName, int port)
        {
            m_hostName = hostName;
            IPAddress address = Dns.GetHostAddresses(hostName).First();
            m_remoteEndpoint = new IPEndPoint(address, port);
        }

        public Stream GetFinalStream => (Stream)m_sslStream ?? m_networkStream;

        public void Connect()
        {
            var client = new TcpClient();
            client.SendTimeout = 3000;
            client.ReceiveTimeout = 3000;
            client.Connect(m_remoteEndpoint);

            m_networkStream = client.GetStream();
            if (m_useSSL)
            {
                X509CertificateCollection collection = null;
                if (m_userCertificate != null)
                {
                    collection = new X509CertificateCollection(new[] { m_userCertificate });
                    m_sslStream = new SslStream(m_networkStream, false, UserCertificateValidationCallback, UserCertificateSelectionCallback, EncryptionPolicy.RequireEncryption);
                }
                else
                {
                    m_sslStream = new SslStream(m_networkStream, false, UserCertificateValidationCallback, null, EncryptionPolicy.RequireEncryption);
                }
                m_sslStream.AuthenticateAsClient(m_hostName ?? string.Empty, collection, SslProtocols.Tls12, false);
            }
            Stream stream = (Stream)m_sslStream ?? m_networkStream;

            if (m_srpUsername != null)
            {
                stream.WriteByte((byte)AuthenticationProtocols.SRP);
                stream.Flush();
                SrpAsClient();
            }
            else if (m_credential != null)
            {
                stream.WriteByte((byte)AuthenticationProtocols.NegotiateStream);
                stream.Flush();

                m_negotiateStream = new NegotiateStream(m_networkStream, true);
                m_negotiateStream.AuthenticateAsClient(CredentialCache.DefaultNetworkCredentials, m_hostName ?? string.Empty, ProtectionLevel.EncryptAndSign, TokenImpersonationLevel.Identification);
            }
            else if (m_ldapUsername != null)
            {
                stream.WriteByte((byte)AuthenticationProtocols.LDAP);
                stream.Flush();
            }
            else
            {
                stream.WriteByte((byte)AuthenticationProtocols.None);
                stream.Flush();
            }
        }

        private X509Certificate UserCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        private void SrpAsClient()
        {
            Stream stream = (Stream)m_sslStream ?? m_networkStream;
            Srp6aClient.Authenticate(m_srpUsername, m_srpPassword, stream, m_sslStream?.LocalCertificate, m_sslStream?.RemoteCertificate);
        }

        private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
