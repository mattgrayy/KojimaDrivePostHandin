using UnityEngine;
using System.Collections;

public class GrappleLaunchManager : MonoBehaviour
{
    Transform m_GrappleLaucher;

    public void setGrappleLauncher(Transform _Launcher)
    {
        m_GrappleLaucher = _Launcher;
    }
    public Transform getGrappleLauncher()
    {
        return m_GrappleLaucher;
    }
}
