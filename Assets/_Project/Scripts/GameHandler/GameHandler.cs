using UnityEngine;

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
        Time.timeScale = 0f;
        OpenPlayerUICanvas("YouWin_Canvas");
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
