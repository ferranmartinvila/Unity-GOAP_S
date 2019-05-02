using UnityEngine;
using UnityEditor;
using GOAP_S.Tools;
using GOAP_S.Planning;
using GOAP_S.Blackboard;

namespace GOAP_S.UI
{
    public class PropertySelectMenu_GS : PopupWindowContent
    {
        //Content fields
        private PropertyUIMode _property_UI_mode = PropertyUIMode.IS_UNDEFINED; //Depending of the mode the user will be able to modify the properties with different options
        private string _menu_title = null; //Condition Select or Effect Select
        private ActionNode_GS_Editor _target_action_node_editor = null; //Focused node action
        //Variable A fields
        private string[] _A_variable_keys = null; //Avaliable variables in the target and global blackboard
        private string[] _original_A_variable_keys = null; //A keys without prefix
        private int prev_selected_variable_index = -1; //To check variable type change
        private VariableType _selected_variable_type = VariableType._undefined_var_type; //Selected variable type
        private int _selected_A_key_index = -1; //Selected A key dropdown index
        //Operator fields
        private OperatorType[] _valid_operators = null; //Valid operators in relation of the variable type
        private int _selected_operator_index = -1; //Operator selected by user in dropdown index
        //Variable B fields
        private short _value_or_key = 0; //1 = value / 2 = key
        private object _selected_value = null; //Selected in property using value case
        private int _selected_B_key_index = -1; //Selected B key in property using variable case
        private string[] _B_variable_keys = null; //Avaliable B keys (variable with the same variable type of A in blackboard)
        private string[] _original_B_variable_keys = null; //B keys without prefix
        //Dropdowns slots
        private int _A_key_dropdown_slot = -1; //A key dropdown slot
        private int _operator_dropdown_slot = -1; //Operator dropdown slot
        private int _B_key_dropdown_slot = -1; //B key dropdown slot

        //Contructors =================
        public PropertySelectMenu_GS(ActionNode_GS_Editor new_target_action_node_editor, PropertyUIMode new_property_UI_mode)
        {
            //Set property UI mode
            _property_UI_mode = new_property_UI_mode;
            //Set menu title
            if(_property_UI_mode == PropertyUIMode.IS_CONDITION)
            {
                //Condition title case
                _menu_title = "Condition Select";
            }
            else if(_property_UI_mode == PropertyUIMode.IS_EFFECT)
            {
                //Effect title case
                _menu_title = "Effect Select";
            }
            //Set the action node is this menu working with
            _target_action_node_editor = new_target_action_node_editor;
            
            //Get local blackboard variables keys
            string[] local_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeys();
            //Get global blackboard variables keys
            string[] global_keys = GlobalBlackboard_GS.blackboard.GetKeys();
            //Allocate string array to store all the keys
            _A_variable_keys = new string[local_keys.Length + global_keys.Length];
            _original_A_variable_keys = new string[local_keys.Length + global_keys.Length];
            //Add the local keys with a prefix for the dropdown
            for(int k = 0; k < local_keys.Length; k++)
            {
                _A_variable_keys[k] = "Local/" + local_keys[k];
                _original_A_variable_keys[k] = local_keys[k];
            }
            //Add the global keys with a prefix for the dropdown
            for(int k = local_keys.Length, i = 0; k < _A_variable_keys.Length; k++, i++)
            {
                _A_variable_keys[k] = "Global/" + global_keys[i];
                _original_A_variable_keys[k] = global_keys[i];
            }

            //Get dropdown slots
            _A_key_dropdown_slot = ProTools.GetDropdownSlot();
            _operator_dropdown_slot = ProTools.GetDropdownSlot();
            _B_key_dropdown_slot = ProTools.GetDropdownSlot();
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            editorWindow.maxSize = new Vector2(200.0f, 170.0f);
            editorWindow.minSize = new Vector2(200.0f, 170.0f);

            GUILayout.BeginVertical();

            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(_menu_title, UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


            //Variable select
            GUILayout.BeginHorizontal();
            GUILayout.Label("Variable:",GUILayout.MaxWidth(60.0f));
            //Generate dropdown with the variables in the target blackboard
            ProTools.GenerateButtonDropdownMenu(ref _selected_A_key_index, _A_variable_keys, "Not Set", true, 120.0f, _A_key_dropdown_slot);

            //Check variable selection change
            if (prev_selected_variable_index != _selected_A_key_index)
            {
                if (_selected_A_key_index != -1)
                {
                    //If the selected index is valid we get the variable type
                    if(_selected_A_key_index + 1 > NodeEditor_GS.Instance.selected_agent.blackboard.variables.Count)
                    {
                        //Global variable case
                        _selected_variable_type = GlobalBlackboard_GS.blackboard.variables[_original_A_variable_keys[_selected_A_key_index]].type;
                    }
                    else
                    {
                        //Local variable case
                        _selected_variable_type = NodeEditor_GS.Instance.selected_agent.blackboard.variables[_original_A_variable_keys[_selected_A_key_index]].type;
                    }
                    
                    //Then we can allocate the property value with the variable type
                    ProTools.AllocateFromVariableType(_selected_variable_type, ref _selected_value);
                    //Get valid operators
                    //Need to check if si a condition or an effect
                    if (_property_UI_mode == PropertyUIMode.IS_CONDITION)
                    {
                        //Condition passive operators case
                        _valid_operators = _selected_variable_type.GetValidPassiveOperatorTypes();
                    }
                    else if (_property_UI_mode == PropertyUIMode.IS_EFFECT)
                    {
                        //Effect active operators case
                        _valid_operators = _selected_variable_type.GetValidActiveOperatorTypes();
                    }
                    //Reset operator selected
                    _selected_operator_index = -1;

                    //Get local blackboard variables keys with the same type
                    string[] local_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeysByVariableType(_selected_variable_type);
                    //Get global blackboard variables keys with the same type
                    string[] global_keys = GlobalBlackboard_GS.blackboard.GetKeysByVariableType(_selected_variable_type);
                    //Allocate string array to store all the keys
                    _B_variable_keys = new string[local_keys.Length + global_keys.Length];
                    _original_B_variable_keys = new string[local_keys.Length + global_keys.Length];
                    //Add the local keys with a prefix for the dropdown
                    for (int k = 0; k < local_keys.Length; k++)
                    {
                        _B_variable_keys[k] = "Local/" + local_keys[k];
                        _original_B_variable_keys[k] = local_keys[k];
                    }
                    //Add the global keys with a prefix for the dropdown
                    for (int k = local_keys.Length, i = 0; k < _B_variable_keys.Length; k++, i++)
                    {
                        _B_variable_keys[k] = "Global/" + global_keys[i];
                        _original_B_variable_keys[k] = global_keys[i];
                    }
                    
                    //Reset B key selected
                    _selected_B_key_index = -1;
                }
                else
                {
                    //In non valid index case we reset the property data
                    _selected_variable_type = VariableType._undefined_var_type;
                    _selected_value = null;
                    _valid_operators = null;
                    _selected_operator_index = -1;
                }
                prev_selected_variable_index = _selected_A_key_index;
            }
            GUILayout.EndHorizontal();

            //Operator select
            GUILayout.BeginHorizontal();
            if (_valid_operators != null)
            {
                GUILayout.Label("Operator:", GUILayout.MaxWidth(60.0f));
                //Generate enumerator popup with the operator type
                ProTools.GenerateButtonDropdownMenu(ref _selected_operator_index, _valid_operators.ToShortStrings(), "Not Set", true, 120.0f, _operator_dropdown_slot);
            }
            GUILayout.EndHorizontal();

            //Value area 
            //Target select
            GUILayout.BeginVertical();
            if (_selected_A_key_index != -1)
            {
                if (_value_or_key == 0 || _value_or_key == 1)
                {
                    if (GUILayout.Button("Use Variable", GUILayout.MaxWidth(100.0f)))
                    {
                        _value_or_key = 2;
                    }
                }
                else if (_value_or_key == 2)
                {
                    if (GUILayout.Button("Use Value", GUILayout.MaxWidth(100.0f)))
                    {
                        _value_or_key = 1;
                    }
                }
            }
            //Value select
            GUILayout.BeginHorizontal();
            if (_selected_A_key_index != -1)
            {
                if (_value_or_key == 0 || _value_or_key == 1)
                {
                    GUILayout.Label("Value:", GUILayout.MaxWidth(60.0f));
                }
                else if (_value_or_key == 2)
                {
                    GUILayout.Label("Variable:", GUILayout.MaxWidth(60.0f));
                }

                //Show input field in value case
                if (_value_or_key == 1 || _value_or_key == 0)
                {
                    //Show field on valid index case
                    ProTools.ValueFieldByVariableType(_selected_variable_type, ref _selected_value);
                }
                //Show input field in variable case
                else if (_value_or_key == 2)
                {
                    //Generate enumerator popup with the avaliable B keys
                    ProTools.GenerateButtonDropdownMenu(ref _selected_B_key_index, _B_variable_keys, "Not Set", true, 120.0f, _B_key_dropdown_slot);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Separation
            GUILayout.FlexibleSpace();

            //Add/Close buttons
            GUILayout.BeginHorizontal();
            //Add button
            if (GUILayout.Button("Add", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                //First we need to check if all the fields are correctly filled
                if (_selected_A_key_index == -1 || _selected_operator_index == -1 || (_value_or_key == 2 && _selected_B_key_index == -1))
                {
                    Debug.LogWarning("Fields are not filled correctly!");
                }
                //If everithing is correct we generate the condition
                else
                {
                    //First allocate the property class
                    Property_GS new_property = new Property_GS();
                    //Set A key
                    new_property.A_key = _original_A_variable_keys[_selected_A_key_index];
                    //Set variable type
                    new_property.variable_type = _selected_variable_type;
                    //Set operator type
                    new_property.operator_type = _valid_operators[_selected_operator_index];
                    //Set value or key in property B part
                    if(_value_or_key == 2)
                    {
                        new_property.B_key = _original_B_variable_keys[_selected_B_key_index];
                    }
                    else
                    {
                        new_property.value = _selected_value;
                    }
                    //Add property to the action node we are working with
                    //Need to check if is a condition or an effect
                    if (_property_UI_mode == PropertyUIMode.IS_CONDITION)
                    {
                        //Add condition case
                        _target_action_node_editor.AddCondition(new_property);
                    }
                    else if(_property_UI_mode == PropertyUIMode.IS_EFFECT)
                    {
                        //Add effect case
                        _target_action_node_editor.AddEffect(new_property);
                    }
                    //Close this menu
                    editorWindow.Close();
                    //Update node editor
                    NodeEditor_GS.Instance.Repaint();
                }
            }
            //Close button
            if (GUILayout.Button("Close", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public override void OnClose()
        {
            //Free dropdown slots
            ProTools.FreeDropdownSlot(_A_key_dropdown_slot);
            ProTools.FreeDropdownSlot(_operator_dropdown_slot);
            ProTools.FreeDropdownSlot(_B_key_dropdown_slot);
        }
    }
}
