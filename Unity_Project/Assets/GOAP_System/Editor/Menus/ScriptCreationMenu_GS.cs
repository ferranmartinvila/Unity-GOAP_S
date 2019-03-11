using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
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
        private static ScriptCreationMenu_GS _this;
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
        private static string _new_script_full_path = null;
        private int _folder_dropdown_slot = -1;
        private int _folder_selected_index = -1;

        //Constructors ================
        public ScriptCreationMenu_GS(object data, ScriptCreationType new_creation_type)
        {
            //Set statis this
            _this = this;
            //Set script creation type
            _script_creation_type = new_creation_type;
            //Set target data
            _target_data = data;

            //Check what 
            switch (_script_creation_type)
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
            ProTools.GenerateButtonDropdownMenu(ref _folder_selected_index, ResourcesTool.assets_folders, _folder_selected_index == -1 ? "Not Set" : ResourcesTool.assets_folders[_folder_selected_index].FolderToName(), false, 150.0f, _folder_dropdown_slot);
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
                //Adapt class name (class name can't have spaces)
                _new_script_name = _new_script_name.Replace(' ', '_');
                //Generate new script path
                _new_script_full_path = ResourcesTool.assets_folders[_folder_selected_index] + '/' + _new_script_name + ".cs";
                //Check if the file exists
                if (File.Exists(_new_script_full_path) == true)
                {
                    //Script already exists case
                    Debug.LogWarning("Already exists a script named: " + _new_script_name + "in the folder: " + ResourcesTool.assets_folders[_folder_selected_index]);
                }
                else
                {
                    //New script case
                    using (StreamWriter new_script = new StreamWriter(_new_script_full_path))
                    {
                        switch (_script_creation_type)
                        {
                            case ScriptCreationType.ScriptC_action:
                                {
                                    //Set new action script code
                                    new_script.WriteLine("using UnityEngine;");
                                    new_script.WriteLine("using GOAP_S.Planning;");
                                    new_script.WriteLine("public class " + _new_script_name + " : Action_GS");
                                    new_script.WriteLine("{");
                                    //TODO
                                    new_script.WriteLine("}");
                                }
                                break;
                            case ScriptCreationType.ScriptC_behaviour:
                                {
                                    //Set new behaiour script code
                                    new_script.WriteLine("using UnityEngine;");
                                    new_script.WriteLine("using GOAP_S.AI;");
                                    new_script.WriteLine("public class " + _new_script_name + " : AgentBehaviour_GS");
                                    new_script.WriteLine("{");
                                    //TODO
                                    new_script.WriteLine("}");
                                }
                                break;
                        }
                    }

                    //Refresh assets
                    AssetDatabase.Refresh();
                    //Load asset and save it in the generated script static object to use it in future actions
                    ScriptCreationCompile._Instance.Create();    
                /*if (!ScriptCreationCompile.IsOpen())
                    {
                        ScriptCreationCompile creation_compile = Selection.activeGameObject.AddComponent(typeof(ScriptCreationCompile).ToString());
                    }
                    else ScriptCreationCompile.Create();*/
                }

                /*//Generate the selected script at the selected path
                MonoScript new_script = new MonoScript();
                //Create the monoscript asset
                AssetDatabase.CreateAsset(new_script, ResourcesTool.assets_folders[_folder_selected_index] + '/' + _new_script_name + ".cs");
                //Save it at the generated script object for future actions
                _generated_script = new_script;*/


                /*//Check if there's a method to call
                if (on_script_creation_delegate != null)
                {
                    //Call the script creation delegate
                    on_script_creation_delegate();
                    //Reset on script creation delegate
                    on_script_creation_delegate = null;
                }
                */
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

        //Functionality Methods =======
        public static void DoCreateActions()
        {
            //Check if there's a method to call
            if (on_script_creation_delegate != null)
            {
                //Call the script creation delegate
                on_script_creation_delegate();
                //Reset on script creation delegate
                on_script_creation_delegate = null;
            }
        }

        public static void SaveGeneratedScript()
        {
            _generated_script = AssetDatabase.LoadAssetAtPath(_new_script_full_path, Type.GetType(_new_script_full_path));
            Debug.Log(_generated_script);
        }
    }

    [InitializeOnLoad]
    public class ScriptCreationCompile : MonoBehaviour
    {
        public static ScriptCreationCompile _Instance;
        private static Action action_after_compiling;

        private ScriptCreationCompile()
        {
            _Instance = this;
            Debug.LogError(ScriptCreationMenu_GS.on_script_creation_delegate);
            if(ScriptCreationMenu_GS.on_script_creation_delegate != null)
            {
                Create();
            }
        }

        public static bool IsOpen()
        {
            return _Instance != null;
        }

        static IEnumerator DoActionAfterCompiling()
        {
            while (EditorApplication.isCompiling)
            {
                yield return null;
            }
            action_after_compiling();
            action_after_compiling = null;
        }

        public void Create()
        {
            action_after_compiling += () => ScriptCreationMenu_GS.SaveGeneratedScript();
            action_after_compiling += () => ScriptCreationMenu_GS.DoCreateActions();
            
            StartCoroutine(DoActionAfterCompiling());

        }
    }
}

