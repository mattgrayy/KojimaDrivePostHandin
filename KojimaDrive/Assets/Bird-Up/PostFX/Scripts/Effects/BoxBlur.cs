//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: A simple box-blur effect
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	[System.Serializable]
	public class BoxBlur : PostFXObject {
		public override void SetupMaterial() {
			
		}

		public override void RenderEffect(RenderTexture src, RenderTexture dst) {
			
		}

		[Header("Box-Blur")]
		[Range(0, 20)]
		public int Iterations;
		[Range(0, 4)]
		public int DownRes;
		public Texture2D m_BlurMask;
	}
}