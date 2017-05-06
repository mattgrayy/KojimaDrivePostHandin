using UnityEngine;
using System.Collections;

public class TreeCollision : MonoBehaviour {

    public GameObject m_particle;
    private Transform m_location;

	void Start () {
        m_location = transform.FindChild("leaf");

	}

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("hit");
        GameObject tempP = (GameObject)Instantiate(m_particle, m_location.position, Quaternion.identity);

        Destroy(tempP, 5f);

    }

}
