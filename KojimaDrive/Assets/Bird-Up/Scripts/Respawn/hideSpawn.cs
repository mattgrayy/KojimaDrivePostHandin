using UnityEngine;
using System.Collections;

public class hideSpawn : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach(Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().enabled = false;
        }	
	}

}
