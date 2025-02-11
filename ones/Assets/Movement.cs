using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Movement : MonoBehaviour
{

    [Header("Walk/sprint")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;

    [Header("Momentum Building")]
    public float maxVelocityChange = 10f;
    public float momentumBuilder = 1f;
    private float builtMomentum = 0f;
    public float momentumBuilderCD = 10.0f;
    private float momentumBuilderCDTimer = 0;
    public float maintainMomentumWindow = 20.0f;
    public float maxBuiltJumpMomentum = 15.0f;
    public TextMeshProUGUI movementText;
    [Space]

    public float jumpHeight = 5f;


    [Space]

    [Header("dash info")]
    public Transform lookingDirection;
    public float dashStrength;
    public float maxDashYVelocity = 1.5f;
    public Image dashCooldownImage;
    public float dashCooldown;
    public float dashDuration;
    public float dashMomentumIncrease = 0.25f;
    public float dahsBoostedMomentumThreshold = 5.0f;


    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;
    private bool dashing;
    private bool jumpPressedLastFrame;
    private bool jump = false;

    private float currentDashCooldown;
    private float currentDashDurationCooldown;


    private bool grounded = false;
    private int timeGrounded = 0;



    [Header("Animations")]
    public Animation handAnimation;
    public AnimationClip handWalkAnimation;
    public AnimationClip idleAnimation;
    public PhotonView playerPhotonView;




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

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");
        dashing = Input.GetButton("Dash");

        setDashCooldownCircle();
        if(jumping){
            if(!jumpPressedLastFrame){
                jump = true;
            }
            jumpPressedLastFrame = true;
        }else{
            jumpPressedLastFrame = false;
        }
        updateMovementText();
    }

    public void updateMovementText(){
        if(movementText){
            int intVelocity = (int)new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
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
            if(playerPhotonView){
                playerPhotonView.RPC("PlayIdleAnimation",RpcTarget.All);
            }
        }

        
        if(currentDashCooldown > 0){
            currentDashCooldown--;
        }
        if(currentDashDurationCooldown > 0){
            currentDashDurationCooldown--;
        }
        if(momentumBuilderCDTimer > 0){
            momentumBuilderCDTimer--;
        }
        
        if (grounded){
            if (jump){
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                float movementBonus = 0;
                if(timeGrounded < maintainMomentumWindow){
                    movementBonus = momentumBuilder / timeGrounded;
                }
                if(momentumBuilderCDTimer <= 0){
                    if(builtMomentum < maxBuiltJumpMomentum){
                        builtMomentum = builtMomentum + (movementBonus);
                    }
                    momentumBuilderCDTimer = momentumBuilderCD;
                }


            }else if(input.magnitude > 0.5f){
                playerPhotonView.RPC("PlayWalkingAnimation",RpcTarget.All);
            }

            if(timeGrounded > maintainMomentumWindow){
                builtMomentum = 0;
            }
        }

        if(builtMomentum/2 > rb.velocity.magnitude){
            builtMomentum = 0;
        }

        rb.AddForce(CalculateMovement(sprinting ? sprintSpeed + builtMomentum : walkSpeed + builtMomentum), ForceMode.VelocityChange);

        if(dashing && currentDashCooldown < 1 && input.magnitude > 0.5f){ //can only dash if player is pressing a movment key
            currentDashCooldown = dashCooldown;
            currentDashDurationCooldown = dashDuration;
        }
        if(!grounded){
            timeGrounded = 0;
        }
        grounded = false; 
        
        jump = false;
    }

    private void OnTriggerStay(Collider other){ //handling checking if the player is grounded. we will change this later(probably)
        grounded = true;
        timeGrounded++;
    }

    //basic movment script, we can change anything in here whenever we want. this is not set in stone
    Vector3 CalculateMovement(float speed){
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y); //input is a vector2, so we are converting it to vector3
        targetVelocity = transform.TransformDirection(targetVelocity);

        

        targetVelocity *= speed;

        Vector3 velocity = rb.velocity;

        Vector3 dashingDirection = lookingDirection.forward;
        

        if(input.magnitude > 0.5f){
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = 0;

            //handle dashing
            if(dashing && currentDashDurationCooldown > 0){
                velocityChange = velocityChange + (dashingDirection * dashStrength);
                if (velocityChange.y > maxDashYVelocity){
                    velocityChange.y = maxDashYVelocity;
                }
                if(builtMomentum < dahsBoostedMomentumThreshold){
                    builtMomentum = builtMomentum + dahsBoostedMomentumThreshold/dashDuration;
                }else{
                    builtMomentum = builtMomentum + dashMomentumIncrease;
                }
            }

            if((rb.velocity + velocityChange).magnitude < velocity.magnitude - (walkSpeed + builtMomentum)){
                return new Vector3(0,0,0);
            }
            return velocityChange;
        } else {
            return new Vector3(0,0,0);
        }
    }
}
