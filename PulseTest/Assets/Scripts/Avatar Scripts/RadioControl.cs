﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioControl : MonoBehaviour
{
    
    public static int currentMood;
    public static string musicListener = "";
    public static bool mcIsAffected = false;
    public static bool npcIsAffected = false;
    public static bool isMusic = false;
    public static float actionDist;

    private bool musicNoteCreated;
    private bool emitParticleNow;
    private bool isBG;
    private bool foundTheMC;

    private readonly float mcAffectedInterval = 3f;

    public LayerMask Carriers;
    public ParticleSystem ps;
   
    private SpriteRenderer sr;
    private enum Mood { happy, sad, idle};
    [SerializeField] private AudioClip sadSong;
    [SerializeField] private AudioClip happySong;
    [SerializeField] private GameObject happyMusicNote;
    [SerializeField] private GameObject sadMusicNote;
    private AudioSource audioSource;
    private AudioSource backgroundMusic;

    public Sprite happy;
    public Sprite sad;
    public Sprite idle;

    public Sprite happyNote1;
    public Sprite happyNote2;
    public Sprite happyNote3;

    public Sprite sadNote1;
    public Sprite sadNote2;
    public Sprite sadNote3;

    public Material happyNoteMat;
    public Material sadNoteMat;

    Sprite[] sprites;
    Sprite[] happyNoteSprites;
    Sprite[] sadNoteSprites;
    AudioClip[] audioClips;

    private Animator anim;
    public Animator mc_anim;

    private ParticleSystem.EmissionModule em;

    private void Awake()
    {
        emitParticleNow = false;
        musicNoteCreated = false;
    }

    private void Start()
    {
        sprites = new Sprite[] { happy, sad, idle};
        happyNoteSprites = new Sprite[] { happyNote1, happyNote2, happyNote3 };
        sadNoteSprites = new Sprite[] { sadNote1, sadNote2, sadNote3 };
        audioClips = new AudioClip[] { happySong, sadSong };
        currentMood = (int) Mood.idle;

        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        backgroundMusic = GameObject.Find("/GameController").GetComponent<AudioSource>();

        isBG = true;
        actionDist = 4f;

        anim = GetComponent<Animator>();
        mc_anim = GameObject.Find("MC").GetComponent<Animator>();

        em = ps.emission;
        em.enabled = false;


    }

    // Update is called once per frame
    private void Update()
    {

        McAnimCheck();
        UIControl();
        if (characterSwitcher.isMusicGuyInCharge && !Movement.timeToLeave)
        {
            
            DetectAction();
            DetectMusic();
            SendNotesToMusicListenerOrDestroy();
        }
        else
        {
            TurnBgOn();
            ResetThisGuy();
        }

        //if (emitParticleNow)
        //{
        //    ChooseParticleColor();

        //    em.enabled = true;
        //    Invoke("PauseEmoPs", 0.5f);
        //}
    }

    private void DetectAction()
    {
        if (Input.GetKeyDown(Control.positiveAction) && currentMood != (int)Mood.happy)
        {
            PlaySong(0);

            TurnBgOff();
            anim.SetTrigger("click");
        }
        else if (Input.GetKeyDown(Control.negativeAction) && currentMood != (int)Mood.sad)
        {
            PlaySong(1);
            TurnBgOff();
            anim.SetTrigger("click");

        }
        else if ((Input.GetKeyDown(Control.negativeAction) && currentMood == (int)Mood.sad) || (Input.GetKeyDown(Control.positiveAction) && currentMood == (int)Mood.happy))
        {
            ResetThisGuy();
            TurnBgOn();
            anim.SetTrigger("click");
        }
    }
    private void McAnimCheck()
    {
        if (mcIsAffected && currentMood == (int)Mood.happy)
        {
            mc_anim.SetBool("isHappySong", true);
        }
        else
        {
            mc_anim.SetBool("isHappySong", false);
        }
    }
    private void DetectMusic()
    {
        if (isMusic)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, actionDist, Carriers);
            foundTheMC = false;
            //Check if array is empty or if there was anything collided with
            // Codes from Justin's Rabbit script 
            if (colliders.Length != 0)
            {
                Array.Reverse(colliders);
                foreach (Collider2D coll in colliders)
                {
                    if (!musicNoteCreated)
                    {
                        CreateMusicNote();
                        var musicCreatedInterval = (currentMood == 0 ? 0.3f : 0.6f);
                        Invoke("ResetMusicNote", musicCreatedInterval);
                    }

                    if (coll.gameObject.tag == "MC" && musicListener != coll.gameObject.name)
                    {
                        musicListener = coll.gameObject.name;
                        foundTheMC = true;
                        break;
                    }

                }
                if (!foundTheMC)
                {
                    foreach (Collider2D coll in colliders)
                    {
                        if (!musicNoteCreated)
                        {
                            CreateMusicNote();
                            var musicCreatedInterval = (currentMood == 0 ? 0.3f : 0.6f);
                            Invoke("ResetMusicNote", musicCreatedInterval);
                        }

                        if (coll.gameObject.tag == "NPC" && !npcIsAffected && !mcIsAffected)
                        {
                            musicListener = coll.gameObject.name;
                            npcIsAffected = true;
                            // 4 seconds later call this function and reset MC 
                            break;
                        }

                    }
                }



            }
            else
            {
                CancelInvoke("MCNotAffected");
                musicListener = "";
                mcIsAffected = false;
                npcIsAffected = false;
            }

            
        }
        else
        {
            ResetAffectedPeople();
        }

        
        

    }
    // play songs, change sprites and particles according to the mood 0=happy 1=sad 
    private void PlaySong(int index)
    {
        currentMood = (index==0) ? (int)Mood.happy: (int)Mood.sad;

        audioSource.clip = audioClips[index];

        sr.sprite = sprites[index];

        audioSource.Play();

        isMusic = true;

        ChooseParticleColor();
        em.enabled = true;
        //emitParticleNow = true;
    }
   


    private void ResetThisGuy()
    {
        currentMood = (int)Mood.idle;
        sr.sprite = sprites[currentMood];
        audioSource.clip = null;
        audioSource.Pause();
        isMusic = false;
        DestroyRemainingNote();
        CancelInvoke("McNotAffected");

    }

    private void TurnBgOff()
    {
        if (isBG && currentMood != (int)Mood.idle)
        {

            backgroundMusic.Pause();
            isBG = false;
        }
    }

    private void TurnBgOn()
    {
        if (!isBG)
        {
            backgroundMusic.Play();
            isBG = true;
        }
    }

    private void UIControl()
    {
        if (PauseUI.IsPaused)
        {
            audioSource.Pause();
        }
        else if (!PauseUI.IsPaused)
        {
            audioSource.UnPause();
        }
    }

    private void McNotAffected()
    {
        // doesne't send message from emoControl anymore 
        var msg="";
        if (currentMood == (int)Mood.happy)
        {
            msg = "Happy Song";
        }
        else if (currentMood == (int)Mood.sad)
        {
            msg = "Sad Song";
        }
        else
        {
            Debug.Log("How did this happen?");
            return;
        }
        MentalState.sendMsg(msg);
        mcIsAffected = false;
    }

    private void NpcNotAffected()
    {
        npcIsAffected = false;
    }

    private void SendMusicToTarget(GameObject target)
    {
        var tempList = GameObject.FindGameObjectsWithTag("MusicNote");
        foreach (var temp in tempList)
        {
            if (Vector2.Distance(temp.transform.position, target.transform.position) > actionDist + 5f)
            {
                Destroy(temp);
                continue;
            }
            temp.transform.position = Vector3.MoveTowards(temp.transform.position, target.transform.position, 10f * Time.deltaTime);
            if (temp.transform.position == target.transform.position)
            {
                Destroy(temp);
                
            }
        }
       
        
    }



    private void CreateMusicNote()
    {
        if (currentMood == (int) Mood.happy)
        {
            InstantiateNoteObj(happyNoteSprites);
        }
        else if (currentMood == (int) Mood.sad)
        {
            InstantiateNoteObj(sadNoteSprites);
        }
        
        musicNoteCreated = true;
        
        
    }

    private void ResetMusicNote()
    {
        musicNoteCreated = false;
    }

    private GameObject InstantiateNoteObj(Sprite[] spriteArray)
    {
        GameObject go = Instantiate(happyMusicNote, transform.position, Quaternion.identity) as GameObject;
        SpriteRenderer srsr = go.GetComponent<SpriteRenderer>();
        var index = UnityEngine.Random.Range(0, 3);
        srsr.sprite = spriteArray[index];
        var col = srsr.color;
        col.a = 1.0f;
        srsr.color = col;
        return go;
    }

    private void DestroyRemainingNote()
    {
        var temp = GameObject.FindGameObjectsWithTag("MusicNote");
        foreach (var t in temp) { Destroy(t); }
        //musicNoteCreated = false;
    }

    private void ResetAffectedPeople()
    {
        mcIsAffected = false;
        npcIsAffected = false;
        musicListener = "";
        em.enabled = false;
    }

    private void SendNotesToMusicListenerOrDestroy()
    {
        if (musicListener != "")
        {
            SendMusicToTarget(GameObject.Find(musicListener));
        }
        else
        {
            DestroyRemainingNote();
        }
    }

    private void ChooseParticleColor()
    {
        if (currentMood==(int)Mood.happy)
        {

            ps.GetComponent<ParticleSystemRenderer>().material = happyNoteMat;
        }
        else if (currentMood == (int)Mood.sad)
        {

            ps.GetComponent<ParticleSystemRenderer>().material = sadNoteMat;
        }

    }

    private void PauseEmoPs()
    {

        em.enabled = false;
        emitParticleNow = false;
    }

    //private float RoatateParticles(Vector3 myself, GameObject other)
    //{
    //    var dir = other.transform.position - myself;
    //    dir = other.transform.InverseTransformDirection(dir);
    //    var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
    //    Debug.Log(" particle is rotating!! " + angle);
    //    return angle;


    //}
}
