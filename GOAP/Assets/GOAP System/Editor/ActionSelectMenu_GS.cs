using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace GOAP_S.UI
{
    
    public class ActionSelectMenu_GS : PopupWindowContent
    {
        //UI fields
        [System.NonSerialized] static string header; //Header string
        [System.NonSerialized] static GUIStyle header_style; //Header style
        [System.NonSerialized] static Rect main_rect; //Menu window rect
        //Content fields
        [System.NonSerialized] static private ActionNode_GS_Editor target = null; //Focused Node Action
        [System.NonSerialized] static private Dictionary<string, UnityEngine.Object> all_action_scripts = new Dictionary<string, UnityEngine.Object>(); //Action scripts dic

        //Constructors ================
        public ActionSelectMenu_GS(ActionNode_GS_Editor _target)
        {
           //Set header string
            header = "Action Select";
            //Configure the header style
            header_style = new GUIStyle("label");
            header_style.alignment = TextAnchor.UpperCenter;
            header_style.fontSize = 15;
            header_style.fontStyle = FontStyle.Bold;
            //Focus the action node
            target = _target;
            //List all action scripts
            ListActionScripts();
        }

        //This is the correct way to change a window size(I dont like it :v)
        public override Vector2 GetWindowSize()
        {
            return new Vector2(300,300);
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Beginning are
            editorWindow.BeginWindows();
            
            //Create the main 
            main_rect = new Rect(rect);

            //Begin de menu area
            GUILayout.BeginArea(main_rect);
            //Mark - header space
            GUILayout.Space(5);

            //Header
            GUILayout.Label(header, header_style);

            GUILayout.BeginArea(new Rect(main_rect.x + 25, main_rect.y + 50, main_rect.width, main_rect.height));
            //Show all the action scripts listed
            foreach (KeyValuePair<string, UnityEngine.Object> script in all_action_scripts)
            {
                if (GUILayout.Button(script.Value.name, GUILayout.Width(150), GUILayout.Height(30)))
                {
                    //Allocate a class with the same type of script value
                    Action_GS new_script = GOAP_S.PT.ProTools.AllocateClass<Action_GS>(script.Value);
                    //Set the allocated class to the action node
                    target.SetAction(new_script);
                    //Close the pop window when the action is selected & set
                    editorWindow.Close();
                }
                GUILayout.Space(2);
            }

            GUILayout.EndArea();

            GUILayout.EndArea();

            //Ending areas
            editorWindow.EndWindows();
        }

        //Functionality Methods =======
        public static void ListActionScripts()
        {
            //Clear the action scripts dic to place the current action scripts
            all_action_scripts.Clear();

            //A list of all the object assets imported to the project
            List<UnityEngine.Object> object_assets = PT.ProTools.FindAssetsByType<UnityEngine.Object>();

            //The action attribute that we use to identify if a class inherit from action class       
            object action_attribute = typeof(Action_GS).GetCustomAttributes(false)[0];

            //Iterate the assets list
            foreach (UnityEngine.Object script in object_assets)
            {
                //Check if the object is a script
                if (script.GetType() == typeof(MonoScript))
                {
                    //Get class type
                    System.Type class_ty = ((MonoScript)script).GetClass();
                    if (class_ty == null) continue;

                    //Array with the class custom attributes
                    object[] script_attributes = class_ty.GetCustomAttributes(true);

                    //Iterate the script attributes
                    foreach (object script_attribute in script_attributes)
                    {
                        //If there's the action attribute the script inherit from action script
                        if (script_attribute.Equals(action_attribute) && class_ty != typeof(Action_GS))
                        {
                            //Add the script to the action scripts dic
                            all_action_scripts.Add(script.name, script);
                        }
                    }
                }
            }
        }
    }
}
