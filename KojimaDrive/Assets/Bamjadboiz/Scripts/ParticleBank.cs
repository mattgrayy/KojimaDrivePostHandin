using UnityEngine;
using System.Collections;

public class ParticleBank : MonoBehaviour {

    public static ParticleBank singleton;

    public GameObject grass, sand, rock;
    public AudioClip grassSnd;

    public GameObject impact1, explosion1;

    void Awake()
    {
        singleton = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
