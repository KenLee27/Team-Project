


using UnityEngine;
using System.Collections;
namespace RotateShapeParticle
{





	public class RotateShapeParticle : MonoBehaviour
	{

		// Start Rotation
		public Vector3 m_StartRotation;     // Euler angles when this script starts.

		// Start Rotation overtime
		public Vector3 m_RotationOvertime;  // Rotation to rotate this object overtime.


		// ######################################################################
		// MonoBehaviour functions
		// ######################################################################

		#region MonoBehaviour functions

		// Use this for initialization
		void Start()
		{
			// Set start local rotation
			transform.localEulerAngles = m_StartRotation;
		}

		// Update is called once per frame
		void Update()
		{
			// Rotate around local pivot overtime.
			transform.localEulerAngles = transform.localEulerAngles + (m_RotationOvertime * Time.deltaTime);
		}

		#endregion //MonoBehaviour functions
	}
}