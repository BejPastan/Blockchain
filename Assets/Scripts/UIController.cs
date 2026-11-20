using BlockchainDemo.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System.Numerics;

namespace BlockchainDemo.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        PanelRenderer panelRenderer;

        [SerializeField]
        UIViewModel _viewModel;

        public static UIController instance;

        public bool IsMarketMode { get; private set; }

        public TaskCompletionSource<bool> initialized = new TaskCompletionSource<bool>();

        private DropdownField userSelect;
        public event Action<string> OnUserSelect;
        public event Action OnMarketSelect;


        private void Start()
        {
            instance = this;
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
            _viewModel.OnUsersChange += () => _ = UpdateDropdown();
            _ = UpdateDropdown();

        }

        void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            var userModeButton = rootElement.Q<Button>("userModeButton");
            var marketModeButton = rootElement.Q<Button>("marketModeButton");
            var userPanel = rootElement.Q<VisualElement>("userPanel");
            var marketPanel = rootElement.Q<VisualElement>("marketPanel");
            userSelect = rootElement.Q<DropdownField>("userSelector");
            Debug.Log($"user select={userSelect==null}");

            if (userSelect != null)
            {
                userSelect.RegisterValueChangedCallback((e) =>
                {
                    OnUserSelect?.Invoke(e.newValue);
                });
            }

            if (userModeButton != null && marketModeButton != null && userPanel != null && marketPanel != null)
            {
                userModeButton.RegisterCallback<ClickEvent>(_ =>
                {
                    userPanel.style.display = DisplayStyle.Flex;
                    marketPanel.style.display = DisplayStyle.None;
                    IsMarketMode = false;
                });

                marketModeButton.RegisterCallback<ClickEvent>(_ =>
                {
                    userPanel.style.display = DisplayStyle.None;
                    marketPanel.style.display = DisplayStyle.Flex;
                    IsMarketMode = true;
                    OnMarketSelect?.Invoke();
                });

                userPanel.style.display = DisplayStyle.Flex;
                marketPanel.style.display = DisplayStyle.None;
                IsMarketMode = false;
            }

            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
            initialized.SetResult(true);
        }
    
        private async Task UpdateDropdown()
        {
            await initialized.Task;
            userSelect.choices.Clear();
            List<string> strings = new List<string>();
            foreach (var s in _viewModel.Users)
            {
                strings.Add(s.Name);
            }
            userSelect.choices = strings;
            Debug.Log($"nums:{strings.Count}");
        }
    }
}