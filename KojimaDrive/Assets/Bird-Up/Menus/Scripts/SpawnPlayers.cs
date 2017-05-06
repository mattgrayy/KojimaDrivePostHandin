using UnityEngine;
using System.Collections;
namespace Bird {
	public class SpawnPlayers : MonoBehaviour {
		[System.Serializable]
		public class spawnPositions_t { // Player 1-4
			public Vector3[] m_spawnPosition = new Vector3[Kojima.GameController.s_nMaxPlayers];
			public Vector3[] m_spawnOrientation = new Vector3[Kojima.GameController.s_nMaxPlayers];
		}

		// This solution upsets me, but it beats having two prefabs for everything this close to ship
		[Header("KOJIMA ISLAND")]
		public spawnPositions_t[] m_SpawnPositionsK; // Will randomly pick from this list
		public GameObject m_BackupPlayerSpawnerK;
		public spawnPositions_t m_TransitionPositionK;
		[Header("OHIYAKU BAY")]
		public spawnPositions_t[] m_SpawnPositionsO;
		public GameObject m_BackupPlayerSpawnerO;
		public spawnPositions_t m_TransitionPositionO;

		private void Start() {
			if(Kojima.GameController.s_PlayerCreationData == null ||
				Kojima.GameController.s_PlayerCreationData[0] == null) {
				// If the array is null, or any of the contents are null... (just check the first one, it's easier to write ) 👀
				SpawnDefaultPlayers();
				return;
			}

			if (Kojima.GameController.s_eLoadMode == Kojima.GameController.loadMode_e.STANDARD) {
				spawnPositions_t[] spawnPositions;
				switch(Kojima.GameController.s_eIsland) {
					case Kojima.GameController.island_e.KOJIMA:
					default:
						spawnPositions = m_SpawnPositionsK;
						break;
					case Kojima.GameController.island_e.OHIYAKU:
						spawnPositions = m_SpawnPositionsO;
						break;
				}

				if (spawnPositions != null && spawnPositions.Length != 0) {
					// Set some spawn positions
					int nChosenLocation = Random.Range(0, spawnPositions.Length);
					spawnPositions_t positions = spawnPositions[nChosenLocation];
					for (int i = 0; i < Kojima.GameController.s_PlayerCreationData.Length; i++) {
						Kojima.GameController.s_PlayerCreationData[i].m_SpawnOrientation = positions.m_spawnOrientation[i];
						Kojima.GameController.s_PlayerCreationData[i].m_SpawnPosition = positions.m_spawnPosition[i];
					}
				}
			} else if (Kojima.GameController.s_eLoadMode == Kojima.GameController.loadMode_e.ISLAND_TRANSITION) {
				// Use only our transition locations
				spawnPositions_t spawnPosition;
				switch (Kojima.GameController.s_eIsland) {
					case Kojima.GameController.island_e.KOJIMA:
					default:
						spawnPosition = m_TransitionPositionK;
						break;
					case Kojima.GameController.island_e.OHIYAKU:
						spawnPosition = m_TransitionPositionO;
						break;
				}

				if (spawnPosition != null) {
					// Set some spawn positions
					for (int i = 0; i < Kojima.GameController.s_PlayerCreationData.Length; i++) {
						Kojima.GameController.s_PlayerCreationData[i].m_SpawnOrientation = spawnPosition.m_spawnOrientation[i];
						Kojima.GameController.s_PlayerCreationData[i].m_SpawnPosition = spawnPosition.m_spawnPosition[i];
					}
				}

				Kojima.GameController.s_eLoadMode = Kojima.GameController.loadMode_e.STANDARD; // Reset the load mode for next time, just in case
			}

			// Fire our spawn command
			if(!Kojima.GameController.SpawnPlayers()) {
				SpawnDefaultPlayers();
			}
		}

		void SpawnDefaultPlayers() {
			GameObject m_BackupPlayerSpawner;
			switch (Kojima.GameController.s_eIsland) {
				case Kojima.GameController.island_e.KOJIMA:
				default:
					m_BackupPlayerSpawner = m_BackupPlayerSpawnerK;
					break;
				case Kojima.GameController.island_e.OHIYAKU:
					m_BackupPlayerSpawner = m_BackupPlayerSpawnerO;
					break;
			}

			Debug.Log("Bird::SpawnPlayers - Game did not start from menu, will spawn default players!");
			GameObject defaultPlayers = Instantiate(m_BackupPlayerSpawner);
			defaultPlayers.transform.DetachChildren(); // Unparent the kids
			Destroy(defaultPlayers);
		}
	}
}