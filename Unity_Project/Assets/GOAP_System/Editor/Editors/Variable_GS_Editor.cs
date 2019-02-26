﻿using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    public class Variable_GS_Editor
    {
        //Conten fields
        private Variable_GS _target_variable = null;
        private Blackboard_GS _target_bb = null;
        //State fields
        private string edit_button_str = "O";
        private bool on_edit_state = false;

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

            //Edit button, swap between edit and show state
            if (GUILayout.Button(edit_button_str, GUILayout.Width(20), GUILayout.Height(20)))
            {
                //Change state
                on_edit_state = !on_edit_state;
                //Change edit button str
                if (on_edit_state)
                {
                    edit_button_str = "-";
                }
                else
                {
                    edit_button_str = "O";
                }
            }

            //Show variable type
            GUILayout.Label(_target_variable.type.ToString());

            //Show variable name
            if (!on_edit_state)
            {
                GUILayout.Label(_target_variable.name);
            }
            //Edit variable name
            else
            {
                _target_variable.name = EditorGUILayout.TextField(_target_variable.name);
            }

            //Show variable value
            switch (_target_variable.type)
            {
                case VariableType._undefined:
                    {
                        GUILayout.Label("Type Error");
                    }
                    break;
                case VariableType._int:
                    {
                        _target_variable.object_value = EditorGUILayout.IntField((int)_target_variable.object_value);
                    }
                    break;
                case VariableType._float:
                    {
                        _target_variable.object_value = EditorGUILayout.FloatField((float)_target_variable.object_value);
                    }
                    break;

            }

            //Remove button
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                //Remove the current var
                _target_bb.RemoveVariable(_target_variable.id);
                //Remove current var editor from blackboard editor
                NodeEditor_GS.Instance.blackboard_editor.DeleteVariableEditor(this);
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
}
