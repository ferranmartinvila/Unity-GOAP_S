using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;
using GOAP_S.Tools;
using GOAP_S.UI;
using System.Reflection;

namespace GOAP_S.UI
{
    public class MethodSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        private Variable_GS _target_variable = null;
        private MethodInfo _target_method = null;
        //Bind fields
        private ProTools.DropDownData_GS[] input_dropdowns = null;
        private ParameterInfo[] method_parameters = null;
        private object[] local_values = null;

        //Constructor =================
        public MethodSelectMenu_GS(Variable_GS target_variable)
        {
            //Set the target variable
            _target_variable = target_variable;
            //Get method info
            _target_method = ProTools.FindMethodFromPath(_target_variable.binded_method_path, target_variable.system_type, NodeEditor_GS.Instance.selected_agent.gameObject).Key;
            //Get method parameters
            method_parameters = _target_method.GetParameters();
            //Allocate input array in null case
            if (_target_variable.binded_method_input == null)
            {
                _target_variable.binded_method_input = new KeyValuePair<string, object>[method_parameters.Length];
                //Allocate input data from type

                for (int k = 0; k < method_parameters.Length; k++)
                {
                    KeyValuePair<string, object> input = _target_variable.binded_method_input[k];
                    if (string.IsNullOrEmpty(input.Key) && input.Value == null)
                    {
                        object temp_value = new object();
                        ProTools.AllocateFromVariableType(method_parameters[k].ParameterType.ToVariableType(), ref temp_value);
                        _target_variable.binded_method_input[k] = new KeyValuePair<string, object>("", temp_value);
                    }
                }
            }
            //Allocate editor values and dropdowns
            local_values = new object[method_parameters.Length];
            input_dropdowns = new ProTools.DropDownData_GS[method_parameters.Length];
            for (int k = 0; k < method_parameters.Length; k++)
            {
                //Initialize local value
                if (_target_variable.binded_method_input[k].Value != null)
                {
                    local_values[k] = _target_variable.binded_method_input[k].Value;
                }
                else
                {
                    ProTools.AllocateFromVariableType(method_parameters[k].ParameterType.ToVariableType(), ref local_values[k]);
                }

                //Generate the new dropdown data
                ProTools.DropDownData_GS new_dropdown = new ProTools.DropDownData_GS();
                new_dropdown.dropdown_slot = ProTools.GetDropdownSlot();

                //Get local and global variables
                string[] local_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeysByVariableType(method_parameters[k].ParameterType.ToVariableType());
                string[] global_keys = GlobalBlackboard_GS.blackboard.GetKeysByVariableType(method_parameters[k].ParameterType.ToVariableType());

                //Generate dropdown paths
                new_dropdown.paths = new string[local_keys.Length + global_keys.Length];
                new_dropdown.display_paths = new string[local_keys.Length + global_keys.Length];

                //Add the local keys with a prefix for the dropdown
                for (int j = 0; j < local_keys.Length; j++)
                {
                    new_dropdown.display_paths[j] = "Local/" + local_keys[j];
                    new_dropdown.paths[j] = local_keys[j];
                }

                //Add the global keys with a prefix for the dropdown
                for (int j = local_keys.Length, s = 0; j < new_dropdown.paths.Length; j++, s++)
                {
                    new_dropdown.display_paths[j] = "Global/" + global_keys[s];
                    new_dropdown.paths[j] = global_keys[s];
                }

                //Add the generated dropdown
                input_dropdowns[k] = new_dropdown;
            }
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Method Input", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.BeginVertical();

            //Method name
            GUILayout.Label(_target_method.Name,UIConfig_GS.left_bold_style);
            
            //Method inputs
            for(int k = 0; k < method_parameters.Length; k++)
            {
                GUILayout.BeginHorizontal();

                //Input type
                GUILayout.Label(method_parameters[k].ParameterType.ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(80), GUILayout.ExpandWidth(true));
                //Not binded input case
                if (string.IsNullOrEmpty(_target_variable.binded_method_input[k].Key))
                {
                    //Input value
                    ProTools.ValueFieldByVariableType(method_parameters[k].ParameterType.ToVariableType(), ref local_values[k]);

                    //Bind dropdown
                    ProTools.GenerateButtonDropdownMenu(ref input_dropdowns[k].selected_index, input_dropdowns[k].display_paths, "Bind", false, 40.0f, input_dropdowns[k].dropdown_slot);

                    //On variable selected we update the method input of the target variable
                    if (input_dropdowns[k].selected_index > -1)
                    {
                        //Generate the new method input with the bind path
                        _target_variable.binded_method_input[k] = new KeyValuePair<string, object>(input_dropdowns[k].display_paths[input_dropdowns[k].selected_index], null);
                    }
                }
                //Binded input case
                else
                {
                    //Binded input path
                    GUILayout.Label(_target_variable.binded_method_input[k].Key, GUILayout.MaxWidth(80.0f));
                    //Unbind button
                    if (GUILayout.Button("UnBind", GUILayout.MaxWidth(50.0f)))
                    {
                        //Remove method input path string and place tje local correct type value
                        _target_variable.binded_method_input[k] = new KeyValuePair<string, object>("", local_values[k]);
                        //Reset the dropdown selected index
                        ProTools.SetDropdownIndex(input_dropdowns[k].dropdown_slot, -1);
                    }
                }
                //Binded input case
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            //Apply changes button
            if (GUILayout.Button("Apply", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                for (int k = 0; k < method_parameters.Length; k++)
                {
                    if (string.IsNullOrEmpty(_target_variable.binded_method_input[k].Key))
                    {
                        //Local object values are applied to the non binded method input parameters
                        _target_variable.binded_method_input[k] = new KeyValuePair<string, object>("", local_values[k]);
                    }
                }
                
                //Release the allocated dropdown slots
                for(int k = 0; k < input_dropdowns.Length; k++)
                {
                    ProTools.FreeDropdownSlot(input_dropdowns[k].dropdown_slot);
                }

                //Close popupwindow
                editorWindow.Close();
            }

            //Close button
            if (GUILayout.Button("Close", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                //Release the allocated dropdown slots
                for (int k = 0; k < input_dropdowns.Length; k++)
                {
                    ProTools.FreeDropdownSlot(input_dropdowns[k].dropdown_slot);
                }

                //Close popupwindow
                editorWindow.Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
