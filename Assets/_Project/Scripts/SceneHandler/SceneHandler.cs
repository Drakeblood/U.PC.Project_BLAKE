using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private const string mainMenu = "MainMenu";
    private const string loadingScene = "LoadingScene";
    private const string build1505 = "Build1505";

    [HideInInspector]
    public float loadingProgress;
    
    [HideInInspector] 
    public bool isSceneLoadedProperly = true;

    [HideInInspector] 
    public float roomsGenerated = 0;

    [HideInInspector]
    public float roomsToGenerate = 0;

    [HideInInspector]
    public bool isNormalDifficulty = true; //only for DD purposes
    
    private void Awake()
    {
        if (ReferenceManager.SceneHandler != null && ReferenceManager.SceneHandler != this)
        {
            Destroy(gameObject);
        }
        else 
        {
            ReferenceManager.SceneHandler = this;
            DontDestroyOnLoad(ReferenceManager.SceneHandler);
        }
    }

    public void StartNewGame()
    {
        GetComponent<LevelHandler>().ResetValues();
        StartCoroutine(LoadNewSceneAdditive(build1505));
    }

    public void LoadGame()
    {
        Debug.Log("loadgame");
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadNewSceneAdditive(mainMenu));
    }

    public void LoadNewLevel(string sceneToLoadString)
    {
        StartCoroutine(LoadNewSceneAdditive(sceneToLoadString));
    }

    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator LoadNewSceneAdditive(string sceneToLoadString)
    {
        isSceneLoadedProperly = false;
        SceneManager.LoadSceneAsync(loadingScene);

        var asyncOperation = SceneManager.LoadSceneAsync(sceneToLoadString, LoadSceneMode.Additive);
        
        while (!asyncOperation.isDone)
        {
            loadingProgress = Mathf.Clamp01(asyncOperation.progress / .9f);
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadString));
        isSceneLoadedProperly = true;

        while (roomsGenerated < roomsToGenerate)
        {
            loadingProgress = Mathf.Clamp01((roomsGenerated / roomsToGenerate) / .9f);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.UnloadSceneAsync(loadingScene);
        ResetValues();
    }

    private void ResetValues()
    {
        loadingProgress = 0;
        roomsGenerated = 0;
        roomsToGenerate = 0;
        Time.timeScale = 1;
    }
}
