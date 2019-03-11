using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        NodeCanvas.Framework.Blackboard b = GetComponent<NodeCanvas.Framework.Blackboard>();

        int bh = (int)b.GetVariable("_a").value;
        Debug.Log(bh);
        
    }
}
