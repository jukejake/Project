﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using EnemySpawners;

public class Tutorial_Manager : SerializedMonoBehaviour {

    public bool On = true;
    public int Stage = -1;
    public Text EventPrompt;

    [BoxGroup(group:"Prefabs")]
    public GameObject HouseFog;
    private GameObject T_HouseFog;
    [BoxGroup(group: "Prefabs")]
    public GameObject BunkerFog;
    private GameObject T_BunkerFog;
    [BoxGroup(group: "Managers")]
    public Interface_SpawnTable SpawnManager;
    private int OldLevel = 0;
    [BoxGroup(group: "Triggers")]
    public GameObject BarnArea;
    [BoxGroup(group: "Triggers")]
    public GameObject HouseArea;
    [BoxGroup(group: "Triggers")]
    public GameObject BunkerArea;


    [BoxGroup(group: "HouseDoors"), HideLabel]
    public RotateDoor HouseDoor1;
    [BoxGroup(group: "HouseDoors"), HideLabel]
    public RotateDoor HouseDoor2;

    private Spawner_Hub SP_Hub;
    private int Counter = 0;
    private int PromptTime = 10;
    private int WaveClient = 0;


    // Use this for initialization
    void Start () {
        if (SP_Hub == null) { SP_Hub = GameObject.FindObjectOfType(typeof(Spawner_Hub)) as Spawner_Hub; }
        if (On) {
            Stage = 0;
            T_HouseFog = (GameObject)Instantiate(HouseFog);
            T_HouseFog.name = HouseFog.name;
            T_BunkerFog = (GameObject)Instantiate(BunkerFog);
            T_BunkerFog.name = BunkerFog.name;
        }

        if (Client_Manager.instance) { InvokeRepeating("SlowUpdateClient", 0.50f, 1.0f); }//Start In, Repeat Every
        else if (Server_Manager.instance) { InvokeRepeating("SlowUpdateServer", 0.50f, 1.0f); }//Start In, Repeat Every
        else { InvokeRepeating("SlowUpdateSP", 0.50f, 1.0f); }//Start In, Repeat Every
    }
	
	// Update for the Server
	void SlowUpdateServer () {
        if (!On) { return; }

        switch (Stage)
        {
            //Unlock (Set-Up)
            case -1: { 
                    Stage = 0;
                    T_HouseFog = (GameObject)Instantiate(HouseFog);
                    T_HouseFog.name = HouseFog.name;

                    T_BunkerFog = (GameObject)Instantiate(BunkerFog);
                    T_BunkerFog.name = BunkerFog.name;

                    SpawnManager.ToggleAll = true;
                    EventPrompt.text = "Look around the Barn.";
                    Counter = 0;

                    break;
            }
            //Trigger in Barn
            case 0: {
                    if (BarnArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        BarnArea.SetActive(false);
                        Destroy(BarnArea);
                        Stage = 1;
                        Server_Manager.instance.SendData("&TS1|");
                        if (SP_Hub != null) {
                            SP_Hub.SpawnAtBarn = true;
                            SP_Hub.SpawnAtHouse = false;
                            SP_Hub.SpawnAtBunker = false;
                        }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack Barn (Gnomes)
            case 1: {

                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.TimeBetweenRounds = 1.0f;
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        EventPrompt.text = "Gnomes are attacking the Barn.";
                        Counter = 0;
                        Stage = 2;
                        Server_Manager.instance.SendData("&TS2|");
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock House
            case 2: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        EventPrompt.text = "Look through the House.";
                        Counter = 0;
                        Stage = 3;
                        Server_Manager.instance.SendData("&TS3|");
                        T_HouseFog.SetActive(false);
                        Destroy(T_HouseFog);
                        //Open the House Doors
                        if (HouseDoor1 != null) { HouseDoor1.Activate = true; }
                        if (HouseDoor2 != null) { HouseDoor2.Activate = true; }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger in House
            case 3: {
                    if (HouseArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        HouseArea.SetActive(false);
                        Destroy(HouseArea);
                        Stage = 4;
                        Server_Manager.instance.SendData("&TS4|");
                        if (SP_Hub != null) {
                            SP_Hub.SpawnAtBarn = true;
                            SP_Hub.SpawnAtHouse = true;
                            SP_Hub.SpawnAtBunker = false;
                        }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack House (Gnomes)
            case 4: {
                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.TimeBetweenRounds = 1.0f;
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        EventPrompt.text = "Gnomes are attacking the House.";
                        Counter = 0;
                        Stage = 5;
                        Server_Manager.instance.SendData("&TS5|");
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock Bunker
            case 5: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        EventPrompt.text = "Look for the power switch in the Bunker.";
                        Counter = 0;
                        Stage = 6;
                        Server_Manager.instance.SendData("&TS6|");
                        T_BunkerFog.SetActive(false);
                        Destroy(T_BunkerFog);
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger in Bunker
            case 6: {
                    if (BunkerArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        BunkerArea.SetActive(false);
                        Destroy(BunkerArea);
                        Stage = 7;
                        Server_Manager.instance.SendData("&TS7|");
                        if (SP_Hub != null) {
                            SP_Hub.SpawnAtBarn = true;
                            SP_Hub.SpawnAtHouse = true;
                            SP_Hub.SpawnAtBunker = true;
                        }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack Bunker (Gnomes)
            case 7: {
                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.TimeBetweenRounds = 1.0f;
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        EventPrompt.text = "Gnomes are attacking the Bunker.";
                        Counter = 0;
                        Stage = 8;
                        Server_Manager.instance.SendData("&TS8|");
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock (Tutorial Finished)
            case 8: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        EventPrompt.text = "";
                        Stage = 9;
                        Server_Manager.instance.SendData("&TS9|");
                        SpawnManager.TimeBetweenRounds = 20.0f;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
                }
            //Trigger... none
            //Attack continuously (Gnomes)
            case 9: {
                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        //Debug.Log("Round Start!");
                        Stage = 10;
                        Server_Manager.instance.SendData("&TS10|");
                    }
                    break;
            }
            case 10: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        //Debug.Log("Round Over!");
                        Stage = 9;
                        Server_Manager.instance.SendData("&TS9|");
                    }
                    break;
            }
            default: { break; }
        }


    }
    
	// Update for the Client
	void SlowUpdateClient() {
        if (!On) { return; }

        switch (Stage)
        {
            //Unlock Game (Set-Up)
            case -1: { 
                    Stage = 0;
                    T_HouseFog = (GameObject)Instantiate(HouseFog);
                    T_HouseFog.name = HouseFog.name;

                    T_BunkerFog = (GameObject)Instantiate(BunkerFog);
                    T_BunkerFog.name = BunkerFog.name;
                    
                    EventPrompt.text = "Look around the Barn.";
                    Counter = 0;

                    break;
            }
            //Trigger in Barn
            case 0: {
                    if (BarnArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        BarnArea.SetActive(false);
                        Destroy(BarnArea);
                        Stage = 1;
                        WaveClient = 1;
                        EventPrompt.text = "Gnomes are attacking the Barn.";
                        Counter = 0;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack Barn (Gnomes)
            case 1: {
                    //Stage = 2;
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock House
            case 2: {
                    EventPrompt.text = "Look through the House.";
                    Counter = 0;
                    Stage = 3;
                    T_HouseFog.SetActive(false);
                    Destroy(T_HouseFog);
                    //Open the House Doors
                    if (HouseDoor1 != null) { HouseDoor1.Activate = true; }
                    if (HouseDoor2 != null) { HouseDoor2.Activate = true; }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger in House
            case 3: {
                    if (HouseArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        HouseArea.SetActive(false);
                        Destroy(HouseArea);
                        Stage = 4;
                        WaveClient = 2;
                        EventPrompt.text = "Gnomes are attacking the House.";
                        Counter = 0;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack House (Gnomes)
            case 4: {
                    //Stage = 5;
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock Bunker
            case 5: {
                    EventPrompt.text = "Look for the power switch in the Bunker.";
                    Counter = 0;
                    Stage = 6;
                    T_BunkerFog.SetActive(false);
                    Destroy(T_BunkerFog);
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger in Bunker
            case 6: {
                    if (BunkerArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        BunkerArea.SetActive(false);
                        Destroy(BunkerArea);
                        Stage = 7;
                        WaveClient = 3;
                        EventPrompt.text = "Gnomes are attacking the Bunker.";
                        Counter = 0;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack Bunker (Gnomes)
            case 7: {
                    //Stage = 8;
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock (Tutorial Finished)
            case 8: {
                    EventPrompt.text = "";
                    Stage = 9;
                    SpawnManager.TimeBetweenRounds = 20.0f;
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger... none
            //Attack continuously (Gnomes)
            case 9: {
                    WaveClient++;
                    Stage = 10;
                    break;
            }
            case 10: {
                    //Stage = 9;
                    break;
            }
            default: { break; }
        }


    }

    // Update for the Single player
	void SlowUpdateSP () {
        if (!On) { return; }

        switch (Stage)
        {
            //Unlock (Set-Up)
            case -1: { 
                    Stage = 0;
                    T_HouseFog = (GameObject)Instantiate(HouseFog);
                    T_HouseFog.name = HouseFog.name;

                    T_BunkerFog = (GameObject)Instantiate(BunkerFog);
                    T_BunkerFog.name = BunkerFog.name;

                    SpawnManager.ToggleAll = true;
                    EventPrompt.text = "Look around the Barn.";
                    Counter = 0;

                    break;
            }
            //Trigger in Barn
            case 0: {
                    if (BarnArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        BarnArea.SetActive(false);
                        Destroy(BarnArea);
                        Stage = 1;
                        if (SP_Hub != null) {
                            SP_Hub.SpawnAtBarn = true;
                            SP_Hub.SpawnAtHouse = false;
                            SP_Hub.SpawnAtBunker = false;
                        }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack Barn (Gnomes)
            case 1: {

                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.TimeBetweenRounds = 1.0f;
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        EventPrompt.text = "Gnomes are attacking the Barn.";
                        Counter = 0;
                        Stage = 2;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock House
            case 2: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        EventPrompt.text = "Look through the House.";
                        Counter = 0;
                        Stage = 3;
                        T_HouseFog.SetActive(false);
                        Destroy(T_HouseFog);
                        //Open the House Doors
                        if (HouseDoor1 != null) { HouseDoor1.Activate = true; }
                        if (HouseDoor2 != null) { HouseDoor2.Activate = true; }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger in House
            case 3: {
                    if (HouseArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        HouseArea.SetActive(false);
                        Destroy(HouseArea);
                        Stage = 4;
                        if (SP_Hub != null) {
                            SP_Hub.SpawnAtBarn = true;
                            SP_Hub.SpawnAtHouse = true;
                            SP_Hub.SpawnAtBunker = false;
                        }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack House (Gnomes)
            case 4: {
                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.TimeBetweenRounds = 1.0f;
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        EventPrompt.text = "Gnomes are attacking the House.";
                        Counter = 0;
                        Stage = 5;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock Bunker
            case 5: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        EventPrompt.text = "Look for the power switch in the Bunker.";
                        Counter = 0;
                        Stage = 6;
                        T_BunkerFog.SetActive(false);
                        Destroy(T_BunkerFog);
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Trigger in Bunker
            case 6: {
                    if (BunkerArea.GetComponent<DidPlayerCollide>().IsTriggered) {
                        BunkerArea.SetActive(false);
                        Destroy(BunkerArea);
                        Stage = 7;
                        if (SP_Hub != null) {
                            SP_Hub.SpawnAtBarn = true;
                            SP_Hub.SpawnAtHouse = true;
                            SP_Hub.SpawnAtBunker = true;
                        }
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Attack Bunker (Gnomes)
            case 7: {
                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.TimeBetweenRounds = 1.0f;
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        EventPrompt.text = "Gnomes are attacking the Bunker.";
                        Counter = 0;
                        Stage = 8;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
            }
            //Unlock (Tutorial Finished)
            case 8: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        EventPrompt.text = "";
                        Stage = 9;
                        SpawnManager.TimeBetweenRounds = 20.0f;
                    }
                    if (Counter < PromptTime) { Counter += 1; }
                    else if (Counter == PromptTime) { Counter += 1; EventPrompt.text = ""; }

                    break;
                }
            //Trigger... none
            //Attack continuously (Gnomes)
            case 9: {
                    if (OldLevel == SpawnManager.OldLevel && SpawnManager.OldLevel == SpawnManager.CurrentLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = false;
                        //Activate all spawners
                        SpawnManager.ActivateAllSpawnersInCurrentRound();
                        //Debug.Log("Round Start!");
                        Stage = 10;
                    }
                    break;
            }
            case 10: {
                    if (OldLevel+1 == SpawnManager.CurrentLevel && OldLevel+1 == SpawnManager.OldLevel && SpawnManager.EverythingDead) {
                        OldLevel = SpawnManager.CurrentLevel;
                        SpawnManager.ToggleAll = true;
                        //Debug.Log("Round Over!");
                        Stage = 9;
                    }
                    break;
            }
            default: { break; }
        }


    }
}
