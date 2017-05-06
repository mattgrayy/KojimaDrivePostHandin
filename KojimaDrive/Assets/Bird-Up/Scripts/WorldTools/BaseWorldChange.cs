using UnityEngine;
using System.Collections;

public abstract class BaseWorldChange : MonoBehaviour {

    public WorldManager.ZONEVAL myState;
    public bool MyStateAndAbove;

    public abstract void enableState();
    public abstract void disableState();

    public abstract void translateState();

    public bool translate;

    public void flipState(WorldManager.ZONEVAL _in)
    {
        if (MyStateAndAbove)
        {
            if (_in <= myState)
            {
                enableState();
            }
            else
            {
                disableState();
            }
        }
        else
        {
            if (_in == myState)
            {
                enableState();
            }
            else
            {
                disableState();
            }
        }

    }

}
