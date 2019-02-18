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
    
    //Construtors =====================
    public Blackboard_GS_Editor(Blackboard_GS target_bb)
    {
        _target_bb = target_bb;
    }

    //Loop methods ====================
    public void DrawUI(int id)
    {
        GUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Variables");
        int var_count = _target_bb.variables.Count;
        for (int k = 0; k < var_count; k++)
        {
            Dictionary<string,Variable_GS>.ValueCollection[
            ((Variable_GS)target_bb.variables.Values.k[k].value).DrawUI();
        }
        GUILayout.EndVertical();
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
}
