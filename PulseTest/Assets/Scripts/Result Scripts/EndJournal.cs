﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndJournal : MonoBehaviour
{
    public GameObject journalIcon;
    public GameObject sleepIcon;
    public GameObject bed;
    public GameObject emptyBed;
    public static bool journalIsOpened = false;
    public static bool deemLight = false;
    private Animator anim;
    private Animator bedAnim;
    // Start is called before the first frame update
    void Start()
    {
        anim = journalIcon.GetComponent<Animator>();
        bedAnim = bed.GetComponent<Animator>();
        bed.GetComponent<SpriteRenderer>().enabled = false;
        journalIcon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (LightController.isNightGlowFinish && !journalIcon.activeSelf)
        {
            journalIcon.SetActive(true);
            anim.SetBool("newAccom", true);
        }

        if (journalIcon.activeSelf)
        {
            if (Input.GetKeyDown(Control.pullJournal) && !journalIsOpened)
            {
                
                journalIsOpened = true;
                
                
            }
            else if (Input.GetKeyDown(Control.pullJournal) && journalIsOpened)
            {
                journalIsOpened = false;
            }
        }

        if (FlipJournal.finishReadingJournal && !journalIsOpened)
        {
            sleepIcon.SetActive(true);
            anim.SetBool("newAccom", false);
            if (Input.GetKeyDown(Control.evacuate))
            {
                if (GameObject.Find("MC") != null)
                {
                    bed.GetComponent<SpriteRenderer>().enabled = true;
                    emptyBed.GetComponent<SpriteRenderer>().enabled = false;
                    GameObject.Find("MC").SetActive(false);
                    Invoke("GoToBedPlsKid", 2f);
                    Invoke("DeemTheLight", 3f);
                }
                
            }
        }
    }

    void GoToBedPlsKid()
    {
        bedAnim.SetBool("goToBed", true);
        
    }

    void DeemTheLight()
    {
        deemLight = true;
    }

}
