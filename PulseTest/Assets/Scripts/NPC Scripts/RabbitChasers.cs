﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI; may need this in the future??

public class RabbitChasers : NPCs
{
    private bool wantToFeed;
    protected override void Awake()
    {       
        base.Awake();

        wantToFeed = true;
        int ranX = Random.Range((int)Playground.LeftX, (int)Playground.RightX);
        int ranY = Random.Range((int)Playground.LowerY, (int)Playground.UpperY);
        /*
        scale = transform.localScale;
        scaleOpposite = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        */
        scale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        scaleOpposite = transform.localScale;
        target = new Vector3(ranX, ranY, -1);
    }

    protected override void Update()
    {
        //isWalking = anim.GetBool("IsWalking");
        if (schoolBell == false)
        {
            time = Time.fixedUnscaledTime;
            bool rabbitDist = checkDist(NpcInstantiator.rabbitPos, transform.position);
            directionCheck(target.x, transform.position.x);
            avatarChecks();
            checkBallBunny(rabbitDist, NpcInstantiator.rabbitPos);
            DetectMovement();
            //if (Input.GetKeyDown(Control.evacuate) && !MentalState.journalInProgress)
            //{
            //    schoolBell = true;
            //}
        }
        else
        {
            toClass();
            anim.SetBool("wantToFeed", false);
            anim.SetBool("isFeeding", false);
        }
    }
    protected override void checkBallBunny(bool inDist, Vector3 avatarPos)
    {
        if (inDist)
        {
            float dist = Vector3.Distance(avatarPos, transform.position);
            //target = avatarPos;
            if (dist > 10.0f)
            {
                target = avatarPos;
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

                // if the rabbit is not carried by anyone at the moment, rabbit girl want to feed 
                anim.SetBool("wantToFeed", false);
                anim.SetBool("isFeeding", false);


            }
            else
            {
                if (!RabbitJump.beingCarried && wantToFeed)
                {
                    anim.SetBool("wantToFeed", true);
                    wantToFeed = false;
                    Invoke("ResetFeedingTime", 10f);

                }


            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (transform.position == target)
            {
                int ranX = Random.Range((int)Playground.LeftX, (int)Playground.RightX);
                int ranY = Random.Range((int)Playground.LowerY, (int)Playground.UpperY);
                target = new Vector3(ranX, ranY, -1);
            }
        }
    }

    protected override void checkRabbitBit()                   
    {
        if (rabNameChange)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                if (transform.GetChild(i).gameObject.tag != "Avatars" && holdBunny == false)
                {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }
            timer = time + 2.0f;

            // if she is not trying to feed and the rabbit bite her, she'll be upset too 
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("feeding"))
            {
                anim.SetBool("isFeeding", true);
                Emo = master.GetComponent<NpcInstantiator>().happyFace;
            }
            else
            {
                anim.SetBool("isBit", true);
                Emo = master.GetComponent<NpcInstantiator>().madFace;
            }
            addEmo();

        }
    } 

    protected override void checkRabbitCarry()
    {
        if (RabbitJump.beingCarried)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                if (transform.GetChild(i).gameObject.tag == "Avatars" && holdBunny == false)
                {
                    holdBunny = true;
                    Emo = master.GetComponent<NpcInstantiator>().surpriseFace;
                    addEmo();
                    if (Time.time >= rabbitTime)
                    {
                        rabbitTime = Time.time + rabbitCoolDown;
                        addQueue(5);
                    }
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (transform.position == target)
            {
                int ranX = Random.Range((int)Playground.LeftX, (int)Playground.RightX);
                int ranY = Random.Range((int)Playground.LowerY, (int)Playground.UpperY);
                target = new Vector3(ranX, ranY, -1);
            }
            /*
            if (rabNameChange)
            {
                int count = transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    if (transform.GetChild(i).gameObject.tag != "Avatars" && holdBunny == false)
                    {
                        GameObject.Destroy(transform.GetChild(i).gameObject);
                    }
                }
                timer = time + 2.0f;
                Emo = master.GetComponent<NpcInstantiator>().sadFace;
                addEmo();
            }
            */

        }
    }

    // Rabbit girl will feed again after 10 seconds
    private void ResetFeedingTime()
    {
        wantToFeed = true;
    }


}
