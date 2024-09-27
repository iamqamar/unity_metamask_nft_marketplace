using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MetaMask.Cryptography;
using MetaMask.Models;
using MetaMask.SocketIOClient;
using Newtonsoft.Json;
using UnityEngine;

namespace MetaMask.Unity.Samples
{

    public class MetaMaskDemo : MonoBehaviour
    {

        #region Events

        /// <summary>Raised when the wallet is connected.</summary>
        public event EventHandler onWalletConnected;
        /// <summary>Raised when the wallet is disconnected.</summary>
        public event EventHandler onWalletDisconnected;
        /// <summary>Raised when the wallet is ready.</summary>
        public event EventHandler onWalletReady;
        /// <summary>Raised when the wallet is paused.</summary>
        public event EventHandler onWalletPaused;
        /// <summary>Raised when the user signs and sends the document.</summary>
        public event EventHandler onSignSend;
        /// <summary>Occurs when a transaction is sent.</summary>
        public event EventHandler onTransactionSent;
        /// <summary>Raised when the transaction result is received.</summary>
        /// <param name="e">The event arguments.</param>
        public event EventHandler<MetaMaskEthereumRequestResultEventArgs> onTransactionResult;

        #endregion

        #region Fields

        /// <summary>The configuration for the MetaMask client.</summary>
        [SerializeField]
        protected MetaMaskConfig config;

        [SerializeField]
        protected string ConnectAndSignMessage = "This is a test connect and sign message from MetaMask Unity SDK";

        public GameObject tokenList;
        public GameObject mainMenu;
        public GameObject nftList;
        
        private GameObject currentUI;

        #endregion

        #region Unity Methods

        /// <summary>Initializes the MetaMaskUnity instance.</summary>
        private void Awake()
        {
            this.currentUI = mainMenu;
            
            MetaMaskUnity.Instance.Initialize();
            MetaMaskUnity.Instance.Events.WalletAuthorized += walletConnected;
            MetaMaskUnity.Instance.Events.WalletDisconnected += walletDisconnected;
            MetaMaskUnity.Instance.Events.WalletReady += walletReady;
            MetaMaskUnity.Instance.Events.WalletPaused += walletPaused;
            MetaMaskUnity.Instance.Events.EthereumRequestResultReceived += TransactionResult;
        }

        private void OnDisable()
        {
            MetaMaskUnity.Instance.Events.WalletAuthorized -= walletConnected;
            MetaMaskUnity.Instance.Events.WalletDisconnected -= walletDisconnected;
            MetaMaskUnity.Instance.Events.WalletReady -= walletReady;
            MetaMaskUnity.Instance.Events.WalletPaused -= walletPaused;
            MetaMaskUnity.Instance.Events.EthereumRequestResultReceived -= TransactionResult;
        }
        
        #endregion

        #region Event Handlers

        /// <summary>Raised when the transaction result is received.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public void TransactionResult(object sender, MetaMaskEthereumRequestResultEventArgs e)
        {
            UnityThread.executeInUpdate(() => { onTransactionResult?.Invoke(sender, e); });
        }

        /// <summary>Raised when the wallet is connected.</summary>
        private void walletConnected(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() =>
            {
                onWalletConnected?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>Raised when the wallet is disconnected.</summary>
        private void walletDisconnected(object sender, EventArgs e)
        {
            if (this.currentUI != null)
            {
                this.currentUI.SetActive(false);
            }
            
            onWalletDisconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Raised when the wallet is ready.</summary>
        private void walletReady(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() =>
            {
                onWalletReady?.Invoke(this, EventArgs.Empty);
            });
        }
        
        /// <summary>Raised when the wallet is paused.</summary>
        private void walletPaused(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() => { onWalletPaused?.Invoke(this, EventArgs.Empty); });
        }

        #endregion

        #region Public Methods

        public void OpenDeepLink() {
            if (MetaMask.Transports.Unity.UI.MetaMaskUnityUITransport.DefaultInstance != null)
            {
                MetaMask.Transports.Unity.UI.MetaMaskUnityUITransport.DefaultInstance.OpenConnectionDeepLink();
            }
        }

        /// <summary>Calls the connect method to the Metamask Wallet.</summary>
        public void Connect()
        {
            MetaMaskUnity.Instance.Connect();
        }

        public void ConnectAndSign()
        {
            MetaMaskUnity.Instance.ConnectAndSign("This is a test message");
        }

        public class ChainIdObj
        {
            [JsonProperty("chainId")]
            [JsonPropertyName("chainId")]
            public string ChainId;
        }
        public async void ConnectWith()
        {
            var paramsArray = new object[] { 
                new ChainIdObj()
                {
                    ChainId = "0x89"
                }
            };

            var result = await MetaMaskUnity.Instance.ConnectWith<string>(RpcMethods.WalletSwitchEthereumChain, paramsArray);
            Debug.Log(result);
        }

        /// <summary>Sends a transaction to the Ethereum network.</summary>
        /// <param name="transactionParams">The parameters of the transaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async void SendTransaction()
        {
            var transactionParams = new MetaMaskTransaction
            {
                To = MetaMaskUnity.Instance.Wallet.SelectedAddress,
                From = MetaMaskUnity.Instance.Wallet.SelectedAddress,
                Value = "0"
            };

            await MetaMaskUnity.Instance.Wallet.SendTransaction(transactionParams);
            onTransactionSent?.Invoke(this, EventArgs.Empty);

        }

        /// <summary>Signs a message with the user's private key.</summary>
        /// <param name="msgParams">The message to sign.</param>
        /// <exception cref="InvalidOperationException">Thrown when the application isn't in foreground.</exception>
        public async void Sign()
        {
            string msgParams = "{\"domain\":{\"chainId\":1,\"name\":\"Ether Mail\",\"verifyingContract\":\"0xCcCCccccCCCCcCCCCCCcCcCccCcCCCcCcccccccC\",\"version\":\"1\"},\"message\":{\"contents\":\"Hello, Bob!\",\"from\":{\"name\":\"Cow\",\"wallets\":[\"0xCD2a3d9F938E13CD947Ec05AbC7FE734Df8DD826\",\"0xDeaDbeefdEAdbeefdEadbEEFdeadbeEFdEaDbeeF\"]},\"to\":[{\"name\":\"Bob\",\"wallets\":[\"0xbBbBBBBbbBBBbbbBbbBbbbbBBbBbbbbBbBbbBBbB\",\"0xB0BdaBea57B0BDABeA57b0bdABEA57b0BDabEa57\",\"0xB0B0b0b0b0b0B000000000000000000000000000\"]}]},\"primaryType\":\"Mail\",\"types\":{\"EIP712Domain\":[{\"name\":\"name\",\"type\":\"string\"},{\"name\":\"version\",\"type\":\"string\"},{\"name\":\"chainId\",\"type\":\"uint256\"},{\"name\":\"verifyingContract\",\"type\":\"address\"}],\"Group\":[{\"name\":\"name\",\"type\":\"string\"},{\"name\":\"members\",\"type\":\"Person[]\"}],\"Mail\":[{\"name\":\"from\",\"type\":\"Person\"},{\"name\":\"to\",\"type\":\"Person[]\"},{\"name\":\"contents\",\"type\":\"string\"}],\"Person\":[{\"name\":\"name\",\"type\":\"string\"},{\"name\":\"wallets\",\"type\":\"address[]\"}]}}";
            string from = MetaMaskUnity.Instance.Wallet.SelectedAddress;

            var paramsArray = new string[] { from, msgParams };

            var request = new MetaMaskEthereumRequest
            {
                Method = "eth_signTypedData_v4",
                Parameters = paramsArray
            };
            onSignSend?.Invoke(this, EventArgs.Empty);
            await MetaMaskUnity.Instance.Wallet.Request(request);
        }

        public async void BatchSend()
        {
            var metaMask = MetaMaskUnity.Instance.Wallet;
            
            var batch = metaMask.BatchRequests();
    
            string from = metaMask.SelectedAddress;
            List<Task<string>> requests = new List<Task<string>>();
            for (var i = 0; i < 10; i++)
            {
                var paramsArray = new string[] { Encoding.UTF8.GetBytes($"Some data {i+1}/10").ToHex(), from };
                requests.Add(batch.Request<string>("personal_sign", paramsArray));
            }

            await batch.Send();

            foreach (var task in requests)
            {
                var response = await task;
                Debug.Log(response);
            }
        }

        public void Disconnect()
        {
            MetaMaskUnity.Instance.Disconnect();
        }

        public void EndSession()
        {
            //MetaMaskUnity.Instance.Disconnect(true);
            MetaMaskUnity.Instance.EndSession();
        }

        public void ShowTokenList()
        {
            SetCurrentMenu(tokenList);
        }

        public void ShowMainMenu()
        {
            SetCurrentMenu(mainMenu);
        }

        public void ShowNftList()
        {
            SetCurrentMenu(nftList);
        }

        public void DebugLog(string message)
        {
            Debug.Log($"[MetaMaskDemo] {message}");
        }

        #endregion

        #region Private Methods

        private void SetCurrentMenu(GameObject menu)
        {
            if (this.currentUI != null)
            {
                this.currentUI.SetActive(false);
            }
            
            menu.SetActive(true);
            this.currentUI = menu;
        }
        #endregion
    }

}