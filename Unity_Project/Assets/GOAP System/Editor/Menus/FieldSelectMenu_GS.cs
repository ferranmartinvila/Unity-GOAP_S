using UnityEngine;
using UnityEditor;
using System.Linq;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public class FieldSelectMenu_GS : ScriptableWizard
    {
        //Content fields
        private Agent_GS _selected_agent = null;
        private int _selected_field_index = -1;
        private string _selected_field_path = "";
        private string[] _paths;

        //Loop methods ================
        private void OnFocus()
        {
            int total_paths_count = 0;

            //Count blackboard fields
            int blackboard_fields_count = _selected_agent.blackboard.variables.Count;
            total_paths_count += blackboard_fields_count;

            //Count game object fields

            //Count components fields
            int components_count = _selected_agent.gameObject.GetComponents(typeof(Component)).Length;
            Component[] agent_components = _selected_agent.GetComponents(typeof(Component));

            foreach(Component comp in agent_components)
            {
                total_paths_count += comp.GetType().GetFields().Length;
            }

            
            //Add blackboard fields

            //Add game object fields

            //Add components fields
        }

        //Funciotnality methods =======
        public void CreateDropdown(Agent_GS target)
        {
            //Set the agent where wi search fields
            _selected_agent = target;
            //Display dropdown
            DisplayWizard<FieldSelectMenu_GS>("Field Select Menu");
        }

        //Get/Set methods =============
        public string selected_field_path
        {
            get
            {
                if (_selected_field_index == 0 || _selected_field_index > _paths.Length - 1)
                {
                    return "NO_FIELD_PATH";
                }
                else
                {
                    return _paths[_selected_field_index];
                }
            }
        }

        private string selected_short_path
        {
            get
            {
                if(_selected_field_index < 0 ||_selected_field_index > _paths.Length - 1)
                {
                    return string.Empty;
                }
                else
                {
                    return selected_field_path.Split(new char[] { '/', '\\' }).Last();
                }
            }
        }
    }
}
