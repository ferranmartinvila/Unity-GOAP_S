using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        Debug.Log(GetComponent<GOAP_S.Blackboard.BlackboardComp_GS>().blackboard.GetValue<int>("test"));

        //int bh = (int)b.GetVariable("test").value;
        //Debug.Log(bh);
        
    }
}
