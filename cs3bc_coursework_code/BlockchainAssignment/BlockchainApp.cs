using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockchainAssignment
{
    public partial class BlockchainApp : Form
    {
        Blockchain blockchain;
        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            richTextBox1.Text = "New Blockchain Started";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)                  // Print Block Button
        {
            if (Int32.TryParse(textBox1.Text, out int index))
                richTextBox1.Text = blockchain.getBlock(index);
            else
                richTextBox1.Text = "Not a number";

        }

        private void button2_Click(object sender, EventArgs e)                  // Create Wallet Button
        {
            String privKey;
            Wallet.Wallet myNewWallet = new Wallet.Wallet(out privKey);
            textBox2.Text = myNewWallet.publicID;
            this.textBox3.Text = privKey;
        }

        private void button3_Click(object sender, EventArgs e)                  // Validate Keys Button
        {
            if (Wallet.Wallet.ValidatePrivateKey(textBox3.Text, textBox2.Text))
            {
                richTextBox1.Text = "Keys are Valid";
            }
            else
            {
                richTextBox1.Text = "Keys are Invalid";
            }
        }

        private void button4_Click(object sender, EventArgs e)                  // Create Transaction Button
        {

            Transaction newTransaction = new Transaction(textBox2.Text, textBox6.Text, Double.Parse(textBox4.Text), Double.Parse(textBox5.Text), textBox3.Text);
            blockchain.transactionPool.Add(newTransaction);
            richTextBox1.Text = newTransaction.ToString();
        }

        private void button5_Click(object sender, EventArgs e)                  // Print All Button
        {
            richTextBox1.Text = blockchain.ToString();
        }

        private void button6_Click(object sender, EventArgs e)                  // Pring Pending Transactions Button
        {
            richTextBox1.Text = String.Join("\n", blockchain.transactionPool);
        }

        private void button7_Click(object sender, EventArgs e)                  // Generate Block Button
        {
            blockchain.minerAddress = textBox2.Text; // Public key of the miner
            List<Transaction> transactions = blockchain.getPendingTransactions();
            Block newBlock = new Block(blockchain.GetLastBlock(), transactions, textBox2.Text);
            blockchain.Blocks.Add(newBlock);

            richTextBox1.Text = blockchain.ToString() ;
            }

        private void button8_Click(object sender, EventArgs e)                  // Validate Chain Button
        {
            // Contiguity Checks
            bool valid = true;

            if (blockchain.Blocks.Count == 1)
            {
                if (!blockchain.validateMerkleRoot(blockchain.Blocks[0]))
                {
                    richTextBox1.Text = "Blockchain is invalid";
                }
                else
                {
                    richTextBox1.Text = "Blockchain is valid";
                }
                return;
            }
            for (int i = 1; i < blockchain.Blocks.Count - 1; i++)
            {
                Block current = blockchain.Blocks[i];
                Block previous = blockchain.Blocks[i - 1];
                // Compare hashes
                if (current.prevHash != previous.hash ||
                !blockchain.validateMerkleRoot(current) ||
                !blockchain.validateBlockHash(current) ||
                !blockchain.validateTransactionSignatures(current))
                    
                {
                    richTextBox1.Text = "Blockchain is invalid";
                    return;

                }

            }
            if (valid)
            {
                richTextBox1.Text = "Blockchain is valid";
            }
            else
            {
                richTextBox1.Text = "Blockchain is invalid";
            }
            
        }
       
        private void button9_Click(object sender, EventArgs e)              // Check Balance Button
        {
            richTextBox1.Text = blockchain.GetBalance(textBox2.Text).ToString() + " Assignment Coin";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)     // Selection strategy drop-down menu
        
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Greedy":
                    blockchain.selectionStrategy = TransactionSelectionStrategy.Greedy;
                    break;

                case "Altruistic":
                    blockchain.selectionStrategy = TransactionSelectionStrategy.Altruistic;
                    break;

                case "Random":
                    blockchain.selectionStrategy = TransactionSelectionStrategy.Random;
                    break;

                case "AddressPreference":
                    blockchain.selectionStrategy = TransactionSelectionStrategy.AddressPreference;
                    break;
            }

            richTextBox1.Text = "Transaction strategy: {blockchain.selectionStrategy}";
        }

    }
}


