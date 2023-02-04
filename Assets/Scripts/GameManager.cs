using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   

    public static GameManager Instance;
    public GameObject loadingScreen;

    private Camera mainCamera;

    public GameObject Player;

    private GameState currentState = GameState.FollowPlayer;

    public enum GameState {
        FollowPlayer,
        Lock
    };

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        
    }

    public void LoadScene( int sceneId )
    {
        StartCoroutine( LoadSceneAsync( sceneId ) );
    }

    IEnumerator LoadSceneAsync( int sceneId )
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync( sceneId );

        loadingScreen.SetActive( true );

        while ( operation.isDone )
        {
            float progress = Mathf.Clamp01( operation.progress / 0.9f );

            yield return null;
        }
    }
    
}
