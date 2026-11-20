using BlockchainDemo.Data;
using BlockchainDemo.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Numerics;

namespace BlockchainDemo.Market
{
    public class UserController : MonoBehaviour
    {
        [SerializeField]
        TextAsset _usersCSV;

        [SerializeField]
        UIViewModel _uiViewModel;

        [SerializeField]
        public List<User> users = new();

        [SerializeField]
        public UserUIData _userData;


        /// <summary>
        /// Currently selected user, set when a user is pressed in the UI.
        /// MarketController reads this instead of holding its own copy.
        /// </summary>
        public static User SelectedUser { get; private set; }

        private void Start()
        {
            InitilizeUsers();
        }

        private void InitilizeUsers()
        {
            string[] rows = _usersCSV.text.Split("\r\n");
            for (int i=1; i<rows.Length; i++)
            {
                string[] data = rows[i].Split(",");
                users.Add(new User(data[0], data[1], data[2]));
            }

            UIController.instance.OnUserSelect += (u) => _ = OnUserChange(u);
            Debug.Log(users.Count);
            _uiViewModel.Users = users;
        }

        private async Task OnUserChange(string userName)
        {
            for(int i=0; i<users.Count; i++)
            {
                if(users[i].Name==userName)
                {
                    SelectedUser = users[i];
                    await SelectedUser.UpdateCash();
                    await SelectedUser.UpdateTokens();
                    _uiViewModel.selectedUserName = userName;
                    _userData.name = userName;
                    _userData._moneyText = (SelectedUser.Cash/BigInteger.Pow(10, 18)).ToString();
                    _userData._walletText = SelectedUser.PublicKey;
                    _userData.Tokens = SelectedUser.userTokens;
                    return;
                }
            }
        }
    }
}