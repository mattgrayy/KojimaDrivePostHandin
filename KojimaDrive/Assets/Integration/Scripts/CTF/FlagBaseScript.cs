using System;
using UnityEngine;
using System.Collections;

public class FlagBaseScript : MonoBehaviour
{
    public GameObject inZone;
    private CaptureTheFlag CTF;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            inZone = col.gameObject;
            if (inZone == CTF.heldByPlayer)
            {
                CTF.Score(inZone);
            }
        }
        

    }
}
