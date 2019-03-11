using UnityEngine;
using GOAP_S.Blackboard;

public class ClassBMyTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        int bh = GetComponent<BlackboardComp_GS>().blackboard.GetVariable<int>("test").value;
        Debug.Log(bh);
    }
}