using UnityEngine;
using System.Collections;

public class ObjectFader : MonoBehaviour {

    Material[] allMats;

    Renderer rend;
    public float fadeRate =0.1f;

    bool currentState;
    bool inputState= true;

    void Start()
    {
        allMats = GetComponent<Renderer>().materials;
 
    }

    public void changeState()
    {
        inputState = !inputState;
    }

    public Renderer getRenderer()
    {
        return rend;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            inputState = !inputState;
        }

        if(inputState != currentState)
        {
            if(transfurOpacity())
            {
                 currentState= inputState;
            }
        }

    }

    bool transfurOpacity()
    {
        float currentAlpha;
        if (currentState)
        {

            currentAlpha = (allMats[0].color.a + fadeRate);
        }
        else
        {
            currentAlpha = (allMats[0].color.a - fadeRate);
        }
        bool transitionCompleate= false;

        foreach(Material mat in allMats)
        {
            Color newCol;
            newCol = new Color(mat.color.r, mat.color.g, mat.color.b, currentAlpha);
            mat.color = newCol;       
        }
        if(currentAlpha <= 0 || currentAlpha >= 1)
        {
            transitionCompleate = true;
        }

        return transitionCompleate;
    }

}
