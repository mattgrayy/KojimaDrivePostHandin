using UnityEngine;
using System.Collections;

namespace Bam
{
    public class BowlingHUDScript : MonoBehaviour
    {
        public static BowlingHUDScript singleton;

        [SerializeField]
        VolcanoMadness bowlingMode;

        public TypogenicText pinsHit;
        public TypogenicText strike;

        bool fadeAway = false;

        int pinsHitCount = 0;
        bool strikeAnimating = false;
        bool playerGotAStrike = false;

        [SerializeField]
        AudioClip strikeSound;
        [SerializeField]
        AudioSource source;

        void Awake()
        {
            singleton = this;
        }

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            strike.Size = 0;
            pinsHitCount = 0;
            pinsHit.transform.localScale = Vector3.zero;
            fadeAway = false;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 size = Vector3.one;

            if (fadeAway || pinsHitCount == 0)
            {
                size = Vector3.zero;
                pinsHit.transform.parent.gameObject.SetActive(false);
            }

            pinsHit.transform.localScale = Vector3.Lerp(pinsHit.transform.localScale, size, 8 * Time.deltaTime);

            float sin = Mathf.Sin(Time.timeSinceLevelLoad * 2.5f) * 1;
            strike.transform.localPosition = new Vector3(strike.transform.localPosition.x, 120 + (sin), strike.transform.localPosition.z);
        }

        public void PinHasBeenHit()
        {
            pinsHit.transform.parent.gameObject.SetActive(true);
            pinsHitCount++;
            UpdatePinCount(pinsHitCount);
            HF.PlayerExp.AddEXP(bowlingMode.GetRunnerID, 15, true, true, "Pin hit");
        }

        public void UpdatePinCount(int pins)
        {
            pinsHitCount = pins;
            pinsHit.Text = "Pins hit: " + pins;
            pinsHit.transform.localScale = Vector3.one * 2;

            if (pinsHitCount >= 10 && !strikeAnimating)
            {
                Strike();
            }
        }

        public void FadeAway()
        {
            fadeAway = true;

            if(playerGotAStrike)
            {
                pinsHitCount *= 2;
            }

            bowlingMode.GiveScore(pinsHitCount);
        }

        public void Strike()
        {
            HF.PlayerExp.AddEXP(bowlingMode.GetRunnerID, 50, true);
            bowlingMode.EndRoundEarly(4);

            StopCoroutine(StrikeAnim());
            StartCoroutine(StrikeAnim());
        }

        IEnumerator StrikeAnim()
        {
            strike.Size = 0;
            strike.Tracking = 35;

            strikeAnimating = true;
            strike.gameObject.SetActive(true);

            float spd = 0.1f;

            source.PlayOneShot(strikeSound);

            while (strike.Tracking > 0.05f)
            {          
                strike.Tracking = Mathf.Lerp(strike.Tracking, 0, spd * Time.deltaTime);
                strike.Size = Mathf.Lerp(strike.Size, 80, spd * Time.deltaTime);

                spd += Time.deltaTime * 20;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(3.250f);
            spd = 0.1f;

            while (strike.Size > 5)
            {
                

                strike.Tracking = Mathf.Lerp(strike.Tracking, 35, spd * Time.deltaTime);
                strike.Size = Mathf.Lerp(strike.Size, 0, spd * 2 * Time.deltaTime);

                spd += Time.deltaTime * 100;
                //Debug.Log("dgff");
                yield return new WaitForEndOfFrame();
            }

            strike.Size = 0;
            strikeAnimating = false;
            strike.gameObject.SetActive(false);
        }
    }
}