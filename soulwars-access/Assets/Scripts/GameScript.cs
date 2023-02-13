using Thirdweb;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour
{
    private ThirdwebSDK sdk;
    public static Wallet wallet;
    public TextMeshProUGUI resultText;
    public Renderer wizardRenderer;
    public GameObject wizard;
    [SerializeField] private Material[] materials;
    private int chosenMaterial;

    private async void Start()
    {
        resultText.text = "Let me just put my robe...";
        wizard.SetActive(false);
        sdk = new ThirdwebSDK("Mumbai");
        await InitializeState();
    }

    private async Task InitializeState()
    {
        wizardRenderer = wizard.GetComponent<Renderer>();
        await wallet.ReConnectWallet(sdk, LoginScript.chosenProvider);
        ChangeMaterial(ChooseScript.castleUri);
        chosenMaterial = 0;
        wizard.SetActive(true);
        resultText.text = "Change Wizard`s Clothes";
    }

    public void ChangeMaterial(int _chosenMaterial)
    {
        wizardRenderer.material = materials[_chosenMaterial];
        chosenMaterial = _chosenMaterial;
    }

    public async void OnUpdateColor()
    {
        if(chosenMaterial == 0)
        {
            resultText.text = "You need to pick a color, genius.";
        } else
        {
            resultText.text = "Finally new clothes.";
            await UpdateWizardClothes(ChooseScript.choosenTokenId, chosenMaterial.ToString());
            chosenMaterial = 0;
        }
        resultText.text = "Change Wizard`s Clothes";
    }

    public async void OnClickQuit()
    {
        await wallet.DisconnectWallet(sdk);
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("ChooseScene");
    }

    public async void OnClickBurn()
    {
        await BurnCastle(ChooseScript.choosenTokenId);
        SceneManager.LoadScene("ChooseScene");
    }

    private async Task BurnCastle(int _tokenId)
    {
        var contract = sdk.GetContract("0x1E9604BbD5B55B1Aca4f6EDBCCaf074013C34D0d");

        try
        {
            var result = await contract.ERC721.Burn(_tokenId.ToString());
            Debug.Log("Token burned: " + result);
            resultText.text = "Castle Deleted";
            SceneManager.LoadScene("ChooseScene");
        }
        catch (System.Exception e)
        {
            resultText.text = "Delete Failed";
            Debug.Log("Burn Failed: " + e.Message);
        }
    }

    private async Task UpdateWizardClothes(int _tokenId, string _newURI)
    {
        var contract = sdk.GetContract("0x1E9604BbD5B55B1Aca4f6EDBCCaf074013C34D0d");

        try
        {
            await contract.Write("setNewTokenURI", _tokenId, _newURI);
            resultText.text = "Nice clothes.... for a clown.";
        }
            catch (System.Exception e)
        {
            resultText.text = "Failed to change clothes";
            Debug.Log("Failed to Burn: " + e.Message);
        }


    }
}