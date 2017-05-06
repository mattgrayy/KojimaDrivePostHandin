using UnityEngine;
using System.Collections;

//A camera shake script that makes use of different properties classes that act like profiles for different terrain types on the island -Thomas Moreton

//Add this script to the camera object
public class DynamicCameraShake : MonoBehaviour {


    //The maximum angle at which the camera's rotation can change each shake iteration
    const float m_maxAngle = 10f;
    //The different shake profiles set up for driving over different types of surfaces (these can be customised in the editor)
    public Properties m_sandShake;
    public Properties m_grassShake;
    public Properties m_rockShake;

    IEnumerator m_currentShakeCoroutine;

    //Call this function to start the shaking of the camera with the specific profile (The shake is ran inside a co routine)
    public void StartShake(Properties properties)
    {
        if (m_currentShakeCoroutine != null)
        {
            StopCoroutine(m_currentShakeCoroutine);
        }

        m_currentShakeCoroutine = Shake(properties);
        StartCoroutine(Shake(properties));
    }

    IEnumerator Shake(Properties properties)
    {
        //Bunch of maths with the use of waypoints to dictate camera movement
        float completionPercent = 0;
        float movePercent = 0;

        float angle_radians = properties.angle * Mathf.Deg2Rad - Mathf.PI;
        Vector3 previousWaypoint = Vector3.zero;
        Vector3 currentWaypoint = Vector3.zero;
        float moveDistance = 0;

        Quaternion targetRotation = Quaternion.identity;
        Quaternion previousRotation = Quaternion.identity;

        do
        {
            if (movePercent >= 1 || completionPercent == 0)
            {
                float dampingFactor = DampingCurve(completionPercent, properties.dampingPercent);
                float noiseAngle = (Random.value - .5f) * 2 * Mathf.PI;
                angle_radians += Mathf.PI + noiseAngle;
                currentWaypoint = new Vector3(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians)) * properties.strength * dampingFactor;
                previousWaypoint = transform.localPosition;

                moveDistance = Vector3.Distance(currentWaypoint, previousWaypoint);

                targetRotation = Quaternion.Euler(new Vector3(currentWaypoint.y, currentWaypoint.x).normalized * properties.rotationPercent * dampingFactor * m_maxAngle);
                previousRotation = transform.localRotation;
                movePercent = 0;
            }

            completionPercent += Time.deltaTime / properties.duration;
            movePercent += Time.deltaTime / moveDistance * properties.speed;
            transform.localPosition = Vector3.Lerp(previousWaypoint, currentWaypoint, movePercent);
            transform.localRotation = Quaternion.Slerp(previousRotation, targetRotation, movePercent);

            yield return null;
        } while (moveDistance > 0);
    }

    //Used to calculate how quickly to slow down the shake to a halt
    float DampingCurve(float x, float dampingPercent)
    {
        x = Mathf.Clamp01(x);
        float a = Mathf.Lerp(2, .25f, dampingPercent);
        float b = 1 - Mathf.Pow(x, a);

        return b * b * b;
    }
    //Sort of acts like a struct that contains several ways to customise the shake
    [System.Serializable]
	public class Properties
    {
        public float angle;
        public float strength;
        public float speed;
        public float duration;
        [Range(0,1)]
        public float noisePercent;
        [Range(0, 1)]
        public float dampingPercent;
        [Range(0, 1)]
        public float rotationPercent;
    }
}
