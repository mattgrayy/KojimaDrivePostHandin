using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HF
{
    public class UpdateText : MonoBehaviour
    {
        //fade and speed for the text not yet working as intended
        public float fade = 1.0f;
        public float speed = 0.1f;

        //has an xp event been triggered? this will tell us if it has.
        public bool m_demoText;

        public TypogenicText demoText;
        //public RectTransform rect;

        // Use this for initialization
        void Start()
        {
            //find the text component in the gameobjectc
            foreach(Transform child in transform)
            {
                if(child.gameObject.name == "ExpText")
                {
                    demoText = child.gameObject.GetComponent<TypogenicText>();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_demoText == true)
            {
                //add fancy effects to the text componenet
                demoText.ColorTopLeft -= new Color(0, 0, 0, fade * Time.deltaTime);
                //rect.position += Vector3.up * Time.deltaTime * speed;

                Color demoTextColour = demoText.ColorTopLeft;
                Color fadeColour = new Color(0, 0, 0, 0);

                if (demoText.ColorTopLeft.a < fadeColour.a)
                {
                    //reset the xp text 
                    //rect.localPosition = new Vector3(8, -6, -3);
                    demoText.GetComponent<TypogenicText>().Text = "";
                    demoText.ColorTopLeft = new Color(0, 0, 0, 1);
                    m_demoText = false;
                }
            }
        }
    }
}
