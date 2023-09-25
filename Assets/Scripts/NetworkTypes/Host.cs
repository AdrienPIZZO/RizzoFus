using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Host : Controler
{

    private int selectionX = -1;
    private int selectionZ = -1;
    public bool isASpellSelected = false;
    public GameObject gamePrefab;
    public GameObject gameHUDPrefab;
    private HUDManager hm;


    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        GameObject gameGO = Instantiate(gamePrefab) as GameObject;
        gameGO.GetComponent<NetworkObject>().Spawn();
        game = gameGO.GetComponent<Game>();
        game.init();

        hm = Instantiate(gameHUDPrefab, GameObject.FindGameObjectWithTag("Canvas").transform.position, Quaternion.identity,
        GameObject.FindGameObjectWithTag("Canvas").transform).GetComponent<HUDManager>();
        Debug.Log("Client ID: " + NetworkManager.ServerClientId);
        game.clients.Add(NetworkManager.ServerClientId);
        game.playerID = 0;
        hm.initHUD(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        game.DrawGrid();
        game.board.DrawChosens();
        if(Input.GetMouseButtonDown(0) && (selectionX != -1 || selectionZ != -1)){
            game.lastSquareSelected = game.board.squares[selectionX, selectionZ];
            if (game.spellSelected!=null)
            {
                Debug.Log("Spell selected: " + game.spellSelected.getName());
                if(game.board.reachableSquares[selectionX, selectionZ]==2){
                    game.chosens[game.IDplayerTurn.Value].useSpell(game.lastSquareSelected, game.spellSelected);
                    //game.hm.updateHUDInfo();
                } else{
                    Debug.Log("Target out of reach!");
                }
            } else if (game.board.IsSquareAvailable(selectionX, selectionZ)){
                if (NetworkManager.ServerClientId == game.clients[game.IDplayerTurn.Value])
                    game.ChosenMove(selectionX, selectionZ);
            }
            game.board.resetReachableSquares();
            game.spellSelected=null; //Unselect spell on click
        }
    }
    private void UpdateSelection()
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
    public void DrawGrid(){ //TODO: Do better for redraw board model
        Vector3 widthLine = Vector3.right * game.board.getSquareSize() * game.board.getNbSquares();
        Vector3 heigthLine = Vector3.forward * game.board.getSquareSize() * game.board.getNbSquares();
        
        for(int i=0; i<=game.board.getNbSquares(); i++){
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start + game.transform.position, start + widthLine + game.transform.position);
            for(int j=0; j<=game.board.getNbSquares(); j++){
                start = Vector3.right * i;
                Debug.DrawLine(start + game.transform.position, start + heigthLine + game.transform.position);
            }
        }

        // Draw selection
        if(selectionX >= 0 && selectionZ >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionZ + Vector3.right * selectionX + game.transform.position,
            Vector3.forward * (selectionZ + 1) + Vector3.right * (selectionX + 1) + game.transform.position);
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("Client ID: " + clientId);
        game.clients.Add(clientId);
    }
}

