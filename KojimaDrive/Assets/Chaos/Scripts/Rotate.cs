using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    [SerializeField] float m_rotationSpeed = 100.0f;

	void Update ()
    {
        transform.Rotate(Vector3.up * (m_rotationSpeed * Time.deltaTime));
    }
}
