using BlockchainDemo.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BlockchainDemo.Config
{
    [CreateAssetMenu(fileName = "Configuration", menuName = "Scriptable Objects/Configuration")]
    public class Configuration : ScriptableObject
    {
        [SerializeField]
        string baseUrl;

        public string BaseURL { get { return baseUrl; }}

        [SerializeField]
        WorkMode workMode = WorkMode.localNode;

        [SerializeField]
        List<DictionarySerialization<string, ContractData>> contracts;

        public Dictionary<string, ContractData> Contracts { get;  private set; } = new();

        private void OnEnable()
        {
            foreach(var contract in contracts)
            {
                Contracts.Add(contract.Key, contract.Value);
            }
            //contracts = null;

            if(workMode == WorkMode.sandbox)
            {
                Debug.LogWarning("sandbox mode is not implemented, switching to localNode mode");
                workMode = WorkMode.localNode;
            }

            if (workMode == WorkMode.localNode)
            {
                _= StartLocalNode();
            }
        }

        private async Task<bool> StartLocalNode()
        {
            throw new NotImplementedException();
        }
    }

    enum WorkMode
    {
        localNode,
        sandbox
    }

    [System.Serializable]
    public class ContractData
    {
        public string address;
        public TextAsset abi;
    }
}