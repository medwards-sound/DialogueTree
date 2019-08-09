/****************************************************************************
	ReadOnlyDrawer.cs

    Allows use of read only attribute, which disables abilty to 
    change entry in inspector.

    Code from:
    https://gist.github.com/LotteMakesStuff/c0a3b404524be57574ffa5f8270268ea
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
