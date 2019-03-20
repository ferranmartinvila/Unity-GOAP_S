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
        private KeyValuePair<bool, PropertyInfo> [] _properties = null; //Non blocked properties array, bool is true if the property setter is defined
        private int _properties_num = 0; //Properties num

        //Contructors =================
        public Action_GS_Editor(Action_GS target)
        {
            //Set target action
            _target_action = target;
            //Get action properties info
            List<KeyValuePair<bool, PropertyInfo>> valid_properties = new List<KeyValuePair<bool, PropertyInfo>>();

            PropertyInfo [] all_properties = _target_action.GetType().GetProperties();
            //Iterate all the action properties
            foreach (PropertyInfo property in all_properties)
            {
                //Blocked properties are not stored
                object[] blocked_attribute = property.GetCustomAttributes(typeof(BlockedProperty_GS), true);
                if (blocked_attribute.Length != 0)
                {
                    continue;
                }
                //Try to get property set method
                MethodInfo set_method = property.GetSetMethod();
                //Allocate a new pair with the property and the set method info
                KeyValuePair<bool, PropertyInfo> new_property = new KeyValuePair<bool, PropertyInfo>(set_method != null, property);
                //Add the generated pair in the properties info list
                valid_properties.Add(new_property);
            }
            //Finally tranform the generated list to an array and store it
            _properties = valid_properties.ToArray();
            _properties_num = valid_properties.Count;
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
            
            //Hide button
            if (GUILayout.Button("Hide"))
            {
                _UI_mode = EditorUIMode.HIDE_STATE;
            }
            //Draw properties
            //Title
            GUILayout.Label("Properties:");
            //Iterate the previiously selected properties
            for(int k = 0; k < _properties_num; k++)
            {
                GUILayout.BeginHorizontal();

                //Get property value in the target action instance
                object value = _properties[k].Value.GetGetMethod().Invoke(_target_action, null);
                //In null case show error message
                if (value == null)
                {
                    GUILayout.Label(_properties[k].Value.Name + " = null", UIConfig_GS.left_bold_red_style);
                }
                else
                {
                    //Property field type string
                    GUILayout.Label(value.GetType().ToVariableType().ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(60.0f));
                    //Property name string
                    GUILayout.Label(_properties[k].Value.Name, GUILayout.MaxWidth(80.0f));
                    //Property value
                    if (_properties[k].Key == true)
                    {
                        //Defined setter case
                        //Generate field UI
                        ProTools.ValueFieldByVariableType(value.GetType().ToVariableType(), ref value);
                        //Set field input
                        _properties[k].Value.GetSetMethod().Invoke(_target_action, new object[] { value });
                    }
                    else
                    {
                        //Non defined setter case
                        GUILayout.Label(value.ToString(), GUILayout.MaxWidth(70.0f));
                    }
                }
                GUILayout.EndHorizontal();
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
