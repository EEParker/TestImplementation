﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sttp.WireProtocol.NegotiateSession
{
    public enum SubCommand : byte
    {
        /// <summary>
        /// An invalid command to indicate that nothing is assigned.
        /// This cannot be sent over the wire.
        /// </summary>
        Invalid = 0x00,

        /// <summary>
        /// When negotiating options, the protocol defaults to the listener acting as the 'Server' who gets to make all of the decisions.
        /// In a scenario where outbound connections are made for security purposes, the client can request to behave as the 'Server'. 
        /// This command will either be rejected or accepted by the current 'server'. After this, the role swaps or the connection is terminated.
        /// 
        /// Payload: None
        /// </summary>
        InitiateReverseConnection,

        /// <summary>
        /// Specifies a list of all of the functionality that is supported along with some kind if Version string or pick-list.
        /// This command is first sent by the client to the server, 
        /// then the server echoes a list back to the client with the selected modes of operation.
        /// 
        /// Payload:
        /// SttpNamedSet Options
        /// 
        /// </summary>
        SupportedFunctionality,

        /// <summary>
        /// Changes the default instance.
        /// Payload:
        /// 
        /// string InstanceName
        /// </summary>
        ChangeInstance,

        /// <summary>
        /// Gets all of the named instances for the server.
        /// </summary>
        GetAllInstances,

        /// <summary>
        /// Requests that the UDP cipher key is changed.
        /// 
        /// Payload:
        /// byte[] nonce
        /// 
        /// Remarks: 
        /// In order to ensure that the key is never compromised the key is derived from random numbers generated by both the client and server.
        /// key = SHA256(ServerNonce | ClientNonce)
        /// iv = SHA256(ClientNonce | ServerNonce)
        /// 
        /// </summary>
        ChangeUdpCiper,
    }


}
