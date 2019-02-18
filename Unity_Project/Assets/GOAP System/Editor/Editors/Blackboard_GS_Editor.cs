using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Blackboard_GS_Editor
{
    //UI fields
    private Rect _window; //Rect used to place bb window
    //Content fileds
    private Blackboard_GS _target_bb; //Bb is this editor showing
    private NodeEditor_GS _target_editor; //NodeEditor is this editor in

    //Construtors =====================
    public Blackboard_GS_Editor(Blackboard_GS target_bb, NodeEditor_GS target_editor)
    {
        //Set the target bb
        _target_bb = target_bb;
        //Set the target node editor
        _target_editor = target_editor;
    }

    //Loop methods ====================
    public void DrawUI(int id)
    {
        //Separaion between title and variables
        GUILayout.BeginVertical();
        GUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.EndVertical();

        //Blit all the variables
        GUILayout.BeginVertical();
        GUILayout.Label("Variables", _target_editor.UI_configuration.blackboard_title_style);
        foreach(Variable_GS variable in target_bb.variables.Values)
        {
            variable.DrawUI();
        }
        GUILayout.EndVertical();

        //Button to add new variables
        if(GUILayout.Button("Add",GUILayout.Width(50)))
        {
            float new_var = 1.0f;
            Variable_GS var = new Variable_GS("new_var",new_var);

            _target_bb.variables.Add(var.id, var);
        }
    }

    //Get/Set methods =================
    public Rect window
    {
        get
        {
            return _window;
        }
        set
        {
            _window = value;
        }
    }

    public Vector2 window_size
    {
        get
        {
            return new Vector2(_window.width, _window.height);
        }
        set
        {
            _window.width = value.x;
            _window.height = value.y;
        }
    }

    public Vector2 window_position
    {
        get
        {
            return new Vector2(_window.x, _window.y);
        }
        set
        {
            _window.x = value.x;
            _window.y = value.y;
        }
    }

    public Blackboard_GS target_bb
    {
        get
        {
            return _target_bb;
        }
        set
        {
            _target_bb = value;
        }
    }

    public NodeEditor_GS target_editor
    {
        get
        {
            return _target_editor;
        }
        set
        {
            _target_editor = value;
        }
    }
}
