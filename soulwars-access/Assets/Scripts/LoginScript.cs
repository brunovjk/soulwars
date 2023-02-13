using UnityEngine;
using Thirdweb;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script handles the login process for the game.
public class LoginScript : MonoBehaviour
{
    // Thirdweb SDK
    private ThirdwebSDK sdk;

    // Wallet
    public static Wallet wallet;
    public static WalletProvider chosenProvider;

    // Game objects
    public TextMeshProUGUI resultText;
    public GameObject connectButtons;

    // Buttons
    public Button loginButton;
    public Button createAccountButton;

    private void Start()
    {
        // Create an instance of ThirdwebSDK.
        sdk = new ThirdwebSDK("Mumbai");
        InitializeState();
    }

    private void InitializeState()
    {
        resultText.text = "WELCOME";
        // Set the initial state of the buttons.
        loginButton.gameObject.SetActive(true);
        connectButtons.SetActive(false);
        createAccountButton.gameObject.SetActive(false);
    }

    private void Update()
    {
    }

    public void OnClickLoginButton()
    {
        resultText.text = "Choose your wallet";
        // Set the login button to be inactive.
        loginButton.gameObject.SetActive(false);
        // Set the connectButtons game object to be active.
        connectButtons.SetActive(true);
    }

    public async void OnClickConnectMetamask()
    {
        resultText.text = "Connecting...";
        // Connect the user using the MetaMask wallet provider.
        await ConnectUser(WalletProvider.MetaMask);
        // Set the chosen wallet provider to MetaMask.
        chosenProvider = WalletProvider.MetaMask;
    }

    public async void OnClickConnectCoinbaseWallet()
    {
        resultText.text = "Connecting...";
        // Connect the user using the Coinbase Wallet provider.
        await ConnectUser(WalletProvider.CoinbaseWallet);
        // Set the chosen wallet provider to Coinbase Wallet.
        chosenProvider = WalletProvider.CoinbaseWallet;
    }

    public async void OnClickCreateAccount()
    {
        resultText.text = "Creating account...";
        // Create a new account for the user.
        await CreateAccount();
    }

    public async void OnClickQuit()
    {
        // Disconnect the user's wallet.
        await wallet.DisconnectWallet(sdk);
    }

    // Connect the user using the specified wallet provider.
    private async Task ConnectUser(WalletProvider provider)
    {
        var contract = sdk.GetContract("0x062a6f76c5A1078709127D7cfd4D86D2eadA7131");

        try
        {
            string address = await sdk.wallet.Connect(new WalletConnection
            {
                provider = provider,
                chainId = 80001
            });
           
            Debug.Log("Connected wallet: " + address);

            resultText.text = "Signing...";
            var data = await sdk.wallet.Authenticate("SoulWars.com");
            Debug.Log("Sig: " + data.payload.address.Substring(0, 6) + "...");

            resultText.text = "Checking if you have a token...";
            var result = await contract.ERC1155.BalanceOf(address, "0");
            Debug.Log("NFT in the wallet: " + result);

            if (int.TryParse(result, out int number) && number > 0)
            {
                resultText.text = "Account found.";
                SceneManager.LoadScene("ChooseScene");
            }
            else
            {
                resultText.text = "Please create a token.";
                connectButtons.SetActive(false);
                createAccountButton.gameObject.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            resultText.text = "Error connecting to wallet";
            Debug.LogError("Error connecting to wallet: " + e.Message);
            InitializeState();
        }
    }

    // Create a new account for the user.
    private async Task CreateAccount()
    {
        Debug.Log("Claim button clicked, start checking contract");

        var contract = sdk.GetContract("0x062a6f76c5A1078709127D7cfd4D86D2eadA7131");

        try
        {
            var result = await contract.ERC1155.Claim("0", 1);
            Debug.Log("Minted profile token: " + result);
            resultText.text = "Account created successfully!";
            SceneManager.LoadScene("ChooseScene");
        }
        catch (System.Exception e)
        {
            resultText.text = "Failed to create account";
            Debug.LogError("Failed to create account: " + e.Message);
        } 
    }
}

