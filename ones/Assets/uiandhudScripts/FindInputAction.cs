using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;

public class FindInputAction : MonoBehaviour
{


    public InputActionAsset inputActions;

    public TextMeshProUGUI actionTextObject;
    public string keybindToGet = "";
    
    
    void Update(){
        if(actionTextObject){
            actionTextObject.text = formatActionButtons(GetAction("InGame", keybindToGet));
        }
    }
    
    
    public string GetAction(string rootActionMap, string actionName){
        InputActionMap _actionMap;
        _actionMap = inputActions.FindActionMap(rootActionMap);
        return _actionMap.FindAction(actionName).controls[0].displayName;
    }

    public string formatActionButtons(string _input){
        if(_input == "Right Button"){
            return "RMB";
        }
        if(_input == "Left Button"){
            return "LMB";
        }
        return _input;
    }



}
