﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class characterController : MonoBehaviour
{
    public UnityEvent playerAttacked;
    public UnityEvent attackStopped;
    public UnityEvent leftflipOccurred;
    public UnityEvent rightflipOccured;
    public GameObject footBox;
    //public GameObject playerComponents;
    SpriteRenderer characterRenderer;
    Animator characterAnimator;
    Rigidbody2D characterBody;
    bool grounded;
    bool hasJumpFunction = false;
    bool hasArms = false;
    bool hasWeapon = false;
    bool isAttacking = false;
    string conveyerType = "None";
    int movementspeed = 2;
    public float terminalVelocity = -100;
    public float maxJumpSpeed = 40;
    public float maxMoveSpeed = 20;
    public float jumpHeight = 7.5f;
    Vector2 startPosition;


    void Start()
    {
        characterRenderer = GetComponent<SpriteRenderer>();
        characterAnimator = GetComponent<Animator>();
        grounded = false;
        characterBody = GetComponent<Rigidbody2D>();
        startPosition = characterBody.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //This checks all collisions, if they have the platform tag, the player is set to grounded
        if (collision.gameObject.tag == "conveyerLeft")
        {
            conveyerType = "left";
        }
        else if (collision.gameObject.tag == "conveyerRight")
        {
            conveyerType = "right";
        }
        else if (collision.gameObject.tag == "jumpEnabler")
        {
            movementspeed = 10;
            hasJumpFunction = true;
            characterAnimator.SetBool("hasLegs", true);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "arms")
        {
            hasArms = true;
            characterAnimator.SetBool("hasArms", true);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "weapon")
        {
            hasWeapon = true;
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "conveyerLeft")
        {
            conveyerType = "None";
        }
        else if (collision.gameObject.tag == "conveyerRight")
        {
            conveyerType = "None";
        }

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("f"))
        {
            if (grounded)
            {
                Debug.Log("Player Grounded");
            }
            else
            {
                Debug.Log("Player Airbourne");
            }
        }
        //If the user presses space and they are grounded the player goes up into the air.
        if (Input.GetKey("space") && grounded == true && hasJumpFunction){
            Vector2 newVelocity = new Vector2(characterBody.velocity.x, jumpHeight);
            characterBody.velocity = newVelocity;
        }

        if (Input.GetKey("left") && hasJumpFunction == true)
        {
            characterAnimator.SetBool("isWalking", true);
            characterRenderer.flipX = true;
            leftflipOccurred.Invoke();
            Vector2 leftVector;
            leftVector = new Vector2(-movementspeed, 0);
            characterBody.AddForce(leftVector);

        }

        if (Input.GetKey("right") && hasJumpFunction == true)
        {
            characterAnimator.SetBool("isWalking", true);
            characterRenderer.flipX = false;
            rightflipOccured.Invoke();
            Vector2 rightVector;
            rightVector = new Vector2(movementspeed, 0);
            characterBody.AddForce(rightVector);

        }


        //Here I set animation states

        if(grounded == false || characterBody.velocity.x == 0)
        {
            characterAnimator.SetBool("isWalking", false);
        }


        if (characterBody.velocity.y > 0 && grounded == false)
        {
            characterAnimator.SetBool("isAscending", true);
            characterAnimator.SetBool("isDecending", false);
        }
        else if(characterBody.velocity.y < 0 && grounded == false)
        {
            characterAnimator.SetBool("isAscending", false);
            characterAnimator.SetBool("isDecending", true);

        }
        else
        {
            characterAnimator.SetBool("isAscending", false);
            characterAnimator.SetBool("isDecending", false);

        }

        //The below two functions handle conveyer belts

        if (conveyerType == "left")
        {
            Vector2 leftVector;
            leftVector = new Vector2(-10, 0);
            characterBody.AddForce(leftVector);

        }

        if (conveyerType == "right")
        {
            Vector2 rightVector;
            rightVector = new Vector2(10, 0);
            characterBody.AddForce(rightVector);

        }

        //This one handles capping velocity

        if(characterBody.velocity.y < terminalVelocity)
        {
            characterBody.velocity = new Vector2(characterBody.velocity.x, terminalVelocity); 
        }

        if(characterBody.velocity.y > maxJumpSpeed)
        {
            characterBody.velocity = new Vector2(characterBody.velocity.x, maxJumpSpeed);

        }

        if(characterBody.velocity.x < -maxMoveSpeed)
        {
            characterBody.velocity = new Vector2(-maxMoveSpeed, characterBody.velocity.y);

        }

        if(characterBody.velocity.x > maxMoveSpeed)
        {
            characterBody.velocity = new Vector2(maxMoveSpeed, characterBody.velocity.y);

        }




    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && hasWeapon == true && isAttacking == false && grounded == true)
        {
            characterAnimator.SetBool("isAttacking", true);
            isAttacking = true;
            playerAttacked.Invoke();
            Invoke("unsetAttackState", 0.3f);
        }
    }
    public void setGrounded()
    {
        grounded = true;
    }
    public void setUngrounded()
    {
        grounded = false;
    }

    private void unsetAttackState()
    {
        characterAnimator.SetBool("isAttacking", false);
        isAttacking = false;
        attackStopped.Invoke();
    }

    private void die()
    {
        //Handles player death
        //Currently they are just reset to their starting position.
        SceneManager.LoadScene("botLevel");

    }
}
