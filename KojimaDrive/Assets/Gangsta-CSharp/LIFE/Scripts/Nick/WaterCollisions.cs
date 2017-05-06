using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class WaterCollisions : MonoBehaviour
    {
        private int waveNumber;
        public float distanceX, distanceZ;
        public List<float> waveAmplitude;
        public float m_magnitudeDivider;
        public Vector2[] m_ImpactPos;
        public float[] m_distance;
        public float m_speedWaveSpreader;

        Mesh mesh;

        // Use this for initialization
        void Start()
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < 8; i++)
            {

                waveAmplitude[i] = GetComponent<Renderer>().material.GetFloat("_WaveAmplitude" + (i + 1));
                if (waveAmplitude[i] > 0)

                {
                    m_distance[i] += m_speedWaveSpreader;
                    GetComponent<Renderer>().material.SetFloat("_Distance" + (i + 1), m_distance[i]);
                    GetComponent<Renderer>().material.SetFloat("_WaveAmplitude" + (i + 1), waveAmplitude[i] * 0.98f);
                }
                if (waveAmplitude[i] < 0.01)
                {
                    GetComponent<Renderer>().material.SetFloat("_WaveAmplitude" + (i + 1), 0);
                    m_distance[i] = 0;
                }


            }
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.rigidbody)
            {
                waveNumber++;
                if (waveNumber == 9)
                {

                    waveNumber = 1;
                }
                waveAmplitude[waveNumber - 1] = 0;
                m_distance[waveNumber - 1] = 0;

                distanceX = this.transform.position.x - col.gameObject.transform.position.x;
                distanceZ = this.transform.position.z - col.gameObject.transform.position.z;

                m_ImpactPos[waveNumber - 1].x = col.transform.position.x;
                m_ImpactPos[waveNumber - 1].y = col.transform.position.z;

                GetComponent<Renderer>().material.SetFloat("_xImpact" + waveNumber, col.transform.position.x);
                GetComponent<Renderer>().material.SetFloat("_zImpact" + waveNumber, col.transform.position.z);


                GetComponent<Renderer>().material.SetFloat("_OffsetX" + waveNumber, distanceX / mesh.bounds.size.x * 2.5f);
                GetComponent<Renderer>().material.SetFloat("_OffsetZ" + waveNumber, distanceZ / mesh.bounds.size.z * 2.5f);

                GetComponent<Renderer>().material.SetFloat("_WaveAmplitude" + waveNumber, col.rigidbody.velocity.magnitude * m_magnitudeDivider);


            }
        }
    }
}