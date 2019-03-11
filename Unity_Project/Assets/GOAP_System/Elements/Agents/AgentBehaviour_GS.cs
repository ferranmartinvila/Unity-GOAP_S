using UnityEngine;
using System;

namespace GOAP_S.AI
{
    //AgentBehaviour class attribute
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class AgentBehaviour_Attribute_GS: Attribute
    {

    }

    //Agent behaviour have a custom class attribute usefull to detect agent behaviour derived classes
    [AgentBehaviour_Attribute_GS]
    public class AgentBehaviour_GS
    {
        //Content fields
        [NonSerialized] private Agent_GS _agent = null; //The agent is under control of this behaviour

        //Loop Methods ================
        public virtual void Start()
        {
            //Agent behaviour can initialize necessary stuff at start
        }

        public virtual void Update()
        {
            //Here we code the actions the agent should do depending of the results/state
        }

        //Get/Set Methods =============
        public Agent_GS agent
        {
            get
            {
                return _agent;
            }
            set
            {
                _agent = value;
            }
        }
    }
}
