using UnityEngine;
using System.Collections;

public class ToggleGlider : MonoBehaviour {
    Kojima.CarScript m_Car;
    public bool m_Open = true;
	// Use this for initialization
	void Start () {
        m_Car = GetComponentInParent<Kojima.CarScript>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(m_Open)
            {
                m_Open = false;
                m_Car.PutAwayGlider();
            }
            else
            {
                m_Open = true;
                m_Car.PullOutGlider();
            }
        }

    }
}
