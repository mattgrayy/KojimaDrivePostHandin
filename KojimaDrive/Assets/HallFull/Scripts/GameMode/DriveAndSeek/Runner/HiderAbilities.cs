using UnityEngine;
using System.Collections;

namespace HF
{
    public class HiderAbilities : MonoBehaviour
    {

        GameObject m_oilReference;

        GameObject m_oilHolder;

        GameObject m_oil;

        bool m_timer;

        float m_oilWaitTime = 5.0f;

        void Start()
        {

            m_oilReference = (GameObject)Resources.Load("OilSlick");
            m_oilHolder = Instantiate((GameObject)Resources.Load("OilHolder"));
            m_timer = true;
        }

        
        void Update() //check to see if the event has started, check to see if the car needs to start a timer to drop oil...
        {
            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.DS_RUNNING, StartOilSpawns);
            //EventManager.m_instance.SubscribeToEvent(Events.Event.DS_CHASE, StartOilSpawns);

            if (m_timer == false)
            {
                StartCoroutine(OilTimer());
            }

            if (Kojima.GameModeManager.m_instance.m_currentMode == Kojima.GameModeManager.GameModeState.FREEROAM) //... check to see if I should be alive
            {
                Destroy(m_oilHolder);
                //Destroy(m_oilReference);
                Destroy(gameObject.GetComponent<HiderAbilities>());
            }

        }
        
        void StartOilSpawns() //this is the event trigger which flips a bool to allow the car to start leaking oil
        {
            m_timer = false;
        }

        void SpawnOil() //this spawns the oil by the back spawn
        {
            m_oil = Instantiate(m_oilReference);
            // m_oil.transform.position = m_spawnPos;
            m_oil.transform.position = gameObject.transform.Find("BackSpawn").transform.position;
            m_oil.transform.parent = m_oilHolder.transform;
        }

        IEnumerator OilTimer() //this is a timer to seperate out the oil slick drops
        {
            m_timer = true;
            yield return new WaitForSeconds(m_oilWaitTime);
            SpawnOil();
            m_timer = false;
        }
    }
}