using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Block
    {
        public int index;
        DateTime timestamp;
        public String hash;
        public String prevHash;
        public String merkleRoot;


        public List<Transaction> transactionList = new List<Transaction>();
        
        // Proof-of-work
        public long nonce = 0;
        public int difficulty = 4;
        // Rewards and fees
        public double reward = 1.0;  // Fixed logic
        public double fees = 0.0;
        public String minerAddress = String.Empty;

      
        

        /*Genesis Block*/
        public Block()
        {
            this.timestamp = DateTime.Now;
            this.index = 0;
            this.prevHash = String.Empty;
            this.hash = Mine();
            this.reward = 0;
            this.transactionList = new List<Transaction>();
        }
        public Block(int index, String prevHash)
        {
            this.timestamp = DateTime.Now;
            this.index = index + 1;
            this.prevHash = hash;
            this.hash = CreateHash();

        }
        public Block(Block lastBlock, List<Transaction> transactions, String minerAddress = "")  // Constructor for new blocks 
        {
            this.timestamp = DateTime.Now;
            this.index = lastBlock.index + 1;
            this.prevHash = lastBlock.hash;
            this.hash = Mine();
            transactionList = transactions;
            this.minerAddress = minerAddress;
            transactions.Add(CreateRewardTransaction(transactions));

            this.transactionList = transactions;
            this.merkleRoot = MerkleRoot(transactionList);
        }

        public String CreateHash()                      // Hash constructor
        {
            String hash = String.Empty;

            SHA256 hasher = SHA256Managed.Create();
            String input = index.ToString() + timestamp.ToString() + prevHash + nonce.ToString() + reward.ToString() + merkleRoot;  /* concatenate for hashing */
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            //Convert hash from byte aray to string
            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);
            }

            return hash;
        }
        public String Mine()                    // Mining function
        {
            nonce = 0;
            String hash = CreateHash();
            // Defining the difficulty criteria
            String re = new string('0', difficulty);   // Create regex string related to the difficulty
            // Until the difficulty criteria is met we will hash
            while (!hash.StartsWith(re))
            {
                nonce++;  // Nonce increment
                hash = CreateHash();
            }
            return hash;
        }
        public static String MerkleRoot(List<Transaction> transactionList)          // Create the Merkle root
        {
            List<String> hashes = transactionList.Select(t => t.hash).ToList();
            if (hashes.Count == 0)
            {
                return String.Empty;
            }
            if (hashes.Count == 1)
            {
                return HashCode.HashTools.CombineHash(hashes[0], hashes[0]);
            }
            while (hashes.Count != 1)
            {
                List<String> merkleLeaves = new List<String>();
                for (int i = 0; i < hashes.Count; i += 2)
                {
                    if (i == hashes.Count - 1)
                    {
                        merkleLeaves.Add(HashCode.HashTools.CombineHash(hashes[i], hashes[i]));
                    }
                    else
                    {
                        merkleLeaves.Add(HashCode.HashTools.CombineHash(hashes[i], hashes[i + 1]));
                    }
                }
                hashes = merkleLeaves;
            }

            return hashes[0];
        }

        public Transaction CreateRewardTransaction(List<Transaction> transactions)
        {
            // Sum the fees in the list
            double fees = transactions.Aggregate(0.0, (acc, t) => acc + t.fee);
            // Create the Rewards Transaction being sum of fees and rewards being transferred from Mine Rewards to miner
            return new Transaction("Mine Rewards", minerAddress, (reward + fees), 0, "");
        }
        public override string ToString()
        {
            return "Index: " + index.ToString() +
                "\nTimestamp: " + timestamp.ToString() +
                "\nHash: " + hash +
                "\nPrevious Hash: " + prevHash +
                "\nDifficulty: " + difficulty.ToString() +
                "\nTransactions: " + String.Join("\n", transactionList) +
                "\nReward: " + reward.ToString() +
                "\nFees: " + fees.ToString() +
                "\nMiner Address: " + minerAddress +
                "\nNonce: " + nonce.ToString() +
                "\nMerkle Root: " + merkleRoot;
        }
    }
}
