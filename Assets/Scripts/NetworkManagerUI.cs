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
    [SerializeField] private GameObject gamePrefab;
    // Start is called before the first frame update

    private void Awake(){
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            GameObject gameGO = Instantiate(gamePrefab) as GameObject;
            gameGO.GetComponent<Game>().init(this.GetComponentInParent<HUDManager>());
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            GameObject gameGO = Instantiate(gamePrefab) as GameObject;
            gameGO.GetComponent<Game>().init(this.GetComponentInParent<HUDManager>());
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}