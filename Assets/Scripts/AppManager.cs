using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

//Moralis
using MoralisWeb3ApiSdk;
using Moralis.WebGL;
using Moralis.WebGL.Platform.Objects;

#if UNITY_WEBGL
public class AppManager : MonoBehaviour
{
    public MoralisController moralisController;
    public GameObject authButton;
    public GameObject loginScreen;
    public Text walletAddressLabel;
    private MoralisUser user;

    private async void Start()
    {
        if (moralisController != null)
        {
            await moralisController.Initialize();
        }
        else
        {
            Debug.LogError("MoralisController is null.");
            return;
        }
      
        authButton.SetActive(!MoralisInterface.IsLoggedIn());
    }

    public async UniTask LoginWithWeb3()
    {
        string userAddr = "";
        
        if (!Web3GL.IsConnected())
        {
            userAddr = await MoralisInterface.SetupWeb3();
        }
        else
        {
            userAddr = Web3GL.Account();
        }

        if (string.IsNullOrWhiteSpace(userAddr))
        {
            Debug.LogError("Could not login or fetch account from web3.");
        }
        else
        {
            string address = Web3GL.Account().ToLower();
            string appId = MoralisInterface.GetClient().ApplicationId;
            long serverTime = 0;

            // Retrieve server time from Moralis Server for message signature
            Dictionary<string, object> serverTimeResponse = await MoralisInterface.GetClient().Cloud.RunAsync<Dictionary<string, object>>("getServerTime", new Dictionary<string, object>());

            if (serverTimeResponse == null || !serverTimeResponse.ContainsKey("dateTime") ||
                !long.TryParse(serverTimeResponse["dateTime"].ToString(), out serverTime))
            {
                Debug.Log("Failed to retrieve server time from Moralis Server!");
            }

            string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

            string signature = await Web3GL.Sign(signMessage);

            Debug.Log($"Signature {signature} for {address} was returned.");

            // Create moralis auth data from message signing response.
            Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", signature }, { "data", signMessage } };

            Debug.Log("Logging in user.");

            // Attempt to login user.
            user = await MoralisInterface.LogInAsync(authData);

            await MoralisInterface.
            if (user != null)
            {
                Debug.Log($"User {user.username} logged in successfully. ");
                
                authButton.SetActive(false);
                loginScreen.SetActive(true);
                walletAddressLabel.gameObject.SetActive(true);
                walletAddressLabel.text = "Connected: " + GetWalletAddress().Substring(0,6) + "..." + GetWalletAddress().Substring(38);
            }
            else
            {
                authButton.SetActive(true);
                loginScreen.SetActive(false);
                walletAddressLabel.gameObject.SetActive(true);
                walletAddressLabel.text = "Problem when connecting to blockchain.";
                Debug.Log("User login failed.");
            }
        }
    }
    
    public string GetWalletAddress()
    {
        if (user != null)
        {
            return user.ethAddress;
        } else
        {
            return "";
        }
    }

    public async void HandleAuthButtonClick()
    {
        await LoginWithWeb3();
    }
}
#endif