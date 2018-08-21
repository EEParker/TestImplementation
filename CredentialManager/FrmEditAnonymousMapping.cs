﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CTP.Net;

namespace CredentialManager
{
    public partial class FrmEditAnonymousMapping : Form
    {
        public FrmEditAnonymousMapping(CtpAnonymousMapping ip)
        {
            InitializeComponent();

            txtAddress.Text = ip.AccessList.IpAddress.ToString();
            numMask.Value = ip.AccessList.MaskBits;
            txtAccount.Text = ip.AccountName;

        }

        public CtpAnonymousMapping SaveData()
        {
            var ip = new IpAndMask();
            ip.MaskBits = (int)numMask.Value;
            ip.IpAddress = txtAddress.Text;
            var rv = new CtpAnonymousMapping();
            rv.AccessList = ip;
            rv.AccountName = txtAccount.Text;
            return rv;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var mask = new IpMatchDefinition(IPAddress.Parse(txtAddress.Text), (int)numMask.Value);
            if (!mask.IsValid)
            {
                MessageBox.Show("IP and Mask combination is invalid");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}