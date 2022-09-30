using UnityEngine;

namespace HiGames.BladeCollector
{
    public class AroundKnife : MonoBehaviour
    {
        private void Update()
        {
            var transform1 = transform;
            transform.RotateAround(
                transform1.position,
                transform1.parent.up,
                100f*Time.deltaTime);
        }
    }
}
