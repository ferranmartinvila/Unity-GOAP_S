using UnityEngine;
using UnityEditor;
using System;
using GOAP_S.AI;

namespace GOAP_S.Blackboard
{
    [Serializable]
    public class BlackboardComp_GS : MonoBehaviour
    {
        //Content fields
        [NonSerialized] private Agent_GS _target_agent = null; //Blackboard is not independent sorry

        //Loop Methods ================
        private void OnValidate()
        {
            if (agent == null)
            {
                //Destroy blackboard component if there's no agent to link
                Debug.LogError("Blackboard with no agent will be destroyed.");
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
                //In null case blackboard component is destroyed
                if (agent == null)
                {
                    Debug.LogError("Blackboard with no agent will be destroyed.");
                    return null;
                }
                return _target_agent.blackboard;
            }
        }

        public Agent_GS agent
        {
            get
            {
                if (_target_agent == null)
                {
                    //In null case check if there's the component
                    _target_agent = this.GetComponent<Agent_GS>();
                    if (_target_agent == null)
                    {
                        //If agent is null blackboard can't exist
                        EditorApplication.delayCall += () => DestroyImmediate(this);
                    }
                }
                return _target_agent;
            }
            set
            {
                _target_agent = value;
            }
        }

        public TVariable_GS<T> GetVariable<T>(string name)
        {
            return blackboard.GetVariable<T>(name);
        }

        public TVariable_GS<T> SetVariable<T>(string name, object value)
        {
            return blackboard.SetVariable<T>(name, value);
        }

        public T GetValue<T>(string name)
        {
            return blackboard.GetValue<T>(name);
        }
    }
}