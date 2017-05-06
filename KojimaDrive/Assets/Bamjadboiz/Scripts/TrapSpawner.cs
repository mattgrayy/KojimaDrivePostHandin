using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Bam
{
    public class TrapSpawner : MonoBehaviour
    {
		public RunnerBoundsScript m_runnerBounds;
        public bool spawnTraps;
        public float spawnInterval;
        public float currentTimer = 0.0f;
        public Vector3 spawnPosition;
        public int playerID;
        public List<GameObject> mySpawnedTraps;
        //public Text nextTrap;

        int randomArrayIndex;

        //Quaternion trapRotation;
        Object[] traps;

        // Use this for initialization
        void Start()
        {
            playerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            mySpawnedTraps = new List<GameObject>();
            traps = Resources.LoadAll("Traps");
            randomArrayIndex = Random.Range(0, traps.Length);
        }



        // Update is called once per frame
        void Update()
        {
            if (spawnTraps == true)
            {
                foreach (GameObject trap in mySpawnedTraps)
                {
                    if (trap.name == "Barrel(Clone)" || trap.name == "Basketball(Clone)")
                    {
                        trap.GetComponent<Rigidbody>().AddForce(-transform.forward * 2000);
                    }                                   
                }
                currentTimer += Time.deltaTime;

                if (currentTimer >= spawnInterval)
                {
                    Quaternion trapRotation = Quaternion.LookRotation(transform.up, transform.forward);
                    currentTimer = 0.0f;
                    //trapRotation = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
                    //Vector3 offset = new Vector3(transform.position.x - transform.forward.x * 6, transform.position.y - transform.forward.y * 12, )
                    if (traps[randomArrayIndex].name == "Barrel")
                    {
                        trapRotation = Quaternion.LookRotation(transform.right, transform.up);
                    }
					GameObject trap = (GameObject)Instantiate(traps[randomArrayIndex], transform.position - transform.forward * 6 + (transform.up * 6), trapRotation);

					mySpawnedTraps.Add(trap);
					Collider col = trap.GetComponent<Collider>();
					if (col != null && m_runnerBounds != null)
					{
						Physics.IgnoreCollision(col, m_runnerBounds.m_collider, true);
					}
					randomArrayIndex = Random.Range(0, traps.Length);
                    //MainHUDScript.singleton.ShowNextItem(playerID, traps[randomArrayIndex].name);
                }
            }
        }

        public void DeleteTraps()
        {
            foreach (GameObject trap in mySpawnedTraps)
            {
                Destroy(trap);
            }
            mySpawnedTraps.Clear();
                 
        }
    }
}