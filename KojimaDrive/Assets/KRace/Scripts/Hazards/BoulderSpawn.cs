using UnityEngine;
using System.Collections;

namespace KRace
{
	public class BoulderSpawn : MonoBehaviour
	{
		public GameObject Boulder;

		float boulderSpawnrate = 5.0f;
		float boulderTick = 0.0f;
		bool canSpawn = true;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

			//Destroy(Boulder, 5.0f);

			//if (canSpawn == true)
			//{
			Instantiate(Boulder, transform.position, transform.rotation);
			//    canSpawn = false;
			//}
			//else
			//{
			//    boulderTick++;
			//    if (boulderTick >= boulderSpawnrate)
			//    {
			//        boulderTick = 0;
			//        canSpawn = true;
			//    }
			//}

		}

	}
}