using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject[,] element{set;get;} // a terme remplacer par classe mere element pour pouvoir enregistrer obstacles

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private const int NB_TILES = 8;

    private int selectionX = -1;
    private int selectionY = -1;

    private int playerTurn = 0;

    public List<GameObject> chosensPrefabs;
    public List<Chosen> players;

    private void Start()
    {
       SpawnAllChosen();
    }

    private void Update()
    {
        /*
        UpdateSelection();
        DrawBoard();

        if(Input.GetMouseButtonDown(0) && IsTileAvailable()){
            //Debug.Log("clic ok");
            ChosenMove(selectionX, selectionY);
        }*/
    }

    private void UpdateSelection()
    {  
        if(!Camera.main)
            return;
        
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Plane"))){
            //Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }
 
    private void SpawnChosen(int indexPrefab, int x, int z)
    {
        GameObject go = Instantiate(chosensPrefabs[indexPrefab], GetTileCenter(x, z) + Vector3.up * chosensPrefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        element[x, z] = go;
        go.GetComponent<Chosen>().SetPosition(x, z);
        players.Add(go.GetComponent<Chosen>());
    }

    private void SpawnAllChosen()
    {
        players = new List<Chosen>();
        element = new GameObject[NB_TILES, NB_TILES];
        SpawnChosen(0, 0, 0);
        SpawnChosen(0, 7, 7);
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

    private Vector3 GetTileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * z) + TILE_OFFSET;
        return origin;
    }

    private bool IsTileAvailable()
    {
        return selectionX >= 0 && selectionY >= 0 &&                        //Check if mouse has moved at least one time on the board
        element[selectionX, selectionY] == null;                            //Check if there is no object on the tile we are trying to move on
    }

    private void ChosenMove(int x, int z)
    {  
        element[x, z] = element[players[playerTurn].currentX, players[playerTurn].currentZ];
        element[players[playerTurn].currentX, players[playerTurn].currentZ] = null;
        players[playerTurn].SetPosition(x, z);
        element[x, z].transform.position = GetTileCenter(x, z)  + Vector3.up * chosensPrefabs[0].GetComponent<Renderer>().bounds.size.y / 2;
    }

    public void EndTurn()
    {
        Debug.Log("clic ok");
        playerTurn = (playerTurn + 1) % players.Count;
    }
}

