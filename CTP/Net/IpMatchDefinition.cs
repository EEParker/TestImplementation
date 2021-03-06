﻿using System;
using System.Collections.Generic;
using System.Net;

namespace CTP.Net
{
    /// <summary>
    /// This does a longest match comparison of an IP Address with a bit mask.
    /// </summary>
    public class IpMatchDefinition
        : IComparable<IpMatchDefinition>, IEquatable<IpMatchDefinition>
    {
        private readonly IPAddress Ip;
        private readonly int MaskBits;

        private readonly int m_maskBits;
        private readonly byte[] m_ipBytes;
        private readonly byte[] m_mask;

        /// <summary>
        /// Creates a <see cref="IpMatchDefinition"/>
        /// </summary>
        /// <param name="address">The IP address to use.</param>
        /// <param name="maskBits">The number of bits to include in the mask</param>
        public IpMatchDefinition(IPAddress address, int maskBits)
        {
            Ip = address;
            MaskBits = maskBits;
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            m_ipBytes = address.GetAddressBytes();

            if (maskBits < 0 || maskBits > m_ipBytes.Length * 8)
                throw new ArgumentOutOfRangeException(nameof(maskBits), "Not valid");

            m_maskBits = maskBits;
            m_mask = new byte[m_ipBytes.Length];

            List<byte> mask = new List<byte>();
            while (maskBits > 0)
            {
                if (maskBits >= 8)
                {
                    mask.Add(255);
                }
                else
                {
                    mask.Add((byte)((1 << maskBits) - 1));
                }
                maskBits -= 8;
            }

            m_mask = mask.ToArray();
        }

        /// <summary>
        /// Sorts so the longest match will be the first one parsed.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IpMatchDefinition other)
        {
            //Since compares must be the longest match, maskbits is compared first.
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            var cmp = -m_maskBits.CompareTo(other.m_maskBits);
            if (cmp != 0)
                return cmp;
            cmp = m_ipBytes.Length.CompareTo(other.m_ipBytes.Length);
            if (cmp != 0)
                return cmp;
            for (int x = 0; x < m_ipBytes.Length; x++)
            {
                cmp = m_ipBytes[x].CompareTo(other.m_ipBytes[x]);
                if (cmp != 0)
                    return cmp;
            }
            return cmp;
        }

        /// <summary>
        /// Does a longest match comparison of the specified IP bytes.
        /// </summary>
        /// <param name="ipBytes">the bytes to do the comparison of.</param>
        /// <returns></returns>
        public bool IsMatch(byte[] ipBytes)
        {
            if (ipBytes.Length != m_ipBytes.Length)
                return false;

            for (int x = 0; x < m_mask.Length; x++)
            {
                if (((m_ipBytes[x] ^ ipBytes[x]) & m_mask[x]) != 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets if the IP address and mask are properly set up as Network/Mask
        /// </summary>
        public bool IsValid
        {
            get
            {
                for (int x = 0; x < m_mask.Length; x++)
                {
                    if ((m_ipBytes[x] | m_mask[x]) != m_mask[x])
                        return false;
                }
                for (int x = m_mask.Length; x < m_ipBytes.Length; x++)
                {
                    if (m_ipBytes[x] != 0)
                        return false;
                }

                return true;
            }
        }

        public bool Equals(IpMatchDefinition other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Ip.Equals(other.Ip) && MaskBits == other.MaskBits;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((IpMatchDefinition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Ip.GetHashCode() * 397) ^ MaskBits;
            }
        }

        public static bool operator ==(IpMatchDefinition left, IpMatchDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IpMatchDefinition left, IpMatchDefinition right)
        {
            return !Equals(left, right);
        }
    }
}
