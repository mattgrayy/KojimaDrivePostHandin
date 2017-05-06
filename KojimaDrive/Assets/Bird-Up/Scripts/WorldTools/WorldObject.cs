using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour {

    public bool hasPlayer = false;
    private bool setState;

    BaseWorldChange[] effectObjects; 
    public WorldManager.ZONEVAL newZoneActivityLevel;
    bool lostPlayer;
    float countdownTime = 5.0f;
    float lostPlayerCountDown = 0.0f;
    public bool getHasPlayer()
    {
        return hasPlayer;
    }
	// Use this for initialization
	void Start () {
        effectObjects = GetComponents<BaseWorldChange>();
        lostPlayerCountDown = countdownTime;

    }

    public void setNewLevel(WorldManager.ZONEVAL _in)
    {
            if (_in != newZoneActivityLevel)
            {
                newZoneActivityLevel = _in;
                setState = false;
            }
    }


	// Update is called once per frame
	public void manualUpdate () {

        if (setState == false)
        {
            foreach (BaseWorldChange BWC in effectObjects)
            {
                BWC.flipState(newZoneActivityLevel);
            }

            setState = true;
        }
        if(lostPlayer)
        {
           if( (lostPlayerCountDown = lostPlayerCountDown - Time.deltaTime) < 0)
            {
                hasPlayer = false;
                lostPlayerCountDown = countdownTime;
            }

        }
        //manual update children
        foreach (BaseWorldChange BWC in effectObjects)
        {
            BWC.translateState();
        }

    }





    void OnTriggerEnter(Collider _in)
    {
        if(_in.tag == "Player" && hasPlayer == false)
        {
            hasPlayer = true;
            //if(hasFade)
            //{
            //    objF.changeState();
            //}
        }
    }

    void OnTriggerStay(Collider _in)
    {
        if (_in.tag == "Player" && hasPlayer == false)
        {
            hasPlayer = true;
            //if(hasFade)
            //{
            //    objF.changeState();
            //}
        }
    }
 
    void OnTriggerExit(Collider _in)
    {
        if (_in.tag == "Player")
        {
            lostPlayer = true;
            //if (hasFade)
            //{
            //    objF.changeState();
            //}
        }
    }
}
