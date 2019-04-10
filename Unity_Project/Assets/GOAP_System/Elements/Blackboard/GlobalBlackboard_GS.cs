using System;
using UnityEditor;
using UnityEngine;

namespace GOAP_S.Blackboard
{
    [InitializeOnLoad, Serializable]
    public static class GlobalBlackboard_GS
    {
        //Content fields
        [SerializeField] private static Blackboard_GS _blackboard = null;

        //Get/Set Methods =============
        public static Blackboard_GS blackboard
        {
            get
            {
                if(_blackboard == null)
                {
                    _blackboard = new Blackboard_GS(null);
                }
                return _blackboard;
            }
        }
    }
}
