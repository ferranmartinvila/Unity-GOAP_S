using UnityEngine;
using UnityEditor;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    public class ConditionSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private ActionNode_GS_Editor _target_action_node = null; //Focused node action
        private string[] _variable_keys = null;
        private int prev_selected_variable_index = -1;
        private VariableType _selected_variable_type = VariableType._undefined_var_type;
        private int _selected_variable_index = -1;
        private OperatorType[] _valid_operators = null;
        private string[] _valid_operators_strings = null;
        private int _selected_operator_index = -1;
        private object _selected_value = null;

        //Contructors =================
        public ConditionSelectMenu_GS(ActionNode_GS_Editor target_action_node)
        {
            //Set the action node is this menu working with
            _target_action_node = target_action_node;
            //Get blackboard variables
            _variable_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeys();
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            editorWindow.maxSize = new Vector2(200.0f, 150.0f);
            editorWindow.minSize = new Vector2(200.0f, 150.0f);

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
            if (GUILayout.Button(_selected_variable_index != -1 ? _variable_keys[_selected_variable_index] : "not set"))
            {
                GenericMenu dropdown = new GenericMenu();
                for (int k = 0; k < _variable_keys.Length; k++)
                {
                    dropdown.AddItem(
                        //Generate gui content from property path strin
                        new GUIContent(_variable_keys[k]),
                        //show the currently selected item as selected
                        k == _selected_variable_index,
                        //lambda to set the selected item to the one being clicked
                        selectedIndex => _selected_variable_index = (int)selectedIndex,
                        //index of this menu item, passed on to the lambda when pressed.
                        k
                   );
                }
                dropdown.ShowAsContext(); //finally show the dropdown
            }
            //Check variable selection change
            if (prev_selected_variable_index != _selected_variable_index)
            {
                if (_selected_variable_index != -1)
                {
                    //If the selected index is valid we get the variable type
                    _selected_variable_type = NodeEditor_GS.Instance.selected_agent.blackboard.variables[_variable_keys[_selected_variable_index]].type;
                    //Then we can allocate the property value with the variable type
                    ProTools.AllocateFromVariableType(_selected_variable_type, ref _selected_value);
                    //Get valid operators
                    _valid_operators = ProTools.GetValidOperatorTypesFromVariableType(_selected_variable_type);
                    //Reset operator selected
                    _selected_operator_index = -1;
                }
                else
                {
                    //In non valid index case we reset the property data
                    _selected_variable_type = VariableType._undefined_var_type;
                    _selected_value = null;
                    _valid_operators = null;
                    _selected_operator_index = -1;
                }
                prev_selected_variable_index = _selected_variable_index;
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

            //Value select
            GUILayout.BeginHorizontal();
            if (_selected_variable_index != -1)
            {
                GUILayout.Label("Value:", GUILayout.MaxWidth(60.0f));
                //Show field on valid index case
                if (_selected_variable_index != -1)
                {
                    ProTools.ValueFieldByVariableType(_selected_variable_type, ref _selected_value);
                }
            }
            GUILayout.EndHorizontal();

            //Separation
            GUILayout.FlexibleSpace();

            //Add/Close buttons
            GUILayout.BeginHorizontal();
            //Add button
            if (GUILayout.Button("Add", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {

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
