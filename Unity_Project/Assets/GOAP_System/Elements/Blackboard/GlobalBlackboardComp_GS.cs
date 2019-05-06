using System.Collections.Generic;
using UnityEngine;
using System;

namespace GOAP_S.Blackboard
{
    [Serializable]
    public class GlobalBlackboardComp_GS : MonoBehaviour, ISerializationCallbackReceiver
    {
        //Content fields
        [NonSerialized] private static Blackboard_GS _blackboard = null;
        [SerializeField] private string serialized_blackboard = null; //String where the blackboard is serialized
        [SerializeField] private List<UnityEngine.Object> obj_refs = null; //List that contains the references to the objects serialized

        //Get/Set Methods =============
        public Blackboard_GS blackboard
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

        //Serialization Methods =======
        public void OnBeforeSerialize()//Serailize
        {
            //Serialize blackboard
            if (blackboard != null)
            {
                serialized_blackboard = Serialization.SerializationManager.Serialize(_blackboard, typeof(Blackboard_GS), obj_refs);
            }
        }

        public void OnAfterDeserialize()
        {
            //Deserialize blackboard
            if (string.IsNullOrEmpty(serialized_blackboard))
            {
                _blackboard = new Blackboard_GS(null);
            }
            else
            {
                _blackboard = (Blackboard_GS)Serialization.SerializationManager.Deserialize(typeof(Blackboard_GS), serialized_blackboard, obj_refs);
                if (_blackboard == null)
                {
                    _blackboard = new Blackboard_GS(null);
                }
            }
        }
    }
}
