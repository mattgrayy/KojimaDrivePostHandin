using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bam
{
    public class BamRespawnHUDScript : MonoBehaviour
    {

        public Image bg, fill, outline;
        public TypogenicText text;

        public Bam.PlayerCameraScript myCam;

        // Use this for initialization
        void Start()
        {
            gameObject.layer = transform.parent.gameObject.layer;

            bg.gameObject.layer = gameObject.layer;
            fill.gameObject.layer = gameObject.layer;
            outline.gameObject.layer = gameObject.layer;
            text.gameObject.layer = gameObject.layer;
        }

        // Update is called once per frame
        void Update()
        {
            bool active = false;

            if (!myCam.m_mainPlayer)
            {
                return;
            }

            if (myCam.m_mainPlayer.GetRespawnCounter > 0)
            {
                active = true;
            }

            float targetAlpha = 0;
            Vector3 targetPos = new Vector3(-750, -45, 0);

            if (active)
            {
                targetAlpha = 0.9f;
                targetPos = new Vector3(-45, -45, 0);
            }

            bg.color = Color.Lerp(bg.color, new Color(1, 1, 1, targetAlpha), 6 * Time.deltaTime);
            fill.color = bg.color;
            outline.color = bg.color;

            text.transform.localPosition = Vector3.Lerp(text.transform.localPosition, targetPos, 8 * Time.deltaTime);

            fill.fillAmount = myCam.m_mainPlayer.GetRespawnCounter;
        }
    }
}