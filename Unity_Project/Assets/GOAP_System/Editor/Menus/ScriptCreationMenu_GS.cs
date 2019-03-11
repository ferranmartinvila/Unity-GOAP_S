using UnityEngine;
using UnityEditor;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    public class ScriptCreationMenu_GS : PopupWindowContent
    {
        //Script creation type
        public enum ScriptCreationType
        {
            ScriptC_undefined = 0,
            ScriptC_action,
            ScriptC_behaviour
        }

        //Content fields
        public delegate void ScriptCreationCallbackFunction();
        public static ScriptCreationCallbackFunction on_script_creation_delegate;
        public static ScriptCreationCallbackFunction on_script_creation_cancel_delegate;
        //Config fields
        private ScriptCreationType _script_creation_type = ScriptCreationType.ScriptC_undefined; //The script type we are going to generate
        private object _target_data = null;
        private string _window_title = null;
        private string _new_script_name = "";
        private string _new_script_folder = "";
        private static object _generated_script = null;
        private int _folder_dropdown_slot = -1;
        private int _folder_selected_index = -1;

        //Constructors ================
        public ScriptCreationMenu_GS(object data, ScriptCreationType new_creation_type)
        {
            //Set script creation type
            _script_creation_type = new_creation_type;
            //Set target data
            _target_data = data;

            //Check what 
            switch (new_creation_type)
            {
                case ScriptCreationType.ScriptC_action:
                    {
                        _window_title = "Action";
                    }
                    break;
                case ScriptCreationType.ScriptC_behaviour:
                    {
                        _window_title = "Behaviour";
                    }
                    break;
            }

            //Get dropdown slot for folder selection
            _folder_dropdown_slot = ProTools.GetDropdownSlot();
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            editorWindow.minSize = new Vector2(200.0f, 120.0f);
            editorWindow.maxSize = new Vector2(200.0f, 120.0f);

            GUILayout.BeginVertical();

            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(_window_title + "Script Creation", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Folder area
            GUILayout.BeginHorizontal();
            //Folder label
            GUILayout.Label("Folder:", GUILayout.MaxWidth(50.0f));
            //Folder dropdown button
            ProTools.GenerateButtonDropdownMenu(ref _folder_selected_index, ResourcesTool.assets_folders,_folder_selected_index == -1 ? "Not Set" : ResourcesTool.assets_folders[_folder_selected_index].FolderToName(), false, 150.0f, _folder_dropdown_slot);
            GUILayout.EndHorizontal();

            //Name Text field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.MaxWidth(50.0f));
            _new_script_name = EditorGUILayout.TextField("", _new_script_name, GUILayout.MaxWidth(140.0f));
            GUILayout.EndHorizontal();

            //Flexible space separation
            GUILayout.FlexibleSpace();

            //Create/Cancel buttons
            GUILayout.BeginHorizontal();
            //Create
            if (GUILayout.Button("Create", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true))
                && _folder_selected_index != -1 //Path is set
                && !string.IsNullOrEmpty(_new_script_name)) //Name is set
            {
                //First generate the selected script at the selected path
                MonoScript new_script = new MonoScript();
                //Create the monoscript asset
                AssetDatabase.CreateAsset(new_script, ResourcesTool.assets_folders[_folder_selected_index] + '/' + _new_script_name + ".cs");
                //Save it at the generated script object for future actions
                _generated_script = new_script;

                //Check if there's a method to call
                if(on_script_creation_delegate != null)
                {
                    //Call the script creation delegate
                    on_script_creation_delegate();
                    //Reset on script creation delegate
                    on_script_creation_delegate = null;
                }

                //Close popup window
                editorWindow.Close();
            }
            //Cancel
            if (GUILayout.Button("Cancel", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                //First check if there's a method to call
                if (on_script_creation_cancel_delegate != null)
                {
                    //Call the script creation cancel delegate
                    on_script_creation_cancel_delegate();
                    //Reset on script creation cancel delegate
                    on_script_creation_cancel_delegate = null;
                }

                //Close popup window
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public override void OnClose()
        {
            ProTools.FreeDropdownSlot(_folder_dropdown_slot);
        }

        //Get/Set Methods =============
        public static object generated_script
        {
            get
            {
                return _generated_script;
            }
        }
    }
}

