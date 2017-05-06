using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using Bam;
using Kojima;

namespace GCSharp
{
    public class LightBlade : MonoBehaviour
    {
        private GameObject m_lightBladePointPref;

        //public GameObject m_lightBladePointModelPref;
        private GameObject m_collisionBoxPref;

        private GameObject m_spawnPoint;

        [SerializeField]
        private List<GameObject> m_currentPoints;

		//Anthony's stuff
		[SerializeField]
		private float m_maxSpawnDistance = 3.0f;
		private Vector3 m_queuedSpawnPoint;
		private Vector3 m_prevSpawnPoint;
		[SerializeField]
		private GameObject m_deathColliderPrefab;
		[SerializeField]
		private int m_maxDeathColliders = 40;
		[SerializeField]
		private float m_maxDeathColliderLifetime = 2.0f;
		private List<GameObject> m_inactiveDeathColliders;
		private class ActiveDeathCollider
		{
			public float m_durationSpentAlive;
			public GameObject m_object;
		}
		private List<ActiveDeathCollider> m_activeDeathColliders;
		private TrailRenderer m_trailRenderer;

        private float m_spawnTime;
        private float m_delayedSpawnTime;
        private float m_currentSpawntTime;
        private float m_cooldownTime;
        private float m_offset;
        private float m_countdown;
        private bool m_cooldownFinished = false;

        [SerializeField]
        private int m_score;

        public int m_spawnLimit;

        [SerializeField]
        private List<GameObject> m_currentCollisionBoxes;

        private Kojima.CarScript m_carScript;
        private Kojima.RespawnScript m_respawnScript;
        private int m_playerID;
        private Material[] m_materials;

		// Use this for initialization
		private void Start()
		{
			m_activeDeathColliders = new List<ActiveDeathCollider>();
			m_inactiveDeathColliders = new List<GameObject>();
			m_countdown = 0;
			m_cooldownFinished = false;
			m_currentPoints = new List<GameObject>();
			m_currentCollisionBoxes = new List<GameObject>();
			m_spawnPoint = gameObject.transform.FindChild("BladeSpawnPoint").gameObject;
			m_carScript = gameObject.GetComponent<Kojima.CarScript>();
			m_playerID = m_carScript.m_nplayerIndex;
			m_respawnScript = gameObject.GetComponent<Kojima.RespawnScript>();
			//m_spawnPoint.transform.position = new Vector3(gameObject.transform.position.x + m_offset, gameObject.transform.position.y, gameObject.transform.position.z/* - m_offset*/);
			//m_spawnPoint.transform.parent = gameObject.transform;

			//Anthony's stuff
			m_prevSpawnPoint = transform.position;
			m_queuedSpawnPoint = transform.position;
			for(int i=0; i<m_maxDeathColliders; i++)
			{
				m_inactiveDeathColliders.Add(Instantiate(m_deathColliderPrefab));
				m_inactiveDeathColliders[i].SetActive(false);
				m_inactiveDeathColliders[i].GetComponent<LightBladeCollisionLogic>().SetOwner(gameObject);
			}

			m_trailRenderer = gameObject.AddComponent<TrailRenderer>();
			m_trailRenderer.time = m_maxDeathColliderLifetime;
            m_trailRenderer.material = m_materials[m_playerID - 1];
			//m_trailRenderer.material.color = new Color(0, 255, 255);
			m_trailRenderer.shadowCastingMode = ShadowCastingMode.Off;
			m_trailRenderer.receiveShadows = false;
		}

		private void OnDestroy()
		{
			//remove trail renderer
			Destroy(m_trailRenderer);
			RemoveLightBlades();
		}

        // Update is called once per frame
        private void Update()
        {
			Vector3 currentPosition = transform.position;
			if(Vector3.Distance(currentPosition, m_queuedSpawnPoint) > m_maxSpawnDistance)
			{
				ActivateDeathCollider(m_queuedSpawnPoint, currentPosition);
				m_prevSpawnPoint = m_queuedSpawnPoint;
				m_queuedSpawnPoint = currentPosition;
			}
			UpdateActiveDeathColliders();
        }

		private void ActivateDeathCollider(Vector3 _spawnPosition, Vector3 _currentPosition)
		{
			if(m_inactiveDeathColliders.Count>0)
			{
				ActiveDeathCollider newActiveCollider = new ActiveDeathCollider();
				newActiveCollider.m_durationSpentAlive = 0.0f;
				newActiveCollider.m_object = m_inactiveDeathColliders[m_inactiveDeathColliders.Count - 1];
				m_activeDeathColliders.Add(newActiveCollider);
				m_inactiveDeathColliders.RemoveAt(m_inactiveDeathColliders.Count - 1);
				m_activeDeathColliders[m_activeDeathColliders.Count - 1].m_object.transform.position = _spawnPosition;
				m_activeDeathColliders[m_activeDeathColliders.Count - 1].m_object.transform.LookAt(_currentPosition);
				m_activeDeathColliders[m_activeDeathColliders.Count - 1].m_object.SetActive(true);
			}
			else
			{
				ActiveDeathCollider oldestCollider = getOldestDeathCollider();
				if(oldestCollider != null)
				{
					oldestCollider.m_object.transform.position = _spawnPosition;
					oldestCollider.m_durationSpentAlive = 0.0f;
				}
			}
		}

		private void DeactivateDeathCollider(ActiveDeathCollider _deathCollider)
		{
			_deathCollider.m_object.SetActive(false);
			m_activeDeathColliders.Remove(_deathCollider);
			m_inactiveDeathColliders.Add(_deathCollider.m_object);
		}

		private void DeactivateDeathCollider(int _deathColliderIndex)
		{
			if (m_activeDeathColliders.Count > _deathColliderIndex)
			{
				m_activeDeathColliders[_deathColliderIndex].m_object.SetActive(false);
				m_inactiveDeathColliders.Add(m_activeDeathColliders[_deathColliderIndex].m_object);
				m_activeDeathColliders.RemoveAt(_deathColliderIndex);
			}
		}

		private ActiveDeathCollider getOldestDeathCollider()
		{
			if(m_activeDeathColliders.Count == 0)
			{
				return null;
			}
			ActiveDeathCollider result = m_activeDeathColliders[0];
			foreach(ActiveDeathCollider deathCollider in m_activeDeathColliders)
			{
				if(deathCollider.m_durationSpentAlive > result.m_durationSpentAlive)
				{
					result = deathCollider;
				}
			}
			return result;
		}

		private void UpdateActiveDeathColliders()
		{
			for(int i=0; i< m_activeDeathColliders.Count; i++)
			{
				m_activeDeathColliders[i].m_durationSpentAlive += Time.deltaTime;
				if (m_activeDeathColliders[i].m_durationSpentAlive > m_maxDeathColliderLifetime)
				{
					DeactivateDeathCollider(m_activeDeathColliders[i]);
				}
			}
		}

        public void SetUpScript(GameObject _spawnPointPref, GameObject _colBoxPref, float _spawnTime, float _cdTime, float _offset, int _spawnLimit)
        {
			m_deathColliderPrefab = _colBoxPref;
            m_lightBladePointPref = _spawnPointPref;
            m_collisionBoxPref = _colBoxPref;
            m_spawnTime = _spawnTime;
            m_delayedSpawnTime = 99999.0f;
            m_cooldownTime = _cdTime;
            m_spawnLimit = _spawnLimit;
            m_offset = _offset;
        }

        public void ResetTrail()
		{
			for(int i=0; i<m_activeDeathColliders.Count; i++)
			{
				DeactivateDeathCollider(i);
			}
			m_prevSpawnPoint = transform.position;
			m_queuedSpawnPoint = transform.position;
			m_trailRenderer.Clear();
		}

        private void RemoveLightBlades()
        {
            foreach(ActiveDeathCollider deathCollider in m_activeDeathColliders)
			{
				m_inactiveDeathColliders.Add(deathCollider.m_object);
			}
			for(int i =0; i< m_inactiveDeathColliders.Count; i++)
			{
				Destroy(m_inactiveDeathColliders[i]);
			}
        }

        public void AddScore(int _newValueToAdd)
        {
            m_score += _newValueToAdd;
            HF.PlayerExp.AddEXP(m_carScript.m_nplayerIndex - 1, _newValueToAdd, true, true, null, false);
        }

        public int GetScore()
        {
            return m_score;
        }

		public int GetPlayerID()
		{
			return m_playerID;
		}

		private void OnTriggerEnter(Collider other)
		{
			if(other.tag == "DeathTrigger")
			{
				//lose and give points
				LightBladeCollisionLogic oLBCol = other.gameObject.GetComponent<LightBladeCollisionLogic>();
				if (oLBCol)
				{
					LightBlade oLB = oLBCol.GetOwnerLightBlade();
					if (oLB.gameObject != gameObject && oLB != null)
					{
						oLB.AddScore(50);
						GetComponent<Kojima.RespawnScript>().moveToCurrentReset();
						ResetTrail();
					}
					//No self collisions because it's too temperamental, otherwise just put the GetComponent and ResetTrail stuff down here instead
				}
			}
		}

        public void SetRendMats(Material[] _newMats)
        {
            m_materials = _newMats;
        }

	}
}