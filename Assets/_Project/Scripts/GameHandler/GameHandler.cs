using _Project.Scripts.Patterns;
using UnityEngine;

namespace _Project.Scripts.GameHandler
{
    public class GameHandler : Singleton<GameHandler>
    {
        [SerializeField]
        private GameObject pausedGameCanvas;

        private void Start()
        {
            ShowPlayerControlsPopup();
        }

        public void PlayerPause()
        {
            OpenPlayerUICanvas("PauseGame_Canvas");
        }

        public void PlayerWin()
        {
            Debug.LogError("PlayerWin logic is missing");
        }

        public void PlayerLose()
        {
            Time.timeScale = 0f;
            OpenPlayerUICanvas("YouLose_Canvas");
        }

        public void ShowPlayerControlsPopup()
        {
            OpenPlayerUICanvas("ControlsPopup_Canvas");
        }

        private void OpenPlayerUICanvas(string canvasName)
        {
            for(int i = 0; i < pausedGameCanvas.transform.childCount; i++)
            {
                var child = pausedGameCanvas.transform.GetChild(i).gameObject;
                child.SetActive(child.name == canvasName);
            }
            pausedGameCanvas.SetActive(true);
        }
    
        public void ClosePausedGameCanvas()
        {
            pausedGameCanvas.SetActive(false);
            for(int i = 0; i < pausedGameCanvas.transform.childCount; i++)
            {
                var child = pausedGameCanvas.transform.GetChild(i).gameObject;
                child.SetActive(false);
            }
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
