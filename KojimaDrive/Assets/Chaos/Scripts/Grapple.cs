using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour
{
    Transform m_Launcher;
    GrappleRopeManager m_RopeManager;

    Rigidbody m_Rb;
    bool m_HasHit = false;

    // Distances for the initial rope length
    [SerializeField] float m_fMaxDistance = 20, m_fMinDistance = 3;
    float m_fCurrentDistance = 0;

    float m_fCurrentPullMagnitude, m_fCurrentTargetPullMagnitude;

    [SerializeField] float m_fRopeSnapTimeMax;
	float m_fRopeSnapTime = 0;

    void Start()
    {
        m_RopeManager = GetComponent<GrappleRopeManager>();
        m_RopeManager.addLauncher(m_Launcher);
        this.GetComponent<NodeController>().setTarget(m_Launcher.gameObject);

        m_Rb = GetComponent<Rigidbody>();

        m_Rb.velocity += transform.forward * 30 + m_Launcher.GetComponent<Rigidbody>().velocity;
        m_Rb.velocity += Vector3.up * 3;
    }

    void FixedUpdate()
    {
        //drop if we reach max rope length
        if (m_HasHit == false && Vector3.Distance(transform.position, m_Launcher.position) > m_fMaxDistance)
        {
            m_Rb.velocity = Vector3.down * 10;
        }

        float ropeLength = m_RopeManager.calculateRopeLength();
        if (m_Launcher != null && m_fCurrentDistance != 0 && ropeLength > m_fCurrentDistance)
        {
            GameObject nearestNode = m_RopeManager.getLastNode();

            if (nearestNode.transform.parent != null && (transform.parent.GetComponent<Kojima.CarScript>() || transform.parent.GetComponent<Draggable>()))
            {
                m_fRopeSnapTime += Time.deltaTime;

				m_fCurrentPullMagnitude = m_Launcher.GetComponent<Rigidbody>().velocity.magnitude;
               	m_fCurrentTargetPullMagnitude = transform.parent.GetComponent<Rigidbody>().velocity.magnitude;

                Vector3 carToNearestNode = nearestNode.transform.position - m_Launcher.position;

                m_Launcher.GetComponent<Rigidbody>().AddForce((carToNearestNode * m_Launcher.GetComponent<Rigidbody>().mass * 2) + (carToNearestNode * (m_fCurrentTargetPullMagnitude * getPullForceModifier())));

                Vector3 targToNearestNode = GetComponent<NodeController>().getTarget().transform.position - transform.position;
                transform.parent.GetComponent<Rigidbody>().AddForce((targToNearestNode * transform.parent.GetComponent<Rigidbody>().mass * 2) + (targToNearestNode * (m_fCurrentPullMagnitude * 1000)));
			}
            else
            {
                Vector3 carToNearestNode = nearestNode.transform.position - m_Launcher.position;
                m_Launcher.GetComponent<Rigidbody>().velocity += carToNearestNode * 100;
            }
        }
        else if (m_Launcher != null)
        {
            if (m_fCurrentPullMagnitude != 0)
            {
                m_fCurrentPullMagnitude = 0;
                m_fCurrentTargetPullMagnitude = 0;
            }

            m_fRopeSnapTime = 0;

            if (m_RopeManager != null && m_RopeManager.getLastNode().transform.parent != null && m_RopeManager.getLastNode().transform.parent.GetComponent<Kojima.CarScript>())
            {
                m_RopeManager.getLastNode().transform.parent.GetComponent<Kojima.CarScript>().SetCanMove(true);
            }
            m_Launcher.GetComponent<GrappleLaunch>().getCar().GetComponent<Kojima.CarScript>().SetCanMove(true);
        }
    }

	void Update()
	{
        if (m_HasHit)
        {
            if (m_fRopeSnapTime >= m_fRopeSnapTimeMax || transform.parent == null)
            {
                m_RopeManager.killAllNodes();
            }
        }
	}

    public void addLauncher(Transform _launcher)
    {
        m_Launcher = _launcher;
    }

    void OnCollisionEnter(Collision col)
    {
        if (!m_HasHit)
        {
            m_HasHit = true;
			m_Launcher.GetComponent<GrappleLaunch> ().GrappleHit ();
            GetComponent<Collider>().isTrigger = true;
            transform.parent = col.transform;
            transform.localScale = new Vector3(1/transform.parent.localScale.x, 1/transform.parent.localScale.y, 1/transform.parent.localScale.z) * 0.1f;
            m_Rb.velocity = Vector3.zero;
            m_Rb.useGravity = false;
            m_Rb.isKinematic = true;
            m_fCurrentDistance = Vector3.Distance(transform.position, m_Launcher.position);
            if (m_fCurrentDistance < m_fMinDistance)
            {
                m_fCurrentDistance = m_fMinDistance;
            }

            if (transform.parent != null && transform.parent.GetComponent<Kojima.CarScript>())
            {
                AddJoint();
                m_Rb.isKinematic = false;
            }

            if (col.transform.GetComponent<CaravanController>())
            {
                if (!m_Launcher.GetComponent<GrappleLaunch>().getCar().GetComponent<CaravanManager>())
                {
                    m_Launcher.GetComponent<GrappleLaunch>().getCar().gameObject.AddComponent<CaravanManager>();
                }

                m_Launcher.GetComponent<GrappleLaunch>().getCar().GetComponent<CaravanManager>().setIsCaravanGrappled(true);
                m_Launcher.GetComponent<GrappleLaunch>().getCar().GetComponent<CaravanManager>().setGrappledCaravan(col.transform);
            }
        }
    }

    void AddJoint()
    {
        ConfigurableJoint newJoint = gameObject.AddComponent<ConfigurableJoint>();
        newJoint.zMotion = ConfigurableJointMotion.Locked;
        newJoint.xMotion = ConfigurableJointMotion.Locked;
        newJoint.yMotion = ConfigurableJointMotion.Locked;
        newJoint.axis = Vector3.one;
        newJoint.connectedBody = transform.parent.GetComponent<Rigidbody>();
    }

    int getPullForceModifier()
    {
        if (m_Launcher.GetComponent<GrappleLaunch>().getCar().GetComponent<CaravanManager>())
        {
            if (m_Launcher.GetComponent<GrappleLaunch>().getCar().GetComponent<CaravanManager>().getIsCaravanGrappled())
            {
                return 200;
            }
        }
        return 800;
    }
}