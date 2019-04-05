﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RabbitJump : MonoBehaviour
{
    public static bool beingCarried = false;
    public static bool bittenMC = false;
    public static string bitNpcName = "";
    public LayerMask Carriers;
    public float actionDist;
    public float biteTimer;
    private float coolTime;
    private Rigidbody2D rb;
    private double currentPosX;
    private double lastPosX;

    public Animator anim;
    public AudioSource ass;
   
    // Start is called before the first frame update
    void Start()
    {
        lastPosX = transform.position.x;
        actionDist = 4f;
        biteTimer = 2f;
        coolTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //DetectMovement();
        JumpIntoArms();
    }

    public void JumpIntoArms()
    {
        //Swap a flag
        if (bittenMC)
        {
            bittenMC = !bittenMC;
        }

        
        if (Input.GetKeyDown(Control.positiveAction))
        {
            //If already being carried, put the rabbit down
            if (beingCarried)
            {
                PutRabbitDown();
            }
            else
            {
                //Get array of colliders in OverlapCircle
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, actionDist, Carriers);

                //Check if array is empty or if there was anything collided with
                if(colliders.Length != 0)
                {
                    Array.Reverse(colliders);

                    //Run through each item and check collisions. MC will always be first
                    //because it is sorted in increasing order of Z coordinate. 
                    foreach (Collider2D coll in colliders)
                    {
                        if (coll.gameObject.tag == "MC")
                        {
                            PickRabbitUp(coll.gameObject);
                            
                            InvokeRepeating("RabbitHappiness", 0f, 3f);
                            break;
                        }else if (coll.gameObject.tag == "Person")
                        {
                            PickRabbitUp(coll.gameObject);
                            break;
                        }
                    }
                }
            } 

        }else if (Input.GetKeyDown(Control.negativeAction))
        {
            if(Time.time >= coolTime)
            {
                coolTime = Time.time + biteTimer;
                //Rabbit bite code!
                //Send out circle cast to see who's around to munch on
                //RaycastHit2D biteCheck = Physics2D.CircleCast(transform.position, actionDist, Vector2.zero);
                Collider2D[] theBitten = Physics2D.OverlapCircleAll(transform.position, actionDist, Carriers);

                if (theBitten.Length != 0)
                {
                    Array.Reverse(theBitten);

                    //Run through each item and check collisions. MC will always be first
                    //because it is sorted in increasing order of Z coordinate. 
                    foreach (Collider2D victim in theBitten)
                    {
                        if (victim.gameObject.tag == "MC")
                        {
                            bittenMC = true;
                            MentalState.sendMsg("Bit by rabbit");
                            ass.Play();
                            PutRabbitDown();
                        }
                        else if (victim.gameObject.tag == "Person")
                        {
                            Debug.Log("I bit " + victim.gameObject.name + "!");
                            bitNpcName = victim.gameObject.name;
                            ass.Play();
                        }
                        break;
                    }
                }
            }
        }
    }

    private void RabbitHappiness()
    {
        MentalState.sendMsg("Held Rabbit");
    }

    public void PutRabbitDown()
    {
        CancelInvoke("RabbitHappiness");
        transform.parent = null;
        GetComponent<Movement>().enabled = true;
        beingCarried = false;
        GetComponent<SortRender>().offset = 12;
        anim.SetBool("isCarried", false);
    }

    public void PickRabbitUp(GameObject carrier)
    {
        beingCarried = true;
        anim.SetBool("isCarried", true);
        transform.position = new Vector3(carrier.transform.position.x + 0.1f, carrier.transform.position.y, -1);
        transform.parent = carrier.transform;
        GetComponent<Movement>().enabled = false;
        GetComponent<SortRender>().offset = 0;
    }
}
