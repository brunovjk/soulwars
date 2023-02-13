using UnityEngine;
using Thirdweb;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class ChooseScript : MonoBehaviour
{
    // Thirdweb SDK
    private ThirdwebSDK sdk;

    // Wallet
    public static Wallet wallet;

    // Game objects
    public TextMeshProUGUI resultText;
    public GameObject chooseMenu;
    public GameObject optionsMenu;

    // Buttons
    public Button firstCastleButton;
    public Button secondCastleButton;
    public Button thirdCastleButton;
    private Button[] castleButtons;
    
    public Button optionsButton;
    public Button backButton;

    // Internal variables
    public static List<int> userTokenIds = new List<int>();
    public static int choosenTokenId;
    public static int castleUri;

    private void Start()
    {
        sdk = new ThirdwebSDK("Mumbai");
        InitializeState();
    }

    private async void InitializeState()
    {
        resultText.text = "Choose Your Castle";
        chooseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        castleButtons = new Button[] { firstCastleButton, secondCastleButton, thirdCastleButton };
        optionsButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);

        await wallet.ReConnectWallet(sdk, LoginScript.chosenProvider);
        await CastleTokenBalance();

        for (int i = 0; i < userTokenIds.ToArray().Length; i++) {
        castleButtons[i].gameObject.SetActive(true);
        }
    }

    void Update()
    {
    }

    public void OnClickOptionsButton()
    {
        chooseMenu.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        optionsMenu.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void OnClickBackOptionsButton()
    {
        InitializeState();
    }

    public async void OnClickCreateCastle()
    {
        resultText.text = "Oh, you're in the market for a castle?";
        await MintCastleToken();
        InitializeState();
    }

    public async void OnClickCastle(int _castle)
    {
        if (userTokenIds.ToArray().Length > 0){
            if (_castle == 0)
            {
                choosenTokenId = userTokenIds.ToArray()[0];
            } else if (_castle == 1)
            {
                choosenTokenId = userTokenIds.ToArray()[1];
            } else if (_castle == 2)
            {
                choosenTokenId = userTokenIds.ToArray()[2];
            } else return;
            await GetIntoCastle(choosenTokenId);
        } else
        {
            Debug.LogError("userTokenIds.Length = 0");
        }
    }

    public async void OnClickQuit()
    {
        await wallet.DisconnectWallet(sdk);
    }

    public async void OnClickBurnProfileButton()
    {
        resultText.text = "Deleting account...";
        await BurnAccount();
    }

    private async Task BurnAccount()
    {
        var contract = sdk.GetContract("0x062a6f76c5A1078709127D7cfd4D86D2eadA7131");

        try
        {
            var result = await contract.ERC1155.Burn("0", 1);

            Debug.Log("Token burned: " + result);
            resultText.text = "Account deleted Successfully!";
            SceneManager.LoadScene("LoginScene");
        }
        catch (System.Exception e)
        {
            resultText.text = "Failed to delete";
            Debug.Log("Failed to Burn: " + e.Message);
        }
    }

    private async Task CastleTokenBalance()
    {
        var contract = sdk.GetContract("0x1E9604BbD5B55B1Aca4f6EDBCCaf074013C34D0d");
        try
        {
            resultText.text = "Checking terrain";
            userTokenIds.Clear();

            var result = await contract.ERC721.BalanceOf(wallet.connectedAddress);
            Debug.Log("CastleToken in the wallet: " + result);

            int number = int.Parse(result);
            if ( number > 0 )
            {
                resultText.text = "Hmph. So you have it.";
                for (int i = 0; i < number; i++)
                {
                    string tokenId = await contract.Read<string>("tokenPerUser", wallet.connectedAddress, i);
                    userTokenIds.Add(int.Parse(tokenId));
                }
                resultText.text = "Choose Your Castle";
            }
            else
            {
                resultText.text = "Visitor without a castle, how charming.";
            }
        }
        catch (System.Exception e)
        {
            resultText.text = "Error during terrain check";
            Debug.LogError("Error during terrain check: " + e.Message);
        }
    }

    private async Task GetIntoCastle(int _tokenId)
    {
        var contract = sdk.GetContract("0x1E9604BbD5B55B1Aca4f6EDBCCaf074013C34D0d");
        try
        {
            resultText.text = "Oh joy, a visitor...";

            // custom read
            string tokenUri = await contract.Read<string>("tokenURI", _tokenId);
            castleUri = int.Parse(tokenUri);
            resultText.text = "Time to put on my robes and play the gracious host";

            Debug.Log("Read custom token uri: " + tokenUri);
            SceneManager.LoadScene("GameScene");
        }
        catch (System.Exception e)
        {
            resultText.text = "Error during castle enter";
            Debug.LogError("Error during castle enter: " + e.Message);
            InitializeState();
        }
        

    }

    private async Task MintCastleToken()
    {
        var contract = sdk.GetContract("0x1E9604BbD5B55B1Aca4f6EDBCCaf074013C34D0d");

        try        {
            var result = await contract.Write("mintTo", wallet.connectedAddress, "0");
            resultText.text = "Oh, a new castle. How original.";
            Debug.Log("CastleToken minted: " + result);
            
        }
        catch (System.Exception e)
        {
            resultText.text = "Failed to mint";
            Debug.Log("Failed to Burn: " + e.Message);
        }
    }


}
