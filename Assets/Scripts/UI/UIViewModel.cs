using BlockchainDemo.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockchainDemo.UI
{
    /// <summary>
    /// View model for the main UI. Binding a property updates the UI automatically
    /// through events subscribed by <see cref="UIController"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "UIViewModel", menuName = "Blockchain/UI View Model")]
    public class UIViewModel : ScriptableObject
    {
        [SerializeField]
        public string selectedUserName = "";

        private List<User> users;

        public List<User> Users
        {
            get { return users; }
            set
            {
                OnUsersChange?.Invoke();
                users = value;
            }
        }

        public event Action OnUsersChange;
    }
}
