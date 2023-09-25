using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using Unity.Networking.Transport;

public class Server : Controler
{
    public GameObject gamePrefab;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        GameObject gameGO = Instantiate(gamePrefab) as GameObject;
        gameGO.GetComponent<NetworkObject>().Spawn();
        game = gameGO.GetComponent<Game>();
        game.init();
    }

    // Update is called once per frame
    void Update()
    {
        game.DrawGrid();
        game.board.DrawChosens();
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("Client ID: " + clientId);
        game.clients.Add(clientId);
    }
}
