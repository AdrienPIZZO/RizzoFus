using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

    public Board board;
    public int x;
    public int z;
    public Entity occupant;
    
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
        occupant = null;
    }
}