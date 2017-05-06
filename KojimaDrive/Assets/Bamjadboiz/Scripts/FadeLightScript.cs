using UnityEngine;
using System.Collections;

public class FadeLightScript : MonoBehaviour
{

    [SerializeField]
    Light light;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (light.intensity > 0)
            light.intensity -= Time.deltaTime * 10;
        else
            Destroy(this);
    }
}
