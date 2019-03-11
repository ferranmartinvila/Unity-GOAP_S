using UnityEngine;
using UnityEditor;
using GOAP_S.PT;
using GOAP_S.Planning;

namespace GOAP_S.UI
{
    public class ConditionSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private ActionNode_GS_Editor _target_action_node = null; //Focused node action
        //Variable A fields
        private string[] _A_variable_keys = null;
        private int prev_selected_variable_index = -1;
        private VariableType _selected_variable_type = VariableType._undefined_var_type;
        private int _selected_A_key_index = -1;
        //Operator fields
        private OperatorType[] _valid_operators = null;
        private int _selected_operator_index = -1;
        //Variable B fields
        private int _value_or_key = 0; //1 = value / 2 = key
        private object _selected_value = null;
        private int _selected_B_key_index = -1;
        private string[] _B_variable_keys = null;

        //Contructors =================
        public ConditionSelectMenu_GS(ActionNode_GS_Editor target_action_node)
        {
            //Set the action node is this menu working with
            _target_action_node = target_action_node;
            //Get blackboard variables
            _A_variable_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeys();
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
            GUILayout.Label("Condition Select", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


            //Variable select
            GUILayout.BeginHorizontal();
            GUILayout.Label("Variable:",GUILayout.MaxWidth(60.0f));
            if (GUILayout.Button(_selected_A_key_index != -1 ? _A_variable_keys[_selected_A_key_index] : "not set"))
            {
                GenericMenu dropdown = new GenericMenu();
                for (int k = 0; k < _A_variable_keys.Length; k++)
                {
                    dropdown.AddItem(
                        //Generate gui content from property path strin
                        new GUIContent(_A_variable_keys[k]),
                        //show the currently selected item as selected
                        k == _selected_A_key_index,
                        //lambda to set the selected item to the one being clicked
                        selectedIndex => _selected_A_key_index = (int)selectedIndex,
                        //index of this menu item, passed on to the lambda when pressed.
                        k
                   );
                }
                dropdown.ShowAsContext(); //finally show the dropdown
            }
            //Check variable selection change
            if (prev_selected_variable_index != _selected_A_key_index)
            {
                if (_selected_A_key_index != -1)
                {
                    //If the selected index is valid we get the variable type
                    _selected_variable_type = NodeEditor_GS.Instance.selected_agent.blackboard.variables[_A_variable_keys[_selected_A_key_index]].type;
                    //Then we can allocate the property value with the variable type
                    ProTools.AllocateFromVariableType(_selected_variable_type, ref _selected_value);
                    //Get valid operators
                    _valid_operators = ProTools.GetValidPassiveOperatorTypesFromVariableType(_selected_variable_type);
                    //Reset operator selected
                    _selected_operator_index = -1;
                    //Get variables in the blackboard with the same type
                    _B_variable_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeysByVariableType(_selected_variable_type);
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
                if (GUILayout.Button(_selected_operator_index != -1 ? _valid_operators[_selected_operator_index].ToShortString() : "not set"))
                {
                    GenericMenu dropdown = new GenericMenu();
                    for (int k = 0; k < _valid_operators.Length; k++)
                    {
                        dropdown.AddItem(
                            //Generate gui content from property path strin
                            new GUIContent(_valid_operators[k].ToShortString()),
                            //show the currently selected item as selected
                            k == _selected_operator_index,
                            //lambda to set the selected item to the one being clicked
                            selectedIndex => _selected_operator_index = (int)selectedIndex,
                            //index of this menu item, passed on to the lambda when pressed.
                            k
                       );
                    }
                    dropdown.ShowAsContext(); //finally show the dropdown
                }
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
                    //Generate enumerator popup with the operator type
                    if (GUILayout.Button(_selected_B_key_index != -1 ? _B_variable_keys[_selected_B_key_index] : "not set"))
                    {
                        GenericMenu dropdown = new GenericMenu();
                        for (int k = 0; k < _B_variable_keys.Length; k++)
                        {
                            dropdown.AddItem(
                                //Generate gui content from property path strin
                                new GUIContent(_B_variable_keys[k]),
                                //show the currently selected item as selected
                                k == _selected_B_key_index,
                                //lambda to set the selected item to the one being clicked
                                selectedIndex => _selected_B_key_index = (int)selectedIndex,
                                //index of this menu item, passed on to the lambda when pressed.
                                k
                           );
                        }
                        dropdown.ShowAsContext(); //finally show the dropdown
                    }
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
                    //First allocate the condition class
                    Property_GS new_condition = new Property_GS();
                    //Set A key
                    new_condition.A_key = _A_variable_keys[_selected_A_key_index];
                    //Set operator type
                    new_condition.operator_type = _valid_operators[_selected_operator_index];
                    //Set value or key in property B part
                    if(_value_or_key == 2)
                    {
                        new_condition.B_key = _B_variable_keys[_selected_B_key_index];
                    }
                    else
                    {
                        new_condition.value = _selected_value;
                    }
                    //Add condition to the action node we are working with
                    _target_action_node.AddCondition(new_condition);
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
    }
}
