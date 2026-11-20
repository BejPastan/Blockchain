using BlockchainDemo.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlockchainDemo.Data
{
    [System.Serializable]
    public class User
    {
        public BigInteger Cash { get; private set; } = 0;


        [SerializeField]
        public string Name;
        public string PublicKey;
        public string PrivateKey;

        public List<Token> userTokens = new();

        public User(string name, string publicKey, string privateKey)
        {
            Name = name;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            UpdateCash();
        }

        /// <summary>
        /// Update amount of monty for this user
        /// </summary>
        /// <returns></returns>
        public async Task UpdateCash()
        {
            Cash = await APIWrapper.instance.GetUserBalance(PublicKey);
        }

        public async Task<List<Token>> UpdateTokens()
        {
            userTokens = await APIWrapper.instance.GetUserTokens(this);
            return userTokens;
        }
    }
}
