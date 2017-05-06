using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bam
{
    public class LobbyLocationScript : MonoBehaviour
    {
        public Canvas m_canvas;
        public Image m_timerImage, m_mask, m_timerBG, m_timerOutline;
        public bool m_gameReady = false;
        public Vector3 m_offset;
        public Text m_descText;
        public TypogenicText m_nameText;
        public float m_fScrollTimer, m_fPauseTimer;
        public Vector3 m_startPos, m_endPos;
        bool m_active;
        public Image m_descImage, m_descBack;
        public Vector3 m_openScale;
        public ParticleSystem m_nameBurst, m_nameBack, m_nameRays, m_timerRays;

        Camera m_cam;
        float m_fScrollSpeed = 20.3f;
        float m_fStartTime, m_fStartPauseTime;
        bool m_resetTextPos;
        public float m_fBobSpeed;
        Vector3 m_textBeginPos;
        public int m_camRefNum;
        float m_fontSize, m_emissionScale;

        // Use this for initialization
        void Start()
        {
            if(m_camRefNum>=4)
            {
                Debug.LogWarning(gameObject.name + " is being asked to look at player " + (m_camRefNum +1) + "'s camera");
            }

            m_timerImage.fillAmount = 0.0f;
            m_cam = Kojima.CameraManagerScript.singleton.playerCameras[m_camRefNum].GetComponent<Camera>();
            m_fStartTime = m_fScrollTimer;
            m_fStartPauseTime = m_fPauseTimer;
            m_descImage.transform.localScale = Vector3.zero;
            m_textBeginPos = m_nameText.transform.localPosition;
            m_fontSize = 18;
            SetLayer();
            m_timerRays.Stop();
            m_nameRays.Stop();
            m_nameBack.Stop();
        }

        void FixedUpdate()
        {
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.LookRotation(m_cam.transform.forward), 10 * Time.deltaTime);
        }

        void OnEnable()
        {
            m_timerRays.Stop();
            m_nameRays.Stop();
            m_nameBack.Stop();

            m_timerImage.fillAmount = 0.0f;
            m_active = false;
            m_nameText.Size = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_active)
            {
                m_nameText.transform.localPosition = m_textBeginPos;
                m_nameText.Size = Mathf.Lerp(m_nameText.Size, m_fontSize, Time.deltaTime * 6);
                ScrollText();
                m_timerBG.transform.localScale = Vector3.Lerp(m_timerBG.transform.localScale, Vector3.one, Time.deltaTime * 4f);

                if (!m_timerRays.isPlaying)
                {
                    m_timerRays.Play();
                    m_nameRays.Play();
                    m_nameBack.Play();
                }
            }
            else if(!m_active && m_fScrollTimer != m_fStartTime)
            {
                m_descText.transform.localPosition = m_startPos;
                m_fScrollTimer = m_fStartTime;
                m_fPauseTimer = m_fStartPauseTime;
                m_resetTextPos = false;
            }
            else if(!m_active)
            {
                DisplayGameModeName();
                TextBob(m_nameText, m_textBeginPos);
                m_descImage.transform.localScale = Vector3.Lerp(m_descImage.transform.localScale, Vector3.zero, Time.deltaTime * 10);
                m_timerBG.transform.localScale = Vector3.Lerp(m_timerBG.transform.localScale, Vector3.zero, Time.deltaTime * 4.0f);

                if (m_timerRays.isPlaying)
                {
                    m_timerRays.Stop();
                    m_nameRays.Stop();
                    m_nameBack.Stop();
                }
            }
        }

        public void LerpTimer(float timeModifier, bool active)
        {
            m_emissionScale = timeModifier;

            if (active)
            {
                m_timerImage.fillAmount = Mathf.Lerp(m_timerImage.fillAmount, m_timerImage.fillAmount + 0.2f, Time.deltaTime * timeModifier);
            }
            else
            {
                m_timerImage.fillAmount = Mathf.Lerp(m_timerImage.fillAmount, 0.0f, Time.deltaTime);
            }

            if (m_timerImage.fillAmount == 1.0f)
            {
                m_gameReady = true;
            }
        }

        void ScrollText()
        {
            if (m_fScrollTimer > 0)
            {
                m_fScrollTimer -= Time.deltaTime;
            }
            else if(m_fScrollTimer <= 0.0f && !m_resetTextPos)
            {
                m_descText.transform.localPosition = Vector3.MoveTowards(m_descText.transform.localPosition, m_endPos, Time.deltaTime * m_fScrollSpeed);
            }

            if(Vector3.Distance(m_descText.transform.localPosition, m_endPos) <= 0.0f)
            {               
                m_resetTextPos = true;
            }

            if(m_resetTextPos)
            {
                if (m_fPauseTimer > 0)
                {
                    m_fPauseTimer -= Time.deltaTime;
                }
                else
                {
                    m_descText.transform.localPosition = Vector3.MoveTowards(m_descText.transform.localPosition, m_startPos, Time.deltaTime * 22);

                    if (Vector3.Distance(m_descText.transform.localPosition, m_startPos) <= 0.0f)
                    {
                        m_fScrollTimer = m_fStartTime;
                        m_fPauseTimer = m_fStartPauseTime;
                        m_resetTextPos = false;
                    }
                }
            }
        }

        IEnumerator DisplayGameDesc()
        {
            float downscaleMultiplier = 0.02f;
            yield return new WaitForSeconds(0.1f);

                while (m_descImage.transform.localScale.x < (m_openScale.x * downscaleMultiplier) - 0.1f * downscaleMultiplier)
                {
                    m_descImage.transform.localScale = Vector3.Lerp(m_descImage.transform.localScale, m_openScale * downscaleMultiplier, Time.deltaTime * 11);

                    yield return new WaitForEndOfFrame();
                }

                while (m_descImage.transform.localScale.x > 1 * downscaleMultiplier)
                {
                    m_descImage.transform.localScale = Vector3.Lerp(m_descImage.transform.localScale, Vector3.one * downscaleMultiplier, Time.deltaTime * 11);

                    yield return new WaitForEndOfFrame();
                }                        
        }

        void DisplayGameModeName()
        {
            if(Vector3.Distance(m_cam.transform.position, gameObject.transform.position) < 90.0f)
            {
                m_nameText.Size = Mathf.Lerp(m_nameText.Size, 38, Time.deltaTime * 6);
            }
            else
            {
                m_nameText.Size = Mathf.Lerp(m_nameText.Size, 0, Time.deltaTime * 6);
            }
        }

        public void SetActive(bool isActive)
        {
            StopAllCoroutines();
            m_active = isActive;

            if(m_active)
            {
                StartCoroutine("DisplayGameDesc");
            }
        }

        void TextBob(TypogenicText textItem, Vector3 startPos)
        {
            textItem.transform.localPosition = Vector3.Lerp(textItem.transform.localPosition, startPos +
                (Vector3.up * Mathf.Sin(Time.time * m_fBobSpeed)), Time.deltaTime * 99);
        }

        public void SetCamRefNum(int num)
        {
            m_camRefNum = num;
        }

        void SetLayer()
        {
            m_timerImage.gameObject.layer = gameObject.layer;
            m_nameText.gameObject.layer = gameObject.layer;
            m_descImage.gameObject.layer = gameObject.layer;
            m_descBack.gameObject.layer = gameObject.layer;
            m_descText.gameObject.layer = gameObject.layer;
            m_mask.gameObject.layer = gameObject.layer;
            m_nameRays.gameObject.layer = gameObject.layer;
            m_nameBurst.gameObject.layer = gameObject.layer;
            m_nameBack.gameObject.layer = gameObject.layer;
            m_timerRays.gameObject.layer = gameObject.layer;
        }

        public void UpdateTimer(float time)
        {
            m_timerImage.fillAmount = time;
        }

        public void SetCamera(int camID)
        {
            m_cam = Kojima.CameraManagerScript.singleton.playerCameras[camID].GetComponent<Camera>();
        }
    }
}
