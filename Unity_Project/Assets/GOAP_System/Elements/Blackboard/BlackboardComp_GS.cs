using UnityEngine;
using UnityEditor;
using System;
using GOAP_S.AI;

namespace GOAP_S.Blackboard
{
    public class BlackboardComp_GS : MonoBehaviour
    {
        //Content fields
        [NonSerialized] private Agent_GS _target_agent = null; //Blackboard is not independent sorry

        //Loop Methods ================
        private void OnValidate()
        {
            if(gameObject.GetComponent<Agent_GS>() == null)
            {
                //Destroy blackboard component if there's no agent to link
                EditorApplication.delayCall += () => DestroyImmediate(this);
            }
        }

        private void Awake()
        {
            //Initialize variables bindings
            foreach (Variable_GS variable in blackboard.variables.Values)
            {
                //Init the current variable bind and the target game object is the blackboard container
                variable.InitializeBinding(gameObject);
            }
        }

        //Get/set Methods =============
        public Blackboard_GS blackboard
        {
            get
            {
                if (_target_agent == null)
                {
                    DestroyImmediate(this);
                    return null;
                }
                else return _target_agent.blackboard;
            }
        }

        public Agent_GS agent
        {
            get
            {
                return _target_agent;
            }
            set
            {
                _target_agent = value;
            }
        }
    }
}