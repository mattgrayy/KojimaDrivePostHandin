using UnityEngine;
using System.Collections;

public class MaterialID : MonoBehaviour {

    [SerializeField]
    private int m_materialID;

    public int GetID() { return m_materialID; }
}
