using UnityEngine;
using System.Collections;

namespace KRace
{

    public class CameraScript : MonoBehaviour {

        public GameObject player;
        //Camera carCamera;
        Vector3 camTargetPos;
        Vector3 camOffsetPos;
        Vector3 camTargetRot;
        Quaternion camOffsetRot;

        // Use this for initialization
        void Start () {
            CameraSetup();
	    }
	
	    // Update is called once per frame
	    void Update () {
            CameraLogic();
        }

        void CameraSetup()
        {
            //carCamera = gameObject.GetComponent<Camera>();
            //carCamera = new Camera();
            //carCamera.GetComponentInChildren<Camera>();
            //carCamera.transform.Rotate(0.0f, 180.0f, 0.0f); //HERE

            //carCamera.transform.rotation = Quaternion.Euler(camTargetRot.x, 1.0f, camTargetRot.z);

            //carCamera.transform.LookAt(transform.position);
            //carCamera.transform.localPosition = new Vector3(100.0f, 100.0f, 100.0f);
            //carCamera.transform.Translate(100.0f, 100.0f, 100.0f);
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f));
        
            //transform.position = new Vector3(100.0f, 100.0f, 100.0f);


        }

        void CameraLogic()
        {
            camOffsetPos = player.transform.localPosition - new Vector3(-2.0f, -1.0f, 0.0f);
            transform.position = camOffsetPos;
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, (player.transform.rotation.y - 90), 0.0f));
            //camTargetRot = new Vector3(0.0f, player.transform.rotation.y, 0.0f);
            //transform.rotation = Quaternion.Euler(camTargetRot.x, camTargetRot.y, camTargetRot.z);


            //transform.LookAt(player.transform.position);

            //camTargetPos = transform.position - camOffsetPos;
            //carCamera.transform.localPosition = camTargetPos;


            //if (camTargetPos != carCamera.transform.position)
            //{

            //}
            ////camTargetRot = transform.rotation - camOffsetRot;
        }
    }
}