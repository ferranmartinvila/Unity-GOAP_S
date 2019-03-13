using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using GOAP_S.AI;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    public sealed class BehaviourSelectMenu_GS : PopupWindowContent
    {

        //Content fields
        private Agent_GS _target_agent = null;
        //Selection fields 
        private int _behaviour_dropdown_slot = -1;
        private int _selected_behaviour_index = -1;

        //Constructors ================
        public BehaviourSelectMenu_GS(Agent_GS new_target_agent)
        {
            //Set the target agent
            _target_agent = new_target_agent;
            //Get dropdown slot for behaviour select
            _behaviour_dropdown_slot = ProTools.GetDropdownSlot();
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            editorWindow.minSize = new Vector2(200.0f, 100.0f);
            editorWindow.maxSize = new Vector2(200.0f, 100.0f);

            GUILayout.BeginVertical();

            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Behaviour Select", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Behaviour select dropdown button
            ProTools.GenerateButtonDropdownMenu(ref _selected_behaviour_index, ResourcesTool.behaviour_paths, "Select", false, 200.0f, _behaviour_dropdown_slot);
            if(_selected_behaviour_index != -1)
            {
                //Get selected behaviour script
                Object script = null;
                ResourcesTool.agent_behaviour_scripts.TryGetValue(ResourcesTool.behaviour_paths[_selected_behaviour_index], out script);
                //Allocate a class with the same type of script value
                AgentBehaviour_GS new_script = ProTools.AllocateClass<AgentBehaviour_GS>(script);
                //Check if the selected behaviour is different to the target agent one
                if (_target_agent.behaviour == null || new_script.GetType() != _target_agent.behaviour.GetType())
                {
                    //Set behaviour name
                    new_script.name = ResourcesTool.behaviour_paths[_selected_behaviour_index].PathToName();
                    //Set behaviour target agent
                    new_script.agent = _target_agent;
                    //Set agent behaviour
                    _target_agent.behaviour = new_script;
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //Repaint the node editor to update the UI
                    if (NodePlanning_GS.IsOpen())
                    {
                        NodePlanning_GS.Instance.Repaint();
                    }
                }
                //Close the pop window at the end of the process
                editorWindow.Close();
            }

            //Behaviour create button
            if (GUILayout.Button("Create New", GUILayout.ExpandWidth(true)))
            {
                //Add popup close on new behaviour script creation
                //ScriptCreationMenu_GS.on_script_creation_delegate += () => editorWindow.Close();

                //Get mouse current position
                Vector2 mousePos = Event.current.mousePosition;
                //Open script creation menu
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ScriptCreationMenu_GS(ScriptCreationMenu_GS.ScriptCreationType.ScriptC_behaviour));
            }

            GUILayout.EndVertical();
        }
    }
}
