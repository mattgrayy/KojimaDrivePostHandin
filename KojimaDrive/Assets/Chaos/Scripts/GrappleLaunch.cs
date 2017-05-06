using UnityEngine;
using System.Collections;
using Rewired;

public class GrappleLaunch : MonoBehaviour
{
    [SerializeField] Transform m_visualLauncher;
    [SerializeField] Transform m_grapplePrefab;
    Transform m_CurrentGrapple;
    Transform m_Car;
    bool m_IsGrappled = false;
    private Rewired.Player m_rewiredPlayer;

    public void init()
    {
        if (transform.root != null && transform.root.GetComponent<Rigidbody>() != null)
        {
            m_Car = transform.parent;
            GetComponent<ConfigurableJoint>().connectedBody = transform.root.GetComponent<Rigidbody>();
            m_visualLauncher.parent = transform.parent;
            transform.parent = null;
        }
    }

    void Update()
    {
        if (m_rewiredPlayer == null && m_Car != null)
        {
            m_rewiredPlayer = m_Car.GetComponent<Kojima.CarScript>().GetRewiredPlayer();
        }

        if ((m_rewiredPlayer != null) && (m_rewiredPlayer.GetButtonDown("Grapple")))
        {
            fireGrapple();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            fireGrapple();
        }

        if (m_CurrentGrapple == null && m_IsGrappled)
        {
            resetGrappled();
        }
    }

    public void fireGrapple()
    {
        if (m_IsGrappled)
        {
            if (m_CurrentGrapple != null)
            {
                m_CurrentGrapple.GetComponent<GrappleRopeManager>().killAllNodes();

                m_CurrentGrapple = null;
            }
            resetGrappled();
        }
        else if (m_Car != null)
        {
            m_IsGrappled = true;

		

            m_CurrentGrapple = Instantiate(m_grapplePrefab, transform.position + m_Car.forward + (m_Car.up / 5), m_Car.rotation) as Transform;
            m_CurrentGrapple.GetComponent<Grapple>().addLauncher(transform);
            m_CurrentGrapple.GetComponent<GrappleRopeManager>().addLauncher(transform);
        }
    }

    public void killGrapple()
    {
        if (m_CurrentGrapple != null)
        {
            m_CurrentGrapple.GetComponent<GrappleRopeManager>().killAllNodes();

            m_CurrentGrapple = null;
            resetGrappled();
        }
    }

    public void resetGrappled()
    {
        m_IsGrappled = false;

        if (m_Car.GetComponent<CaravanManager>())
        {
            m_Car.GetComponent<CaravanManager>().setIsCaravanGrappled(false);
        }
		m_Car.GetComponent<Kojima.CarScript> ().DisableWallClimbing ();
    }

    public Transform getCar()
    {
        return m_Car;
    }

    private void OnDestroy()
    {
        if (m_CurrentGrapple != null)
        {
            m_CurrentGrapple.GetComponent<GrappleRopeManager>().killAllNodes();
        }
        if (m_visualLauncher != null)
        {
            Destroy(m_visualLauncher.gameObject);
        }
    }

	public void GrappleHit()
	{
		m_Car.GetComponent<Kojima.CarScript> ().AllowWallClimbing ();
	}
}
