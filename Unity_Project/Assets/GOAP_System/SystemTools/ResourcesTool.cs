using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GOAP_S.Planning;
using GOAP_S.AI;

namespace GOAP_S.Tools
{
    [InitializeOnLoad]
    public sealed class ResourcesTool
    {
        //Content fields
        private static Dictionary<string, UnityEngine.Object> _action_scripts = null;
        private static string[] _action_paths = null;
        private static Dictionary<string, UnityEngine.Object> _agent_behaviour_scripts = null;
        private static string[] _behaviour_paths = null;

        private static string[] _assets_folders = null;

        //Constructors ================
        static ResourcesTool()
        {
            //Get action and agent behaviour scripts in the current project
            ListScripts();
            //List all assets folder
            _assets_folders = ListFoldersIn("Assets").ToArray();
        }

        //Functionality Methods =======
        private static void ListScripts()
        {
            //First check dictionaries allocation
            if (_action_scripts == null)
            {
                _action_scripts = new Dictionary<string, Object>();
            }
            else
            {
                //Clear if is already allocated
                _action_scripts.Clear();
            }
            if (_agent_behaviour_scripts == null)
            {
                _agent_behaviour_scripts = new Dictionary<string, Object>();
            }
            else
            {
                //Clear if is already allocated
                _agent_behaviour_scripts.Clear();
            }

            //A list of all the object assets imported to the project
            List<MonoScript> object_assets = ProTools.FindAssetsByType<MonoScript>();

            //The class attribute that we use to identify if a class inherit from action class       
            object action_attribute = typeof(Action_GS).GetCustomAttributes(false)[0];
            //The class attribute that we use to identify if a class inherit from agent behaviour class       
            object behaviour_attribute = typeof(AgentBehaviour_GS).GetCustomAttributes(false)[0];

            //Iterate the assets list
            foreach (MonoScript script in object_assets)
            {
                //Get class type
                System.Type class_type = script.GetClass();
                if (class_type == null || class_type == typeof(Action_GS) || class_type == typeof(AgentBehaviour_GS)) continue;

                //Array with the class custom attributes
                object[] script_attributes = class_type.GetCustomAttributes(true);

                //Iterate the script attributes
                foreach (object script_attribute in script_attributes)
                {
                    //If there's the action attribute the script inherit from action script
                    if (script_attribute.Equals(action_attribute))
                    {
                        //Add the script to the action scripts dic
                        _action_scripts.Add(AssetDatabase.GetAssetPath(script).Substring(7), script);
                    }
                    //If there's the agent behaviour attribute the script inherit from agent behaviour script
                    else if (script_attribute.Equals(behaviour_attribute))
                    {
                        //Add the script to the agent behaviour scripts dic
                        _agent_behaviour_scripts.Add(AssetDatabase.GetAssetPath(script).Substring(7), script);
                    }
                }
            }

            //Generate action scripts paths 
            _action_paths = new string[_action_scripts.Count];
            _action_scripts.Keys.CopyTo(_action_paths, 0);

            //Generate agent behaviour scripts paths
            _behaviour_paths = new string[_agent_behaviour_scripts.Count];
            _agent_behaviour_scripts.Keys.CopyTo(_behaviour_paths, 0);
        }

        private static List<string> ListFoldersIn(string target_folder)
        {
            List<string> folders_list = new List<string>();
            string[] folders = AssetDatabase.GetSubFolders(target_folder);
            
            foreach(string folder in folders)
            {
                folders_list.Add(folder);
                List<string> folder_in_list = ListFoldersIn(folder);
                foreach (string f_in in folder_in_list)
                {
                    folders_list.Add(f_in);
                }
            }
            return folders_list;
        }

        //Get/Set Methods =============
        public static Dictionary<string, UnityEngine.Object> action_scripts
        {
            get
            {
                if(_action_scripts == null)
                {
                    ListScripts();
                }
                return _action_scripts;
            }
        }

        public static string[] action_paths
        {
            get
            {
                return _action_paths;
            }
        }

        public static Dictionary<string, UnityEngine.Object> agent_behaviour_scripts
        {
            get
            {
                if(_agent_behaviour_scripts == null)
                {
                    ListScripts();
                }
                return _agent_behaviour_scripts;
            }
        }

        public static string[] behaviour_paths
        {
            get
            {
                return _behaviour_paths;
            }
        }

        public static string[] assets_folders
        {
            get
            {
                if(_assets_folders == null)
                {
                    _assets_folders = ListFoldersIn("Assets").ToArray();
                }
                return _assets_folders;
            }
        }
    }
}
