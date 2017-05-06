using UnityEngine;
using System.Collections;

public class WaterScript : MonoBehaviour
{
	public float waveFrequency = 0.53f;
	public float waveHeight = 0.48f;
	public float waveLength = 0.71f;
	public bool edgeBlend = true;
	public bool forceFlatShading = true;
	Mesh mesh;
	Vector3[] verts;
	Material mat;


	public bool m_bCameraFade = true;
	public GameObject m_FadeOutPosition;

	private float a, b, c, d; //used in wave function
	void Start()
	{
		mat = GetComponent<MeshRenderer>().material;
	}
	
	void setEdgeBlend()
	{
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			edgeBlend = false;
		}
		if (edgeBlend)
		{
			Shader.EnableKeyword("WATER_EDGEBLEND_ON");
			if (Camera.main)
			{
				Camera.main.depthTextureMode |= DepthTextureMode.Depth;
			}
		}
		else
		{
			Shader.DisableKeyword("WATER_EDGEBLEND_ON");
		}
	}

	// Update is called once per frame
	void Update()
	{
		a = Time.time * Mathf.PI * 2.0f * waveFrequency;
		b = Mathf.PI * 2.0f;
		c = waveHeight;
		d = waveLength;

		mat.SetFloat("_A", a);
		mat.SetFloat("_B", b);
		mat.SetFloat("_C", c);
		mat.SetFloat("_D", d);

		//CalcWave ();
		setEdgeBlend();
		SetFadeDists();

	}

	void SetFadeDists()
	{
		if (m_bCameraFade)
		{
			Shader.DisableKeyword("WATER_WORLDSPACEFADE_OFF");
		}
		else
		{
			Shader.EnableKeyword("WATER_WORLDSPACEFADE_ON");
			if (m_FadeOutPosition != null)
			{
				mat.SetVector("_WorldFadeCoords", m_FadeOutPosition.transform.position);
			}

		}
	}

	void CalcWave()
	{
		for (int i = 0; i < verts.Length; i++)
		{
			verts[i].y = WaveFunction(verts[i]);
		}
		mesh.vertices = verts;
		mesh.RecalculateNormals();
	}

	public float WaveFunction(Vector3 pos)
	{
		return c * Mathf.Sin(((pos.x % d) / d) * b + a) * Mathf.Sin(((pos.z % d) / d) * b + a);
	}

	//Partial derivative /w respect to x (derivative c * sin( ((x MOD d) / d) * b + a)  * sin(((z MOD d) / d *b + a))
	float PartialDerivativeX(Vector3 pos)
	{
		return b * c * Mathf.Cos(a + (b * (pos.x % d) / d)) * Mathf.Sin(a + (b * (pos.z % d) / d)) / d;
	}
	//Partial derivative /w respect to z (derivative c * sin( ((x MOD d) / d) * b + a)  * sin(((z MOD d) / d *b + a)) 
	float PartialDerivativeZ(Vector3 pos)
	{
		return b * c * Mathf.Sin(a + (b * (pos.x % d) / d)) * Mathf.Cos(a + (b * (pos.z % d) / d)) / d;
	}


	public Vector3 NormalFromXZ(Vector3 pos)
	{
		Vector3 normal = new Vector3();

		Vector3 world = pos;
		Vector3 partDirX = new Vector3();
		Vector3 partDirZ = new Vector3();

		partDirX.x = 1;
		partDirX.y = PartialDerivativeX(pos);
		partDirX.z = 0;
		partDirX.Normalize();

		partDirZ.x = 0;
		partDirZ.y = PartialDerivativeZ(pos);
		partDirZ.z = 1;
		partDirZ.Normalize();

		normal = -Vector3.Cross(partDirX, partDirZ);
		normal.Normalize();

		Debug.DrawRay(world, partDirX, Color.red, 0.1f, false);
		Debug.DrawRay(world, normal, Color.green, 0.1f, false);
		Debug.DrawRay(world, partDirZ, Color.blue, 0.1f, false);

		return normal;
	}

	public struct WaveInfo
	{
		public Vector3 m_surfacePosition;
		public Vector3 m_surfacenormal;
	}

	public void GetSurfaceInfoAtXZ(Vector3 pos, out WaveInfo waveInfo)
	{
		pos.y = WaveFunction(pos) + transform.position.y;
		waveInfo.m_surfacePosition = pos;
		waveInfo.m_surfacenormal = NormalFromXZ(pos);
	}
}
