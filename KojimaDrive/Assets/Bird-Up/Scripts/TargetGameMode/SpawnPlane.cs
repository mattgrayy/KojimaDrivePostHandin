using UnityEngine;
using System.Collections;

public class SpawnPlane : MonoBehaviour {

	public GameObject[] m_carSpawns;

	public Transform GetCarSpawn(int _no)
	{
		return m_carSpawns[_no].transform;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
