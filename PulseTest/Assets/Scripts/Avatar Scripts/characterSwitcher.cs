﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterSwitcher : MonoBehaviour
{

    //This enables you to see and set the field from inspector but 
    //it is hidden from other scripts and objects. 
    //charChoice represents which named object to move
    [SerializeField]
    private int charChoice;

    // Use this for initialization
    void Start()
    {
        //Initially disable all but the chosen one
        disableOthers();
    }

    // Update is called once per frame
    void Update()
    {
        //Poll for mouse click
        switchCharacter();
    }

    public int getChar()
    {
        return charChoice;
    }

    //Function to handle character switching when 'E' is pressed
    private void switchCharacter()
    {
        //Looking for 'Left Mouse Button' to be pressed
        if (Input.GetMouseButtonDown(0))
        {
            //Vector for Raycast, takes mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Decompose to 2D vector
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            //Raycast hit register for mouse position
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            //If a hit is registered, find which object was hit
            if (hit.collider != null && hit.collider.gameObject.tag == "Avatars")
            {
                //Take the name of the object and convert to int for charChoice
                string name = hit.collider.gameObject.name;
                Debug.Log(name);
                int.TryParse(name, out charChoice);
            }

            //Activate the object chosen and disable all the others
            GameObject choice = findGO(charChoice);
            Enable(choice);
            disableOthers();
        }
    }

    //This function loops through all the other ones not chosen 
    //and disables their movement script
    private void disableOthers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (charChoice != i)
            {
                GameObject B = findGO(i);
                Disable(B);
            }
        }
    }

    //Helper function for finding game objects
    private GameObject findGO(int i)
    {
        string choice = i.ToString();
        GameObject someB = GameObject.Find(choice);
        return someB;
    }

    //Enables a game object's script
    private void Enable(GameObject B)
    {
        B.GetComponent<Movement>().enabled = true;

        switch (charChoice)
        {
            case 2:
                B.GetComponent<BallThrow>().enabled = true;
                break;
            default:
                break;
        }
    }

    //Disables a game object's script
    private void Disable(GameObject B)
    {
        B.GetComponent<Movement>().enabled = false;

        switch (charChoice)
        {
            case 0:
                findGO(2).GetComponent<BallThrow>().enabled = false;
                break;
            case 1:
                findGO(2).GetComponent<BallThrow>().enabled = false;
                break;
            case 3:
                findGO(2).GetComponent<BallThrow>().enabled = false;
                break;
            default:
                break;
        }
    }
}