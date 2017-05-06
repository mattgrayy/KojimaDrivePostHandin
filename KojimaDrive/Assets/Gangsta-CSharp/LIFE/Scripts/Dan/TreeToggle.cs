using UnityEngine;
using System.Collections;

public class TreeToggle : BaseWorldChange
{
    public GameObject[] m_Objects;

    public override void enableState()
    {
       // Debug.Log("set trees active");

        if (m_Objects != null)
        {
            RaycastHit hit;
            foreach (GameObject G in m_Objects)
            {
                G.SetActive(true);

                foreach (Transform T in G.transform)
                {
                  //  Debug.Log("");
                    if (T.position.y <= 1f)
                    {
                        if (Physics.Raycast(T.position, Vector3.up, out hit))
                        {
                            T.position = hit.point;
                           // Debug.Log(hit.collider.gameObject);
                        }

                    }          

                }
            }

        }


        //throw new NotImplementedException();
    }

    public override void disableState()
    {
       // Debug.Log("set trees deactive");
        if (m_Objects != null)
        {
            foreach (GameObject G in m_Objects)
            {
                G.SetActive(false);
            }
        }
        //throw new NotImplementedException();
    }

    public override void translateState()
    {
        // throw new NotImplementedException();
    }
}

