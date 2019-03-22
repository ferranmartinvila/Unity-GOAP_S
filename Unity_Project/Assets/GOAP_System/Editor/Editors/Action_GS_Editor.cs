using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.Tools;
using GOAP_S.Planning;
using System.Reflection;

namespace GOAP_S.UI
{
    public class Action_GS_Editor
    {
        //Content fields
        private EditorUIMode _UI_mode = EditorUIMode.HIDE_STATE; //Swap between edit and set state, so the user can edit values or not
        private Action_GS _target_action = null; //Target action script
        private PropertyInfo[] _properties = null; //Non blocked properties array, bool is true if the property setter is defined
        private FieldInfo[] _fields = null; //Public fields
        private object[] _values = null; //All the selected values (properties + fields)

        //Contructors =================
        public Action_GS_Editor(Action_GS target)
        {
            //Set target action
            _target_action = target;

            //Temporal values list
            List<object> temp_value_list = new List<object>();

            //Get action properties info
            PropertyInfo[] all_properties = _target_action.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> valid_properties = new List<PropertyInfo>();
            //Iterate all the action properties
            foreach (PropertyInfo property in all_properties)
            {
                //Blocked properties are not stored
                object[] blocked_attribute = property.GetCustomAttributes(typeof(BlockedProperty_GS), true);
                if (blocked_attribute.Length != 0)
                {
                    continue;
                }
                //Add the non blocked property to the list
                if(property.CanRead)
                {
                    //Add the property value
                    temp_value_list.Add(property.GetGetMethod().Invoke(_target_action, null));
                }
                else
                {
                    //Add null value to mantain a logic index
                    temp_value_list.Add(null);
                }
                //Add the non blocked property
                valid_properties.Add(property);
            }
            //Finally tranform the generated list to an array and store it
            _properties = valid_properties.ToArray();

            //Get fields info
            FieldInfo[] all_fields = _target_action.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<FieldInfo> valid_fields = new List<FieldInfo>();
            //Iterate all the action fields
            foreach(FieldInfo field in all_fields)
            {
                //Blocked fields are not stored
                object[] block_attribute = field.GetCustomAttributes(typeof(BlockedField_GS), true);
                if(block_attribute.Length != 0)
                {
                    continue;
                }
                //Add the non blocked field to the list
                temp_value_list.Add(field.GetValue(_target_action));
                //Add the non blocked field
                valid_fields.Add(field);
            }
            //Finally transform the generated list to an array and store it
            _fields = valid_fields.ToArray();

            //Store the temp values list to the final array
            _values = temp_value_list.ToArray();
        }

        //Loop Methods ====================
        public void DrawUI()
        {
            switch(_UI_mode)
            {
                case EditorUIMode.HIDE_STATE:
                    {
                        DrawHide();
                    }
                    break;
                case EditorUIMode.EDIT_STATE:
                    {
                        DrawInEdit();
                    }
                    break;
                case EditorUIMode.SET_STATE:
                    {

                    }
                    break;
            }

            _target_action.BlitUI();
        }

        private void DrawHide()
        {
            if (GUILayout.Button("Edit"))
            {
                ActionNode_GS_Editor[] action_nodes_editors = NodeEditor_GS.Instance.action_node_editors;
                int num = NodeEditor_GS.Instance.action_node_editors_num;
                for (int k = 0; k < num; k++)
                {
                    action_nodes_editors[k].action_editor.Hide();
                }
                _UI_mode = EditorUIMode.EDIT_STATE;
            }
        }

        private void DrawInEdit()
        {
            GUILayout.BeginVertical();

            //Values local data
            int values_index = 0;
            object current_value = null;

            //Hide button
            if (GUILayout.Button("Hide"))
            {
                _UI_mode = EditorUIMode.HIDE_STATE;
            }

            //Draw properties
            if (_properties.Length > 0)
            {
                //Title
                GUILayout.Label("Properties:");
                //Iterate the previously selected properties
                for (int k = 0; k < _properties.Length; k++)
                {
                    GUILayout.BeginHorizontal();

                    //Scope current value
                    current_value = _values[values_index];

                    if (current_value == null)
                    {
                        if (_properties[k].CanRead == false)
                        {
                            //Only write type
                            GUILayout.Label("no_getter", UIConfig_GS.left_bold_style, GUILayout.MaxWidth(80.0f));
                            //Property name string
                            GUILayout.Label(_properties[k].Name, GUILayout.MaxWidth(100.0f));
                        }
                        else
                        {
                            //In null case show error message
                            GUILayout.Label(_properties[k].Name + " = null", UIConfig_GS.left_bold_red_style);
                        }
                    }
                    else
                    {
                        //Property type string
                        GUILayout.Label(current_value.GetType().ToVariableType().ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(80.0f));
                        //Property name string
                        GUILayout.Label(_properties[k].Name, GUILayout.MaxWidth(100.0f));
                        //Property value
                        if (_properties[k].CanWrite)
                        {
                            //Defined setter case
                            //Generate property UI
                            ProTools.ValueFieldByVariableType(current_value.GetType().ToVariableType(), ref _values[values_index]);
                            //Set property input
                            if (current_value.Equals(_values[values_index]) == false)
                            {
                                _properties[k].GetSetMethod().Invoke(_target_action, new object[] { _values[values_index] });
                            }
                        }
                        else
                        {
                            //Non defined setter case
                            GUILayout.Label(current_value.ToString(), GUILayout.MaxWidth(70.0f));
                        }
                    }

                    //Update values index
                    values_index += 1;

                    GUILayout.EndHorizontal();
                }
            }

            //Draw fields
            if (_fields.Length > 0)
            {
                //Title
                GUILayout.Label("Fields:");
                //Iterate the previously selected fields
                for (int k = 0; k < _fields.Length; k++)
                {
                    GUILayout.BeginHorizontal();

                    //Scope current value
                    current_value = _values[values_index];

                    //In null case show error message
                    if (current_value == null)
                    {
                        GUILayout.Label(_fields[k].Name + " = null", UIConfig_GS.left_bold_red_style);
                    }
                    else
                    {
                        //Field type string
                        GUILayout.Label(current_value.GetType().ToVariableType().ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(80.0f));
                        //Field name string
                        GUILayout.Label(_fields[k].Name, GUILayout.MaxWidth(100.0f));
                        //Field value
                        //Generate field UI
                        ProTools.ValueFieldByVariableType(current_value.GetType().ToVariableType(), ref _values[values_index]);
                        //Set field input
                        if (current_value.Equals(_values[values_index]) == false)
                        {
                            _fields[k].SetValue(_target_action, current_value);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //Update values index
                    values_index += 1;
                }
            }

            GUILayout.EndVertical();
        }

        //FunctionalityMethods ========
        public void Hide()
        {
            if(_UI_mode != EditorUIMode.HIDE_STATE)
            {
                _UI_mode = EditorUIMode.HIDE_STATE;
            }
        }
    }
}
