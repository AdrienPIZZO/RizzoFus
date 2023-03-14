using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

    public Board board;
    public int x;
    public int z;
    public Entity entity;
    
    public static GameObject prefab;

    private void Start()
    {

    }
    private void Update()
    {

    }
    public void init(Board board, int x, int z){
        this.board = board;
        this.x = x;
        this.z = z;
        entity = null;
    }
    public bool isEmpty(){
        //Debug.Log(elements[x,z]);
        return entity == null;   //Check if there is no object on the Square we are trying to move on
    }
    public Entity getEntity(){
        //Debug.Log(elements[x,z]);
        return entity;
    }
}