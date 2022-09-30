using UnityEngine;

namespace HiGames.BladeCollector
{
    public class GetKnife : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(new Vector3(50, 0, 0) * Time.deltaTime);
        }
        private PlayerMovement player;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                player = other.GetComponent<PlayerMovement>();
                player.CreateBlade();
                Destroy(this.gameObject);
            }
        }
    }
}
