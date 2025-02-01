using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMenuButton : MonoBehaviour
{
    public GameObject thisCanvas;
    public GameObject menuCanvas;

    public void backToMenu(){
        thisCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }
}
