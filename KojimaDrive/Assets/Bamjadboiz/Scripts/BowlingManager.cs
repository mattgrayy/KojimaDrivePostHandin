using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowlingManager : MonoBehaviour {

    public GameObject m_bowlingPinPrefab;
    public int m_xChange;
    public int m_zChange;
    public int m_pinRows;
    public int m_pinScore;
    public bool m_strike;
    int m_pinAmount = 0;
    int m_fallenPins = 0;   
    Vector3 m_StartingPosition;
    List<GameObject> m_bowlingPins = new List<GameObject>();  
    float m_startingX;

	public int m_baseCount;
	public float m_spacing;
	public bool m_spawned { get; private set; }
	private int m_internalNumPins;
	

	public int m_numPins
	{
		get
		{
			if (m_spawned) return m_internalNumPins;
			else
			{
				int num = 0;
				for (int i = 1; i <= m_baseCount; ++i)
				{
					num += i;
				}
				return num;
			}
		}
	}

    // Use this for initialization
    void Start () {
        SetupPins();
        	

	}
	
	// Update is called once per frame
	void Update () {

	
	}

    public void RemovePins()
    {
        foreach(GameObject bowlingPin in m_bowlingPins)
        {
            Destroy(bowlingPin);
        }
        m_bowlingPins.Clear();
		m_internalNumPins = 0;
		m_spawned = false;
	}

	

    public void SetupPins()
    {
		m_internalNumPins = 0;
		//size : width of base (same as height of triangle)
		float size = (m_baseCount -1)* m_spacing /2;
		//pos : position offset from local 0,0,0 
		Vector4 pos = Vector4.zero;
		pos.w = 1;
		//inset : how far in to start the row of pins
		float inset = 0;
		//loop for num rows
		for (int i = 0; i < m_baseCount; ++i)
		{
			//pins on this row
			int numPins = m_baseCount - i;
			float offset = 0;
			inset = m_spacing * i / 2;
			pos.z = i * m_spacing;
			for (int j = 0; j < numPins; ++j)
			{
				pos.x = inset + offset;
				//position of pin in local space (with centre of triangle = 0,0,0)
				Vector4 local = pos - new Vector4(size, 0, size, 0);
				//position of pin in world space
				Vector3 world = transform.localToWorldMatrix * local;
				//instantiate the pin
				m_bowlingPins.Add((GameObject)Instantiate(m_bowlingPinPrefab, world, Quaternion.LookRotation(transform.up, transform.forward)));
				offset += m_spacing;
				++m_internalNumPins;
			}
		}
		m_spawned = true;
	}


    public int CheckBowlingScore()
    {
        m_fallenPins = 0;
        //Checks each pin in the list to see if it has 'fallen over' and add to the score accordingly
        foreach (GameObject pin in m_bowlingPins)
        {
            if (Vector3.Dot(pin.transform.up, Vector3.up) <= 0)
            {
                m_fallenPins++;
                m_pinScore += 10;
            }
        }
        
        //If all the pins have fallen then a 'Strike' has taken place
        if (m_fallenPins == m_pinAmount)
        {
            m_strike = true;
            m_pinScore *= 2;
            
        }

        return m_pinScore;
    }
}
