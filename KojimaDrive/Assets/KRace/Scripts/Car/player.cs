using UnityEngine;
using System.Collections;

namespace KRace
{
	public class player : MonoBehaviour
	{
		//Variables
		//public Light LeftStop; // Left Stop Point Light 
		//public Light RightStop; // Reght Stop Point Light
		public Transform Car; // Your Car Model
							  //public Transform MyCamera; // Your Camera
		float movespeed = 0.0F;
		public float fuel = 10000000.0F;
		int gas_pedal = 0;
		public int health = 100; // Not Used YET
		int rpm = 0;
		int gear = 1;
		bool isengine = false;
		public KeyCode addgas = KeyCode.W;
		public KeyCode removegas = KeyCode.S;
		public KeyCode clutchkey = KeyCode.Space;
		public KeyCode gearup = KeyCode.UpArrow;
		public KeyCode geardown = KeyCode.DownArrow;
		public KeyCode nogear = KeyCode.Alpha0;
		public KeyCode firstgear = KeyCode.Alpha1;
		public KeyCode seccondgear = KeyCode.Alpha2;
		public KeyCode thirdgear = KeyCode.Alpha3;
		public KeyCode fourthgear = KeyCode.Alpha4;
		public KeyCode fifthgear = KeyCode.Alpha5;
		public KeyCode reargear = KeyCode.Alpha9;
		//public KeyCode startengine = KeyCode.E;
		public KeyCode ChangeCameraView = KeyCode.C;
		public KeyCode SteerLeft = KeyCode.A;
		public KeyCode SteerRight = KeyCode.D;
		//public AudioClip EngineStart;
		//public AudioClip EngineStop;
		//public AudioClip IdleSound;
		//public AudioClip GasPressed;
		//public AudioClip BreakSound;

		void OnGUI()
		{
			GUI.Label(new Rect(5, 20, 100, 20), "RMP : " + rpm);
			GUI.Label(new Rect(5, 35, 100, 20), "Fuel : " + fuel);
			GUI.Label(new Rect(5, 50, 100, 20), "Gear : " + gear);
			GUI.Label(new Rect(5, 65, 100, 20), "Engine : " + isengine);
		}
		void Update()
		{

			if (fuel <= 0)
			{
				isengine = false;
				rpm = 0;

			}
			if (isengine == false)
			{
				if (fuel >= 1.50F)
				{
					isengine = true;
					fuel -= 1.5F;
					rpm = 1000;
					//AudioSource.PlayClipAtPoint(EngineStart, Car.position);
					Debug.Log("Engine ON");
				}
				else
				{
					Debug.Log("Not Enough Fuel!");
				}
			}
			else
			{
				isengine = false;
				rpm = 0;
				Debug.Log("Engine OFF");
				//AudioSource.PlayClipAtPoint(EngineStart, Car.position);
			}


			if (isengine == true)
			{

				//Fuel System
				if (rpm <= 1000)
				{
					fuel -= 0.025F;
				}
				else if (rpm > 1000 && rpm <= 2500)
				{
					fuel -= 0.5F;
				}
				else if (rpm > 2500 && rpm <= 4000)
				{
					fuel -= 0.8F;
				}
				else if (rpm > 4000 && rpm <= 5000)
				{
					fuel -= 1.0F;
				}
				else if (rpm > 5000)
				{
					fuel -= 1.9F;
				}
				//End Fuel
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Steer Syste
				if (Input.GetKey(SteerLeft))
				{
					if (movespeed <= 0)
					{
					}
					else
					{
						if (movespeed > 0F && movespeed <= 50F)
						{
							Car.transform.Rotate(Vector3.down, 2F);
						}
						else if (movespeed > 50F && movespeed < 90F)
						{
							Car.transform.Rotate(Vector3.down, 1.3F);
						}
						else if (movespeed > 90F)
						{
							Car.transform.Rotate(Vector3.down, 0.9F);
						}

					}
				}
				if (Input.GetKey(SteerRight))
				{
					if (movespeed <= 0)
					{
					}
					else
					{
						if (movespeed > 0F && movespeed <= 50F)
						{
							Car.transform.Rotate(Vector3.up, 2F);
						}
						else if (movespeed > 50F && movespeed < 90F)
						{
							Car.transform.Rotate(Vector3.up, 1.3F);
						}
						else if (movespeed > 90F)
						{
							Car.transform.Rotate(Vector3.up, 0.9F);
						}

					}
				}

				//End Steer
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//Gas System
				if (Input.GetKey(addgas))
				{
					if (gas_pedal <= 100)
					{
						if (movespeed > 0)
						{
							if (gear == 1)
							{
								Car.transform.Translate(-Vector3.forward * Time.deltaTime * movespeed);
								gas_pedal += 1;
								Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + fuel);
								Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + gear + " " + rpm + " " + fuel);
								//AudioSource.PlayClipAtPoint(GasPressed, Car.position);

							}
							else
							{
								isengine = false;
								//AudioSource.PlayClipAtPoint(EngineStop, Car.position);
							}
						}
						else
						{
							Car.transform.Translate(-Vector3.forward * Time.deltaTime * movespeed);
							gas_pedal += 1;
							Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + fuel);
							Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + gear + " " + rpm + " " + fuel);
							//AudioSource.PlayClipAtPoint(GasPressed, Car.position);
						}
					}
					else
					{
						Car.transform.Translate(-Vector3.forward * Time.deltaTime * movespeed);
						Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + gear + " " + rpm + " " + fuel);
						//AudioSource.PlayClipAtPoint(GasPressed, Car.position);
					}
				}
				else
				{
					if (gas_pedal > 0)
					{
						Car.transform.Translate(-Vector3.forward * Time.deltaTime * movespeed);
						gas_pedal -= 1;
						Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + gear + " " + rpm + " " + fuel);
					}
				}
				if (Input.GetKeyDown(firstgear))
				{
					gear = 1;
				}
				else if (Input.GetKeyDown(seccondgear))
				{
					gear = 2;
				}
				else if (Input.GetKeyDown(thirdgear))
				{
					gear = 3;
				}
				else if (Input.GetKeyDown(fourthgear))
				{
					gear = 4;
				}
				else if (Input.GetKeyDown(fifthgear))
				{
					gear = 5;
				}
				else if (Input.GetKeyDown(nogear))
				{
					gear = 0;
				}
				if (Input.GetKey(removegas))
				{
					//LeftStop.enabled = true;
					//RightStop.enabled = true;
					if (gas_pedal < 0.0F)
					{
						gas_pedal -= 1;
						//AudioSource.PlayClipAtPoint(BreakSound, Car.position);



					}
				}
				else
				{
					// LeftStop.enabled = false;
					//RightStop.enabled = false;
				}
				//End Gas
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//RMP System
				if (gas_pedal >= 10 && gas_pedal <= 20)
				{
					switch (gear)
					{
						case 0:
							rpm = 3000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 1800;
							movespeed = 8.0F;
							break;
						case 2:
							rpm = 1500;
							movespeed = 12.0F;
							break;
						case 3:
							rpm = 1200;
							movespeed = 5.0F;
							break;
						case 4:
							rpm = 1100;
							movespeed = 3.0F;
							break;
						case 5:
							rpm = 800;
							movespeed = 0.3F;
							break;
						case 9:
							rpm = 1700;
							movespeed = -9.0F;
							break;
					}
				}
				else if (gas_pedal < 10)
				{
					switch (gear)
					{
						case 0:
							rpm = 2000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 1200;
							movespeed = 8.0F;
							break;
						case 2:
							rpm = 1000;
							movespeed = 12.0F;
							break;
						case 3:
							rpm = 800;
							movespeed = 5.0F;
							break;
						case 4:
							rpm = 400;
							movespeed = 3.0F;
							break;
						case 5:
							rpm = 200;
							movespeed = 0.3F;
							break;
						case 9:
							rpm = 1200;
							movespeed = -9.0F;
							break;
					}
				}
				else if (gas_pedal > 20 && gas_pedal <= 40)
				{
					switch (gear)
					{
						case 0:
							rpm = 5000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 2800;
							movespeed = 14.0F;
							break;
						case 2:
							rpm = 2300;
							movespeed = 17.0F;
							break;
						case 3:
							rpm = 1800;
							movespeed = 24.0F;
							break;
						case 4:
							rpm = 1600;
							movespeed = 20.0F;
							break;
						case 5:
							rpm = 1200;
							movespeed = 15.0F;
							break;
						case 9:
							rpm = 2200;
							movespeed = -15.0F;
							break;
					}
				}
				else if (gas_pedal > 40 && gas_pedal <= 60)
				{
					switch (gear)
					{
						case 0:
							rpm = 7000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 4500;
							movespeed = 18.0F;
							break;
						case 2:
							rpm = 2900;
							movespeed = 25.0F;
							break;
						case 3:
							rpm = 2400;
							movespeed = 35.0F;
							break;
						case 4:
							rpm = 2000;
							movespeed = 46.0F;
							break;
						case 5:
							rpm = 1700;
							movespeed = 56.0F;
							break;
						case 9:
							rpm = 4500;
							movespeed = -30.0F;
							break;
					}
				}
				else if (gas_pedal > 60 && gas_pedal <= 80)
				{
					switch (gear)
					{
						case 0:
							rpm = 9000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 6500;
							movespeed = 24.0F;
							break;
						case 2:
							rpm = 4500;
							movespeed = 38.0F;
							break;
						case 3:
							rpm = 4000;
							movespeed = 75.0F;
							break;
						case 4:
							rpm = 3600;
							movespeed = 95.0F;
							break;
						case 5:
							rpm = 3000;
							movespeed = 120.0F;
							break;
						case 9:
							rpm = 4800;
							movespeed = -35.0F;
							break;
					}
				}
				else if (gas_pedal > 80 && gas_pedal <= 90)
				{
					switch (gear)
					{
						case 0:
							rpm = 14000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 8700;
							movespeed = 32.0F;
							break;
						case 2:
							rpm = 7800;
							movespeed = 50.0F;
							break;
						case 3:
							rpm = 6700;
							movespeed = 106.0F;
							break;
						case 4:
							rpm = 4200;
							movespeed = 120.0F;
							break;
						case 5:
							rpm = 3800;
							movespeed = 140.0F;
							break;
						case 9:
							rpm = 5200;
							movespeed = -48.0F;
							break;
					}
				}
				else if (gas_pedal > 90 && gas_pedal <= 100)
				{
					switch (gear)
					{
						case 0:
							rpm = 17000;
							movespeed = 0.0F;
							break;
						case 1:
							rpm = 12700;
							movespeed = 42.0F;
							break;
						case 2:
							rpm = 8500;
							movespeed = 60.0F;
							break;
						case 3:
							rpm = 7400;
							movespeed = 126.0F;
							break;
						case 4:
							rpm = 5700;
							movespeed = 140.0F;
							break;
						case 5:
							rpm = 5200;
							movespeed = 180.0F;
							break;
						case 9:
							rpm = 8400;
							movespeed = -68.0F;
							break;
					}
				}
				//End RMP
				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//
				//AudioSource.PlayClipAtPoint(IdleSound, Car.position);
			}
			else
			{
				if (Input.GetKey(SteerLeft))
				{
					if (movespeed <= 0)
					{

					}
					else
					{
						Car.transform.Rotate(Vector3.down, 0.4F);

					}
				}
				if (Input.GetKey(SteerRight))
				{
					if (movespeed <= 0)
					{
					}
					else
					{
						Car.transform.Rotate(Vector3.up, 0.4F);

					}
				}
				//LeftStop.enabled = false;
				//RightStop.enabled = false;
				if (movespeed > 0)
				{
					if (!isengine)
					{
						Car.transform.Translate(-Vector3.forward * Time.deltaTime * movespeed);
						movespeed -= 1.0F;
						Debug.Log("GAS PRESSED " + movespeed + " " + gas_pedal + " " + gear + " " + rpm + " " + fuel);
					}
				}
			}

		}

	}
}