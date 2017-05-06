using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KRace
{
	public class CarDraft : MonoBehaviour
	{

		public GameObject triggerPrefrab;
		List<GameObject> carDraftTriggers;
		float timer;
		float m_spawnRate;

		Kojima.CarScript m_carScript;

		// Use this for initialization
		void Start()
		{
			timer = 0.0f;
			m_spawnRate = 0.2f;

			m_carScript = transform.GetComponent<Kojima.CarScript>();

			carDraftTriggers = new List<GameObject>();
		}

		// Update is called once per frame
		void Update()
		{

			timer += Time.deltaTime;

			// Spawn new trigger every m_spawnRate seconds if the car is going faster than 10
			if (timer >= m_spawnRate && m_carScript.m_forwardVelocity >= 10.0f)
			{
				timer = 0.0f;
				GameObject newTrigger = Instantiate(triggerPrefrab);
				newTrigger.transform.position = transform.position;
				newTrigger.GetComponent<DraftTrigger>().parentName = transform.name;
				carDraftTriggers.Add(newTrigger);

			}

			// List to store any triggers to delete
			List<GameObject> triggersToDelete = new List<GameObject>();

			//Find any dead triggers and add them to the list of triggers to delete
			foreach (GameObject trigger in carDraftTriggers)
			{
				DraftTrigger triggerScript = trigger.GetComponent<DraftTrigger>();

				if (triggerScript.getDead())
				{
					triggersToDelete.Add(trigger);
				}
			}

			//Remove all triggers to delete
			foreach (GameObject trigger in triggersToDelete)
			{
				carDraftTriggers.Remove(trigger);
				GameObject.Destroy(trigger);
			}

			//Clear the list of triggers to delete
			triggersToDelete.Clear();
		}

	}
}