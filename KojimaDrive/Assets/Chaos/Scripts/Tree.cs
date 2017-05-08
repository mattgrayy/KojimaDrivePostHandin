using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {


    public GameObject tree = null;


	// Use this for initialization
	void Start () {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            transform.position = hit.point;

        }
        
	}
	
}
