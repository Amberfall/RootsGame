using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] private int depthRequiredToWin;


    [Header("Events")]
    public GameEvent playerWon;

    private CanvasController canvasController;
    private AudioController audioController;

    private Scene currentLevel;
    private ServerNetworking serverNetworking;

    private void OnEnable()
    {
        canvasController = FindObjectOfType<CanvasController>();
        audioController = FindObjectOfType<AudioController>();
        serverNetworking = FindObjectOfType<ServerNetworking>();
    }

    private void Start()
    {
        canvasController.setDepthRequiredForWin(depthRequiredToWin);
        currentLevel = SceneManager.GetActiveScene();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    public void RestartLevel()
    {
        UnityEngine.Debug.Log("restarting from game controller...");
        ResumeGame();
        SceneManager.LoadScene(currentLevel.name);
    }
    public void LoseGame()
    {
        //TODO: animate withering root, wait a moment, then do the rest of this logic
        
        StartCoroutine(DeathSequence());
    }

    public void CheckForWin(object data)
    {
        if((int)data >= depthRequiredToWin)
        {
            UnityEngine.Debug.Log("reached required depth for win condition.");
            playerWon.Raise(default);
        }
    }
    public void WinGame()
    {
        if(serverNetworking!= null)
        {
            serverNetworking.RecordWin();
        }
        
    }

    IEnumerator DeathSequence()
    {
        PauseGame();
        yield return new WaitForSecondsRealtime(.25f);
        PlayDeathShrivelSound();
        yield return new WaitForSecondsRealtime(.3f);
        canvasController.OpenLosePanel();
        yield break;
    }

    private void PlayDeathShrivelSound()
    {
        if(audioController != null) 
        {
            audioController.PlayShrivelSound();
        }
    }

    public void RecordLoseCoord(Vector3Int coordinate)
    {
        if (serverNetworking != null)
        {
            Vector2Int lossCoordinate = new Vector2Int();
            lossCoordinate.x = coordinate.x; 
            lossCoordinate.y = coordinate.y;
            int sceneNumber = currentLevel.buildIndex;
            serverNetworking.RecordLoss(lossCoordinate);
        }
    }

}
