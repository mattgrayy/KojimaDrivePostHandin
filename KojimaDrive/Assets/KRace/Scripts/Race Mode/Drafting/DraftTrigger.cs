using UnityEngine;
using System.Collections;

namespace KRace
{
	public class DraftTrigger : MonoBehaviour
	{

		public string parentName;


        public bool m_isBumper;

		float startScale;   //Start scale of draft trigger
		float triggerTime;  //Time it takes to scale to 0
		float scaleTime;    //Scale to transform object by
		float m_spawnTime;



		bool dead;

		// Use this for initialization
		void Start()
		{
			//Time for trigger to fade away
			triggerTime = 3.0f;

			//Maths to calculate scale time
			startScale = transform.localScale.x;
			scaleTime = triggerTime / startScale;

			m_spawnTime = Time.time;

			dead = false;
		}

		// Update is called once per frame
		void Update()
		{


			//Calculate how much to scale this update
			float scaleBy = Time.deltaTime / scaleTime;

			//Scale only if it doesn't make scale less than 0
			if (!(transform.localScale.x - scaleBy <= 0) && !m_isBumper)
			{
				transform.localScale -= new Vector3(scaleBy, scaleBy, scaleBy);
			}


			//If scale is <= 0 the object has shrunk to nothing, destroy.
			if (Time.time >= m_spawnTime + triggerTime && !m_isBumper)
			{
				dead = true;
			}
		}

		public bool getDead()
		{
			return dead;
		}

        public void setDead()
        {
            dead = true;
        }
	}
}