using UnityEngine;
using System.Collections;

public class LightningFlashScript : MonoBehaviour {

	public float m_flashStartup = 0.05f;
	public float m_flashRecovery = 0.1f;
	public float m_maxFlashIntensity = 100.0f;
	private float m_progress = 0.0f;
	private float m_intensity = 0.0f;
	private bool m_flashing = false;
	private Light m_flash;
		
	void Awake()
	{
		m_flash = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{
		if(m_flashing)
		{
			m_progress += Time.deltaTime;
			if(m_progress <= m_flashStartup)
			{
				m_intensity = (m_progress / m_flashStartup) * m_maxFlashIntensity;
			}
			else
			{
				m_intensity = m_maxFlashIntensity - (((m_progress - m_flashStartup) / m_flashRecovery) * m_maxFlashIntensity);
			}
			if(m_intensity < 0)
			{
				Cease();
			}
		}
		m_flash.intensity = m_intensity;
	}

	public void Flash()
	{
		m_flashing = true;
		m_progress = 0.0f;
		m_intensity = 0.0f;
	}

	public void Cease()
	{
		m_flashing = false;
		m_progress = 0.0f;
		m_intensity = 0.0f;
	}
}
