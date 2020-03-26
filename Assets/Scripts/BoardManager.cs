using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Chosen[,] chosens{set;get;}

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private const int NB_TILES = 8;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chosensPrefabs;
    public List<GameObject> activeChosen = new List<GameObject>();

    private void Start()
    {
       SpawnAllChosen();
    }

    private void Update()
    {
        UpdateSelection();
        DrawBoard();
    }

    private void UpdateSelection()
    {
        if(!Camera.main)
            return;
        
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Plane"))){
            Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }
 
    private void SpawnChosen(int index, Vector3 position)
    {
        GameObject go = Instantiate(chosensPrefabs[index], position, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        chosens[(int)position.x, (int)position.z] = go.GetComponent<Chosen>();
        chosens[(int)position.x, (int)position.z].SetPosition((int)position.x, (int)position.z);
        activeChosen.Add(go);
    }

    private void SpawnAllChosen()
    {
        activeChosen = new List<GameObject> ();
        chosens = new Chosen[NB_TILES, NB_TILES];
        SpawnChosen(0, (Vector3.right + Vector3.forward + Vector3.up) * TILE_SIZE / 2);
        SpawnChosen(1, (Vector3.right + Vector3.forward) * TILE_SIZE * 8 + (-Vector3.right - Vector3.forward + Vector3.up) * TILE_SIZE / 2);
    } 

    private void DrawBoard()
    {
        Vector3 widthLine = Vector3.right * TILE_SIZE * NB_TILES;
        Vector3 heigthLine = Vector3.forward * TILE_SIZE * NB_TILES;
    
        for(int i=0; i<=NB_TILES; i++){
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for(int j=0; j<=NB_TILES; j++){
                start = Vector3.right * i;
                Debug.DrawLine(start, start + heigthLine);
            }
        }

        // Draw selection
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
            Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
        }
    }

}

