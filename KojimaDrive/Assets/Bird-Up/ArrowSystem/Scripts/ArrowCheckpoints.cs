using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird
{
	public class hudArrowReachedData_t {
		public int m_nPlayerID; // 1 - 4
		public GameObject m_CheckpointReached;
	}

    public class ArrowCheckpoints : MonoBehaviour
    {

        public List<GameObject> currentCheckpoints = new List<GameObject>();
        public HUD_NavArrow UIArrow = null;
        public int activeCheckpoint = 0;
        public Vector2 colourChangeDist = new Vector2();
        public Vector2 fadeChangeDist = new Vector2();
        private float maxDistance = 999;

        private bool isInRange = false;

        void Start() {
			CheckpointManager.CM.AddArrowCheckpoint(this);
			currentCheckpoints = CheckpointManager.CM.GetCheckpoints(UIArrow.m_ParentController.m_nPlayer);
            maxDistance = CheckpointManager.CM.distanceToCheckpoint;
        }

        void Update()
        {

			// Guarantee clamp active checkpoint
			activeCheckpoint = Mathf.Clamp(activeCheckpoint, 0, currentCheckpoints.Count);

			if (currentCheckpoints.Count == 0) {
				/*// Disable our drawable arrow
				UIArrow.m_bDisplay = false;*/
				return;
			}

            isInRange = CheckCloseToCheckpoint();
            if (isInRange)
            {
                OnCheckpoint();
            }

            float currentDistance;

            if (CheckpointManager.CM.c_Type == CheckpointManager.CheckpointType.DISTANCE)
            {
                float topDistance = -1;
                int topDistanceID = 0;

                for (int i = 0; i < currentCheckpoints.Count; i++)
                {
                    float dist = GetDistanceToCheckpoint(i);

                    if (dist < topDistance || topDistance == -1)
                    {
                        topDistance = dist;
                        topDistanceID = i;
                    }
                }

                activeCheckpoint = topDistanceID;
                currentDistance = topDistance;
            }
            else
            {
                currentDistance = GetDistanceToCheckpoint(activeCheckpoint);
            }

            if (UIArrow != null)
            {
                UpdateArrowAppearance(currentDistance);
            }
        }

        bool CheckCloseToCheckpoint()
        {
            float distance = GetDistanceToCheckpoint(activeCheckpoint);

            if (distance <= maxDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        float GetDistanceToCheckpoint(int chToCheck)
        {
			if (currentCheckpoints.Count <= chToCheck) {
				return 1000000000000000000000000000.0f;
			}
            float distance = Vector3.Distance(currentCheckpoints[chToCheck].transform.position, transform.position);

            return distance;
        }

        public void AddCheckpoint(GameObject ch)
        {
			if(currentCheckpoints.Contains(ch)) {
				return; // Don't re-add it
			}

            currentCheckpoints.Add(ch);
        }

		public void ClearCheckpoints() {
			currentCheckpoints.Clear();
		}

        public void RemoveCheckpoint(GameObject ch)
        {
			if (!currentCheckpoints.Contains(ch)) {
				return; // Don't remove something we don't have
			}

			currentCheckpoints.Remove(ch);
        }

        //edit this to do something you want, like delete a checkpoint, or tell you "Good Boy"
        void OnCheckpoint() {
			//print("nice");
			hudArrowReachedData_t data = new hudArrowReachedData_t();
			data.m_nPlayerID = UIArrow.m_ParentController.m_nPlayer;
			data.m_CheckpointReached = currentCheckpoints[activeCheckpoint];

			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_ARROW_REACHED_CHECKPOINT, data);
		}

        public void SetUIArrow(HUD_NavArrow arrow)
        {
            UIArrow = arrow;
        }

        public void UpdateArrowAppearance(float distance)
        {
            float colourPercentage = (distance - colourChangeDist.x) / (colourChangeDist.y - colourChangeDist.x);
			float colourPercentageInv = Mathf.Clamp((distance - colourChangeDist.y) / (colourChangeDist.x - colourChangeDist.y), 0.0f, 1.0f);
			float fadePercentage = (distance - fadeChangeDist.x) / (fadeChangeDist.y - fadeChangeDist.x);
            colourPercentage = Mathf.Clamp(colourPercentage, 0, 1);
            fadePercentage = Mathf.Clamp(fadePercentage, 0, 1);

			//UIArrow.LerpMaterialColour(colourPercentage);
			//UIArrow.LerpMeshAlpha(fadePercentage);
			UIArrow.LerpMaterial(colourPercentage, colourPercentageInv, fadePercentage);

		}

        public Transform CurrentActiveCheckpoint
        {
            get
            {
                if (currentCheckpoints.Count > activeCheckpoint)
                {
                    return currentCheckpoints[activeCheckpoint].transform;
                }

                return null;
            }
        }

        void OnDestroy()
        {
            CheckpointManager.CM.RemoveArrowCheckpoint(this);
        }
    }
}
