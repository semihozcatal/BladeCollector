using UnityEngine;

namespace HiGames.BladeCollector
{
    public class Wall : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("AroundBlade"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
