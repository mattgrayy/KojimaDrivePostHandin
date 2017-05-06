using UnityEngine;
using System.Collections;
using Kojima;

namespace GCSharp
{
    public class LightBladeCollisionLogic : MonoBehaviour
    {
        private GameObject m_ownerPlayer;
        private LightBlade m_ownerLightBlade;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

		public LightBlade GetOwnerLightBlade()
		{
			return m_ownerLightBlade;
		}

        public void SetOwner(GameObject _newOwner)
        {
            m_ownerPlayer = _newOwner;
            m_ownerLightBlade = m_ownerPlayer.GetComponent<LightBlade>();
        }
    }
}