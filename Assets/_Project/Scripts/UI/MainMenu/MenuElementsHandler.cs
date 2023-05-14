using UnityEngine;

public class MenuElementsHandler : MonoBehaviour
{
    public void ContinueGame()
    {
        Debug.LogWarning("Continue Button has no implementation!");
    }

    public void NewGameNormalDifficulty()
    {
        SceneHandler.Instance.StartNewGame();
        SceneHandler.Instance.isNormalDifficulty = true;
        //Debug.LogWarning("New Game Normal Difficulty has no implementation!");
    }
    
    public void NewGameHardDifficulty()
    {
        SceneHandler.Instance.StartNewGame();
        SceneHandler.Instance.isNormalDifficulty = false;
        //Debug.LogWarning("New Game Hard Difficulty has no implementation!");
    }
    
    
    public void QuitButton()
    {
        SceneHandler.Instance.QuitGame();
    }
    
}
