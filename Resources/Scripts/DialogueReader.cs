/******************************************************************
	DialogueReader.cs

    Makes selected DialogueTree available to Dialogue_UI_General.
    Simple dropdown for selection of dialogue file (defined in 
    DialogueReaderEditor.cs)
******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueReader : MonoBehaviour
{
    [ReadOnly]
    public string dialogueID;

    DialogueTree tree;

	void Awake () 
	{
        Debug.Log("loading tree");
        Saver s = new Saver();

        tree = s.LoadSingle<DialogueTree>(Saver.saveType.dialogue, dialogueID);
      
        Debug.Log("loaded tree from file");
        tree.Unpack();
        Debug.Log("tree unpacked");
        tree.GoToRoot();
        Debug.Log("loaded tree completed");
	}
	
    public DialogueTree GetTree()
    {
        return tree;
    }

}
