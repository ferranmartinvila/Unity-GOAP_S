using UnityEngine;
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
        private NodeEditor_GS _target_node_editor = null;
        //State fields
        private bool on_edit_state = false;

        //Constructors ====================
        public Variable_GS_Editor(Variable_GS target_variable, Blackboard_GS target_bb, NodeEditor_GS target_node_editor)
        {
            //Set target variable
            _target_variable = target_variable;
            //Set target bb
            _target_bb = target_bb;
            //Set node editor
            _target_node_editor = target_node_editor;
        }

        //Loop Methods ====================
        public void DrawUI()
        {
            GUILayout.BeginHorizontal();

            //Edit button, swap between edit and show state
            if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
            {
                on_edit_state = !on_edit_state;
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
                case VariableType._int:
                    _target_variable.value = EditorGUILayout.IntField((int)_target_variable.value);
                    break;
            }

            //Remove button
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                //Remove the current var
                _target_bb.RemoveVariable(_target_variable.id);
                //Remove current var editor from blackboard editor
                _target_node_editor.blackboard_editor.DeleteVariableEditor(this);
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
