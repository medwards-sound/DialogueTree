/***********************************************************************
    DialogueNode.cs

    Each node holds parent and the following dialogue branches.
    Each response (user selected) has a single dialogue (NPC)
    associated with it.
************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNode
{
  
    [SerializeField]
    public bool EndNode { get; private set; }

    [SerializeField]
    DialogueNode parent;

    [NonSerialized]
    DialogueNode[] branches; 

    [SerializeField]
    string response;

    [SerializeField]
    string dialogue;

    [SerializeField]
    string gist; //condensed version of response, not yet implemented

    [SerializeField]
    string fName_responseAudio; //filename of the response

    [SerializeField]
    public string fName_dialogueAudio; //filename of the dialogue

    [NonSerialized]
    AudioClip dialogueAudio; //useful for limiting loads, needs to be defined on creation of node

    [NonSerialized]
    AudioClip responseAudio; //useful for limiting loads, needs to be defined on creation of node

    //constructor
    public DialogueNode(string givenResponse, string givenDialogue, DialogueNode givenParent, int n)
    {
        branches = new DialogueNode[3];
        parent = givenParent;
        response = givenResponse;
        dialogue = givenDialogue;
        EndNode = true;
    }

    //used after deserializing to initialize branches array
    public void Init()
    {
        branches = new DialogueNode[3];
    }

    //setter: dialogue
    public void SetDialogue(string givenDialogue)
    {
        dialogue = givenDialogue;
    }

    //setter: response
    public void SetResponse(string givenResponse)
    {
        response = givenResponse;
    }

    //setter: gist (not yet impelemented)
    public void SetGist(string givenGist)
    {
        gist = givenGist;
    }

    //setter: fname_dialogueAudio
    public void SetAudioDialogue(string fName)
    {
        fName_dialogueAudio = fName;
        GetAudioDialogue();
        Debug.Log("set dialog fName " + fName);
    }

    //setter: fname_responseAudio
    public void SetAudioResponse(string fName)
    {
        fName_responseAudio = fName;
        GetAudioResponse();
        Debug.Log("set dialog fName " + fName);
    }

    //getter: response
    public string GetResponse()
    {   
        return response;
    }

    //getter: dialogue
    public string GetDialogue()
    {
        return dialogue;
    }

    //getter: gist (not yet implemented)
    public string GetGist()
    {
        return gist;
    }

    //getter: responseAudio
    public AudioClip GetAudioResponse()
    {
        if(responseAudio == null && fName_responseAudio != "")
        {
            responseAudio = Resources.Load<AudioClip>("Audio/" + fName_responseAudio);
        }

        return responseAudio;
    }

    //getter: dialogueAudio
    public AudioClip GetAudioDialogue()
    {
        if(dialogueAudio == null && fName_dialogueAudio != "")
        {
            dialogueAudio = Resources.Load<AudioClip>("Audio/" + fName_dialogueAudio);
        }

        return dialogueAudio;
    }

    //getter: branches (-1 for parent)
    public DialogueNode Branch(int path)
    {
        DialogueNode ret = null;
    
        switch(path)
        {
            case 0:
                ret = branches[0];
                break;

            case 1:
                ret = branches[1];
                break;

            case 2:
                ret = branches[2];
                break;

            case -1:
                ret = parent;
                break;
        }


        return ret;
    }

    //setter: branches
    public void SetBranch(int path, DialogueNode givenBranch)
    {
        EndNode = false;

        branches[path] = givenBranch;
    }

}
