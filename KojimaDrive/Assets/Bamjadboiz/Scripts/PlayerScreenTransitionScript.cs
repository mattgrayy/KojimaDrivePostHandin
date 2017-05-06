using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bam
{
    public class PlayerScreenTransitionScript : MonoBehaviour
    {
        Image myImg;

        bool hideScreen = false;

        // Use this for initialization
        void Start()
        {
            myImg = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            Color targetCol = new Color(1f, 1f, 1f, 0);

            if (hideScreen)
            {
                targetCol.a = 2;
            }

            myImg.color = Color.Lerp(myImg.color, targetCol, 4.5f * Time.deltaTime);
        }

        public void BeginTransition()
        {
            hideScreen = true;
        }

        public void EndTransition()
        {
            hideScreen = false;
        }
    }
}
