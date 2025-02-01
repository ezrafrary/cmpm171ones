using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchMenus : MonoBehaviour
{
    public GameObject thisScreen;
    public GameObject screenYouWantToSwitchTo;

    public void switchScreens(){
        screenYouWantToSwitchTo.SetActive(true);
        thisScreen.SetActive(false);
    }
}
