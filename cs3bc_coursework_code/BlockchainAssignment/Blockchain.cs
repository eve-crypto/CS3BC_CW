using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    public enum TransactionSelectionStrategy   // Enum for transaction selection strategy
    {
        Greedy,
        Altruistic,
        Random,
        AddressPreference
    }


    class Blockchain
    {
        public TransactionSelectionStrategy selectionStrategy = TransactionSelectionStrategy.Greedy;  // Greedy strategy setting
        public string minerAddress = "";
       

        public List<Block> Blocks;

        public List<Transaction> transactionPool = new List<Transaction>();   // Create a transaction pool
        int transactionPerBlock = 5;

        public Blockchain()                                                // Ctreate a new list and add blocks after
        {
            Blocks = new List<Block>() 
            {
                new Block()
            };
            transactionPool = new List<Transaction>();
        }
        
        public String getBlock(int index)
        {
            if (index >= 0 && index < Blocks.Count)  // Check if the index is valid
                return Blocks[index].ToString();
            return "Block does not Exist";
        }
        public Block GetLastBlock()              // Returns last block in the chain
        {
            return Blocks[Blocks.Count - 1];
        }
       
        public List<Transaction> getPendingTransactions()        // Pending transactions method including selection strategies
        {
            int n = Math.Min(transactionPerBlock, transactionPool.Count);
            List<Transaction> selected;

            switch (selectionStrategy)
            {
                case TransactionSelectionStrategy.Greedy:
                                                                // Pick transactions with the highest fees
                    selected = transactionPool
                        .OrderByDescending(t => t.fee)
                        .Take(n)
                        .ToList();
                    break;

                case TransactionSelectionStrategy.Altruistic:
                                                                // Pick transactions with the lowest fees
                    selected = transactionPool
                        .OrderBy(t => t.fee)
                        .Take(n)
                        .ToList();
                    break;

                case TransactionSelectionStrategy.Random:
                    selected = transactionPool
                        .OrderBy(t => Guid.NewGuid())           // Simple random shuffle
                        .Take(n)
                        .ToList();
                    break;

                case TransactionSelectionStrategy.AddressPreference:
                                                                // Prioritising transactions involving the minerAddress
                    selected = transactionPool
                        .OrderByDescending(t =>
                            t.senderAddress == minerAddress || t.recipientAddress == minerAddress ? 1 : 0
                        )
                        .ThenByDescending(t => t.fee)           // Secondary sort by fee 
                        .Take(n)
                        .ToList();
                    break;

                default:
                   
                    selected = transactionPool
                        .Take(n)
                        .ToList();
                    break;
            }

                                                                // Remove selected transactions from the pool
            foreach (var tx in selected)
            {
                transactionPool.Remove(tx);
            }

            return selected;
        }

        public override string ToString()
        {
            String output = String.Empty;
            Blocks.ForEach(b => output += (b.ToString() + "\n"));   // Loop through the blocks and add them to the output
            return output;
        }

        public double GetBalance(String address)                    // Get the balance of a given address
        {
            double balance = 0;
            foreach (Block b in Blocks)
            {
                foreach (Transaction t in b.transactionList)
                {
                    if (t.recipientAddress.Equals(address))
                    {
                        balance += t.amount;
                    }
                    if (t.senderAddress.Equals(address))
                    {
                        balance -= (t.amount + t.fee);
                    }
                }
            }
            return balance;
        }
        public bool validateBlockHash(Block b)                      // Validate the block hash
        {
            string originalHash = b.hash;
            string recalculatedHash = b.CreateHash();

            return originalHash == recalculatedHash;
        }
        public bool validateTransactionSignatures(Block b)          // Validate the transaction signatures
        {
            foreach (Transaction t in b.transactionList)
            {
                if (t.senderAddress == "Mine Rewards")
                    continue; // Skip reward transaction

                if (!Wallet.Wallet.ValidateSignature(t.senderAddress, t.hash, t.signature))
                    return false;
            }
            return true;
        }

        public bool validateMerkleRoot(Block b)                       // Validate the Merkle root
        {
            String reMerkle = Block.MerkleRoot(b.transactionList);
            return reMerkle.Equals(b.merkleRoot);
        }
    }
}
