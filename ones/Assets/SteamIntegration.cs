using UnityEngine;
using TMPro;

public class SteamIntegration : MonoBehaviour
{


    public string steamName = "default value";
    public TextMeshProUGUI steamTextMesh;

    void Start()
    {
        try{
            Steamworks.SteamClient.Init(480);
            updateSteamName();
        }catch (System.Exception e){
            Debug.Log("steam is broek");
            Debug.Log(e);
        }
    }



    public bool updateSteamName(){
        try{
            steamName = Steamworks.SteamClient.Name;
            saveName(steamName);
            return true;
        }catch{
            saveName("UnnamedPlayer");
            return false;
        }
    }

    private void PrintYourName(){
        Debug.Log(Steamworks.SteamClient.Name);
    }

    
    public void saveName(string _name){
        PlayerPrefs.SetString("playerName", _name);
        if(steamTextMesh){
            steamTextMesh.text = _name;
        }
    }

}
