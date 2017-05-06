using UnityEngine;
using System.Collections;


namespace KRace
{
	public class VeryBasicCarScript : MonoBehaviour
	{
		public float verticalAxis;
		public float enginePower;
		public float horizontalAxis;
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			verticalAxis = Input.GetAxis("Vertical");
			horizontalAxis = Input.GetAxis("Horizontal");
			if (verticalAxis < 0)
			{
				transform.position -= transform.forward * (enginePower / 10) * Time.deltaTime;
			}
			else if (verticalAxis > 0)
			{
				transform.position += transform.forward * (enginePower / 10) * Time.deltaTime;
			}

			if (horizontalAxis > 0)
			{
				transform.Rotate(0.0f, 3.0f, 0.0f);
			}
			else if (horizontalAxis < 0)
			{
				transform.Rotate(0.0f, -3.0f, 0.0f);
			}
		}
	}
}