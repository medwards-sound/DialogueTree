/******************************************************************
	DialogueReaderEditor.cs

    Allows selection via dropdown of dialogueTree to be read
    by DialogueReader.

******************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueReader))]
public class DialogueReaderEditor : Editor
{
    DialogueReader dr;
    string[] options;

    public int currSelection;
    int lastSelection;

    public override void OnInspectorGUI()
    {
        //if dr is null some kind of load occurred
        if(dr == null)
            dr = (DialogueReader)target;

        options = FillPopUp();

        int ct = 0;
        foreach(string s in options)
        {
            if(s == dr.dialogueID)
            {
                currSelection = ct;
                break; //just this once
            }

            ct++;
        }
        
        base.OnInspectorGUI();

        lastSelection = currSelection;
        currSelection = EditorGUILayout.Popup("Select Dialogue Tree", currSelection, options);

        if(lastSelection != currSelection)
        {
            Undo.RecordObject(dr, "Dialogue ID Change");
            dr.dialogueID = options[currSelection];
        }
    }

    //fills popup list with dialogue tree names
    string[] FillPopUp()
    {
        Debug.Log("DialogeReader list updated.");

        string[] retString = null;
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/StreamingAssets/Dialogue/");
        FileInfo[] info = di.GetFiles("*.dat");

        retString = new string[info.Length + 1];
        retString[0] = "";

        int ct = 1;
        foreach(FileInfo fi in info)
        {
            retString[ct] = fi.Name.Split('.')[0];
            ct++;
        }

        return retString;
    }
}
