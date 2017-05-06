using UnityEngine;
using System.Collections;

//===================== Kojima Drive - Half-Full 2017 ====================//
//
// Author: SAM BRITNALL
// Purpose: Base class for twists
// Namespace: HALF-FULL
//
//===============================================================================//

namespace HF
{
    public class TwistBase : MonoBehaviour
    {
        public Twists.Twist thisTwist;
        GameObject m_GOtwistPrefab;
        
        void Start()
        {
            
        }

        
        void Update()
        {

        }

        public void StartAfterAssignment()
        {
            if(thisTwist == Twists.Twist.test)
            {

            }
            else if(thisTwist == Twists.Twist.tsunami)
            {

            }
            else if(thisTwist == Twists.Twist.tornado)
            {
                AssignPrefab("TornadoTwist");
                
            }
        }

        public void AssignPrefab(GameObject _prefab)
        {
            m_GOtwistPrefab = _prefab;
        }

        public void AssignPrefab(string _prefab)
        {
            m_GOtwistPrefab = (GameObject)Resources.Load(_prefab);
        }

        public void SpawnTwistPrefab()
        {
            TwistsManager.m_instance.m_TwistManagers.Add(Instantiate(m_GOtwistPrefab));
        }
    }
}