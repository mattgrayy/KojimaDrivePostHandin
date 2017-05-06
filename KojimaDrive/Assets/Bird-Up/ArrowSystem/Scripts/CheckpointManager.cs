using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird
{
    public class CheckpointManager : MonoBehaviour
    {

        #region Singleton
        public static CheckpointManager s_pCheckpointManager;
        bool SetupSingleton()
        {
            if (s_pCheckpointManager == null)
            {
                s_pCheckpointManager = this;
                return true;
            }
            else if (s_pCheckpointManager != this)
            {
                Debug.Log("Two Checkpoint Managers in scene. Deleting the new one.");

                Destroy(gameObject);
                return false;
            }

            return false;
        }

        public static CheckpointManager CM
        {
            get
            {
                return s_pCheckpointManager;
            }
        }
        #endregion

        public enum CheckpointType
        {
            DISTANCE,
            LIST
        }

        public CheckpointType c_Type = CheckpointType.DISTANCE;

		private List<GameObject>[] globalCheckpoints;
        private List<ArrowCheckpoints> arrowCheckpoints;
        public float distanceToCheckpoint = 10;
        public float maxDistanceTillColourChange = 200;
        public float minDistanceTillColourChange = 50;
        public float maxDistanceTillFade = 50;
        public float minDistanceTillFade = 10;

        void OnEnable()
        {
            if (SetupSingleton())
            {
                CreateCheckpointList();
			}
		}

		public class hudCheckpointData_t {
			public int m_nPlayerID; // 0 = all players
			public GameObject m_TargetGO;
		}

		private void Start() {
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_ARROW_ADD_CHECKPOINT, Event_AddCheckpoint);
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_ARROW_REMOVE_CHECKPOINT, Event_RemoveCheckpoint);
		}

		private void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_ARROW_ADD_CHECKPOINT, Event_AddCheckpoint);
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_ARROW_REMOVE_CHECKPOINT, Event_RemoveCheckpoint);
		}

		void Event_AddCheckpoint(object data) {
			hudCheckpointData_t objdata = (hudCheckpointData_t)data;
			if(objdata == null) {
				return;
			}

			AddCheckpoint(objdata.m_TargetGO, objdata.m_nPlayerID);
		}

		void Event_RemoveCheckpoint(object data) {
			hudCheckpointData_t objdata = (hudCheckpointData_t)data;
			if (objdata == null) {
				return;
			}

			RemoveCheckpoint(objdata.m_TargetGO, objdata.m_nPlayerID);
		}

		void CreateCheckpointList()
        {
            if (globalCheckpoints == null) {
				globalCheckpoints = new List<GameObject>[Kojima.GameController.s_nMaxPlayers];
				for (int i = 0; i < globalCheckpoints.Length; i++) {
					globalCheckpoints[i] = new List<GameObject>();
				}
				arrowCheckpoints = new List<ArrowCheckpoints>();
            }
        }

		public void AddCheckpoint(Checkpoint ch, int nPlayerID = 0) {
			if (nPlayerID == 0) {
				for (int i = 0; i < globalCheckpoints.Length; i++) {
					if(globalCheckpoints[i].Contains(ch.gameObject)) {
						continue;
					}

					globalCheckpoints[i].Add(ch.gameObject);
				}

				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					arrowCheckpoints[i].AddCheckpoint(ch.gameObject);
				}

			} else {

				globalCheckpoints[nPlayerID - 1].Add(ch.gameObject);
				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					if (arrowCheckpoints[i].UIArrow.m_ParentController.m_nPlayer == nPlayerID) {
						arrowCheckpoints[i].AddCheckpoint(ch.gameObject);
					}
				}
			}
		}

		public void ClearCheckpoints(int nPlayerID = 0)  {
			if(nPlayerID == 0) {
				for (int i = 0; i < globalCheckpoints.Length; i++) {
					globalCheckpoints[i].Clear();
				}

				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					arrowCheckpoints[i].ClearCheckpoints();
				}
			} else {
				globalCheckpoints[nPlayerID - 1].Clear();
				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					if (arrowCheckpoints[i].UIArrow.m_ParentController.m_nPlayer == nPlayerID) {
						arrowCheckpoints[i].ClearCheckpoints();
					}
				}
			}
		}

		public void RemoveCheckpoint(Checkpoint ch, int nPlayerID = 0) {
			if (nPlayerID == 0) {
				for (int i = 0; i < globalCheckpoints.Length; i++) {
					if (!globalCheckpoints[i].Contains(ch.gameObject)) {
						continue;
					}

					globalCheckpoints[i].Remove(ch.gameObject);
				}

				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					arrowCheckpoints[i].RemoveCheckpoint(ch.gameObject);
				}

			} else {

				globalCheckpoints[nPlayerID - 1].Remove(ch.gameObject);
				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					if (arrowCheckpoints[i].UIArrow.m_ParentController.m_nPlayer == nPlayerID) {
						arrowCheckpoints[i].RemoveCheckpoint(ch.gameObject);
					}
				}
			}
		}

		public void AddCheckpoint(GameObject ch, int nPlayerID = 0) {
			if (nPlayerID == 0) {
				for (int i = 0; i < globalCheckpoints.Length; i++) {
					if (globalCheckpoints[i].Contains(ch)) {
						continue;
					}

					globalCheckpoints[i].Add(ch);
				}

				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					arrowCheckpoints[i].AddCheckpoint(ch);
				}

			} else {

				globalCheckpoints[nPlayerID - 1].Add(ch);
				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					if (arrowCheckpoints[i].UIArrow.m_ParentController.m_nPlayer == nPlayerID) {
						arrowCheckpoints[i].AddCheckpoint(ch);
					}
				}
			}
		}

		public void RemoveCheckpoint(GameObject ch, int nPlayerID = 0) {
			if (nPlayerID == 0) {
				for (int i = 0; i < globalCheckpoints.Length; i++) {
					if (!globalCheckpoints[i].Contains(ch)) {
						continue;
					}

					globalCheckpoints[i].Remove(ch);
				}

				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					arrowCheckpoints[i].RemoveCheckpoint(ch);
				}

			} else {

				globalCheckpoints[nPlayerID - 1].Remove(ch);
				for (int i = 0; i < arrowCheckpoints.Count; i++) {
					if (arrowCheckpoints[i].UIArrow.m_ParentController.m_nPlayer == nPlayerID) {
						arrowCheckpoints[i].RemoveCheckpoint(ch);
					}
				}
			}
		}


		public void AddArrowCheckpoint(ArrowCheckpoints ac)
        {
            ac.colourChangeDist = new Vector2(minDistanceTillColourChange, maxDistanceTillColourChange);
            ac.fadeChangeDist = new Vector2(minDistanceTillFade, maxDistanceTillFade);
            arrowCheckpoints.Add(ac);
        }

        public void RemoveArrowCheckpoint(ArrowCheckpoints ac)
        {
            arrowCheckpoints.Remove(ac);
        }

		public List<GameObject> GetCheckpoints(int nPlayerID = 0) {
			return globalCheckpoints[nPlayerID - 1];
		}
	}
}
