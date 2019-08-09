/**********************************************************
    DialogueTree.cs

    Constructs tree of DialogueNodes.
    Saves to .dat file in \Assets\StreamingAssets\Dialogue.
    One tree per conversation.

**********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueTree
{
    [SerializeField]
    DialogueNode root;

    [SerializeField, HideInInspector]
    DialogueNode currNode;

    [SerializeField, HideInInspector]
    string speaker;

    [SerializeField]
    List<DialogueNode> values;

    [NonSerialized, HideInInspector]
    DialogueNode[] valueArray;

    [SerializeField]
    int count = 0;

    [SerializeField]
    int pos;

    //constructor
    public DialogueTree()
    {
        root = new DialogueNode(null, null, null, count);
        currNode = root;
    }

    //serialize
    public void Pack()
    {
        //serialized nodes go into list
        values = new List<DialogueNode>();

        //begin adding to list
        Pack(root);
    }

    public void Pack(DialogueNode n)
    {
        //just in case
        if(root == null || n == null)
            return;

        //pack
        else
        {
            //add node to list
            values.Add(n);

            //recursion ends
            if(n.EndNode)
                return;

            //pack node branches
            for(int i = 0; i < 3; i++)
                Pack(n.Branch(i));         
        }
    }

    //deserialize
    public void Unpack()
    {
        pos = 0; //current position in list
        valueArray = values.ToArray(); //array for ease of use

        //unpack root
        Unpack(null, valueArray[0], -1);
    }

    public void Unpack(DialogueNode parent, DialogueNode node, int givenBranch)
    {
        node.Init(); //initialize node

        //the only node with null parent is root
        if(parent == null)
            root = node;

        //end recursion after node added
        if(node.EndNode)
        {
            Add(node, parent, givenBranch);
            return;
        }

        //continue recursion
        else
        {
            //add node
            if(node != root)
                Add(node, parent, givenBranch);             

            //unpack node branches
            for(int i = 0; i < 3; i++)
            {
                DialogueNode currBranch = valueArray[++pos];
                Unpack(node, currBranch, i);
            }   
        }     
    }

    /*
    public string[] ShowRoot()
    {
        if(root == null)
            Debug.Log("No values in tree.");

        return ShowNode(root);
    }

    public string[] ShowNode(DialogueNode node)
    {
        DialogueNodeReader reader = new DialogueNodeReader(node, this);
        string[] ret = reader.Read();

        return ret;
    }*/
    //end new

    //add node using dialogue strings
    public DialogueNode Add(string response, string dialogue, DialogueNode addTo, int index)
    {
       // Debug.Log("add" + response + dialogue);
        DialogueNode addedNode = new DialogueNode(response, dialogue, addTo, ++count);
        addTo.SetBranch(index, addedNode);

        return addedNode;
    }

    public DialogueNode Add(string response, DialogueNode addTo, int index)
    {
      //  Debug.Log("add");

        DialogueNode addedNode = new DialogueNode(response, null, addTo, ++count);
        addTo.SetBranch(index, addedNode);

        return addedNode;
    }

    //add already created node
    public DialogueNode Add(DialogueNode adding, DialogueNode addTo, int index)
    {
      //  Debug.Log("add");

        addTo.SetBranch(index, adding);

        return adding;
    }

    //set current node to root for traversal
    public DialogueNode GoToRoot()
    {
        currNode = root;

        return root;
    }

    //getter: speaker
    public string GetSpeaker()
    {
        return speaker;
    }

    //setter: speaker
    public void SetSpeaker(string givenName)
    {
        speaker = givenName;
    }

    //getter: root
    public DialogueNode GetRoot()
    {
        return root;
    }

    //getter: branches (used to traverse tree)
    public DialogueNode Branch(int branch)
    {
        currNode = currNode.Branch(branch);

        return currNode;
    }

}
