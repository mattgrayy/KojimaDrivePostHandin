using UnityEngine;
using System.Collections;


namespace KRace
{

	enum CurrentPoint { POINT_ONE, POINT_TWO };

	public class WaypointScript : MonoBehaviour
	{
		Vector3 point1 = new Vector3(-5.7f, -1.5f, -48.0f);
		Vector3 point2 = new Vector3(92.4f, -1.5f, 33.2f);

		CurrentPoint currentPoint = new CurrentPoint();

		// Use this for initialization
		void Start()
		{
			currentPoint = CurrentPoint.POINT_ONE;
			transform.position = point1;
		}

		// Update is called once per frame
		void Update()
		{

		}

		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player")
			{
				CharacterScore playerScore = other.transform.GetComponent<CharacterScore>();
				playerScore.addScore();
			}



			switch (currentPoint)
			{
				case CurrentPoint.POINT_ONE:
					currentPoint = CurrentPoint.POINT_TWO;
					transform.position = point2;
					break;
				case CurrentPoint.POINT_TWO:
					currentPoint = CurrentPoint.POINT_ONE;
					transform.position = point1;
					break;
				default:
					//do nothing
					break;
			}
		}
	}
}