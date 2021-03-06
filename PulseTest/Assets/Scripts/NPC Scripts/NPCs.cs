﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCs : MonoBehaviour
{
    public Animator anim;
    protected float speed;// = Random.Range(3f, 5f);

    protected Vector3 scale;
    protected Vector3 scaleOpposite;

    private float currentPosX;
    private float lastPosX;

    private float musicCoolDown;
    private float curTime;
    protected float rabbitCoolDown;
    protected float rabbitTime;

    protected GameObject master;
    protected GameObject Emo;
    public static Queue<int> actions; //a queue of integers as Ball: 1, 2; Music: 3, 4; Rabbit: 5, 6 (bigger number is the negative)

    private int music;
    private int check;

    private SpriteRenderer sr;

    protected bool holdBunny = false;
    protected bool nameChange = false;
    protected bool rabNameChange = false;
    public bool isAllergic = false;
    public static bool sneeze = false;
    //public bool isWalking = false;
    public static bool schoolBell = false;
    public static bool playScreamSound = false;
    public Vector3 target;

    protected float time;
    protected float timer;

    public float stopTime;
    public float stopTimer;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim.SetBool("IsWalking", true);
        //Playground.RandomizeNpcAssets(anim, sr, gameObject);
        master = GameObject.Find("GameController");
        scale = transform.localScale;
        scaleOpposite = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        music = MusicKidBT.currentMood;
        check = music;
        time = Time.fixedUnscaledTime;
        timer = time;
        stopTime = 0f;
        stopTimer = 1.2f;
        speed = Random.Range(3f, 6f);
        Debug.Log("speed: " + speed);
        actions = new Queue<int> { };
        musicCoolDown = 4f;
        curTime = 0f;
        rabbitCoolDown = 4f;
        rabbitTime = 0f;
    }



    protected virtual void Update()
    {
        //isWalking = anim.GetBool("IsWalking");
        if (schoolBell == false)
        {
            time = Time.fixedUnscaledTime;
            directionCheck(target.x, transform.position.x);
            avatarChecks();
            DetectMovement();
            //if (Input.GetKeyDown(Control.evacuate) && !MentalState.journalInProgress) 
            //{ 
            //    schoolBell = true;
            //}
        }
        else
        {
            target = master.GetComponent<NpcInstantiator>().rightBound.transform.position;
            directionCheck(target.x, transform.position.x);
            runOff();
            //DetectMovement();
            if (transform.position == target)
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void avatarChecks()
    {
        bool emoDist = checkDist(NpcInstantiator.musicKidPos, transform.position);
        check = music;
        music = MusicKidBT.currentMood;
        //if (music != check || (emoDist && MusicKidBT.isMusic))
        if (this.gameObject.name == MusicKidBT.musicListener)
        {
            checkMusic();
        }
        if (this.gameObject.name == BallProjectile.NpcName && BallProjectile.meanBallThrown)
        {
            Emo = master.GetComponent<NpcInstantiator>().madFace;
            addEmo();
            //addQueue(2);
        }
        if (BallProjectile.NpcName == this.gameObject.name)
        {
            BallProjectile.NpcName = "";
            nameChange = true;
            playBall();
            //addQueue(1);
        }
        if (RabbitJump.bitNpcName == this.gameObject.name)
        {
            Debug.Log("this guy got bit lmaooo: " + RabbitJump.bitNpcName);
            RabbitJump.bitNpcName = "";
            rabNameChange = true;
            checkRabbitBit();
            if (this.gameObject.name.Contains("RabbitChaser"))
            {
                addQueue(5);
            }
            else
            {
                addQueue(6);
            }
            
        }
        checkBools(emoDist);
        checkRabbitCarry();      
    }

    protected virtual void checkBools(bool emoDist)
    {
        //if ((characterSwitcher.isMusicGuyInCharge == false && RabbitJump.beingCarried == false) || emoDist == false)
        if ((MusicKidBT.musicListener != this.gameObject.name && RabbitJump.beingCarried == false) || emoDist == false)
        {
            if (nameChange == false && rabNameChange == false)
            {
                holdBunny = false;
                int count = transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    if (transform.GetChild(i).gameObject.tag != "Avatars" && holdBunny == false)
                    {
                        GameObject.Destroy(transform.GetChild(i).gameObject);
                    }
                }
            }
        }
        if (timer <= time)
        {
            nameChange = false;
            rabNameChange = false;
        }
    }

    protected virtual void checkRabbitCarry()
    {
        if (RabbitJump.beingCarried)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                if (transform.GetChild(i).gameObject.tag == "Avatars" && holdBunny == false)
                {
                    holdBunny = true;

                    if (isAllergic)
                    {
                        Emo = master.GetComponent<NpcInstantiator>().hurtFace;
                        sneeze = true;
                        //addSneeze();
                        Debug.Log("Achoo!");
                    }
                    else
                    {
                        Emo = master.GetComponent<NpcInstantiator>().surpriseFace;
                    }
                    addEmo();
                    if (Time.time >= rabbitTime)
                    {
                        rabbitTime = Time.time + rabbitCoolDown;
                        if (isAllergic)
                        {
                            addQueue(6);
                        }
                        else
                        {
                            addQueue(5);
                        }
                    }
                }
            }
        }
    }

    protected virtual void checkRabbitBit()              
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
            playScreamSound = true;
            Emo = master.GetComponent<NpcInstantiator>().hurtFace;
            addEmo();
            anim.SetTrigger("isBit");
        }
    }

    protected virtual void checkMusic()
    {
        if (MusicKidBT.currentMood == 1)                      //sad song
        {
            Emo = master.GetComponent<NpcInstantiator>().sadFace;
            if (Time.time >= curTime)
            {
                addQueue(4);
                curTime = Time.time + musicCoolDown;
            }
        }
        else if (MusicKidBT.currentMood == 0)                 //happy song
        {
            Emo = master.GetComponent<NpcInstantiator>().groovinFace;
            if (Time.time >= curTime)
            {
                Debug.Log("I'm happy");
                addQueue(3);
                curTime = Time.time + musicCoolDown;
            }
        }
        addEmo();


    }

    protected virtual void directionCheck(float target1, float pos)
    {
        if (target1 >= 0)
        {
            if (pos >= 0)
            {
                if (target1 >= pos) { transform.localScale = scale; }
                else if (target1 <= pos) { transform.localScale = scaleOpposite; }
            }
            else if (pos <= 0) { transform.localScale = scale; }
        }
        else if (target1 <= 0)
        {
            if (pos >= 0) { transform.localScale = scaleOpposite; }
            else if (pos <= 0)
            {
                if (target1 >= pos) { transform.localScale = scale; }
                else if (target1 < pos) { transform.localScale = scaleOpposite; }
            }
        }
    }

    protected virtual void addEmo()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            if (transform.GetChild(i).gameObject.tag != "Avatars")
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
        Vector3 offset = new Vector3(0, 4.5f, 0);
        GameObject balloon = Instantiate(Emo, transform.localPosition + offset, transform.rotation);
        balloon.GetComponent<SpriteRenderer>().sortingLayerName = "Front Props";
        balloon.transform.parent = transform;
    }

    protected virtual bool checkDist(Vector3 pos1, Vector3 pos2)  //for AOE
    {
        float dist = Vector3.Distance(pos1, pos2);
        if (dist <= MusicKidBT.actionDist && characterSwitcher.isMusicGuyInCharge) { return true; }
        else if (dist <= 20.0f) { return true; }
        return false;
    }

    protected virtual void DetectMovement()
    {
        currentPosX = transform.position.x;
        if (currentPosX != lastPosX)
        {
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

        lastPosX = transform.position.x;
    }

    protected virtual void playBall()
    {
        if (nameChange)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                if (transform.GetChild(i).gameObject.tag != "Avatars" && holdBunny == false)
                {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }
            if (BallProjectile.meanBallThrown)
            {
                //ScreamSounds.playScream();
                playScreamSound = true;
                Debug.Log("ouch");
                anim.SetTrigger("isHit");
                timer = time + 2.0f;
                Emo = master.GetComponent<NpcInstantiator>().madFace;
                addQueue(2);
                addEmo();
                BallProjectile.meanBallThrown = false;
            }
            else if (BallProjectile.meanBallThrown == false)
            {
                anim.SetTrigger("playCatch");
                addQueue(1); 
            }
        }
    }

    protected virtual void checkBallBunny(bool inDist, Vector3 avatarPos)
    {
        if (inDist)
        {
            float dist = Vector3.Distance(avatarPos, transform.position);
            //target = avatarPos;
            if (dist > 10.0f)
            {
                target = avatarPos;
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
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

    protected virtual void runOff()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    protected virtual void toClass()
    {
        target = master.GetComponent<NpcInstantiator>().rightBound.transform.position;
        directionCheck(target.x, transform.position.x);
        runOff();
        DetectMovement();
        if (transform.position == target)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void addQueue(int num)
    {
        actions.Enqueue(num);
        Debug.Log(num);
        if (actions.Count > 5)
        {
            actions.Dequeue();
        }

        MentalState.UpdateNPCMood(num);
    }
}
