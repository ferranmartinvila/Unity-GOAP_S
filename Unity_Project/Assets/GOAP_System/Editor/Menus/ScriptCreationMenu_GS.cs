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
        public delegate void ScriptCreationCallbackFunction();
        public static ScriptCreationCallbackFunction on_script_creation_cancel_delegate;
        //Config fields
        private ScriptCreationType _script_creation_type = ScriptCreationType.ScriptC_undefined; //The script type we are going to generate
        private int _node_index = 0; //
        private string _window_title = null;
        private string _new_script_name = "";
        private string _new_script_full_path = null;
        private int _folder_dropdown_slot = -1;
        private int _folder_selected_index = -1;

        //Constructors ================
        public ScriptCreationMenu_GS(ScriptCreationType new_creation_type, int node_index = 0)
        {
            //Set script creation type
            _script_creation_type = new_creation_type;
            //Set action node case index
            _node_index = node_index;

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
                    GameObject temp_compilation_obj = new GameObject();
                    temp_compilation_obj.transform.parent = Selection.activeGameObject.transform;

                    ScriptCreationSetter_GS temp_comp = temp_compilation_obj.AddComponent<ScriptCreationSetter_GS>();
                    temp_comp._script_full_path = _new_script_full_path;
                    temp_comp._script_creation_type = _script_creation_type;
                    temp_comp._node_index = _node_index;
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
    }

    [ExecuteInEditMode]
    public class ScriptCreationSetter_GS : MonoBehaviour
    {
        [SerializeField] public int _node_index = 0;
        [SerializeField] public ScriptCreationMenu_GS.ScriptCreationType _script_creation_type = ScriptCreationMenu_GS.ScriptCreationType.ScriptC_undefined;
        [SerializeField] public string _script_full_path;
        [NonSerialized] private int try_count = 0;

        private void Update()
        {
            //Try to get the script in the action scripts resources tool
            UnityEngine.Object script = null;
            switch(_script_creation_type)
            {
                case ScriptCreationMenu_GS.ScriptCreationType.ScriptC_action:
                    {
                        if (ResourcesTool.action_scripts.TryGetValue(_script_full_path.Substring(7), out script))
                        {
                            //Allocate a class with the same type of script value
                            Planning.Action_GS _new_script = ProTools.AllocateClass<Planning.Action_GS>(script);
                            //Set new script name
                            _new_script.name = _script_full_path.PathToName();
                            //Place the new action in the selected action node
                            NodeEditor_GS.Instance.selected_agent.action_nodes[_node_index].action = _new_script;
                            //Set action node editor action editor
                            NodeEditor_GS.Instance.action_node_editors[_node_index].action_editor = new Action_GS_Editor(NodeEditor_GS.Instance.action_node_editors[_node_index]);
                            //This dummy gameobject job is done, we can delete it
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    break;
                case ScriptCreationMenu_GS.ScriptCreationType.ScriptC_behaviour:
                    {
                        if (ResourcesTool.agent_behaviour_scripts.TryGetValue(_script_full_path.Substring(7), out script))
                        {
                            //Allocate a class with the same type of script value
                            AI.AgentBehaviour_GS _new_script = ProTools.AllocateClass<AI.AgentBehaviour_GS>(script);
                            //Set new script name
                            _new_script.name = _script_full_path.PathToName();
                            //Place the new action in the selected action node
                            NodeEditor_GS.Instance.selected_agent.behaviour = _new_script;
                            //Update node planning canvas
                            NodePlanning_GS.Instance.Repaint();
                            //This dummy gameobject job is done, we can delete it
                            DestroyImmediate(gameObject);
                        }
                    }
                    break;
            }
            //Count test
            try_count += 1;
            if (try_count > ProTools.TRIES_LIMIT)
            {
                //Warning message display
                Debug.LogWarning("New script: " + _script_full_path.PathToName() + " target not found!");
                //Destroy gameobject
                DestroyImmediate(gameObject);
            }
        }
    }
}

