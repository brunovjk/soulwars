using UnityEngine;
using Thirdweb;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class Wallet : MonoBehaviour
{
    // Store the connected wallet address
    public string connectedAddress;

    /// <summary>
    /// Re-connect the wallet using the Thirdweb SDK and specified wallet provider.
    /// </summary>
    /// <param name="sdk">The Thirdweb SDK instance.</param>
    /// <param name="provider">The wallet provider to use for the connection.</param>
    public async Task ReConnectWallet(ThirdwebSDK sdk, WalletProvider provider)
    {
        try
        {   
            // Connect to the wallet using the SDK and specified provider
            string address = await sdk.wallet.Connect(new WalletConnection
            {
                provider = provider,
                chainId = 80001 // Mumbai
            });

            // Store the connected wallet address
            connectedAddress = address;  
            Debug.Log("Connected wallet: " + connectedAddress);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error connecting wallet: " + e.Message);
        } 
    }

    /// <summary>
    /// Disconnect the wallet using the Thirdweb SDK.
    /// </summary>
    /// <param name="sdk">The Thirdweb SDK instance.</param>
    public async Task DisconnectWallet(ThirdwebSDK sdk)
    {
        try
        {
            // Disconnect the wallet using the SDK
            await sdk.wallet.Disconnect(); 
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error disconnecting wallet: " + e.Message);
        }        

        // Back to the "LoginScene"
        SceneManager.LoadScene("LoginScene");
    }
}
