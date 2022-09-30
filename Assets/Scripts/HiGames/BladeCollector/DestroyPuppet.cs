using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace HiGames.BladeCollector
{
    public class DestroyPuppet : MonoBehaviour
    {
        private bool isShooted = false;
        public bool İsShooted 
        {
            get => isShooted;
            set => isShooted = value;
        }

        private int randomPower;
        public int RandomPower 
        {
            get => randomPower;
            set => randomPower = value;
        }

        private bool isDestroyedPuppet;
        public bool İsDestroyedPuppet
        {
            get => isDestroyedPuppet;
            set => isDestroyedPuppet = value;
        }

        [SerializeField] private Rigidbody puppetRb;
        private MeshFilter colors;
        [SerializeField] private Mesh[] colorMesh;
    
        [SerializeField] PlayerMovement puppetBool;
        [SerializeField] TextMeshProUGUI text;

        private AudioSource bladeSound;
        private ParticleSystem particle;
        private static readonly int İsRunning = Animator.StringToHash("isRunning");

        [SerializeField] private GameObject ghostPuppet;
        [SerializeField] private GameObject destroyEffect;

        public void Start()
        {
            randomPower = Random.Range(1, 16);
            text.text = randomPower.ToString();
            colors = GetComponent<MeshFilter>();
            bladeSound = GetComponent<AudioSource>();
            particle = GetComponent<ParticleSystem>();
        
        }

        private void Update()
        {
            if (randomPower <= 4)
                colors.mesh = colorMesh[0];
            
            else if (randomPower > 4 && randomPower <= 8)
                colors.mesh = colorMesh[1];
            
            else if (randomPower > 8 && randomPower <= 12)
                colors.mesh = colorMesh[2];
            
            else if (randomPower > 12 && randomPower <= 16)
                colors.mesh = colorMesh[3];
            
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("AroundBlade"))
            {
                particle.Play();
                bladeSound.Play();
                Destroy(other.gameObject);
                transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.3f);
                //transform.DOShakeScale(0.3f, 0.2f, 1, 0f, true);
                randomPower--;
                text.text = randomPower.ToString();
                isDestroyedPuppet = false;
                
                if (randomPower == 0)
                {
                    isDestroyedPuppet = true;
                    Destroy(this.gameObject);
                    CreateDestroyEffect();
                    CreateGhost();
                    puppetBool.ChallengePuppet = true;
                    puppetBool.animation.SetBool(İsRunning,true);
                }
            }
        }

        public void CreateGhost()
        {
            Vector3 offSet = new Vector3(0,0,1.5f);
            Vector3 spawnPosition = transform.position + offSet;
            GameObject _ghostPuppet = Instantiate(ghostPuppet, spawnPosition, quaternion.identity);
        }

        public void CreateDestroyEffect()
        {
            GameObject _destroyEffect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
    }
}
