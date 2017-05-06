using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace KRace
{
	public class Timer : MonoBehaviour
	{

		public Text timerText;      //Timer text box on UI
		public float timerTime;     //Timer float 

		Color timerTextColour = new Color(1.0f, 1.0f, 1.0f);
		Color timerTextAlertColour = new Color(1.0f, 0.45f, 0.45f);

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

			//Split minutes and seconds from time
			int timeMinutes;
			int timeSeconds;

			timeMinutes = (int)timerTime / 60;
			timeSeconds = (int)timerTime % 60;


			//Set timer text to time (Seconds to have leading 0 if < 10)
			timerText.text = timeMinutes.ToString() + ":" + timeSeconds.ToString("D2");

			//Increment time if > 0, stop if <= 0
			if (timerTime > 0.0f)
			{
				timerTime -= Time.deltaTime;


				if (timerTime < 30.0f)
				{
					//Flash red every odd number
					if ((int)timerTime % 2 != 0)
					{
						timerText.color = timerTextAlertColour;
					}
					else
					{
						timerText.color = timerTextColour;
					}
				}
			}
			else //Timer is <= 0
			{
				timerText.text = "0:00";
			}
		}
	}
}