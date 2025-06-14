using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Movement : MonoBehaviour
{


    public Transform cameraTransform;

    public float recoildegrees = 0;
    public float horizontalRecoilDegrees = 0;
    [Header("Walk/sprint")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    public float movementAdder = 0;

    [Header("Momentum Building")]
    public float maxVelocityChange = 10f;
    
    public TextMeshProUGUI movementText;
    [Space]

    public float jumpHeight = 5f;

    [Space]

    [Header("dash info")]
    public float dashStrength;
    public float dashMovementAdder = 3;
    public float dashCooldown;
    public Image dashCooldownImage;
    public PlayerPhotonSoundManager playerPhotonSoundManager;
    


    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;
    private bool dash = false;
    private bool dashing;
    private bool jumpPressedLastFrame;
    private bool jump = false;

    private float currentDashCooldown;


    private bool grounded = false;
    private int timeGrounded = 0;



    [Header("Animations")]
    public Animation handAnimation;
    public AnimationClip handWalkAnimation;
    public AnimationClip idleAnimation;
    public PhotonView playerPhotonView;



    private Vector3 DashingVector = new Vector3(0,0,0);


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    void Update()
    {
        /*
        getting mouseinputs and movement button inputs in update rather than fixed update in order to make 
        movement feel smoother. Technically this shuould be done in fixed update, but this is a movement 
        based game and I would rather the game feel better to run around in than have the shooting be 100%
        accurate to what you see on your end.
        */

        //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        input = UserInput.instance.MoveInput;
        input.Normalize();

        //sprinting = !UserInput.instance.SprintBeingHeld;
        sprinting = true;
        jump = UserInput.instance.JumpBeingHeld;
        
        
        if(UserInput.instance.DashInput){
            dash = true;
        }

        setDashCooldownCircle();
        

        updateMovementText();
    }



    public void updateMovementText(){
        if(movementText){
            int intVelocity = (int)new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            movementText.text = intVelocity.ToString();
        }else{
            Debug.Log("no movementText");
        }
    }

    public bool getGrounded(){
        return grounded;
    }


    [PunRPC]
    public void PlayWalkingAnimation(){
        handAnimation.clip = handWalkAnimation;
        handAnimation.Play();
    }


    [PunRPC]
    public void PlayIdleAnimation(){
        handAnimation.clip = idleAnimation;
        handAnimation.Play();
    }

    public void setDashCooldownCircle(){
        dashCooldownImage.fillAmount = 1 - ((float) currentDashCooldown / dashCooldown);
    }

    


    void FixedUpdate(){
        if(input.magnitude < 0.5f){
            if(playerPhotonView != null){
                if(playerPhotonView.IsMine){
                    if(playerPhotonView.ViewID != 0){
                        if(GetComponent<Health>().health > 0){
                            playerPhotonView.RPC("PlayIdleAnimation",RpcTarget.All);
                        }
                    }
                }
            }
        }

        
        if(currentDashCooldown > 0){
            currentDashCooldown--;
        }
        
        
        if (grounded){
            if (jump){
                Jump();
            }else if(input.magnitude > 0.5f){
                playerPhotonView.RPC("PlayWalkingAnimation",RpcTarget.All);
            }
        }



        if(dash && currentDashCooldown <= 0){
            
            Dash();
            currentDashCooldown = dashCooldown;
            
        }


        if(timeGrounded > 1){
            if(sprinting){
                rb.linearVelocity += CalculateMovement(sprintSpeed + movementAdder, 1f);
            }else{
                rb.linearVelocity += CalculateMovement(walkSpeed + movementAdder/2, 1f);
            }

            if(movementAdder > 0){
                movementAdder = movementAdder - 0.1f;
            }
            
        }else{
            int intVelocity = (int)new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            if(intVelocity < 7){
                rb.linearVelocity += CalculateMovement(sprintSpeed + movementAdder, 0.6f);
            }else{
                if (sprinting){
                    rb.linearVelocity += CalculateMovement(sprintSpeed + movementAdder, 0.2f);
                }
                else{
                    rb.linearVelocity += CalculateMovement(walkSpeed + movementAdder / 2, 0.2f);
                }
            }

            
        }

        

        //slow player down
        if(input.magnitude < 0.5f){
            noMovementInputs();
        }


        if(new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude < sprintSpeed - 1){
            movementAdder = movementAdder - 1.5f;
        }
        if(movementAdder < 0){
            movementAdder = 0;
        }
        

        



        if(!grounded){
            timeGrounded = 0;
        }
        grounded = false; 
        dash = false;
        jump = false;
    }


    public void backwardsDash(float _dashStrength){
        rb.linearVelocity = (-1 * cameraTransform.forward) * (_dashStrength + rb.linearVelocity.magnitude);
    }


    private void Dash(){
        rb.linearVelocity = cameraTransform.forward * (dashStrength + rb.linearVelocity.magnitude);
        movementAdder = movementAdder + dashMovementAdder;
        playerPhotonSoundManager.playDashSound();
        
    }

    private void noMovementInputs(){
        if(rb.linearVelocity.magnitude > 0){
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.995f, rb.linearVelocity.y, rb.linearVelocity.z * 0.995f);
        }

        if(rb.linearVelocity.magnitude < 0.3){
            rb.linearVelocity = new Vector3(0,rb.linearVelocity.y,0);
        }
    }


    private void Jump(){
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
        playerPhotonSoundManager.PlayJumpSFX();
    }


    private void OnTriggerStay(Collider other){ //handling checking if the player is grounded. we will change this later(probably)
        if(other.gameObject.tag == "NotJumpable" || other.gameObject.tag == "projectile" || other.gameObject.layer == LayerMask.NameToLayer("NotJumpable")){
            //Debug.Log("not jumpable");
        }else{
            grounded = true;
            timeGrounded++;
        }
        
        
    }

    //basic movment script, we can change anything in here whenever we want. this is not set in stone
    Vector3 CalculateMovement(float speed, float maxSpeedChange){
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y); //input is a vector2, so we are converting it to vector3
        targetVelocity = transform.TransformDirection(targetVelocity);

        

        targetVelocity *= speed;

        Vector3 velocity = rb.linearVelocity;
        

        if(input.magnitude > 0.5f){
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxSpeedChange, maxSpeedChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxSpeedChange, maxSpeedChange);



            velocityChange.y = 0;

            

            
            return velocityChange;
        } else {
            return new Vector3(0,0,0);
        }
    }
}
