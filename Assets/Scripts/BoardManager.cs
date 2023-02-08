using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardManager : MonoBehaviour
{
    public Entity[,] element{set;get;} // a terme remplacer par classe mere element pour pouvoir enregistrer obstacles

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private const int NB_TILES = 8;

    private int selectionX = -1;
    private int selectionY = -1;

    private int playerTurn = 0;

    private bool attackSelected = false;

    public List<GameObject> prefabs;
    public List<Chosen> players;

    private Node root;
    private List<Node> leaves = new List<Node>();

    private void Start()
    {
        SpawnAllChosen();
        SpawnAllObstacles();
    }

    private int range(int x, int z, int x2, int z2)
    {
        return Math.Abs(x2 - x) + Math.Abs(z2 - z);
    }

    private void Update()
    {
        UpdateSelection();
        DrawBoard();

        if(Input.GetMouseButtonDown(0) && (selectionX != -1 || selectionY != -1))
        {
            Entity e = element[selectionX, selectionY];
             Debug.Log( attackSelected);
            if (attackSelected && e != null)
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAA");
                players[playerTurn].attack(e);
            } else if (IsTileAvailable(selectionX, selectionY)){
                //Debug.Log("clic ok");
                ChosenMove(selectionX, selectionY);
            } 
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
        GameObject go = Instantiate(prefabs[indexPrefab], GetTileCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        element[x, z] = go.GetComponent<Chosen>();
        go.GetComponent<Chosen>().SetPosition(x, z);
        players.Add(go.GetComponent<Chosen>());
    }

    private void SpawnObstacle(int indexPrefab, int x, int z)
    {
        GameObject go = Instantiate(prefabs[indexPrefab], GetTileCenter(x, z) + Vector3.up * prefabs[indexPrefab].GetComponent<Renderer>().bounds.size.y / 2, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        element[x, z] = go.GetComponent<Obstacles>();
        go.GetComponent<Obstacles>().SetPosition(x, z);
    }

    private void SpawnAllChosen()
    {
        players = new List<Chosen>();
        element = new Entity[NB_TILES, NB_TILES];
        SpawnChosen(0, 0, 0);
        SpawnChosen(0, 7, 7);
    }

    private void SpawnAllObstacles()
    {
        SpawnObstacle(1, 2, 2);
        SpawnObstacle(1, 3, 0);
        SpawnObstacle(1, 2, 0);
        SpawnObstacle(1, 1, 0);
        SpawnObstacle(1, 2, 1);
        SpawnObstacle(1, 5, 5);
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

    private bool IsTileAvailable(int x, int z)
    {
        //Debug.Log(element[x,z]);
        return x >= 0 && z >= 0 && x < NB_TILES && z < NB_TILES && element[x, z] == null;   //Check if there is no object on the tile we are trying to move on
    }

    private Entity TileContent(int x, int z)
    {
        //Debug.Log(element[x,z]);
        if ( x >= 0 && z >= 0 && x < NB_TILES && z < NB_TILES )
        {
             return element[x, z];
        }
           
        return null;
    }

    private void ChosenMove(int x, int z)
    {
        int distance = range(players[playerTurn].currentX, players[playerTurn].currentZ, x, z);

        //Target tile too far from the chosen with his MP
        if ( distance > players[playerTurn].MP)
        {
            Debug.Log("You can't move that far.");
            return;
        }

        //PATHFINDING
        // We try to find the quickest path from current x/z to targeted x/z
        root = new Node(players[playerTurn].currentX, players[playerTurn].currentZ);
        Node leaf = PathFinding(root, x, z, distance);  //We try first with number of MP = Range then we will increase this number if we didn't find a way

        int tmpMP = players[playerTurn].MP;
        tmpMP -= distance;

        int leavesIndex = 0;
        while (leaf == null && tmpMP > 0) // if we aren't on target and we still have MP then we increase MP
        {
            //Debug.Log("mp : " + tmpMP);
            int tmpIndex = leaves.Count;
            for (int i = leavesIndex; i < tmpIndex; i++)
            {
                leaf = PathFinding(leaves[i], x, z, 1);
                if (leaf != null)
                {
                    Debug.Log("mp : " + tmpMP);
                    break;
                }
            }
            tmpMP--;
            leavesIndex = tmpIndex;
        }

        if (leaf == null) //we didn't find a way with all our MP
        {
            //Debug.Log("We didn't find a way to go there");
            leaves = new List<Node>(); //reset leaves AND MAYBE free all of the objects at this point
            return;
        }

        //Debug print path
        while (leaf != null)
        {
            //Debug.Log(leaf.x + " : " + leaf.z);
            leaf = leaf.parent;
        }

        leaves = new List<Node>(); //reset leaves AND MAYBE free all of the objects at this point
        //Debug.Log("MP : " + tmpMP);
        //END PATHFINDING

        players[playerTurn].MP = tmpMP; //MP left to the chosen after moving

        //Moving entities
        element[x, z] = element[players[playerTurn].currentX, players[playerTurn].currentZ];
        element[players[playerTurn].currentX, players[playerTurn].currentZ] = null;
        players[playerTurn].SetPosition(x, z);
        element[x, z].transform.position = GetTileCenter(x, z)  + Vector3.up * prefabs[0].GetComponent<Renderer>().bounds.size.y / 2;
    }

    public void EndTurn()
    {
        players[playerTurn].MPReset();
        playerTurn = (playerTurn + 1) % players.Count;
    }

    private Node PathFinding(Node current, int x, int z, int nbMP)
    {
        //Debug.Log(current.x + " : " + current.z);
        if (current.x == x && current.z == z)
        {
            //Debug.Log("found.");
            return current;
        }
        if(nbMP <= 0)
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

            if (current.x + 1 < NB_TILES && IsTileAvailable(current.x+1, current.z))
            {
                node = new Node(current.x + 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res1 = PathFinding(node, x, z, nbMP - 1);
                if (res1 != null) return res1;
            }
            if (current.z + 1 < NB_TILES && IsTileAvailable(current.x, current.z+1))
            {
                node = new Node(current.x, current.z + 1);
                node.parent = current;
                current.childrens.Add(node);
                res2 = PathFinding(node, x, z, nbMP - 1);
                if (res2 != null) return res2;
            }
            if (current.x - 1 >= 0 && IsTileAvailable(current.x-1, current.z))
            {
                node = new Node(current.x - 1, current.z);
                node.parent = current;
                current.childrens.Add(node);
                res3 = PathFinding(node, x, z, nbMP - 1);
                if (res3 != null) return res3;
            } 
            if (current.z - 1 >= 0 && IsTileAvailable(current.x, current.z-1))
            {
                node = new Node(current.x, current.z - 1);
                node.parent = current;
                current.childrens.Add(node);
                res4 = PathFinding(node, x, z, nbMP - 1);
                if (res4 != null) return res4;
            }
            return null;
        }
    }

    public void SetAttackSelected(bool b)
    {
        attackSelected = b;
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