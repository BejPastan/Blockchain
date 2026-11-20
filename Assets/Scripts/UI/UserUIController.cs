using BlockchainDemo.Data;
using BlockchainDemo.Market;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace BlockchainDemo.UI
{
    /// <summary>
    /// Controls the User UI panel. Binds UserUIData to the UXML and handles
    /// user/token selection and mint / price change button events.
    /// </summary>
    public class UserUIController : MonoBehaviour
    {
        [SerializeField]
        PanelRenderer panelRenderer;

        [SerializeField]
        UserUIData _data;

        [SerializeField]
        VisualTreeAsset _tokenContainer;

        public event Action<Token> OnTokenPressed;
        public event Action<Token> OnChangePricePressed;
        public event Action OnMintPressed;

        VisualElement _tokensList;
        Button _mintButton;
        Button _changePriceButton;

        private void Start()
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
            _data.OnTokenChange += () => { _ = RebuildTokenList(); };
        }

        void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            _tokensList = root.Q<VisualElement>("tokensList");

            _mintButton = root.Q<Button>("mintButton");
            _changePriceButton = root.Q<Button>("changePrice");

            _mintButton.RegisterCallback<ClickEvent>((e) => _ = APIWrapper.instance.MintToken(UserController.SelectedUser, _data.NewTokenPrice));
            _changePriceButton.RegisterCallback<ClickEvent>(ChangePriceHandler);

            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        private async Task RebuildTokenList()
        {
            //_data.Tokens = await APIWrapper.instance.GetUserTokens(UserController.SelectedUser);
            if (_tokensList == null) return;
            _tokensList.Clear();
            foreach (var token in _data.Tokens)
            {
                var element = _tokenContainer.Instantiate();
                element.Q<Label>("tokenName").text = token.tokenId.ToString();
                element.RegisterCallback<PointerDownEvent, Token>(TokenPressed, token);
                _tokensList.Add(element);
            }
        }


        private VisualElement prevTok = null;
        private Token t = null;
        private void TokenPressed(PointerDownEvent e, Token t)
        {
            this.t = t;
            if(prevTok != null)
            {
                prevTok.RemoveFromClassList("activeLabel");
            }

            if (e.target is VisualElement)
            {
                ((VisualElement)e.target).AddToClassList("activeLabel");
                prevTok = (VisualElement)e.target;
            }
            OnTokenPressed?.Invoke(t);
        }
    
        private void ChangePriceHandler(ClickEvent e)
        {
            if(t != null)
            {
                _ = APIWrapper.instance.ChangeTokenPrice(UserController.SelectedUser, t.tokenId, new BigInteger(int.Parse(_data.newTokenPrice)));
            }
        }
    }
}

