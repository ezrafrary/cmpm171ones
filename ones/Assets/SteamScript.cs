// using UnityEngine;
// using System.Collections;
// using Steamworks;
// using TMPro;

// public class SteamScript : MonoBehaviour
// {
//     public string steamName;
//     public TextMeshProUGUI steamTextMesh;
//     public float updateTimer = 10.0f;
//     private float _updateTimer = 2.0f;


//     void Start(){
//         updateSteamName();
//     }


//     void Update(){
//         if(_updateTimer > 0){
//             _updateTimer -= Time.deltaTime;
//         }else{
//             _updateTimer = updateTimer;
//             updateSteamName();
//         }
//     }


//     public void updateSteamName(){
//         try{
//             if(SteamManager.Initialized){
//                 steamName = SteamFriends.GetPersonaName();
//                 Debug.Log("steam name found: " + steamName);
//             }else{
//                 steamName = "UnnamedPlayer";
//             }
//         }catch{
//             steamName = "UnnamedPlaer";
//         }
        
//         saveName(steamName);
//     }

//     public void saveName(string _name){
//         PlayerPrefs.SetString("playerName", _name);
//         if(steamTextMesh){
//             steamTextMesh.text = _name;
//         }
//     }
// }
