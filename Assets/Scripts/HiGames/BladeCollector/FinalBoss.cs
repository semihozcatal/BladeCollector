using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HiGames.BladeCollector
{
    public class FinalBoss : MonoBehaviour
    {
        [SerializeField] private PlayerMovement closePlayerMovement;
        [SerializeField] private GameManager gameManager;
        private int bossPower;

        public int BossPower
        {
            get => bossPower;
            set => bossPower = value;
        }
        [SerializeField] TextMeshProUGUI text;
        private AudioSource bladeSound;
        private ParticleSystem particle;
        [SerializeField] private GameObject finalEffect;
        void Start()
        {
            bossPower = Random.Range(15, 35);
            text.text = bossPower.ToString();
            bladeSound = GetComponent<AudioSource>();
            particle = GetComponent<ParticleSystem>();
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("AroundBlade"))
            {
                bladeSound.Play();
                Destroy(other.gameObject);
                transform.DOPunchScale(new Vector3(-0.02f, -0.02f, -0.02f), 0.3f, 1, 0.3f);
                //transform.DOShakeScale(0.3f, 0.2f, 1, 0f, true);
                bossPower--;
                text.text = bossPower.ToString();
                particle.Play();
                if (bossPower == 0)
                {
                    Destroy(this.gameObject);
                    GameObject _finalEffect = Instantiate(finalEffect, transform.position, quaternion.identity);
                    gameManager.LevelComplete();
                    closePlayerMovement.enabled = false;
                }
            }
        }
    }
}
