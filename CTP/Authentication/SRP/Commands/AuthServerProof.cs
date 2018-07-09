﻿using System;
using CTP.Serialization;

namespace CTP.SRP
{
    /// <summary>
    /// Proves to the client proof that the server is not an imposer, and specifies a ticket that can be used to resume this session at a later date.
    /// </summary>
    [DocumentName("AuthServerProof")]
    public class AuthServerProof
        : DocumentObject<AuthServerProof>
    {
        /// <summary>
        /// The server proof is Derived Key 2
        /// </summary>
        [DocumentField()] public byte[] ServerProof { get; private set; }

        /// <summary>
        /// A ticket that must be presented to the server in order to resume this session.
        /// Tickets can be as simple as a 1 byte identifier, or as complicated as encrypting the session data. 
        /// The session data for simple tickets must be stored on the server, complex tickets can offload this information to the client.
        /// </summary>
        [DocumentField()] public byte[] SessionTicket { get; private set; }

        /// <summary>
        /// The approved start time of the ticket. The client may generate a derived ticket with a date after this date.
        /// </summary>
        [DocumentField()] public DateTime ValidAfter { get; private set; }

        /// <summary>
        /// The approved end time of the ticket. The client may generate a derived ticket with a date before this date.
        /// </summary>
        [DocumentField()] public DateTime ValidBefore { get; private set; }

        /// <summary>
        /// The available roles for this ticket. The client may specify null to grant all roles, or may select the roles is wishes to grant in a derived ticket.
        /// </summary>
        [DocumentField()] public string[] Roles { get; private set; }

        public AuthServerProof(byte[] serverProof)
        {
            ServerProof = serverProof;
        }

        private AuthServerProof()
        {

        }

        public static explicit operator AuthServerProof(CtpDocument obj)
        {
            return FromDocument(obj);
        }
    }
}