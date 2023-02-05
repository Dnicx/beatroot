using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paw : MonoBehaviour
{

    public string message; //PlayerInBoundR if pawR PlayerInBoundL if paw L

    void OnTriggerStay(Collider Other){
        if( Other.gameObject.tag == "Player"){
            // playerInBounds = true;
            SendMessageUpwards( message );
            Debug.Log( "Player Enter");
        }
    }
    
    void OnTriggerExit(Collider Other){
        if( Other.gameObject.tag == "Player"){
            // playerInBounds = false;
            SendMessageUpwards( "PlayerExitBound" );
        }
    }

}
