using UnityEngine;
using GOAP_S.Planning;
using UnityEngine.AI;

public class MoveAction_GS : Action_GS
{
    private NavMeshAgent nav_mesh_agent = null;

    public override bool ActionAwake()
    {
        nav_mesh_agent = agent.gameObject.GetComponent<NavMeshAgent>();
        return base.ActionAwake();
    }

    public override ACTION_RESULT ActionStart()
    {
        Vector3 target = blackboard.GetValue<Vector3>("target_position");
        nav_mesh_agent.SetDestination(target);

        return ACTION_RESULT.A_NEXT;
    }

    public override ACTION_RESULT ActionUpdate()
    {
        if(nav_mesh_agent.remainingDistance == 0 && nav_mesh_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
}
