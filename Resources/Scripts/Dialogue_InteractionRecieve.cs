/*************************************************************************
    Dialogue_InteractionRecieve.cs

    This allows testing of dialogue in game through an inspector 
    panel option.
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_InteractionRecieve : MonoBehaviour
{
    public DialogueReader dialogueReader;
    public GameObject dialoguePanel;
    public AudioSource audioSource;

    public bool TestActive
    {
        get { return dialoguePanel.activeSelf; }
    }


    void Start()
    {
        //must turn off dialoguePanel 
        dialoguePanel.SetActive(false);
    }

    //This is called when you press "Test Dialogue" in inspector
    public void RecieveInteraction()
    {
        dialoguePanel.SetActive(true); //enable dialoguePanel
        Dialogue_UI_General dialogueUI = dialoguePanel.GetComponentInChildren<Dialogue_UI_General>();

        //this handles in-game UI and tree traversal
        dialogueUI.InitDialogue(dialogueReader, audioSource);
    }
}
