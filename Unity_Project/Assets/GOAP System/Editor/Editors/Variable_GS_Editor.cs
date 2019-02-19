using UnityEngine;
using UnityEditor;

public class Variable_GS_Editor
{
    //Conten fields
    private Variable_GS _target_variable;
    private Blackboard_GS _target_bb;

    //Constructors ====================
    public Variable_GS_Editor(Variable_GS target_variable, Blackboard_GS target_bb)
    {
        //Set target variable
        _target_variable = target_variable;
        //Set target bb
        _target_bb = target_bb;
    }

    //Loop Methods ====================
    public void DrawUI()
    {
        GUILayout.BeginHorizontal();
        //Show variable type
        GUILayout.Label(_target_variable.type.ToString());
        //Show variable name
        GUILayout.Label(_target_variable.name);
        //Show/Edit variable value
        switch (_target_variable.type.ToString())
        {
            case "System.Single":
                _target_variable.value = EditorGUILayout.FloatField(_target_variable.name,(float)_target_variable.value,GUILayout.ExpandWidth(true));
                break;

            case "System.Int32":
                _target_variable.value = EditorGUILayout.IntField(_target_variable.name, (int)_target_variable.value, GUILayout.ExpandWidth(true));
                break;
        }
        //EditorGUILayout.IntField()
        //Remove button
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            //Remove the current var
            _target_bb.RemoveVariable(_target_variable.id);
        }

        GUILayout.EndHorizontal();
    }

    //Get/Set Methods =================
    public Variable_GS target_variable
    {
        get
        {
            return _target_variable;
        }
        set
        {
            _target_variable = value;
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
