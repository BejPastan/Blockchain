using BlockchainDemo.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace BlockchainDemo.UI
{
    /// <summary>
    /// Single token entry with price for the marketplace UI.
    /// </summary>
    [System.Serializable]
    public class MarketTokenInfo
    {
        public BigInteger tokenId;
        public BigInteger price;
    }

    /// <summary>
    /// ScriptableObject holding bindable data for the Market UI.
    /// Updating a property raises an event so the UI updates automatically.
    /// </summary>
    [CreateAssetMenu(fileName = "MarketUIData", menuName = "Blockchain/UI Market Data")]
    public class MarketUIData : ScriptableObject
    {
        [SerializeField]
        private List<MarketTokenInfo> _marketTokens = new();
        public List<MarketTokenInfo> MarketTokens
        {
            get => _marketTokens;
            set
            {
                if (_marketTokens == value) return;
                _marketTokens = value;
                OnMarketTokensChanged?.Invoke(_marketTokens);
            }
        }

        public event Action<List<MarketTokenInfo>> OnMarketTokensChanged;
    }
}
