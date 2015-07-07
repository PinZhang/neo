﻿using AntShares.Core;
using AntShares.IO;
using AntShares.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Transaction = AntShares.Core.Transaction;

namespace AntShares.UI
{
    internal partial class DeveloperToolsForm : Form
    {
        private static readonly int[] magic = { 38, 38, 40, 40, 37, 39, 37, 39, 65, 66, 65, 66 };
        private List<int> chars = new List<int>();

        public DeveloperToolsForm()
        {
            InitializeComponent();
            tabControl1.TabPages.Remove(tabPage100);
            textBox3.Text = Blockchain.AntShare.ToArray().ToHexString();
        }

        private void DeveloperToolsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!tabControl1.TabPages.Contains(tabPage100))
            {
                chars.Add(e.KeyValue);
                if (chars.Count >= magic.Length && chars.Skip(chars.Count - magic.Length).SequenceEqual(magic))
                {
                    tabControl1.TabPages.Add(tabPage100);
                    tabControl1.SelectedTab = tabPage100;
                    this.KeyDown -= DeveloperToolsForm_KeyDown;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegisterTransaction antshare = new RegisterTransaction
            {
                RegisterType = RegisterType.AntShare,
                RegisterName = "[{'lang':'zh-CHS','name':'小蚁股'},{'lang':'en','name':'AntShare'}]",
                Amount = (Int64)numericUpDown1.Value,
                Issuer = textBox1.Text.ToScriptHash(),
                Admin = textBox2.Text.ToScriptHash(),
                Inputs = new TransactionInput[0],
                Outputs = new TransactionOutput[0]
            };
            SignatureContext context = new SignatureContext(antshare);
            InformationBox.Show(context.ToString(), "小蚁股签名上下文");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Block block = new Block
            {
                PrevBlock = new UInt256(),
                Timestamp = DateTime.Now.ToTimestamp(),
                Nonce = 2083236893, //向比特币致敬
                Miner = Wallet.CreateRedeemScript((byte)(Blockchain.StandbyMiners.Length / 2 + 1), Blockchain.StandbyMiners).ToScriptHash(),
                Transactions = new Transaction[]
                { 
                    new GenerationTransaction
                    {
                        Nonce = 0,
                        Inputs = new TransactionInput[0],
                        Outputs = new TransactionOutput[0],
                        Scripts = new byte[0][]
                    },
                    textBox3.Text.HexToBytes().AsSerializable<RegisterTransaction>()
                }
            };
            block.RebuildMerkleRoot();
            SignatureContext context = new SignatureContext(block);
            InformationBox.Show(context.ToString(), "创世区块签名上下文");
        }
    }
}
