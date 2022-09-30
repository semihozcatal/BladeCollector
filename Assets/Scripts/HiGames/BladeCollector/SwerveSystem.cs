using UnityEngine;

namespace HiGames.BladeCollector
{
    public class SwerveSystem : MonoBehaviour
    {
        public float NewMoveFactorX => moveFactorX / Screen.width;
        private float lastFingerPosX;
        private float moveFactorX;

        public void Update()
        {
#if !UNITY_EDITOR
        if (Input.touchCount == 0)
        {
            Touch finger = Input.GetTouch(0);
            if (finger.phase == TouchPhase.Began )
            {
                lastFingerPosX = Input.mousePosition.x;
            }
            else if (finger.phase == TouchPhase.Stationary)
            {
                moveFactorX = Input.mousePosition.x - lastFingerPosX;
                lastFingerPosX = Input.mousePosition.x;
            }
            else if (finger.phase == TouchPhase.Ended)
            {
                moveFactorX = 0f;
            }
        }
#endif
            if (Input.GetMouseButtonDown(0))
            {
                lastFingerPosX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButton(0))
            {
                moveFactorX = Input.mousePosition.x - lastFingerPosX;
                lastFingerPosX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                moveFactorX = 0f;
            }
        }
    }
}