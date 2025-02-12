using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    public Vector2 MoveInput {get; private set;}

    public bool JumpJustPressed { get; private set;}
    public bool JumpBeingHeld { get; private set;}
    public bool JumpReleased {get; private set;}

    public bool AttackInput { get; private set; }
    public bool AttackHeld {get; private set;}
    
    public bool DashInput {get; private set;}
    public bool DashBeingHeld {get; private set;}

    public bool MenuOpenInput {get; private set;}

    public bool SprintJustPressed {get; private set;}
    public bool SprintBeingHeld {get; private set;}

    public bool ReloadJustPressed {get; private set;}


    private PlayerInput _playerInput;


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _dashAction;
    private InputAction _menuOpenAction;
    private InputAction _sprintAction;
    private InputAction _reloadAction;


    private void Awake(){
        if(instance == null){
            instance = this;
        }

        _playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    private void Update(){
        UpdateInputs();
    }

    private void SetupInputActions(){
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _attackAction = _playerInput.actions["Fire"];
        _dashAction = _playerInput.actions["Dash"];
        _menuOpenAction = _playerInput.actions["Escape"];
        _sprintAction = _playerInput.actions["Sprint"];
        _reloadAction = _playerInput.actions["Reload"];
    }

    private void UpdateInputs(){
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        JumpReleased = _jumpAction.WasReleasedThisFrame();
        AttackInput = _attackAction.WasPressedThisFrame();
        AttackHeld = _attackAction.IsPressed();
        DashInput = _dashAction.WasPressedThisFrame();
        DashBeingHeld = _dashAction.IsPressed();
        MenuOpenInput = _menuOpenAction.WasPressedThisFrame();
        SprintJustPressed = _sprintAction.WasPressedThisFrame();
        SprintBeingHeld = _sprintAction.IsPressed();
        ReloadJustPressed = _reloadAction.WasPressedThisFrame();
    }


}
