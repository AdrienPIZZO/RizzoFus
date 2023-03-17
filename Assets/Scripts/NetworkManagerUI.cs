using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private GameObject gameGO;
    // Start is called before the first frame update

    private void Awake(){
        Game game = gameGO.GetComponent<Game>();
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            while(!game.IsSpawned);
            game.init();
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Debug.Log("spawn: " + game.IsSpawned);
            while(!game.IsSpawned);
            game.init();

        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
