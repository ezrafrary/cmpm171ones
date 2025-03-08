using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;
public class TextManagerScript : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputActionMap actionMap;
    public TMP_Text wasdtext;
    public TMP_Text shootreloadtext;
    public TMP_Text dashtext;
    // Start is called before the first frame update
    void Start()
    {
        actionMap = inputActions.FindActionMap("InGame");
        //wasdtext.text = actionMap.FindAction("Fire").ReadValue<InputControl>().displayName;
        var jumpcontrolaction = actionMap.FindAction("Jump").controls[0].displayName;
        var wasdaction = actionMap.FindAction("Move").controls[0].displayName + actionMap.FindAction("Move").controls[2].displayName + actionMap.FindAction("Move").controls[1].displayName + actionMap.FindAction("Move").controls[3].displayName;
        //var shootcontrol = shootaction.controls[0];
        //wasdtext.text = shootcontrol.displayName;
        wasdtext.text = "Press " + wasdaction + " to move and " + jumpcontrolaction + " to jump. Holding " + jumpcontrolaction + " preserves your momentum";
        shootreloadtext.text = "Press " + actionMap.FindAction("Fire").controls[0] .displayName + " to shoot and " + actionMap.FindAction("Reload").controls[0].displayName + " to reload. Swap between weapons with 1, 2, and 3.";
        dashtext.text = "Press " + actionMap.FindAction("Dash").controls[0] .displayName + " to dash in the direction you're looking. Holding " + jumpcontrolaction + " after this preserves the increased dash speed";
    }



    public string GetAction(string rootActionMap, string actionName){
        InputActionMap _actionMap;
        _actionMap = inputActions.FindActionMap(rootActionMap);
        return _actionMap.FindAction(actionName).controls[0].displayName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
