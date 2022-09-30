using UnityEngine;

namespace HiGames.BladeCollector
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float camSpeed;
        public Vector3 offSet;
        private void LateUpdate()
        {
            Vector3 desiredPosition = target.position + offSet;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, camSpeed);
            transform.position = smoothPosition;
        }
    }
}
