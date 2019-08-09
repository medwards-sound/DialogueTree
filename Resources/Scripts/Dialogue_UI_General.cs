/**********************************************************
    Dialogue_UI_General.cs

    Handles operation of the in-game dialogue panel.
    Utilizes a given DialogueTree script.
**********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_UI_General: MonoBehaviour
{
    //public Dialogue dialogueScript; 
    public Button[] dialogueButtons;
    public Text speaker;
    public Text[] dialogue;

    [ReadOnly]
    public DialogueReader dialogueReader;

    [HideInInspector]
    public DialogueTree dialogueTree;
    DialogueNode currDialogue;

    [ReadOnly]
    public AudioSource audioSource;

    public float waitTime = 3.0f;

    //activate buttons, set dialogue from root of tree
    public void InitDialogue(DialogueReader givenReader, AudioSource givenAudioSource)
    {
        dialogueReader = givenReader;
        audioSource = givenAudioSource;
        dialogueTree = dialogueReader.GetTree();

        if(dialogueTree != null)
        {
            currDialogue = dialogueTree.GoToRoot();
            speaker.text = dialogueTree.GetSpeaker();

            ActivateButtons(-1, true);
            SetAll();

            audioSource.clip = currDialogue.GetAudioDialogue();
            audioSource.Play();
        }
    }

    //called when user chooses response
    public void Path(int chosen)
    {
        currDialogue = dialogueTree.Branch(chosen);

        audioSource.clip = currDialogue.GetAudioResponse();
        audioSource.Play();
        Transition(chosen);
    }

    //update UI to reflect chosen response
    void Transition(int chosen)
    {
        ActivateButtons(chosen, false);
        StartCoroutine(WaitForBranch(chosen));
    }

    //activate response selection buttons in UI
    void ActivateButtons(int visibleLoc, bool active)
    {

        for(int i = 0; i < 3; i++)
        {
            if(i != visibleLoc)
                dialogueButtons[i].gameObject.SetActive(active);
            else
                dialogueButtons[i].interactable = active;
        }
    }

    //sets
    void SetAll()
    {
        if(currDialogue.Branch(0) != null && currDialogue.Branch(0).GetResponse() != "")
        {
            dialogue[0].text = currDialogue.GetDialogue();

            for(int i = 1; i < 4; i++)
            {
                DialogueNode d;
                if(currDialogue.Branch(i - 1) != null)
                {
                    d = currDialogue.Branch(i - 1);
                    dialogue[i].text = d.GetResponse();
                }
            }
        }

        else
            DialogueEnding();
    }

    //used when end of current tree is reached
    void DialogueEnding()
    {
        string str = currDialogue.GetDialogue();
        string[] splitStrings = null; //for outcomes (not fully implemented)

        dialogue[0].text = str;

        ActivateButtons(-1, false);
        StartCoroutine(WaitForEnd(splitStrings));
    }

    //waits to go to new branch
    IEnumerator WaitForBranch(int chosen)
    {
        yield return new WaitForSeconds(waitTime);
        audioSource.clip = currDialogue.GetAudioDialogue();
        audioSource.Play();


        ActivateButtons(chosen, true);
        SetAll();
    }

    //ends dialogue after short period
    IEnumerator WaitForEnd(string[] splitStrings)
    {
        yield return new WaitForSeconds(waitTime);

        transform.parent.gameObject.SetActive(false);
    }

}
