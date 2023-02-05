using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public string soundTrackName;
    public static GameManager Instance;
    public GameObject loadingScreen;

    private Camera mainCamera;

    public GameObject player;

    [SerializeField] private GameState currentState = GameState.FollowPlayer;
    public float rightThreshold;
    public float leftThreshold;

    public enum GameState {
        FollowPlayer,
        Lock,
        Death,
        Victory
    };

    public GameState getCurrentState()
    { return currentState;}

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        if( soundTrackName != "" )
            AudioManager.instance.PlayMusic( soundTrackName );
    }

    public void updateState( GameState state )
    {
        currentState = state;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if ( currentState == GameState.FollowPlayer && player != null )
            FollowPlayer();
    }

    private void FollowPlayer()
    {
        float xDiff = player.transform.position.x - mainCamera.transform.position.x;

        float speed = 10.0f;

        if ( xDiff > rightThreshold )
        {
            mainCamera.transform.Translate( speed * Time.deltaTime, 0, 0 );
        }
        if ( xDiff < leftThreshold )
        {
            mainCamera.transform.Translate( -speed * Time.deltaTime, 0, 0 );
            
        }
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

    public GameObject getPlayer()
    {
        return player;
    }
    
}
