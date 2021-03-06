﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconControl : MonoBehaviour
{
    public static bool journalActivated;
    public static bool journalTweening;
    public static bool bringTheIconsIn;
    public Animator animator;
    public GameObject journal;
    public GameObject iconsGroup;
    public GameObject tabPrompt;
    public Image bg;
    public GameObject ring;
    public GameObject[] texts;
    public GameObject[] iconPics;
    public GameObject[] shadowPics;


    [SerializeField]private Image rabbitIcon;
    [SerializeField]private Image ballIcon;
    [SerializeField]private Image musicIcon;
    [SerializeField]private Image journalIcon;
    private List<Image> icons;
    private Color red = new Color(0f, 0.38f, 0.9f);
    private Color green = new Color(1f, 0.7007f, 0f);
    private float iconBringinTime = 5f;

    private void Awake()
    {
        journalActivated = false;
        journalTweening = false;
        bringTheIconsIn = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        icons = new List<Image>{ rabbitIcon, ballIcon, musicIcon};
        journal.SetActive(false);
        tabPrompt.SetActive(false);
        ToggleIcons(false);
        animator.SetBool("newAccom", false);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.timeSinceLevelLoad);
        if (Time.timeSinceLevelLoad > iconBringinTime && !bringTheIconsIn) {
            ToggleIcons(true);
            bringTheIconsIn = true;
        }

        if (characterSwitcher.charChoice==1000 && iconsGroup.activeSelf)
        {
            iconsGroup.SetActive(false);
            journalIcon.enabled = false;
            tabPrompt.SetActive(false);
        }

        foreach (var img in icons)
        {
            var currentIndex = icons.IndexOf(img);
            if ( currentIndex + 1 == characterSwitcher.charChoice)
            {
                
                Rescale(img, 105f);
                texts[currentIndex].SetActive(false);
                iconPics[currentIndex].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(40f, 40f);
                iconPics[currentIndex].GetComponent<Image>().rectTransform.localPosition
                    = new Vector2(-15f,
                    iconPics[currentIndex].GetComponent<Image>().rectTransform.localPosition.y);
                shadowPics[currentIndex].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(40f, 40f);
                shadowPics[currentIndex].GetComponent<Image>().rectTransform.localPosition
                    = new Vector2(-12f,
                    iconPics[currentIndex].GetComponent<Image>().rectTransform.localPosition.y);

                //if (Input.GetKey(Control.positiveAction))
                //{
                //    ChangeColor(img, green);
                //}
                //else if (Input.GetKey(Control.negativeAction))
                //{
                //    ChangeColor(img, red);
                //}
                //else
                //{
                //    ChangeColor(img, Color.white);
                //}
            }
            else
            {
                texts[currentIndex].SetActive(true);
                iconPics[currentIndex].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(30f, 30f);
                iconPics[currentIndex].GetComponent<Image>().rectTransform.localPosition
                    = new Vector2(-25f,
                    iconPics[currentIndex].GetComponent<Image>().rectTransform.localPosition.y);
                shadowPics[currentIndex].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(30f, 30f);
                shadowPics[currentIndex].GetComponent<Image>().rectTransform.localPosition
                    = new Vector2(-22f,
                    iconPics[currentIndex].GetComponent<Image>().rectTransform.localPosition.y);
                Rescale(img, 80f);
            }
        }

        if (journalTweening && characterSwitcher.charChoice != 1000)
        {
            tabPrompt.SetActive(true);
            animator.SetBool("newAccom", true);
            
        }
        else
        {
            tabPrompt.SetActive(false);
            animator.SetBool("newAccom", false);
        }


        if (journalActivated)
        {
            
            journal.SetActive(true);
        }
        else
        {
            
            journal.SetActive(false);
        }
    }

    void ChangeColor(Image im, Color col)
    {

        im.color = col;
    }

    void Rescale(Image im, float pixel)
    {
        im.rectTransform.sizeDelta = new Vector2(pixel, pixel);
    }

    private void ToggleIcons(bool boo)
    {
        iconsGroup.SetActive(boo);
        journalIcon.enabled = boo;
        ring.SetActive(boo);

    }
    
    
}
