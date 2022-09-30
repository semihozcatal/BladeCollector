using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HiGames.BladeCollector
{
    public class PlayerMovement : MonoBehaviour
    {
        #region DEĞİŞKENLER

        [SerializeField] private bool challengePuppet;

        public bool ChallengePuppet
        {
            get => challengePuppet;
            set => challengePuppet = value;
        }

        private int bladeCount;

        public int BladeCount
        {
            get => bladeCount;
            set => bladeCount = value;
        }

        [SerializeField] private TextMeshProUGUI _bladeCount;
        [SerializeField] private GameManager gameManager;
        public FinalBoss bossPower;

        private SwerveSystem swerveSystem;
        private float runValue = 1;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform player;
        [SerializeField] private float swerveSpeed;
        [SerializeField] private float playerSpeed;

        [SerializeField] private List<GameObject> aroundBlades = new List<GameObject>();
        [SerializeField] private GameObject aroundBlade;
        [SerializeField] private GameObject circle;

        private RaycastHit puppet;

        private Coroutine shootCoroutine;

        private DestroyPuppet target;

        private bool isGameEnded;

        public new Animator animation;
        private static readonly int İsRunning = Animator.StringToHash("isRunning");

        //private float shootTime;

        #endregion


        public void Start()
        {
            swerveSystem = GetComponent<SwerveSystem>();
            rb = GetComponent<Rigidbody>();
            animation = GetComponent<Animator>();
            animation.GetBool(İsRunning);
            Time.timeScale = 0;
        }

        public void FixedUpdate()
        {

            #region OYUNCU HAREKETİ

            if (challengePuppet)
            {
                var swerveAmount = swerveSpeed * swerveSystem.NewMoveFactorX;
                rb.velocity = new Vector3(
                    swerveAmount * 40 * Time.fixedDeltaTime,
                    0,
                    runValue * playerSpeed * Time.fixedDeltaTime);
                circle.GetComponent<AroundKnife>().enabled = true;
            }

            if (challengePuppet == false)
            {
                var swerveAmount = swerveSpeed * swerveSystem.NewMoveFactorX;
                rb.velocity = new Vector3(swerveAmount * 20 * Time.fixedDeltaTime, 0, 0);
                circle.GetComponent<AroundKnife>().enabled = false;
            }

            #endregion


            #region KUKLA İLE KARŞILAŞMA - RAYCAST

            if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), new Vector3(0, 0, 0.1f), out puppet, 7f))
            {
                if (puppet.collider.gameObject.CompareTag("Puppet"))
                {
                    challengePuppet = false;
                    animation.SetBool(İsRunning, false);
                    var destroyPuppet = puppet.collider.gameObject.GetComponent<DestroyPuppet>();

                    if (target != destroyPuppet)
                    {
                        if (shootCoroutine != null)
                        {
                            StopCoroutine(shootCoroutine);
                        }

                        shootCoroutine = StartCoroutine(ShootBlades(destroyPuppet.RandomPower));
                        target = destroyPuppet;
                    }
                }

                if (puppet.collider.gameObject.CompareTag("GhostPuppet"))
                {
                    var ghostPuppet = puppet.collider.GetComponent<DestroyPuppet>();
                    if (target != ghostPuppet)
                    {
                        challengePuppet = true;
                        StopCoroutine(shootCoroutine);
                        animation.SetBool(İsRunning, true);
                        target = ghostPuppet;
                    }
                }
            }

            #endregion
        }

        #region LEVEL COMPLETE

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("LastDance"))
            {
                StartCoroutine(Test());
                if (aroundBlades.Count > 0)
                {
                    if (aroundBlades.Count >= bossPower.BossPower)
                        isGameEnded = true;
                    else
                        isGameEnded = false;
                    
                    challengePuppet = false;
                    animation.SetBool(İsRunning, false);

                    for (int i = 0; i < bossPower.BossPower; i++)
                    {
                        aroundBlades[0].transform.rotation = Quaternion.Euler(180, 180, 90);
                        aroundBlades[0].GetComponent<Rigidbody>().isKinematic = false;
                        aroundBlades[0].GetComponent<BoxCollider>().enabled = true;
                        aroundBlades[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 3, 10);
                        aroundBlades.RemoveAt(0);
                        bladeCount--;
                        _bladeCount.text = bladeCount.ToString();
                    }

                }
                else if (aroundBlades.Count == 0)
                {
                    Debug.Log("1");
                    isGameEnded = true;
                    challengePuppet = false;
                    animation.SetBool(İsRunning, false);
                    gameManager.GameOver();
                }
            }
        }

        IEnumerator Test()
        {
            yield return new WaitForSeconds(1.5f);
            if (aroundBlades.Count == 0 && !isGameEnded)
            {
                Debug.Log("2");
                challengePuppet = false;
                animation.SetBool(İsRunning, false);
                gameManager.GameOver();
            }
        }

        #endregion

        #region BIÇAK YARATMA

        public void CreateBlade()
        {
            GameObject aroundBlades = Instantiate(aroundBlade,
                Random.onUnitSphere * 1.2f + transform.position + new Vector3(0, 1.5f, 0),
                Quaternion.Euler(new Vector3(180, 180, 90)));
            this.aroundBlades.Add(aroundBlades);
            aroundBlades.GetComponent<BoxCollider>().enabled = false;
            aroundBlades.transform.parent = circle.transform;
            bladeCount++;
            _bladeCount.text = bladeCount.ToString();

            var position1 = aroundBlades.transform.position;
            aroundBlades.transform.RotateAround(player.transform.position, Vector3.up, 100f * Time.deltaTime);
            var position2 = aroundBlades.transform.position;
            aroundBlades.transform.position = position1;
            aroundBlades.transform.LookAt(position2);
        }

        #endregion
        
        #region BIÇAK FIRLATMA

        IEnumerator ShootBlades(int bladePower)
        {
            if (aroundBlades.Count > 0)
            {
                for (int i = 0; i < bladePower; i++)
                {
                    //aroundBlades[0].transform.SetParent(null);
                    yield return new WaitForSeconds(0.2f);
                    aroundBlades[0].transform.position = player.transform.position + new Vector3(0, 1.5f, 0);
                    aroundBlades[0].transform.rotation = Quaternion.Euler(180, 180, 90);
                    aroundBlades[0].GetComponent<Rigidbody>().isKinematic = false;
                    aroundBlades[0].GetComponent<BoxCollider>().enabled = true;
                    aroundBlades[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 3, 15);
                    aroundBlades.RemoveAt(0);

                    bladeCount--;
                    _bladeCount.text = bladeCount.ToString();
                  
                    if (bladeCount == 0)
                    {
                        yield return new WaitForSeconds(0.5f);
                        if (puppet.transform.gameObject.CompareTag("Puppet"))
                        {
                            challengePuppet = false;
                            gameManager.GameOver();
                        }
                    }
                }
            }
            else if (aroundBlades.Count <= 0 && puppet.collider.gameObject.CompareTag("Puppet"))
            {
                yield return new WaitForSeconds(0.5f);
                challengePuppet = false;
                gameManager.GameOver();
            }
        }
        
        #endregion
    }
}