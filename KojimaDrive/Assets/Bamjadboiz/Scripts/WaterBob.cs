using UnityEngine;
using System.Collections;

namespace Bam
{
	public class WaterBob : MonoBehaviour
	{
		public float m_rotLerp = 15;
		public Vector3 m_offset;
		public Vector3 m_up;
		public WaterScript m_waterBody;
		private WaterScript.WaveInfo m_infoCache;
		// Use this for initialization
		void Start()
		{
			m_infoCache = new WaterScript.WaveInfo();

            if (m_waterBody == null && GameObject.Find("Water"))
            {
                m_waterBody = GameObject.Find("Water").GetComponent<WaterScript>();
            }
		}

		// Update is called once per frame
		void Update()
		{
            if (m_waterBody != null)
            {
                m_waterBody.GetSurfaceInfoAtXZ(transform.position, out m_infoCache);

                Vector3 pos = m_infoCache.m_surfacePosition + m_offset;

                if (transform.position.y < pos.y)
                {
                    if (NoElementNaN(pos))
                    {
                        transform.position = pos;
                    }

                    Vector3 dir = m_infoCache.m_surfacenormal;
                    if (Mathf.Abs(Vector3.Dot(dir, m_up)) > 0)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, m_up), Time.deltaTime * m_rotLerp);
                    }
                }
            }
		}
		

		static bool NoElementNaN(Vector3 v) { return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z); }
	}

	
}