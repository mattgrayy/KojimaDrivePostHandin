//===================== Kojima Drive - Bamjadboiz 2017 ====================//
//
// Author:		Orlando 
// Purpose:		[NOT YET IMPLEMENTED] Uses a projector to draw an arrow on the floor,
//				that points towards any particular game object.
// Namespace:	Bam
//
//===============================================================================//
using UnityEngine;
using System.Collections;

//To be implemented!
namespace Bam
{
	[RequireComponent(typeof(Projector))]
	public class FloorArrow : MonoBehaviour
	{
		public Kojima.CarScript m_car;
		public GameObject m_target;
		public float m_offsetY;
		public float m_radius;
		public float m_speedOrbit;
		private Projector m_projector;

		void Awake()
		{
			m_projector = GetComponent<Projector>();
		}
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
			Vector3 vec = m_target.transform.position - m_car.transform.position;
			vec.y = 0;

			Vector3 arrowPos = m_car.transform.position +   vec.normalized * m_radius;
			arrowPos.y += m_offsetY;

			m_projector.transform.position = arrowPos;

			//This is correct, just need to fix the triangle img/ use the arrow instead? Do something anywho.
			float angle = Mathf.Atan2(vec.x, vec.z) * Mathf.Rad2Deg;
			m_projector.transform.rotation = Quaternion.Euler(90, angle, 0);

		}
	}
}