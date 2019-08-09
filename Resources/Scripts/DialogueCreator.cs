/******************************************************************
	DialogueCreator.cs

    Used by the dialogue editor window to create, load, and save
    dialogue trees. 
******************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class DialogueCreator
{
    [HideInInspector]
    public DialogueTree tree;

    [HideInInspector]
    public bool init;

    public string dialogueID;

    public DialogueCreator(string givenID, bool exists)
    {
        if(!exists)
        {
            dialogueID = givenID;
            tree = new DialogueTree();
            init = true;
            Save();
        }

        else
        {
            dialogueID = givenID;
            Load();
            init = true;
        }
    }

    public void Save()
    {
        Debug.Log("SAVE DM");
        Saver s = new Saver();
        tree.Pack();
        s.SaveSingle<DialogueTree>(tree, Saver.saveType.dialogue, dialogueID);
    }

    public void Load()
    {
        Debug.Log("LOAD DM");
        Saver s = new Saver();
        tree = s.LoadSingle<DialogueTree>(Saver.saveType.dialogue, dialogueID);

        if(tree != null)
            tree.Unpack();
        else
            Debug.Log("tree load failed, null");
    }

    //check to see if a filename already exists
    public static bool CheckForFile(string fileName)
    {
        return Saver.CheckForFile(Saver.saveType.dialogue, fileName);
    }

    public DialogueTree GetTree()
    {
        return tree; 
    }

}
