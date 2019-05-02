using System;
using UnityEngine;

namespace GOAP_S.Blackboard
{
    public class GlobalBlackboard_GS
    {
        //Content fields
        [NonSerialized] private static GlobalBlackboardComp_GS _blackboard_component = null; //Component that provides a scene instance of the global blackboard
        
        //Get/Set Methods =============
        public static Blackboard_GS blackboard
        {
            get
            {
                //First Check if the component exists
                if (_blackboard_component == null)
                {
                    //Try to find the component in the scene
                    _blackboard_component = GameObject.FindObjectOfType<GlobalBlackboardComp_GS>();
                    //In null case we create a new gameobject and component
                    if (_blackboard_component == null)
                    {
                        GameObject blackboard_object = new GameObject("Global Blackboard");
                        _blackboard_component = blackboard_object.AddComponent<GlobalBlackboardComp_GS>();
                    }
                }
                return _blackboard_component.blackboard;
            }
        }
    }
}
