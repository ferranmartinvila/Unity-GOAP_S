using System.Collections.Generic;
using GOAP_S.Planning;
using GOAP_S.Tools;
using UnityEngine;

namespace GOAP_S.Planning
{
    public class WorldState_GS
    {
        private string _name = null; //Name to differentiate world states/goals
        private Dictionary<string, Property_GS> _world_state = null; //Properties that define the world state

        //Constructors ====================
        public WorldState_GS()
        {
            //Initialize world state properties dic
            _world_state = new Dictionary<string, Property_GS>();
        }

        //Functionality Methods ===========
        public void SetGoal(string variable_name, Property_GS variable)
        {
            _world_state.Add(variable_name, variable);
        }

        public void RemoveGoal(string variable_name)
        {
            _world_state.Remove(variable_name);
        }

        public void ClearAllGoals()
        {
            _world_state.Clear();
        }

        public bool Compare(ref WorldState_GS target)
        {
            //Get target world state properties
            Dictionary<string, Property_GS> target_properties = target.properties;
            //Iterate target world state properties
            foreach(KeyValuePair<string, Property_GS> target_property in target_properties)
            {
                Property_GS this_property;
                //Find the property
                if (_world_state.TryGetValue(target_property.Key, out this_property))
                {
                    //Check properties
                    if(this_property.Check(target_property.Value) == true)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                //If the property is not found the goal world state is not valid
                else
                {
                    Debug.LogError("Goal world state: " + target.name + " defines the variable: " + target_property.Key + " and it is not defined in the current world state!");
                    return false;
                }
            }

            //In no goals defined case, also return true, because there's no need to calculate a plan
            if (target_properties.Count == 0)
            {
                Debug.Log("Goal world state: " + target.name + " has no goals defined!");
            }

            return true;
        }

        //Get/Set Methods =================
        public string name
        {
            get
            {
                return _name;
            }
        }

        public Dictionary<string, Property_GS> properties
        {
            get
            {
                return _world_state;
            }
        }
    }
}
