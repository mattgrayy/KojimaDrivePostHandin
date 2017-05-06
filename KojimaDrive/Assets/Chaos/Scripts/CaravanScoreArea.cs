using UnityEngine;
using System.Collections;

public class CaravanScoreArea : MonoBehaviour {

    Kojima.CaravanScoreRace race;

    public void setRace(Kojima.CaravanScoreRace _race)
    {
        race = _race;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<CaravanManager>() && other.transform.GetComponent<CaravanManager>().getIsCaravanGrappled())
        {
            race.addScore(1, other.transform);
            Destroy(other.transform.GetComponent<CaravanManager>().getGrappledCaravan().gameObject);
            other.transform.GetComponent<GrappleLaunchManager>().getGrappleLauncher().GetComponent<GrappleLaunch>().fireGrapple();
        }
    }
}
