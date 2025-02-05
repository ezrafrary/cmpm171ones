using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Movement : MonoBehaviour
{

    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;
    [Space]
    public float jumpHeight = 5f;


    [Space]
    public Transform lookingDirection;

    public float dashStrength;
    public float maxDashYVelocity = 1.5f;
    public Image dashCooldownImage;
    public float speedreductionStrength = 2;


    [Header("in game ticks")]
    public float dashCooldown;
    public float dashDuration;


    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;
    private bool dashing;
    
    
    private float currentDashCooldown;
    private float currentDashDurationCooldown;


    private bool grounded = false;


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
        dashCooldownImage.fillAmount = (float) currentDashCooldown / dashCooldown;
    }

    void FixedUpdate(){
        if(input.magnitude < 0.5f){
            
            playerPhotonView.RPC("PlayIdleAnimation",RpcTarget.All);
        }

        
        if(currentDashCooldown > 0){
            currentDashCooldown--;
        }
        if(currentDashDurationCooldown > 0){
            currentDashDurationCooldown--;
        }
        
        if (grounded){
            if (jumping){
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            }else if(input.magnitude > 0.5f){
                playerPhotonView.RPC("PlayWalkingAnimation",RpcTarget.All);
            }else{
                
            }
        }
        rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);

        if(dashing && currentDashCooldown < 1 && input.magnitude > 0.5f){ //can only dash if player is pressing a movment key
            currentDashCooldown = dashCooldown;
            currentDashDurationCooldown = dashDuration;
        }
        grounded = false; 
    }

    private void OnTriggerStay(Collider other){ //handling checking if the player is grounded. we will change this later(probably)
        grounded = true;
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
            if(dashing && currentDashDurationCooldown > 0){
                velocityChange = velocityChange + (dashingDirection * dashStrength);
                if (velocityChange.y > maxDashYVelocity){
                    velocityChange.y = maxDashYVelocity;
                }
            }
            return velocityChange;
        } else {
            if(grounded){ 
                rb.velocity = new Vector3(rb.velocity.x/speedreductionStrength, rb.velocity.y,rb.velocity.z/speedreductionStrength);
            }
            return new Vector3(0,0,0);
        }
    }
}
