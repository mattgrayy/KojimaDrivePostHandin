using UnityEngine;
using System.Collections;

namespace Bird
{
    public class Checkpoint : MonoBehaviour
    {
		//public int checkpointID;
		public int m_nTargetPlayer = 0;

        void Start()
        {
            CheckpointManager.CM.AddCheckpoint(gameObject, m_nTargetPlayer);
        }

        void OnDestroy()
        {
            CheckpointManager.CM.RemoveCheckpoint(gameObject, m_nTargetPlayer);
        }
    }
}

