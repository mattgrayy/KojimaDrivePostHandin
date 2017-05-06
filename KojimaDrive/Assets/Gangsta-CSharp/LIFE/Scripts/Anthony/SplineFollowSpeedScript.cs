using UnityEngine;
using System.Collections;

namespace GCSharp
{
	//Mostly shameful copypasta of SplineFollower by Dave(?)
	public class SplineFollowSpeedScript : MonoBehaviour
	{
		public enum SplineWalkerMode
		{
			Once,
			Loop,
			PingPong
		}

		[SerializeField]
		private Bird.BezierSpline m_defaultSpline;

		private Bird.BezierSpline m_currentSpline;

		[SerializeField]
		private float m_speed = 3.0f;

		//Determines accuracy of approximated length, more accurate (and less quick) the closer it gets to 0.
		[Range(0.001f, 1f)]
		public float m_resolution = 0.1f;

		//Approximated distance/length of the spline
		private float m_approximateDistance;

		//Current progress (measured in distance rather than percentage) across the spline
		private float m_currentDistance;

		public bool m_lookForward;

		//very slow if true, useful for testing
		public bool m_allowRealtimeSplineAlterations = false; 

		[SerializeField]
		private float m_progress;

		[SerializeField]
		private SplineWalkerMode m_mode;

		[SerializeField]
		private bool goingForward = true;

		public bool m_bEaseForwardLooking = false; // Added this to "ease" the turning of an object following a path -sam 17/01/2017
		public float m_fMaximumDegreesOfRotationPerTick = 3.0f;

		public void Start()
		{
			m_currentSpline = m_defaultSpline;
			m_progress = 0.0f;
			m_currentDistance = 0.0f;
			ResetSpline();
		}

		public void SetProgress(float _progress)
		{
			m_currentDistance = m_approximateDistance * _progress;
		}

		public void SetSpeed(float _speed)
		{
			m_speed = _speed;
		}

		public void SetSpline(Bird.BezierSpline _spline)
		{
			m_currentSpline = _spline;
			m_approximateDistance = 0.0f;
			//Determine approximate distance/length of spline
			if (m_currentSpline)
			{
				Vector3 startPoint, endPoint, prevPoint;
				startPoint = m_currentSpline.GetPoint(0.0f);
				prevPoint = m_currentSpline.GetPoint(0.0f);
				endPoint = m_currentSpline.GetPoint(1.0f);
				for (float t = m_resolution; t < 1.0f; t += m_resolution)
				{
					Vector3 curPoint = m_currentSpline.GetPoint(t);
					m_approximateDistance += Vector3.Distance(prevPoint, curPoint);
					prevPoint = curPoint;
				}
				m_approximateDistance += Vector3.Distance(prevPoint, endPoint);
			}
		}

		public void ResetSpline()
		{
			SetSpline(m_defaultSpline);
		}

		// Tweaked this here, to allow me to overload things in a child class -sam 17/01/2017
		public void Update()
		{
			if (m_allowRealtimeSplineAlterations)
			{
				SetSpline(m_currentSpline);
			}
			FollowSpline();
		}


		public void FollowSpline()
		{
			if (goingForward)
			{
				m_currentDistance += Time.deltaTime * m_speed;

				if (m_currentDistance > m_approximateDistance)
				{
					if (m_mode == SplineWalkerMode.Once)
					{
						m_currentDistance = m_approximateDistance;
					}
					else if (m_mode == SplineWalkerMode.Loop)
					{
						m_currentDistance -= m_approximateDistance;
					}
					else
					{
						m_currentDistance = m_approximateDistance * 2f - m_currentDistance; //dunno why *2 but whatev
						goingForward = false;
					}
				}
			}
			else
			{
				m_currentDistance -= Time.deltaTime * m_speed;
				if (m_mode == SplineWalkerMode.Loop)
				{
					if (m_currentDistance < 0f)
					{
						m_currentDistance += m_approximateDistance;
					}

					if (m_currentDistance > m_approximateDistance)
					{
						m_currentDistance -= m_approximateDistance;
					}
				}
				else
				{
					if (m_currentDistance < 0f)
					{
						m_currentDistance = -m_currentDistance;
						goingForward = true;
					}
				}
			}
			m_progress = m_currentDistance / m_approximateDistance;
			Vector3 position = m_currentSpline.GetPoint(m_progress);
			transform.position = position;
			if (m_lookForward)
			{
				if (m_bEaseForwardLooking)
				{
					Quaternion rotation = transform.rotation;
					Quaternion targetRotation = transform.rotation;
					targetRotation.SetLookRotation(m_currentSpline.GetDirection(m_progress));

					// TODO: Add banking! -sam 17/01/2017
					transform.rotation = Quaternion.RotateTowards(rotation, targetRotation, m_fMaximumDegreesOfRotationPerTick);
				}
				else
				{
					transform.LookAt(position + m_currentSpline.GetDirection(m_progress));
				}
			}
		}
	}

}
