using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_Background_Stripes : MonoBehaviour {
		public Vector3 m_StartPos;
		public Vector3 m_LastPos;
		public bool m_bLoop = true;
		public bool m_bGo = true;
		public float m_fSpeed = 1.0f;
		/*public float m_fCloseEnough = 0.1f;
		private float m_fCloseEnoughSq;*/
		private float m_fLerpTime = 0.0f;

		void Start() {
			// Calc how long the lerp we actually are
			Vector3 range = m_LastPos - m_StartPos;
			Vector3 pos = transform.localPosition;
			Vector3 cur = pos - m_StartPos;
			float fRange = range.magnitude;
			float fCur = cur.magnitude;
			float fLerp = (fCur * (1 / fRange));
			m_fLerpTime = fLerp;

		}

		Vector3 LargerThan(Vector3 vec1, Vector3 vec2) {
			Vector3 output;
			output.x = vec1.x > vec2.x ? 1.0f : 0.0f;
			output.y = vec1.y > vec2.y ? 1.0f : 0.0f;
			output.z = vec1.z > vec2.z ? 1.0f : 0.0f;
			return output;
		}

		Vector3 LargerOrEqual(Vector3 vec1, Vector3 vec2) {
			Vector3 output;
			output.x = vec1.x >= vec2.x ? 1.0f : 0.0f;
			output.y = vec1.y >= vec2.y ? 1.0f : 0.0f;
			output.z = vec1.z >= vec2.z ? 1.0f : 0.0f;
			return output;
		}

		bool Vec3All(Vector3 comp) {
			if (comp.x == 1.0f &&
				comp.y == 1.0f &&
				comp.z == 1.0f) {
				return true;
			}

			return false;
		}

		void Update() {
			if(!m_bGo) {
				return;
			}
			Vector3 pos = transform.localPosition;
			/*Vector3 direction = m_LastPos - m_StartPos;
			pos += direction * m_fSpeed * GameManager.GM.DeltaTimeReal;
			if (Vector3.SqrMagnitude(pos - m_LastPos) <= m_fCloseEnoughSq) {
				pos = m_StartPos;
			}*/

			m_fLerpTime += m_fSpeed * Time.unscaledDeltaTime;

			if (m_fLerpTime >= 1.0f && m_bLoop) {
				m_fLerpTime = m_fLerpTime % 1.0f;
			}

			m_fLerpTime = Mathf.Clamp(m_fLerpTime, 0.0f, 1.0f);

			pos = Vector3.Lerp(m_StartPos, m_LastPos, m_fLerpTime);

			transform.localPosition = pos;
		}
	}
}