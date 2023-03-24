using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using System.Threading;

public class Game : NetworkBehaviour
{
    //to be in client controler
    private int selectionX = -1;
    private int selectionZ = -1;
    public Spell spellSelected = null;
    public GameObject Canvas;
    public Square lastSquareSelected = null;

    //To be in server controler
    public List<GameObject> prefabs;

    //Dans les 2
    //public int playerTurn = 0;
    public NetworkVariable<int> IDplayerTurn = new NetworkVariable<int>(-1);
    public List<Chosen> players = new List<Chosen>();

    private Node root;
    private List<Node> leaves = new List<Node>();
    public Board board; 
    public Dictionary<int, Spell> spells = new Dictionary<int, Spell>();
    
    public static Game Instance = null;

    private void Awake()
    {
        //Debug.Log("Awake of Game");
        Game.Instance=this;
    }

/*
    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        base.OnSynchronize(ref serializer);
    }
*/

    public void init()
    {
        GameObject boardGO = Instantiate(prefabs[0], transform.position, Quaternion.identity) as GameObject;
        boardGO.GetComponent<NetworkObject>().Spawn();
        boardGO.transform.SetParent(transform);
        board = boardGO.GetComponent<Board>();
        board.init(1.0f, 0.5f, 16);

        int id = 0;
        //create all spells in the game
        Spell electricBlade = new Spell("Electric blade", 20, new CastingCondition((1,1), true, true));
        electricBlade.effects.Add(new PhysicalDamage(board, 5));
        spells.Add(id++, electricBlade);

        Spell fireBall = new Spell("Fire ball", 30, new CastingCondition((2,6), false, true));
        fireBall.effects.Add(new PhysicalDamage(board, 20));
        spells.Add(id++, fireBall);

        Spell frozenGasp = new Spell("Frozen Gasp", 30, new CastingCondition((1,4), false, true));
        frozenGasp.effects.Add(new PhysicalDamage(board, 20));
        frozenGasp.effects.Add(new MPbuff(2, false, -1));
        spells.Add(id++, frozenGasp);

        Spell shuriken = new Spell("Shuriken", 30, new CastingCondition((1,8), true, true));
        shuriken.effects.Add(new PhysicalDamage(board, 10));
        spells.Add(id++, shuriken);

        Spell celerity = new Spell("Celerity", 10, new CastingCondition((0,0), true, true));
        celerity.effects.Add(new MPbuff(1, true, 3));
        spells.Add(id++, celerity);

        Spell jadePalm = new Spell("Jade palm", 40, new CastingCondition((1,3), true, true));
        jadePalm.effects.Add(new PhysicalDamage(board, 5));
        jadePalm.effects.Add(new MoveTarget(board, 2));
        spells.Add(id++, jadePalm);

        
        players = board.SpawnAllChosen();

        //Affect spell to chosen
        players[0].addSpell(new KeyValuePair<int, Spell> (0, spells[0]));
        players[0].addSpell(new KeyValuePair<int, Spell> (1, spells[1]));
        players[0].addSpell(new KeyValuePair<int, Spell> (2, spells[2]));

        players[1].addSpell(new KeyValuePair<int, Spell> (3, spells[3]));
        players[1].addSpell(new KeyValuePair<int, Spell> (4, spells[4]));
        players[1].addSpell(new KeyValuePair<int, Spell> (5, spells[5]));

        IDplayerTurn.Value = 0;

        //boardGO.GetComponent<NetworkObject>().Spawn();
    }

    public void DrawGrid(){ //TODO: Do better for redraw board model
        Vector3 widthLine = Vector3.right * board.getSquareSize() * board.getNbSquares();
        Vector3 heigthLine = Vector3.forward * board.getSquareSize() * board.getNbSquares();
        
        for(int i=0; i<=board.getNbSquares(); i++){
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start + transform.position, start + widthLine + transform.position);
            for(int j=0; j<=board.getNbSquares(); j++){
                start = Vector3.right * i;
                Debug.DrawLine(start + transform.position, start + heigthLine + transform.position);
            }
        }

        // Draw selection
        if(selectionX >= 0 && selectionZ >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionZ + Vector3.right * selectionX + transform.position,
            Vector3.forward * (selectionZ + 1) + Vector3.right * (selectionX + 1) + transform.position);
        }
    }

    public void ChosenMove(int x, int z)
    {
        int distance = Utils.range(players[IDplayerTurn.Value].x.Value, players[IDplayerTurn.Value].z.Value, x, z);
        //Target Square too far from the chosen with his MP
        if (distance > players[IDplayerTurn.Value].MP.Value)
        {
            Debug.Log("You can't move that far.");
            return;
        }

        //PATHFINDING
        // We try to find the quickest path from current x/z to targeted x/z
        root = new Node(players[IDplayerTurn.Value].x.Value, players[IDplayerTurn.Value].z.Value);
        Node leaf = board.PathFinding(root, x, z, distance, leaves);  //We try first with number of MP = Range then we will increase this number if we didn't find a way

        int tmpMP = players[IDplayerTurn.Value].MP.Value;
        tmpMP -= distance;

        int leavesIndex = 0;
        while (leaf == null && tmpMP > 0) // if we aren't on target and we still have MP then we increase MP
        {
            //Debug.Log("mp : " + tmpMP);
            int tmpIndex = leaves.Count;
            for (int i = leavesIndex; i < tmpIndex; i++)
            {
                leaf = board.PathFinding(leaves[i], x, z, 1, leaves);
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

        players[IDplayerTurn.Value].MP.Value = tmpMP; //MP left to the chosen after moving

        //Moving entities
        board.setEntityAtPos(x, z, players[IDplayerTurn.Value]);
        board.setEntityAtPos(players[IDplayerTurn.Value].x.Value, players[IDplayerTurn.Value].z.Value, null);
        players[IDplayerTurn.Value].ModifyExistingPosition(x, z);
    }

    public void UpdateSelection()
    {
        if(!Camera.main)
            return;
        
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("Plane"))){
            //Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionZ = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionZ = -1;
        }
    }

    public void EndTurn()
    {
        if(NetworkManager.Singleton.IsServer){
            players[IDplayerTurn.Value].passTurn();
            spellSelected = null;
            board.resetReachableSquares();
            IDplayerTurn.Value = (IDplayerTurn.Value + 1) % players.Count;
            players[IDplayerTurn.Value].beginTurn();
            //HUDManager.Singleton.updateHUD();
        } else {
            Debug.Log("Make pass turn request from client");
            PassTurnRequestServerRpc();
        }
        //hm.updateHUD();
        //hm.updateHUDInfo();
    }

    public void updateSpellSelected(int id)
    {
        spellSelected = spells[id];
        board.updateReachableSquare(spellSelected, (players[IDplayerTurn.Value].x.Value, players[IDplayerTurn.Value].z.Value));
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveRequestServerRpc(int x, int z)
    {
        Debug.Log("Move request received from client to this server");
        ChosenMove(x, z);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpellRequestServerRpc(int idSpell, int x, int z)
    {
        Debug.Log("Spell request received from client to this server");
        Debug.Log(spells[idSpell].getName());
        if(board.reachableSquares[x, z]==2){
            players[IDplayerTurn.Value].useSpell(board.squares[x, z], spells[idSpell]);
        } else{
            Debug.Log("Target out of reach!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PassTurnRequestServerRpc()
    {
        Debug.Log("Pass turn by server");
        EndTurn();
        PassTurnReplyClientRpc();

    }

    [ClientRpc]
    public void PassTurnReplyClientRpc()
    {
        Debug.Log("Reply from server to update hud on this client");
        //HUDManager.Singleton.updateHUD();
    }
}