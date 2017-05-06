using UnityEngine;
using System.Collections;

namespace HF
{

    public class TestTwist : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("Twistishappening");
        }

        void OnDestroy()
        {
            Debug.Log("TwistDestroyed");
        }
    }
}