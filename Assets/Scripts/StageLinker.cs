using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLinker : MonoBehaviour
{

    // This class contains link to another scene

    public int nextSceneNum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            GameManager.Instance.LoadScene( nextSceneNum );

        }
    }

}
