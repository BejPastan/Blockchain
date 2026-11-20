using BlockchainDemo.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace BlockchainDemo.UI
{
    /// <summary>
    /// ScriptableObject holding bindable data for the User UI.
    /// Updating a property raises an event so the UI updates automatically.
    /// </summary>
    [CreateAssetMenu(fileName = "UserUIData", menuName = "Blockchain/UI User Data")]
    public class UserUIData : ScriptableObject
    {
        [SerializeField]
        public string _moneyText;

        [SerializeField]
        public string _walletText;


        private List<Token> _tokens;

        public List<Token> Tokens
        {
            get { return _tokens; }
            set 
            {
                _tokens = value;
                OnTokenChange?.Invoke();
            }
        }

        [SerializeField]
        public string newTokenPrice;

        public BigInteger NewTokenPrice
        {
            get { return new BigInteger(int.Parse(newTokenPrice)); }
        }

        public event Action OnTokenChange;
    }
}
