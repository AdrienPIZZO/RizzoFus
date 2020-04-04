using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private Node root;
    private List<Node> leaves = new List<Node>();

    private void Start()
    {
        int MP = 6;
        SpawnAllChosen();
        root = new Node(0, 0);
        // We try to find the quickest path from current x/z to targeted x/z (0,0 to x,z)
        Node leaf = PathFinding(root, 2, 2, range(root.x, root.z, 2, 2));  //We try first with number of MP = Range then we increase this number
        MP -= range(root.x, root.z, 2, 2); 
        int leavesIndex = 0;
        while (leaf == null && MP > 0) // if we aren't on target and we still have MP then we increase MP
        {
            int tmpIndex = leaves.Count;
            for (int i = leavesIndex; i < leaves.Count; i++)
            {
                leaf = PathFinding(leaves[i], 2, 2, 1);
                if (leaf != null) 
                    break;
            }
            MP--;
            leavesIndex = tmpIndex;
        }
        while(leaf != null)
        {
            Debug.Log(leaf.x + " : " + leaf.z);
            leaf = leaf.parent;
        }

        //PENSER A RESET leaves;
    }

    private int range(int x, int z, int x2, int z2)
    {
        return Math.Abs(x2 - x) + Math.Abs(z2 - z);
    }

    private void Update()
    {
        
        UpdateSelection();
        DrawBoard();

        if(Input.GetMouseButtonDown(0) && IsTileAvailable()){
            //Debug.Log("clic ok");
            ChosenMove(selectionX, selectionY);
        }
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
        playerTurn = (playerTurn + 1) % players.Count;
    }

    private Node PathFinding(Node current, int x, int z, int nbMP)
    {
        //Debug.Log(current.x + " : " + current.z);
        if (current.x == x && current.z == z)
        {
            Debug.Log("found.");
            return current;
        }
        if(nbMP == 0)
        {
            leaves.Add(current);
            return null;
        }
        else
        {
            Node node;
            Node res1 = null;
            Node res2 = null;
            Node res3 = null;
            Node res4 = null;
            current.childrens = new List<Node>();

            if (current.x + 1 < NB_TILES)
            {
                node = new Node(current.x + 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res1 = PathFinding(node, x, z, nbMP - 1);
            }
            if (current.z + 1 < NB_TILES)
            {
                node = new Node(current.x, current.z + 1);
                node.parent = current;
                current.childrens.Add(node);
                res2 = PathFinding(node, x, z, nbMP - 1);
            }
            if (current.x - 1 > 0)
            {
                node = new Node(current.x - 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res3 = PathFinding(node, x, z, nbMP - 1);
            } 
            if (current.z - 1 > 0)
            {
                node = new Node(current.x, current.z - 1);
                node.parent = current;
                current.childrens.Add(node);
                res4 = PathFinding(node, x, z, nbMP - 1);
            }
            if (res1 != null) return res1;
            if (res2 != null) return res2;
            if (res3 != null) return res3;
            if (res4 != null) return res4;
            return null;
        }
    }
}

public class Node
{
    public int x;
    public int z;
    public Node parent = null;
    public List<Node> childrens = null;

    public Node(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}