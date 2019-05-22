using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;
using GOAP_S.Tools;
using GOAP_S.Planning;

namespace GOAP_S.UI
{
    public class Blackboard_GS_Editor
    {
        //UI fields
        private Rect _window = Rect.zero; //Rect used to place bb window
        //Content fileds
        private Blackboard_GS _target_blackboard = null; //Bb is this editor showing
        private Variable_GS_Editor[] _variable_editors = null;
        private int _variable_editors_num = 0;
        private string _id = null; //Window UUID

        //Construtors =====================
        public Blackboard_GS_Editor(Blackboard_GS target_bb)
        {
            //Set the target bb
            _target_blackboard = target_bb;
            //Allocate variable editors array
            _variable_editors = new Variable_GS_Editor[ProTools.INITIAL_ARRAY_SIZE];
            //Generate variable editors with the existing variables
            _variable_editors_num = 0;
            if (target_bb != null)
            {
                foreach (Variable_GS variable in target_bb.variables.Values)
                {
                    //Generate a variable editor
                    AddVariableEditor(variable);
                }
            }
        }

        //Loop methods ====================
        public void DrawUI(int id)
        {
            //Separaion between title and variables
            GUILayout.BeginVertical();
            GUILayout.Space(15);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.EndVertical();

            //Blit all the local variables
            GUILayout.BeginVertical();
            GUILayout.Label("Local Variables", UIConfig_GS.left_big_style);
            for(int k = 0; k <_variable_editors_num; k++)
            { 
                _variable_editors[k].DrawUI();
            }
            GUILayout.EndVertical();

            //Button to add new variables
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new VariableSelectMenu_GS());
                }
            }
            
            //Blit the global blackboard
            GUILayout.BeginVertical();
            GlobalBlackboard_GS_Editor.DrawGlobalUI();
            GUILayout.EndVertical();
        }

        public void DrawGlobalUI()
        {

            GUILayout.Label("Global Variables", UIConfig_GS.left_big_style);
            for (int k = 0; k < _variable_editors_num; k++)
            {
                _variable_editors[k].DrawUI();
            }

            //Button to add new variables
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new VariableSelectMenu_GS(true));
                }
            }
        }

        //Functionality methods ===========
        public void AddVariableEditor(Variable_GS new_variable)
        {
            //Check if we need to allocate more items in the array
            if(_variable_editors_num == _variable_editors.Length)
            {
                //Double array capacity
                Variable_GS_Editor[] new_array = new Variable_GS_Editor[_variable_editors_num * 2];
                //Copy values
                for(int k = 0; k < _variable_editors_num; k++)
                {
                    new_array[k] = _variable_editors[k];
                }
            }

            //Generate new variable editor
            Variable_GS_Editor new_variable_editor = new Variable_GS_Editor(new_variable, _target_blackboard);
            //Add it to the array
            _variable_editors[_variable_editors_num] = new_variable_editor;
            //Update variable editors num
            _variable_editors_num += 1;
        }

        public void RemoveVariableEditor(string name, bool global)
        {
            int repaint = 0;

            //Iterate variable editors
            for (int k = 0; k < _variable_editors_num; k++)
            {
                if (string.Compare(_variable_editors[k].target_variable.name, name) == 0)
                {
                    if (k == _variable_editors.Length - 1)
                    {
                        _variable_editors[k] = null;
                    }
                    else
                    {
                        for (int i = k; i < _variable_editors_num - 1; i++)
                        {
                            _variable_editors[i] = _variable_editors[i + 1];
                        }
                    }
                    //Update variable editors count
                    _variable_editors_num -= 1;

                    repaint += 1;
                }
            }

            //Remove variable editor planning instances(conditions and effects)
            string location_key = global ? "Global/" + name : "Local/" + name;
            if(RemoveVariableEditorPlanning(location_key) == true)
            {
                repaint += 1;
            }

            if (repaint > 0)
            {
                //Repaint node editor window
                NodeEditor_GS.Instance.Repaint();
            }
        }

        public bool RemoveVariableEditorPlanning(string key)
        {
            //Remove the target variable from the agent planning editors
            //Used when a blackboard variable is removed

            bool ret = false;
            
            //Iterate the agent action nodes
            for (int k = 0; k < NodeEditor_GS.Instance.action_node_editors_num; k++)
            {
                //Focused action node
                ActionNode_GS_Editor target_node = NodeEditor_GS.Instance.action_node_editors[k];

                //Iterate the action node conditions
                for (int n = 0; n < target_node.condition_editors_num; n++)
                {
                    Property_GS target_condition = target_node.condition_editors[n].target_property;
                    //Check if the condition uses the target variable
                    if (string.Compare(key, target_condition.A_key) == 0 || string.Compare(key, target_condition.B_key) == 0)
                    {
                        //Delete the condition if the target variable is in
                        target_node.RemoveConditionEditor(target_node.condition_editors[n]);
                        ret = true;
                    }
                }

                //Iterate the action node effects
                for (int n = 0; n < target_node.effect_editors_num; n++)
                {
                    Property_GS target_effect = target_node.effect_editors[n].target_property;
                    //Check if the effect uses the target variable
                    if (string.Compare(key, target_effect.A_key) == 0 || string.Compare(key, target_effect.B_key) == 0)
                    {
                        //Delete the effect if the target variable is in
                        target_node.RemoveEffectEditor(target_node.effect_editors[n]);
                        ret = true;
                    }
                }
            }

            return ret;
        }

        //Get/Set methods =============
        public int id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = System.Guid.NewGuid().ToString();
                }
                return _id.GetHashCode();
            }
        }

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

        public Blackboard_GS target_blackboard
        {
            get
            {
                return _target_blackboard;
            }
            set
            {
                _target_blackboard = value;
            }
        }
    }
}
