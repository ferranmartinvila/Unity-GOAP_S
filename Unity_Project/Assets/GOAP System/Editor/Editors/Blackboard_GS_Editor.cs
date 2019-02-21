using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;

namespace GOAP_S.UI
{
    public class Blackboard_GS_Editor
    {
        //UI fields
        private Rect _window; //Rect used to place bb window
                              //Content fileds
        private Blackboard_GS _target_bb; //Bb is this editor showing
        private NodeEditor_GS _target_node_editor; //NodeEditor is this editor in

        //Construtors =====================
        public Blackboard_GS_Editor(Blackboard_GS target_bb, NodeEditor_GS target_editor)
        {
            //Set the target bb
            _target_bb = target_bb;
            //Set the target node editor
            _target_node_editor = target_editor;
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
            GUILayout.Label("Variables", _target_node_editor.UI_configuration.blackboard_title_style);
            foreach (Variable_GS variable in target_bb.variables.Values)
            {
                //Create a variable editor
                Variable_GS_Editor variable_editor = new Variable_GS_Editor(variable, _target_bb);
                //Draw var editor UI
                variable_editor.DrawUI();
            }
            GUILayout.EndVertical();

            //Button to add new variables
            if (GUILayout.Button("Add", GUILayout.Width(50)))
            {
                Vector2 mousePos = Event.current.mousePosition;
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new VariableSelectMenu_GS(_target_node_editor));

                
                /*int i_var = 1;
                Variable_GS var = new Variable_GS("new_var", i_var);
                float f_var = 1.0f;
                Variable_GS fvar = new Variable_GS("new_var", f_var);
                ActionNode_GS_Editor _action = new ActionNode_GS_Editor(null,null);
                Variable_GS a_var = new Variable_GS("a_var", _action);

                Rigidbody rig = new Rigidbody();
                Variable_GS r_var = new Variable_GS("r_var", rig);

                _target_bb.variables.Add(r_var.id, r_var);
                _target_bb.variables.Add(a_var.id, a_var);
                _target_bb.variables.Add(var.id, var);
                _target_bb.variables.Add(fvar.id, fvar);*/
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
                return _target_node_editor;
            }
            set
            {
                _target_node_editor = value;
            }
        }
    }
}
