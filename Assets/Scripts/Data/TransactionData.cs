using System.Numerics;

namespace BlockchainDemo.Data
{
    public class TransactionData
    {
        public User Sender;
        public BigInteger TokenId;
        public string ContractName;
        public BigInteger Price;      // in Wei
        public string ToAddress;
    }

    public class MintingData
    {
        //public User Minter;
        public string ContractName;
        //public string ToAddress;
    }
}