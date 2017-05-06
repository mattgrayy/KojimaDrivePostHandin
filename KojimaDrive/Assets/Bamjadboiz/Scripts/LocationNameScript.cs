using UnityEngine;
using System.Collections;

namespace Bam
{
    public class LocationNameScript : MonoBehaviour
    {

        public TypogenicText myText;

        float displayTimer = 0;

        public Vector3 offscreenPos_Left, onscreenPos, offscreenPos_Right;

        // Use this for initialization
        void Start()
        {
            myText.transform.localPosition = offscreenPos_Right;
            gameObject.layer = transform.parent.gameObject.layer;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 targetPos = offscreenPos_Right;
            Vector3 targetScale = new Vector3(1.5f, 0.0f, 1.0f);

            //Keeps the text in the same general place vertically despite the scale changes
            targetPos.y -= (1 - targetScale.y) * 15.5f;

            float speed = 4f;

            if (displayTimer > 0)
            {
                targetPos = onscreenPos;
                displayTimer -= Time.deltaTime;
                targetScale = Vector3.one;
                speed = 10;
            }

            myText.transform.localPosition = Vector3.Lerp(myText.transform.localPosition, targetPos, speed * Time.deltaTime);
            myText.transform.localScale = Vector3.Lerp(myText.transform.localScale, targetScale, 8 * Time.deltaTime);
        }

        public void DisplayLocation(string locationName)
        {
            if (locationName != myText.Text)
            {
                transform.localPosition = offscreenPos_Left;
                transform.localScale = new Vector3(1.5f, 0.0f, 1.0f);

                myText.Text = locationName;
                displayTimer = 5;
            }
        }
    }
}