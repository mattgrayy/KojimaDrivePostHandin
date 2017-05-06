using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DraggableManager : MonoBehaviour {

	public List<GameObject> Draggables;


	// Use this for initialization
	void Start () {
	
		foreach (GameObject o in Draggables) {

			o.AddComponent<Draggable> ();



		}
	}



	public void AddMeToDraggableList(GameObject o)
		{
		Draggables.Add (o);
		o.AddComponent<Draggable> ();
		}
	

}
