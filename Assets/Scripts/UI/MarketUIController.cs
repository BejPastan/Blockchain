using BlockchainDemo.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Numerics;
using BlockchainDemo.Market;
using System.Threading.Tasks;
using Nethereum.EVM.StateChanges;

namespace BlockchainDemo.UI
{

    public class MarketUIController : MonoBehaviour
    {
        [SerializeField]
        PanelRenderer panelRenderer;

        [SerializeField]
        MarketUIData _data;

        [SerializeField]
        VisualTreeAsset _tokenContainer;

        VisualElement _marketTokensList;
        Button _buyButton;

        public MarketUIData Data => _data;

        private void Start()
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
            UIController.instance.OnMarketSelect += () => _ = RebuildMarketTokenList();
        }

        public void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            _marketTokensList = rootElement.Q<VisualElement>("marketTokensList");
            _buyButton = rootElement.Q<Button>("marketBuyButton");

            _buyButton.RegisterCallback<ClickEvent>(async _ =>
            {
                if (UserController.SelectedUser == null)
                {
                    Debug.LogError("No buyer selected. Go to Market Mode first.");
                    return;
                }
                await HandleBuy();
            });

            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        private async Task RebuildMarketTokenList()
        {
            var lists = await APIWrapper.instance.GetAllTokens();
            List<MarketTokenInfo> marketTokenInfos = new();
            for(int i=0; i < lists.Prices.Count; i++)
            {
                marketTokenInfos.Add(new MarketTokenInfo() { price = lists.Prices[i], tokenId = lists.tokens[i] });
            }

            _data.MarketTokens = marketTokenInfos;
            if (_marketTokensList == null) return;
            _marketTokensList.Clear();
            foreach (var info in _data.MarketTokens)
            {
                var element = _tokenContainer.Instantiate();
                element.Q<Label>("tokenName").text = info.tokenId.ToString();
                element.Q<Label>("tokenPrice").text = info.price.ToString();
                element.RegisterCallback<PointerDownEvent, MarketTokenInfo>(TokenPressed, info);
                _marketTokensList.Add(element);
            }
        }

        private VisualElement prevTok = null;
        private MarketTokenInfo t = null;
        private void TokenPressed(PointerDownEvent e, MarketTokenInfo t)
        {
            this.t = t;
            if (prevTok != null)
            {
                prevTok.RemoveFromClassList("activeLabel");
            }

            if (e.target is VisualElement)
            {
                ((VisualElement)e.target).AddToClassList("activeLabel");
                prevTok = (VisualElement)e.target;
            }
        }

        private async Task HandleBuy()
        {
            if(t != null && UserController.SelectedUser != null)
            {
                var data = new TransactionData()
                {
                    ContractName = "first",
                    Price = t.price * BigInteger.Pow(10, 18),
                    Sender = UserController.SelectedUser,
                    ToAddress = UserController.SelectedUser.PublicKey,
                    TokenId = t.tokenId
                };
                await APIWrapper.instance.MakeTransaction(data);
                Debug.Log("transaction succed");
            }
            else
            {
                Debug.Log("missing some property");
            }
        }
    }
}



