/*************************************************************
    Dialogue_InteractionRecieve_Editor.cs

    Allows use of "Test Dialogue" Button in inspector for
    Dialogue_InteractionRecieve button.
*************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(Dialogue_InteractionRecieve))]
public class Dialogue_InteractionRecieve_Editor : Editor
{
    Dialogue_InteractionRecieve di;

    public override void OnInspectorGUI()
    {
        if(di == null)
            di = (Dialogue_InteractionRecieve)target;
        
        base.OnInspectorGUI();

       

        EditorGUI.BeginDisabledGroup(di.TestActive);
        if(GUILayout.Button("Test Dialogue"))
            di.RecieveInteraction();
        EditorGUI.EndDisabledGroup();

        
    }
}
