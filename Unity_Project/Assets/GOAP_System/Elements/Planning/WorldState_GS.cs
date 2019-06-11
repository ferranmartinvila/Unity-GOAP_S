using System.Collections.Generic;
using GOAP_S.Planning;
using GOAP_S.Tools;
using UnityEngine;

namespace GOAP_S.Planning
{
    public class WorldState_GS
    {
        private string _name = null; //Name to differentiate world states/goals

        private Dictionary<string, Property_GS> _world_state = null;


        
        //Properties that define the world state

        //Constructors ====================
        public WorldState_GS()
        {
            //Initialize world state properties dic
            _world_state = new Dictionary<string, Property_GS>();
        }

        public WorldState_GS(WorldState_GS copy)
        {
            _name = copy.name;
            //Allocate the world state properties dic
            _world_state = new Dictionary<string, Property_GS>();
            //Iterate the clone target and copy all its properties
            foreach(KeyValuePair<string,Property_GS> copy_pair in copy.properties)
            {
                SetGoal(copy_pair.Key, copy_pair.Value);
            }
        }

        //Planning Methods ===========-
        public void SetGoal(string variable_name, Property_GS variable)
        {
            Property_GS current_property = null;
            if (_world_state.TryGetValue(variable_name, out current_property))
            {
                //Apply the target variable operation to the current property value
                current_property.ApplyPropertyEffect(variable);
            }
            else
            {
                Property_GS new_variable = new Property_GS(variable);
                _world_state.Add(variable_name, new_variable);
            }
        }

        public void RemoveGoal(string variable_name)
        {
            _world_state.Remove(variable_name);
        }

        public void ClearAllGoals()
        {
            _world_state.Clear();
        }

        public void MixGoals(WorldState_GS mix_source)
        {
            foreach (KeyValuePair<string, Property_GS> source_pair in mix_source.properties)
            {
                SetGoal(source_pair.Key, source_pair.Value);
            }
        }

        public float DistanceTo(WorldState_GS target, bool no_dist = false)
        {
            //Summatory of all the variables distance
            float total_distance = 0;

            //Get target world state properties
            Dictionary<string, Property_GS> target_properties = target.properties;
            //Iterate target world state properties
            foreach (KeyValuePair<string, Property_GS> target_property in target_properties)
            {
                Property_GS this_property;
                //Find the property
                if (_world_state.TryGetValue(target_property.Key, out this_property))
                {
                    //Check properties
                    total_distance += this_property.DistanceTo(target_property.Value, no_dist);
                }
                //If the property is not found the goal world state is not valid
                else
                {
                    Debug.LogError("Goal world state: " + target.name + " defines the variable: " + target_property.Key + " and it is not defined in the current world state!");
                    return 0;
                }
            }

            //In no goals defined case, also return true, because there's no need to calculate a plan
            if (target_properties.Count == 0)
            {
                Debug.Log("Goal world state: " + target.name + " has no goals defined!");
            }

            return total_distance;
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

        public Property_GS GetProperty(string name)
        {
            Property_GS found_prop = null;
            if (!_world_state.TryGetValue(name, out found_prop))
            {
                Debug.LogError("Property: " + name + " not found in the world state: " + _name);
            }
            return found_prop;
        }
    }
}
