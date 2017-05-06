using UnityEngine;
using System.Collections;

namespace Bam
{
    public class LocationZoneScript : MonoBehaviour
    {
        

        public static bool locationZonesDisabled = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            if (!locationZonesDisabled)
            {
                Kojima.CarScript player;

                player = other.GetComponent<Kojima.CarScript>();

                if (player)
                {
                    player.DisplayLocationName(gameObject.name);
                }
            }
        }

        public static void DisableOrEnableLocationZones(bool enable)
        {
            locationZonesDisabled = !enable;
        }
    }
}
