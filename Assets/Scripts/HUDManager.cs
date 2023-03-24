using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    //private Game game;

    private Controler controler;

    public List<GameObject> prefabs;
    private List<GameObject> spellButtons = new List<GameObject>();
    private GameObject hpBar;
    private GameObject powerGaugeBar;
    private GameObject mpBar;
    public static HUDManager Singleton = null;

    void Awake(){
         Singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initHUD(Controler controler){
        this.controler = controler;
        hpBar = Instantiate(prefabs[1], new Vector3(100, -50, 0) + transform.position,
            Quaternion.identity, transform) as GameObject;
        //hpBar.transform.SetParent(transform);
        RectTransform rtHP = hpBar.GetComponent<RectTransform>();
        rtHP.anchorMin = new Vector2(0, 1);
        rtHP.anchorMax = new Vector2(0, 1);
        rtHP.pivot = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI HPTextMesh = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        HPTextMesh.text = controler.game.players[controler.playerID].HP.Value.ToString() + " HP";

        mpBar = Instantiate(prefabs[1], new Vector3(100, -100, 0) + transform.position,
            Quaternion.identity, transform) as GameObject;
        //mpBar.transform.SetParent(transform);
        RectTransform rtMP = mpBar.GetComponent<RectTransform>();
        rtMP.anchorMin = new Vector2(0, 1);
        rtMP.anchorMax = new Vector2(0, 1);
        rtMP.pivot = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI MPTextMesh = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        MPTextMesh.text = controler.game.players[controler.playerID].MP.Value.ToString() + " MP";

        powerGaugeBar = Instantiate(prefabs[1], new Vector3(150, -150, 0) + transform.position,
            Quaternion.identity, transform) as GameObject;
        //powerGaugeBar.transform.SetParent(transform);
        RectTransform rtPowerGauge = powerGaugeBar.GetComponent<RectTransform>();
        rtPowerGauge.anchorMin = new Vector2(0, 1);
        rtPowerGauge.anchorMax = new Vector2(0, 1);
        rtPowerGauge.pivot = new Vector2(0.5f, 0.5f);
        powerGaugeBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
        TextMeshProUGUI PowerGaugeTextMesh = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        PowerGaugeTextMesh.text = controler.game.players[controler.playerID].powerGauge.Value.ToString() + " POWER";

        GameObject passBtnGO = Instantiate(prefabs[0], new Vector3(-100, 25, 0) + transform.position,
            Quaternion.identity, transform) as GameObject;
        //passBtnGO.transform.SetParent(transform);
        RectTransform rtPassBtnGO = passBtnGO.GetComponent<RectTransform>();
        rtPassBtnGO.anchorMin = new Vector2(1, 0);
        rtPassBtnGO.anchorMax = new Vector2(1, 0);
        rtPassBtnGO.pivot = new Vector2(0.5f, 0.5f);
        Button passBtn = passBtnGO.GetComponent<Button>();
        passBtn.onClick.AddListener(EndTurnPressed);
        TextMeshProUGUI passTextMesh = passBtn.GetComponentInChildren<TextMeshProUGUI>();
        passTextMesh.text = "Pass";

        int i = 0;
        GameObject spellBtnGO;
        Button spellBtn;
        TextMeshProUGUI spellTextMesh;
        foreach(KeyValuePair<int, Spell> s in controler.game.players[controler.playerID].spells){
            spellBtnGO = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0) + transform.position, Quaternion.identity, transform) as GameObject;
            //spellBtnGO.transform.SetParent(transform);
            spellBtn = spellBtnGO.GetComponent<Button>();
            spellBtn.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            spellTextMesh = spellBtn.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform rt = spellBtn.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);
            spellTextMesh.text = s.Value.getName();
            Debug.Log(controler.playerID);
            Debug.Log(s.Value.getName());
            i++;
        }
    }

    //Used for switch between players[playerTurn] spells buttons
    /*
    public void updateHUD(){
        
        foreach(GameObject go in spellButtons){
            Destroy(go);
        }
        int i = 0;
        GameObject spellBtnGO;
        Button spellBtn;
        TextMeshProUGUI spellTextMesh;
        foreach(KeyValuePair<int, Spell> s in game.players[playerID].spells){
            spellBtnGO = Instantiate(prefabs[0], new Vector3(i*200 + 100, 25, 0), Quaternion.identity) as GameObject;
            spellBtnGO.transform.SetParent(transform);
            spellBtn = spellBtnGO.GetComponent<Button>();
            RectTransform rt = spellBtn.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);
            spellBtn.onClick.AddListener(delegate{spellButtonHandler(s.Key);});
            spellTextMesh = spellBtn.GetComponentInChildren<TextMeshProUGUI>();
            spellTextMesh.text = s.Value.getName();
            i++;
        }
    }
    */

//onNetworkValueChanged methods update those stats 
/*
    public void updateHUDInfo(){//hpBar for now, see if we rename both updates
        TextMeshProUGUI HPTextMesh = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        HPTextMesh.text = game.players[playerID].HP.ToString() + " HP";
        TextMeshProUGUI powerGaugeTextMesh = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        powerGaugeTextMesh.text = game.players[playerID].powerGauge.ToString() + " POWER";
        updateHUDMP();
    }

    public void updateHUDMP(){
        TextMeshProUGUI spellTextMesh;
        spellTextMesh = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        spellTextMesh.text = game.players[playerID].MP.ToString() + " MP";
    }
*/


    public void onMPChange(int previous, int current){
        TextMeshProUGUI spellTextMesh = mpBar.GetComponentInChildren<TextMeshProUGUI>();
        spellTextMesh.text = current + " MP";
        Debug.Log("Client should update MP");
    }
    public void onHPChange(int previous, int current){
        TextMeshProUGUI HPTextMesh = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        HPTextMesh.text = current + " HP";
    }

    public void onPowerGaugeChange(int previous, int current){
        TextMeshProUGUI powerGaugeTextMesh = powerGaugeBar.GetComponentInChildren<TextMeshProUGUI>();
        powerGaugeTextMesh.text = current + " POWER";
    }

    public void EndTurnPressed()
    {
        controler.game.EndTurn();
    }

    public void spellButtonHandler(int id)
    {
        controler.game.updateSpellSelected(id);
    }

}