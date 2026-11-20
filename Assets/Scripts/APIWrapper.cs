using BlockchainDemo.Config;
using BlockchainDemo.Data;
using BlockchainDemo.Market;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;

namespace BlockchainDemo
{
    public class APIWrapper : MonoBehaviour
    {
        [SerializeField]
        Configuration config;

        public static APIWrapper instance;


        private void Start()
        {
            instance = this;
        }

        public async Task MakeTransaction(TransactionData data)
        {
            var account = new Account(data.Sender.PrivateKey);
            var contractWeb = new Web3(account, config.BaseURL);
            string abiJson = config.Contracts[data.ContractName].abi.text.Trim();
            var contract = contractWeb.Eth.GetContract(abiJson, config.Contracts[data.ContractName].address);
            var function = contract.GetFunction("buyAndTransfer");
            var transactionHash = await function.SendTransactionAsync(
                data.Sender.PublicKey,
                new HexBigInteger(300000),
                null,
                new HexBigInteger(data.Price),
                data.TokenId,
                data.ToAddress
            );
            Debug.Log("Transaction sent, hash: " + transactionHash);
        }

        public async Task<bool> ValidateTokenOwner(string contractName, BigInteger tokenId)
        {
            var web3 = new Web3(config.BaseURL);
            var service = web3.Eth.ERC721.GetContractService(config.Contracts[contractName].address);
            var owner = await service.OwnerOfQueryAsync(tokenId);
            //return owner == UserController.instance.SelectedUser?.PublicKey;
            return false;
        }

        public async Task<BigInteger> GetUserBalance(string publicKey)
        {
            var web3 = new Web3(config.BaseURL);
            var balance = await web3.Eth.GetBalance.SendRequestAsync(publicKey);
            if (balance == null)
            {
                Debug.LogError("cannot get balance");
            }
            Debug.Log(balance.Value);
            return balance.Value;
        }

        public async Task<List<Token>> GetUserTokens(User user)
        {
            var web3 = new Web3(config.BaseURL);
            var result = new List<Token>();
            var contractAddress = config.Contracts.First().Value.address;
            string abiJson = config.Contracts["first"].abi.text;
            abiJson = abiJson.Trim();
            var contract = web3.Eth.GetContract(abiJson, config.Contracts["first"].address);
            var getTokens = contract.GetFunction("tokenOfOwnerByIndex");
            var getBalance = contract.GetFunction("balanceOf");

            //get user balance in this contract
            var balance = await getBalance.CallAsync<BigInteger>(user.PublicKey);

            //
            for (int i = 0; i < balance; i++)
            {

                try
                {
                    List<object> input = new();
                    input.Add(user.PublicKey);
                    input.Add(new BigInteger(i));
                    var tokenId = await getTokens.CallAsync<BigInteger>(input.ToArray());
                    result.Add(new()
                    {
                        tokenId = tokenId,
                    });
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            return result;
        }

        public async Task MintToken(User mintingUser, BigInteger price)
        {
            if (mintingUser == null)
            {
                Debug.LogError("No user selected");
                return;
            }

            var account = new Account(mintingUser.PrivateKey, 31337);
            var contractWeb = new Web3(account, config.BaseURL);
            string abiJson = config.Contracts["first"].abi.text;
            abiJson = abiJson.Trim();
            var contract = contractWeb.Eth.GetContract(abiJson, config.Contracts["first"].address);
            var function = contract.GetFunction("mintNFT");
            try
            {
                var transactionHash = await function.SendTransactionAsync(
                    mintingUser.PublicKey,
                    new Nethereum.Hex.HexTypes.HexBigInteger(300000),
                    null,
                    null,
                    mintingUser.PublicKey,
                    price
                ).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Nethereum Error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Debug.LogError("Inner Exception: " + ex.InnerException.Message);
                }
            }
        }

        public async Task ChangeTokenPrice(User changer, BigInteger tokenId, BigInteger newPrice)
        {
            var account = new Account(changer.PrivateKey, 31337);
            var contractWeb = new Web3(account, config.BaseURL);
            string abiJson = config.Contracts["first"].abi.text;
            abiJson = abiJson.Trim();
            var contract = contractWeb.Eth.GetContract(abiJson, config.Contracts["first"].address);
            var function = contract.GetFunction("changePrice");
            var transactionHash = await function.SendTransactionAsync(
                changer.PublicKey,
                new HexBigInteger(300000),
                null,
                null,
                tokenId,
                newPrice
            );
        }

        public async Task<GetAllTokensResp> GetAllTokens()
        {
            var web3 = new Web3(config.BaseURL);
            string abiJson = config.Contracts["first"].abi.text;
            abiJson = abiJson.Trim();
            var contract = web3.Eth.GetContract(abiJson, config.Contracts["first"].address);
            var function = contract.GetFunction("getAllTokensWithPrices");
            try
            {
                var respons = await function.CallAsync<GetAllTokensResp>();
                return respons;

            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            throw new Exception();
        }
    }

    [FunctionOutput]
    public class GetAllTokensResp
    {
        public GetAllTokensResp()
        {

        }

        public GetAllTokensResp(List<BigInteger> tokens, List<BigInteger> Prices)
        {
            this.tokens = tokens;
            this.Prices = Prices;
        }

        [Parameter("uint256[]", "tokenIds",1)]
        public List<BigInteger> tokens { get; set; }
        [Parameter("uint256[]", "prices", 1)]
        public List<BigInteger> Prices { get; set; }
    }
}
