using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TestChild : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestChild Strart");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void test(){
        Debug.Log("Le boss");
    }
}
