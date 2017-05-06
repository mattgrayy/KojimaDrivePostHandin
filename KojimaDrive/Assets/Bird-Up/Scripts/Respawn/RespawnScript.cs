using UnityEngine;
using System.Collections;
    namespace Kojima
{
public class RespawnScript : MonoBehaviour {


        public bool m_bAlive = true;
        private Transform currentResetPoint;
        public int controlingPlayer;
        public Vector3 m_DeathPoint;
        public RespawnManager respawnManager;
        //private CarScript CS;
        // Score vals
        public static int[] s_Deaths = new int[4] { 0, 0, 0, 0 }; // Death counters
        public static float s_fResetTime = 1.0f;
        float m_fResetTime = 0.0f;
        public static int s_DeathScoreMult = -500; // Airtime score per-second (multiply the above score by this!)


    void Start()
    {
      // CS = GetComponent<CarScript>();     
       s_Deaths = new int[4] { 0, 0, 0, 0 };
    }

    public void setResetPoint(Transform _in)
    {
        currentResetPoint = _in;
    }

    public void setBodyColour(Material _inMat)
    {

           // CS.m_carBody.GetComponent<Renderer>().material = _inMat;
    }

    public void moveToStartPoint()
    {
        transform.position = currentResetPoint.position;
        transform.rotation = currentResetPoint.rotation;
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }


    public void KillPlayer()
    {
        if (m_bAlive)
        {
           // Soundbank.PlayOneShot("SFX", "SPLASHDOWN");

               // m_DeathPoint = transform.position;
                m_bAlive = false;
                //moveToCurrentReset();

              //  m_fResetTime = GameManager.GM.CurrentTime + s_fResetTime;

               // s_Deaths[controlingPlayer]++;

           // ScorePopup("SPLASHDOWN!", s_DeathScoreMult, true, true);
         //   Soundbank.PlayOneShot("SFX", "POINT_REMOVE");
        }
    }


    public void moveToCurrentReset()
    {
        currentResetPoint = respawnManager.findClosest(transform);

            m_bAlive = true;

        transform.position = currentResetPoint.position;
        transform.rotation = currentResetPoint.rotation;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
      //  GetComponent<Rigidbody>().Sleep();
    }




    public void applyForce(float _inForce, Vector3 _direction)
    {
        gameObject.GetComponent<Rigidbody>().AddForce((_direction) * _inForce);
    }


        void Update()
        {

            if (m_bAlive)
            {
                //Nasty way of doing it but somethimes the reset bugs out and the water should be at base level anyway
                //if (transform.position.y < -2.0f)
                //{ // Final out-of-world fallback

                //    KillPlayer();
                //}

                //TootHorn();
            }
            else
            {
                //if (m_fResetTime < GameManager.GM.CurrentTime)
                //{
                //    moveToCurrentReset();
                //}
            }

            bool bRespawn = false;
          //  bRespawn = Input.GetAxis("Respawn" + (controlingPlayer + 1)) != 0;

            //replace with team wide reset button
            if (bRespawn)
            {
                //moveToCurrentReset();
            }
        }
}
}