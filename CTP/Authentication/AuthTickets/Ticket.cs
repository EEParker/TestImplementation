﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GSF;
using GSF.IO;

namespace CTP.Authentication
{
    [DocumentName("Ticket")]
    public class Ticket
        : DocumentObject<Ticket>
    {
        /// <summary>
        /// Gets the UTC time this ticket is valid from.
        /// </summary>
        [DocumentField()] public DateTime ValidFrom { get; private set; }

        /// <summary>
        /// Gets the UTC time this ticket is valid until.
        /// </summary>
        [DocumentField()] public DateTime ValidTo { get; private set; }

        /// <summary>
        /// A string that identifies the user of this ticket.
        /// </summary>
        [DocumentField()] public string LoginName { get; private set; }

        /// <summary>
        /// The list of roles granted by this ticket.
        /// </summary>
        [DocumentField()] public List<string> Roles { get; private set; }

        /// <summary>
        /// The list of approved certificates that the remote resource may use. 
        /// This is the SHA-256 hash of the public key.
        /// </summary>
        [DocumentField()] public List<byte[]> ApprovedClientCertificates { get; private set; }

        private Ticket()
        {

        }

        public static explicit operator Ticket(CtpDocument obj)
        {
            return FromDocument(obj);
        }

        public Ticket(DateTime validFrom, DateTime validTo, string loginName, List<string> roles, List<byte[]> approvedClientCertificates)
        {
            ValidFrom = validFrom;
            ValidTo = validTo;
            LoginName = loginName;
            Roles = roles;
            ApprovedClientCertificates = approvedClientCertificates;
        }
    }
}