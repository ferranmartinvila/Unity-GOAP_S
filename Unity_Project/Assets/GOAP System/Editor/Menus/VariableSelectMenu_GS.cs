using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private NodeEditor_GS _target_node_editor = null; //Node editor where this window is shown
        static private string _variable_name;
        //static private System.Enum _variable_type = GOAP_S.PT.NodeUIMode;

        //Contructors =================
        public VariableSelectMenu_GS(NodeEditor_GS target_node_editor)
        {
            //Set the node editor
            _target_node_editor = target_node_editor;
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Variable Select", _target_node_editor.UI_configuration.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Variable name field
            _variable_name = EditorGUILayout.TextField("Name",_variable_name,GUILayout.Width(120), GUILayout.ExpandWidth(true));
            //Variable type field
           // _variable_type = EditorGUILayout.EnumFlagsField("Type",_variable_type);
            //Variable value field

        }

        public override void OnClose()
        {
            _variable_name = "";
        }

        //Functionality Methods =======
        public Variable_GS GenerateVariable()
        {
            return null;
        }
    }
}
