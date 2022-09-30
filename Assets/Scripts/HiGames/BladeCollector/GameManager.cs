using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiGames.BladeCollector
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private  GameObject player;
        [SerializeField] private  PlayerMovement playerMovement;
        [SerializeField] private  GameObject startScreen;
        [SerializeField] private  GameObject levelCompleteScreen;
        [SerializeField] private  GameObject gameOverScreen;
        private static readonly int İsRunning = Animator.StringToHash("isRunning");
        

        public void Starts() 
        {
            Time.timeScale = 1;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.ChallengePuppet = true;
            playerMovement.animation.SetBool(İsRunning,true);
            startScreen.gameObject.SetActive(false);
        }
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
        public void LevelComplete()
        {
            levelCompleteScreen.gameObject.SetActive(true);
        }
        public void GameOver()
        {
            gameOverScreen.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
