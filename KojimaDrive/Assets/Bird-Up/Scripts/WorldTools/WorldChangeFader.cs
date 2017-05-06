using UnityEngine;
using System.Collections;
using System;

public class WorldChangeFader : BaseWorldChange {

    
    public override void translateState()
    {
       
        
    }

    public override void enableState()
    {
      
        GetComponent<MeshRenderer>().enabled = true;
      //  translate = true;
        //currentlyVis = true;

    }
    public override void disableState()
    {
      //  currentlyVis = false;
        GetComponent<MeshRenderer>().enabled = false;
      //  translate = true;
    }


}
