using UnityEngine;
using System.Collections;
using System;

public class WorldChangeLowPolSpawn : BaseWorldChange
{
    public GameObject testOBject;
    bool currentlyVis;

    public override void translateState()
    {
        if (translate)
        {
            Material[] mats = testOBject.GetComponent<Renderer>().materials;
            if (!currentlyVis)
            {
                Material newMat = null;
                foreach (Material m in mats)
                {
                    newMat = m;
                    m.color = new Color(newMat.color.r, newMat.color.g, newMat.color.b, (newMat.color.a - Time.deltaTime));

                }
                if (newMat.color.a < 0.0f)
                {
                    testOBject.SetActive(false);
                    translate = false;
                }
            }
            else
            {
                Material newMat = null;
                foreach (Material m in mats)
                {
                    newMat = m;
                    m.color = new Color(newMat.color.r, newMat.color.g, newMat.color.b, (newMat.color.a + Time.deltaTime));

                }
                if (newMat.color.a > 1.0f)
                {
                    translate = false;
                }

            }

        }
    }

    public override void enableState()
    {
        testOBject.SetActive(true);
          translate = true;
          currentlyVis = true;
    }
    public override void disableState()
    {
        //testOBject.GetComponent<MeshRenderer>().enabled = false;
        currentlyVis = false;
        translate = true;
    }

}
