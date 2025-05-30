using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using System;
using Photon.Pun.UtilityScripts;


public class Leaderboard : MonoBehaviour
{
    public GameObject playersHolder;

    [Header("Options")]
    public float refreshRate = 1f; //how often the leaderboard updates, in seconds. 1 second is enough, we dont need this constantly updating
    public bool isToggleableWithTab;

    [Header("UI")]
    public GameObject[] slots;
    [Space]
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] kdTexts;

    public TextMeshProUGUI winnerGUI;

    private int maxScore;
    

    public bool updateLeaderboard = true;

    private void Start(){
        InvokeRepeating(nameof(Refresh), 1f, refreshRate); //this line is straignt from bananadev, idk how it works but it does
    }

    void OnEnable(){
        Refresh();
    }

    public void SetWinner(string PlayerName){
        if(winnerGUI){
            winnerGUI.text = PlayerName + " Wins!";
        }
    }


    public List<Photon.Realtime.Player> printLeaderboard(){
        var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.CustomProperties["score"] descending select player).ToList();
        foreach (var player in sortedPlayerList){
            Debug.Log(player.NickName);
        }
        return sortedPlayerList;
    }


    public void Refresh(){
        if(updateLeaderboard){
            foreach (var slot in slots){
                slot.SetActive(false);
            }

            var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.CustomProperties["score"] descending select player).ToList();


            int i = 0;
            foreach (var player in sortedPlayerList){
                slots[i].SetActive(true);

                if (player.NickName == ""){
                    player.NickName = "unnamed";
                }

                nameTexts[i].text = player.NickName;

                if(player.CustomProperties["score"] != null){
                    scoreTexts[i].text = player.CustomProperties["score"].ToString();
                }else{
                    scoreTexts[i].text = "0";
                }

                if (player.CustomProperties["kills"] != null){
                    kdTexts[i].text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"];
                }else{
                    kdTexts[i].text = "0/0";
                }

                i++;
            }
            if(sortedPlayerList.Count > 0){
                SetWinner(sortedPlayerList[0].NickName);
            }
        }       
    }

    private void Update(){
        if(isToggleableWithTab){  
            playersHolder.SetActive(UserInput.instance.ScoreboardHeld);
        }
    }
}
