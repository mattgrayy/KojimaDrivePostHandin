//===================== Kojima Drive - Bamjadboiz 2017 ====================//
//
// Author:		Orlando 
// Purpose:		Created with Sabotage (game mode) in mind but could easily be used for other tasks.
//				To-Do: Remove Range and YamsDebug, put them somewhere else.
// Namespace:	Bam
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bam
{
	[System.Serializable]
	public struct Range
	{
		public float m_min;
		public float m_max;

		/// <summary>
		/// Lerps between the range's min / max 
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public float Lerp(float t)
		{
			return Mathf.Lerp(m_min, m_max, t);
		}

		/// <summary>
		/// Returns normalized distance from start (clamped)
		/// </summary>
		/// <returns></returns>
		public float Normalize(float value)
		{
			return Mathf.Clamp01((value - m_min)/(m_max-m_min));
		}
	}

	public class YamsDebug
	{
		public static void AssertObjectNotNull<TOwner, TObject>(TOwner owner, TObject obj) where TObject : class
		{
			bool isNotNull = obj != null && !EqualityComparer<TObject>.Default.Equals(obj, default(TObject));
			Debug.Assert(isNotNull, "[Yams] " + owner.GetType().ToString() + " requires a " + obj.GetType().ToString());
		}
	}

	public class ArrowScript : MonoBehaviour
	{
		public GameObject m_target;
		public float m_lerpSpeedRot;
		public Range m_fadeOutDist;
		public Renderer m_head;
		public Renderer m_tail;
		// Use this for initialization
		void Awake()
		{
			YamsDebug.AssertObjectNotNull(this, m_head);
			YamsDebug.AssertObjectNotNull(this, m_tail);
			YamsDebug.AssertObjectNotNull(this, m_target);
		}

		// Update is called once per frame
		void Update()
		{
            if (!m_target)
            {
                Destroy(gameObject);
            }
            else
            {
                float visibility = m_fadeOutDist.Normalize(Vector3.Distance(transform.position, m_target.transform.position));
                SetAllMaterialAlpha(m_head, visibility);
                SetAllMaterialAlpha(m_tail, visibility);

                UpdateRotation();
            }
		}

		void UpdateRotation()
		{
			Vector3 dir = (m_target.transform.position - transform.position).normalized;
			float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.x, dir.z);
			Quaternion target = Quaternion.AngleAxis(angle, Vector3.up);

			transform.rotation = Quaternion.Lerp(transform.rotation, target, m_lerpSpeedRot * Time.deltaTime);
		}

		void SetAllMaterialAlpha(Renderer r, float a)
		{
			foreach (var m in r.materials)
			{
				Color c = m.color;
				c.a = a;
				m.color = c;
			}
		}
	}
}