using UnityEngine;
using System.Collections;

namespace Bird {
	public class PostFXDemoScript : MonoBehaviour {

		// Use this for initialization
		void Start() {
			m_BlurFX = (PostFX_Generic)PostFXStack.GetPostFXStack("PostFX").GetPostFX("BoxBlur");
			m_ImageAdjustment = (PostFX_Generic)PostFXStack.GetPostFXStack("PostFX").GetPostFX("PFX");
		}

		PostFX_Generic m_ImageAdjustment;
		PostFX_Generic m_BlurFX;

		// Update is called once per frame
		float fBlurLerpAmount = 3.0f;
		float fPasses = 0;
		void Update() {
			PostFX_Generic.shaderProperty_t prop = m_ImageAdjustment.GetProperty("_HSVAAdjust");
			prop.m_vecVal.x += 0.1f * Time.deltaTime;

			fPasses += fBlurLerpAmount * Time.deltaTime;
			if(fPasses > 10.0f || fPasses < 0.0f) {
				fBlurLerpAmount = -fBlurLerpAmount;
			}

			m_BlurFX.m_nPasses = Mathf.RoundToInt(fPasses);
		}
	}
}