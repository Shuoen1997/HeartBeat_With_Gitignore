﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BullySpeech : MonoBehaviour
{
    public GameObject text;
    public Image speechBubble;

    private List<string> taunts;
    private bool speechOn;

    private Vector3 pos;
    private Vector3 offset;

    float time;
    float timer;

    private TextMeshProUGUI tmpug;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(-4, 5, 0);
        pos = NpcInstantiator.bullyKidPos;
        time = 0f;
        timer = 0f;

        speechOn = false;

        hideThought();
        taunts = new List<string>() {"Hey new kid", "What a stupid hat", "Go back to your old school", "Hey shrimp", "Hey carrotboy", "Hey what's your name", "Hey you"};
        tmpug = text.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.fixedUnscaledTime;
        if (NPCs.schoolBell)
        {
            hideThought();
        }
        else
        {
            if (time >= timer)
            {
                speechOn = false;
                hideThought();
            }
            if (Runners.bullying && speechOn == false)
            {
                speechOn = true;
                int ran = Random.Range(0, taunts.Count);
                tmpug.text = taunts[ran];
                showThought();
                setTimer();
            }
            if (Runners.bullying == false)
            {
                hideThought();
            }
            pos = NpcInstantiator.bullyKidPos;
            speechBubble.transform.position = pos + offset;
        }
        
    }

    void hideThought()
    {
        speechBubble.GetComponent<Image>().gameObject.SetActive(false);
    }

    void showThought()
    {
        speechBubble.GetComponent<Image>().gameObject.SetActive(true);
    }

    void setTimer()
    {
        time = Time.fixedUnscaledTime;
        timer = time + 2.5f;
        speechOn = true;
    }
}
