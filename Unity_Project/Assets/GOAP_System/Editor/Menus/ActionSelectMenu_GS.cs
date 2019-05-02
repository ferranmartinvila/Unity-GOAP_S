using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;
using GOAP_S.Tools;
using GOAP_S.Planning;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public sealed class ActionSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        private ActionNode_GS_Editor _target_action_node_editor = null; //Focused action node editor
        private ActionNode_GS _target_action_node = null; //Focused action node

        //Selection fields
        private int _action_dropdown_slot = -1;
        private int _selected_action_index = -1;
        
        //Constructors ================
        public ActionSelectMenu_GS(ActionNode_GS_Editor new_target_action_node_editor)
        {
            if (new_target_action_node_editor != null)
            {
                //Focus the action node
                _target_action_node_editor = new_target_action_node_editor;
                _target_action_node = _target_action_node_editor.target_action_node;
            }
            //Get dropdown slot for action select
            _action_dropdown_slot = ProTools.GetDropdownSlot();
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
            GUILayout.Label("Action Select", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Action select dropdown button
            ProTools.GenerateButtonDropdownMenu(ref _selected_action_index, ResourcesTool.action_paths, "Select", false, 200.0f, _action_dropdown_slot);
            if(_selected_action_index != -1)
            {
                //Get selected action script value
                Object script = null;
                ResourcesTool.action_scripts.TryGetValue(ResourcesTool.action_paths[_selected_action_index], out script);
                //Allocate a class with the same type of script value
                Action_GS new_script = ProTools.AllocateClass<Action_GS>(script);
                //Action node case
                if (_target_action_node != null)
                {
                    //Check if the selected action is different to the node action
                    if (_target_action_node.action == null || _target_action_node.action.GetType() != new_script.GetType())
                    {
                        //Set the class name to the new allocated action
                        new_script.name = ResourcesTool.action_paths[_selected_action_index].PathToName();
                        //Set the action target agent
                        new_script.agent = NodeEditor_GS.Instance.selected_agent;
                        //Set the allocated class to the action node
                        _target_action_node.action = new_script;
                        //Set target action node editor action editor
                        _target_action_node_editor.action_editor = new Action_GS_Editor(_target_action_node_editor);
                        //Mark scene dirty
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        //Repaint the node editor to update the UI
                        NodeEditor_GS.Instance.Repaint();
                    }
                }
                //Agent idle action case 
                else
                {
                    Agent_GS target_agent = NodeEditor_GS.Instance.selected_agent;
                    //Check if the selected agent is different to the agent idle action
                    if (target_agent.idle_action == null || target_agent.idle_action.GetType() != new_script.GetType())
                    {
                        //Set the class name to the new allocated action
                        new_script.name = ResourcesTool.action_paths[_selected_action_index].PathToName();
                        //Set the action target agent
                        new_script.agent = NodeEditor_GS.Instance.selected_agent;
                        //Set the agent idle action
                        target_agent.idle_action = new_script;
                        //Mark scene dirty
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        //Repaint the node editor to update the UI
                        NodePlanning_GS.Instance.Repaint();
                    }
                }
                //Close the pop window at the end of the process
                editorWindow.Close();
            }

            //Action create button
            if (GUILayout.Button("Create New", GUILayout.ExpandWidth(true)))
            {
                //Get mouse current position
                Vector2 mousePos = Event.current.mousePosition;
                //Open script creation menu
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ScriptCreationMenu_GS(ScriptCreationMenu_GS.ScriptCreationType.ScriptC_action,_target_action_node.index));
            }

            GUILayout.EndVertical();
        }

        public override void OnClose()
        {
            ProTools.FreeDropdownSlot(_action_dropdown_slot);
        }
    }
}
