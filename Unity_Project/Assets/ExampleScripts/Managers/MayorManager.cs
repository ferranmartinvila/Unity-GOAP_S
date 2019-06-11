using UnityEngine;
using GOAP_S.Blackboard;

public class MayorManager : MonoBehaviour
{
    //This script defines the resources objective and reset it when is reached

    public int rock_goal = 500;
    //public int diamond_goal = 600;

    private float check_rate = 1.0f;
    private float timer = 0.0f;

    private void Start()
    {
        GlobalBlackboard_GS.blackboard.SetVariable<int>("rock_goal", 500);
        //GlobalBlackboard_GS.blackboard.SetVariable<int>("diamond_goal", 600);
    }

    void Update ()
    {
        timer += Time.deltaTime;
        if(timer > check_rate)
        {
            if(GlobalBlackboard_GS.blackboard.GetValue<int>("current_rock") == rock_goal/* && GlobalBlackboard_GS.blackboard.GetValue<int>("current_diamond") == diamond_goal*/)
            {
                //Resources goal reached! Time to restart
                GlobalBlackboard_GS.blackboard.SetVariable<int>("current_rock", 0);
                //GlobalBlackboard_GS.blackboard.SetVariable<int>("current_diamond", 0);
            }

            timer = 0.0f;
        }
	}
}
