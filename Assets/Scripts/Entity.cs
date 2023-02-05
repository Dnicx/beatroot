using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int maxHp = 3;
    
    [SerializeField]
    protected int currentHp;

    // Start is called before the first frame update
    protected void Start()
    {
        currentHp = maxHp;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
