using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private List<GameObject> NetworkTypes;
    // Start is called before the first frame update

    private void Awake(){       
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;      
            serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            Instantiate(NetworkTypes[0]);
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();

            Instantiate(NetworkTypes[1]);

        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Instantiate(NetworkTypes[2]);
        });
    }
}