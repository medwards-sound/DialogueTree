/******************************************************************
	DialogueEditorWindow.cs

    Editor window for creation and modifying of DialogueTrees.

******************************************************************/

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class DialogueEditorWindow : EditorWindow
{   
    //need to store this idependent of gui refresh,
    //keeps track of unity engine audio objects
    UnityEngine.Object[][] audioObjects;

    bool clearAudio; //should audioObjects be updated?
    
    DialogueCreator dialogueCreator;
    DialogueTree tree;
    DialogueNode refNode;

    Vector2 scrollPos;
    Rect r;
    Rect r1;
    Rect r2;
    Rect r3;

    Rect ra;
    Rect ra1;
    Rect ra2;
    Rect ra3;

    Rect rab;
    Rect rab1;
    Rect rab2;
    Rect rab3;

    Rect rs;
    Rect rsl;

    string title = "test";
    string[] files;
    string createName;

    float startPosY;
    float startPosX;

    int fileSelection;
    int lastSelection;

    [MenuItem("Window/Dialogue Tree")] //under window menu in unity
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditorWindow));    
    }

    //allows playing of audio clip without AudioSource
    public static void PlayAudioClip(AudioClip given)
    {
        Assembly unityEdAssembly = typeof(AudioImporter).Assembly;
        Type auUtil = unityEdAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = auUtil.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[]{typeof(AudioClip)}, null);
        method.Invoke(null, new object[]{given});
    }

    void OnEnable()
    {
        clearAudio = true;

        createName = "";
        fileSelection = 0;
        files = FindDialogueFiles();

        titleContent.text = "Dialogue Tree"; //tab name
 
        startPosX = 5.0f;
        startPosY = 300.0f;
        scrollPos = new Vector2(startPosX, 0);

        //below are used for graphical respresentations of dialogue nodes
        r = new Rect(0, 0, 300, 40);
        r1 = new Rect(0, 0, 300, 40);
        r2 = new Rect(0, 0, 300, 40);
        r3 = new Rect(0, 0, 300, 40);

        //below are for audio objects
        ra = new Rect(0, 0, 250, 16);
        ra1 = new Rect(0, 0, 250, 16);
        ra2 = new Rect(0, 0, 250, 16);
        ra3 = new Rect(0, 0, 250, 16);
        
        //below are for audio buttons
        rab = new Rect(0, 0, 50, 16);
        rab1 = new Rect(0, 0, 50, 16);
        rab2 = new Rect(0, 0, 50, 16);
        rab3 = new Rect(0, 0, 50, 16);

        rs = new Rect(startPosX, startPosY - 75, 225, 25);
        rsl = new Rect(startPosX, startPosY - 100, 250, 15);

        ClearAudioObjects();
    }

    void ClearAudioObjects()
    {
        audioObjects = new UnityEngine.Object[4][];

        for(int i = 0; i < 4; i++)
        {
            audioObjects[i] = new UnityEngine.Object[4];
        }

        clearAudio = false;
    }

	void OnGUI()
    {   //AudioClip au = Resources.Load<AudioClip>("Audio/LF Mix");
        //AudioSource as = new AudioSource();
       // Editor ed = Editor.CreateEditor(au);
       // ed.OnInteractivePreviewGUI(new Rect(350, 250, 100, 100), EditorStyles.inspectorDefaultMargins);
      
        if(clearAudio)
            ClearAudioObjects();
        
        //set current scroll position and start scroll area
        scrollPos = GUI.BeginScrollView(new Rect(0,0, position.width, position.height), scrollPos, new Rect(0,0,500f, 1000f), true, true);

        lastSelection = fileSelection;
        fileSelection = EditorGUILayout.Popup(fileSelection, files, GUILayout.Width(250)); //file dropdown

        if(lastSelection != fileSelection)
            ClearAudioObjects();

        //selection is to create new
        if(fileSelection == 0)
        {
            dialogueCreator = null;

            GUILayout.BeginHorizontal();
            createName = GUILayout.TextField(createName, GUILayout.Width(250));
            if(GUILayout.Button("Create", GUILayout.Width(250)))
            {
                //check to see if filename is already in use
                if(DialogueCreator.CheckForFile(createName))
                    Debug.Log("This filename is already in use, use new name or delete dialogue file.");
                else
                {
                    dialogueCreator = new DialogueCreator(createName, false);
                    tree = dialogueCreator.GetTree();
                    refNode = tree.GetRoot();

                    files = FindDialogueFiles();

                    int ct = 0;
                    foreach(string s in files)
                    {
                        if(s == createName)
                            fileSelection = ct;

                        ct++;
                    }
                }
                createName = "";
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();
        }

        //selection is to open file
        else if((dialogueCreator == null || lastSelection != fileSelection) && fileSelection != 0)
        {
            dialogueCreator = new DialogueCreator(files[fileSelection], true);
            tree = dialogueCreator.GetTree();
            refNode = tree.GetRoot();
        }

        //save changes to tree
        if(GUILayout.Button("Save", GUILayout.Width(250)) && dialogueCreator != null)
            dialogueCreator.Save();

        //if tree is set, render tree
        if(dialogueCreator != null && dialogueCreator.tree != null && dialogueCreator.tree.GetRoot() != null)
        {
            title = dialogueCreator.dialogueID;
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.Label("(Always Set Statement After Response)", EditorStyles.miniBoldLabel);

            DrawSpeakerForm();
            GenerateTree(); //make graphical representation of dialogue tree       
        }

        GUI.EndScrollView();
    }
    
    void DrawSpeakerForm()
    {
        string name = "";

        if(tree.GetSpeaker() != null)
            name = tree.GetSpeaker();
        
       

        GUI.Label(rsl, "Speaker", EditorStyles.boldLabel);
        GUI.color = Color.cyan;
        name = GUI.TextField(rs, name);

        tree.SetSpeaker(name);
        
                
    }

    //make graphical representation of tree
     void GenerateTree()
    {
        //nodes following reference node (responses)
        DialogueNode r1 = refNode.Branch(0);
        DialogueNode r2 = refNode.Branch(1);
        DialogueNode r3 = refNode.Branch(2);
        DialogueNode[] giveNodes = new DialogueNode[3];
        giveNodes[0] = r1;
        giveNodes[1] = r2;
        giveNodes[2] = r3;

        //get nodes following follow nodes
        DialogueNode[][] followResponses = new DialogueNode[3][];
        for(int i = 0; i < 3; i++)
        {
            followResponses[i] = new DialogueNode[3];
            for(int j = 0; j < 3; j++)
            {
                if(i == 0 && r1 != null)
                    followResponses[i][j] = r1.Branch(j);

                else if(i == 1 && r2 != null)
                    followResponses[i][j] = r2.Branch(j);

                else if(i == 2 && r3 != null)
                    followResponses[i][j] = r3.Branch(j);
            }
        }

        //draw reference node
        DrawNodeForm(startPosX, startPosY, 0, refNode, giveNodes);

        //back button
        GUI.color = Color.white;
        //change
        if(GUI.Button(new Rect(startPosX, startPosY - 25 - 16, 282, 25), new GUIContent("<")))
        {
            if(refNode != null && refNode.Branch(-1) != null)
            {
                refNode = refNode.Branch(-1);
               clearAudio = true;
            }
        }

        //draw top follow node
        if(r1 != null && r1.GetResponse() != "")
        {

            DrawNodeForm(startPosX + 500, startPosY - 235, 1,  r1, followResponses[0]);

            //forward from button
            GUI.color = Color.white;
            if(GUI.Button(new Rect(startPosX + 500, startPosY - 260 - 16, 282, 25), new GUIContent(">")))
            {
                if(refNode != null && refNode.Branch(0) != null)
                {
                    refNode = refNode.Branch(0);
                    clearAudio = true;
                }
            }
        }

        //draw middle follow node
        if(r2 != null && r2.GetResponse() != "")
        {
            DrawNodeForm(startPosX + 500, startPosY + 17, 2, r2, followResponses[1]);

            //forward from button
            GUI.color = Color.white;
            if(GUI.Button(new Rect(startPosX + 500, startPosY - 25 + 32 - 30, 282, 25), new GUIContent(">")))
            {
                if(refNode != null && refNode.Branch(1) != null)
                {
                    refNode = refNode.Branch(1);
                    clearAudio = true;
                }
            }
        }

        //draw bottom follow node
        if(r3 != null && r3.GetResponse() != "")
        {
            DrawNodeForm(startPosX + 500, startPosY + 235 + 36, 3, r3, followResponses[2]);

            //forward from button
            GUI.color = Color.white;
            if(GUI.Button(new Rect(startPosX + 500, startPosY + 210 - 16 + 36, 282, 25), new GUIContent(">")))
            {
                if(refNode != null && refNode.Branch(2) != null)
                {
                    refNode = refNode.Branch(2);
                    clearAudio = true;
                }
            }
        }
    }


    void DrawNodeForm(float startX, float startY, int boxNum, DialogueNode givenNode, DialogueNode[] followNodes)
    {
        //strings to save text boxed to
        string dialogue = "";
        string response1 = "";
        string response2 = "";
        string response3 = "";

        //convenience
        DialogueNode givenR1 = followNodes[0];
        DialogueNode givenR2 = followNodes[1];
        DialogueNode givenR3 = followNodes[2];


        if(givenNode.GetDialogue() != null)
            dialogue = givenNode.GetDialogue();
        
        //set position of node graphic
        r.x = startX;
        r.y = startY;

        GUI.color = Color.cyan;
        dialogue = GUI.TextArea(r, dialogue);
        givenNode.SetDialogue(dialogue);

        //set position of follow node graphics
        r1.x = startX;
        r1.y = startY + 50 + 16 - 10;

        r2.x = startX;
        r2.y = startY + 100 + 32 - 20;

        r3.x = startX;
        r3.y = startY + 150 + 48 - 30;

        ra.x = startX + 50;
        ra.y = startY - 16;

        ra1.x = r1.x + 50;
        ra1.y = r1.y - 16;

        ra2.x = r2.x + 50;
        ra2.y = r2.y - 16;

        ra3.x = r3.x + 50;
        ra3.y = r3.y - 16;

        rab.x = startX;
        rab.y = startY - 16;

        rab1.x = r1.x;
        rab1.y = r1.y - 16;

        rab2.x = r2.x;
        rab2.y = r2.y - 16;

        rab3.x = r3.x;
        rab3.y = r3.y - 16;

        //DIALOGUE AUDIO
        EditorGUI.BeginChangeCheck();

        if(audioObjects[boxNum][0] == null)
            audioObjects[boxNum][0] = EditorGUI.ObjectField(ra, givenNode.GetAudioDialogue(), typeof(UnityEngine.Object), true);
        else
            audioObjects[boxNum][0] = EditorGUI.ObjectField(ra, audioObjects[boxNum][0], typeof(UnityEngine.Object), true);

        if(EditorGUI.EndChangeCheck())
            givenNode.SetAudioDialogue(((AudioClip)audioObjects[boxNum][0]).name);
        
        if(GUI.Button(rab,"Play") && audioObjects[boxNum][0] != null)
          PlayAudioClip((AudioClip)audioObjects[boxNum][0]);


        //set responses to saved response (null checks might not be necessary anymore)
        if(givenR1 != null && givenR1.GetResponse() != null)
            response1 = givenR1.GetResponse();          
        
        if(givenR2 != null && givenR2.GetResponse() != null)
            response2 = givenR2.GetResponse();
        
        if(givenR3 != null && givenR3.GetResponse() != null)
            response3 = givenR3.GetResponse();
        
        //get user updates to responses
        GUI.color = Color.grey;
        response1 = GUI.TextArea(r1, response1);
        response2 = GUI.TextArea(r2, response2);
        response3 = GUI.TextArea(r3, response3);

        //RESPONSE 1 AUDIO OBJ
        EditorGUI.BeginChangeCheck();
        //Debug.Log(givenR1.GetAudioResponse());
        if(audioObjects[boxNum][1] == null && givenR1 != null)
            audioObjects[boxNum][1] = EditorGUI.ObjectField(ra1, givenR1.GetAudioResponse(), typeof(UnityEngine.Object), true);
        else
            audioObjects[boxNum][1] = EditorGUI.ObjectField(ra1, audioObjects[boxNum][1], typeof(UnityEngine.Object), true);
            
        if(EditorGUI.EndChangeCheck())
            givenR1.SetAudioResponse(((AudioClip)audioObjects[boxNum][1]).name);

        if(GUI.Button(rab1,"Play") && audioObjects[boxNum][1] != null)
            PlayAudioClip((AudioClip)audioObjects[boxNum][1]);

        //RESPONSE 2 AUDIO OBJ
        EditorGUI.BeginChangeCheck();
        if(audioObjects[boxNum][2] == null && givenR2 != null)
            audioObjects[boxNum][2] = EditorGUI.ObjectField(ra2, givenR2.GetAudioResponse(), typeof(UnityEngine.Object), true);
        else
            audioObjects[boxNum][2] = EditorGUI.ObjectField(ra2, audioObjects[boxNum][2], typeof(UnityEngine.Object), true);
            
        if(EditorGUI.EndChangeCheck())
            givenR2.SetAudioResponse(((AudioClip)audioObjects[boxNum][2]).name);

        if(GUI.Button(rab2,"Play") && audioObjects[boxNum][2] != null)
            PlayAudioClip((AudioClip)audioObjects[boxNum][2]);

        //RESPONSE 3 AUDIO OBJ
        EditorGUI.BeginChangeCheck();
        if(audioObjects[boxNum][3] == null && givenR3 != null)
            audioObjects[boxNum][3] = EditorGUI.ObjectField(ra3, givenR3.GetAudioResponse(), typeof(UnityEngine.Object), true);
        else
            audioObjects[boxNum][3] = EditorGUI.ObjectField(ra3, audioObjects[boxNum][3], typeof(UnityEngine.Object), true);
            
        if(EditorGUI.EndChangeCheck())
            givenR3.SetAudioResponse(((AudioClip)audioObjects[boxNum][3]).name);

        if(GUI.Button(rab3,"Play") && audioObjects[boxNum][3] != null)
            PlayAudioClip((AudioClip)audioObjects[boxNum][3]);


        //set response to user updated values (null checks may not be necessary anymore)
        if(givenR1 != null)
            givenR1.SetResponse(response1);        
        
        if(givenR2 != null)
            givenR2.SetResponse(response2);
        
        if(givenR3 != null)
            givenR3.SetResponse(response3);
        
        //if dialogue exists, check if we need to add follow nodes to tree
        if(givenNode.GetDialogue() != null && givenNode.GetDialogue() != "")
        {
            if(givenR1 == null)
                tree.Add(response1, null, givenNode, 0);
            
            if(givenR2 == null)
                tree.Add(response2, null, givenNode, 1);

            if(givenR3 == null)
                tree.Add(response3, null, givenNode, 2);
        }

        //draw connecting lines
        if(givenR1 != null && response1 != "")
            DrawLine(0);

        if(givenR2 != null && response2 != "")
            DrawLine(1);

        if(givenR3 != null && response3 != "")
            DrawLine(2);
    }
    

    //draw lines connecting follow nodes to responses
    void DrawLine(int line)
    {      
        Rect rs = new Rect(5, startPosY + 25, 225, 50);
        Handles.BeginGUI();
        Handles.color = Color.grey;

        switch(line)
        {
            case 0:
                Handles.DrawLine(new Vector2(startPosX + 300, startPosY + 65), new Vector2(startPosX + 500, startPosY + 45 - 265));
                break;
            case 1:
                Handles.DrawLine(new Vector2(startPosX + 300, startPosY + 120), new Vector2(startPosX + 500, startPosY + 25));
                break;
            case 2:
                Handles.DrawLine(new Vector2(startPosX + 300, startPosY + 175), new Vector2(startPosX + 500, startPosY - 10 + 290));
                break;
        }

        Handles.EndGUI();       
    }

    //returns string of dialogue files
    string[] FindDialogueFiles()
    {
        string[] retString = null;
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/StreamingAssets/Dialogue/");
        FileInfo[] info = di.GetFiles("*.dat");

        retString = new string[info.Length + 1];
        retString[0] = "Create New";

        int ct = 1;
        foreach(FileInfo fi in info)
        {
            retString[ct] = fi.Name.Split('.')[0];
            ct++;
        }

        return retString;
    }

    //this would be a nice addition
    void DrawTreeRepresentation()
    {
    }
}
