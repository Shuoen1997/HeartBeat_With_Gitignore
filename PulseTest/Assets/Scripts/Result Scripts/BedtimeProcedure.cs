﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedtimeProcedure : MonoBehaviour
{
    // Game objects
    public GameObject journalIcon;
    public GameObject sleepIcon;
    public GameObject tabIcon;
    public GameObject bed;
    public GameObject fadeObject;
    public GameObject dream;
    public GameObject rabAsset;
    public GameObject ballAsset;
    public GameObject musicAsset;

    // Animators
    private Animator anim;
    private Animator bedAnim;

    public static bool journalIsOpened;

    private bool charlieInBed;
    // Start is called before the first frame update
    private void Awake()
    {
        journalIsOpened = false;
        charlieInBed = false;
    }
    void Start()
    {
        anim = journalIcon.GetComponent<Animator>();
        bedAnim = bed.GetComponent<Animator>();

        journalIcon.SetActive(false);
        tabIcon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (LightController.nightIsHere)
        {
            ReadJournal();
            FinishJournal();
        }

    }

    private void JustGotHome()
    {

    }

    private void FinishJournal()
    {
        if (FlipJournal.finishReadingJournal && !journalIsOpened && !AutoToSchool.mcWokeUp)
        {
            if (!charlieInBed)
            {
                sleepIcon.SetActive(true);
            }
            journalIcon.SetActive(false);
            tabIcon.SetActive(false);
            if (Input.GetKeyDown(Control.evacuate))
            {
                GoToBedProcedure();
            }
        }
    }

    private void GoToBedProcedure()
    {
        if (GameObject.Find("MC") != null)
        {


            //Invoke("fadeOut", 0.1f);
            Invoke("fadeOut", 0.5f);
            Invoke("CharlieGoAway", 1.2f);
            Invoke("fadeIn", 1.5f);
            Invoke("GoToBedPlsKid", 2f);
            Invoke("Dreaming", 8f);
            Invoke("TurnToMorn", 15f);

        }
    }

    void TurnToMorn()
    {
        LightController.morningIsHere = true;
    }

    void CharlieGoAway()
    {
        GameObject.Find("MC").SetActive(false);
        LightController.turnOffRoomLights = true;
        charlieInBed = true;
        sleepIcon.SetActive(false);


    }

    void GoToBedPlsKid()
    {
        bedAnim.SetBool("goToBed", true);
    }



    void Dreaming()
    {
        dream.gameObject.SetActive(true);
        int avatar = MentalState.DecideFriend();
        Debug.Log("Friend: " + avatar);
        if (avatar == 0)
        {
            //what should happen?
            dream.gameObject.SetActive(false);
        }
        else if (avatar == 1)
        {
            rabAsset.gameObject.SetActive(true);
        }
        else if (avatar == 2)
        {
            ballAsset.gameObject.SetActive(true);
        }
        else if (avatar == 3)
        {
            musicAsset.gameObject.SetActive(true);
        }
    }

    private void fadeOut()
    {
        Debug.Log("fadeOut called");
        var meh = fadeObject.GetComponent<ResultFade>();
        meh.FadeOutStay();
    }

    private void fadeIn()
    {
        Debug.Log("fadeIn called");

        var meh = fadeObject.GetComponent<ResultFade>();
        meh.FadeIn();
    }

    private void ReadJournal()
    {
        if (!journalIcon.activeSelf && !charlieInBed) 
        {
            journalIcon.SetActive(true);
            anim.SetBool("newAccom", true);
            tabIcon.SetActive(true);
        }
        else
        {
            if (Input.GetKeyDown(Control.pullJournal) && !journalIsOpened && !charlieInBed)
            {

                journalIsOpened = true;
                tabIcon.SetActive(false);

            }
            else if (Input.GetKeyDown(Control.pullJournal) && journalIsOpened && !charlieInBed)
            {
                journalIsOpened = false;
            }
        }
    }

}