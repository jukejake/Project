﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {


    public static MenuManager instance;
    void Awake() { instance = this; }
    public GameObject Menu;
    private GameObject player;
    private GameObject weapon;

    [Space]
    public GameObject Inventory_Slot;
    public GameObject Weapon_Slot;
    [Space]
    //Combined
    public GameObject DTF_Slot; //drop to floor
    public GameObject Armour_Equip_Slots; //contains 3 gameobjects

    private bool WeaponEquiped = false;
    private float timer = 0.0f;
    public float timerValue = 0.20f;
    [System.NonSerialized]
    public int CurrentSlot = -1;

    // Use this for initialization
    private void Start() {
        Invoke("DelayedStart", 0.1f);
    }

    private void DelayedStart() {
        EnableGraphicRaycaster(false);
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update() {
        if (Weapon_Slot.transform.childCount != 0 && !WeaponEquiped) { EquipWeapon(); }
        if (Weapon_Slot.transform.childCount == 0 && WeaponEquiped) { UnEquipWeapon(); }

        if (timer > 0.0f) {
            timer -= Time.deltaTime;
            Mathf.Clamp(timer, 0.0f, 30.0f);
        }
        else if (timer <= 0.0f) { ScrollThroughInventory(); }

    }

    public void EquipWeapon() {
        if (Weapon_Slot.transform.GetChild(0).name == "placeholder") { return; }
        WeaponEquiped = true;
        weapon = (GameObject)Instantiate(Weapon_Slot.transform.GetChild(0).GetComponent<Drag_Inventory>().ItemOnDrop.transform.GetChild(0).gameObject);
        weapon.name = "weapon";
        if (weapon.GetComponent<Gun_Behaviour>()) { weapon.GetComponent<Gun_Behaviour>().enabled = true; }
        if (weapon.GetComponent<Eyes_Follow_Cursor>()) { weapon.GetComponent<Eyes_Follow_Cursor>().enabled = true; }
        weapon.transform.SetParent(player.transform);
        weapon.transform.localPosition = new Vector3(0.5f, 0.0f, 0.6f);
        weapon.transform.rotation = player.transform.rotation;
        weapon.transform.localScale = Weapon_Slot.transform.GetChild(0).GetComponent<Drag_Inventory>().ItemOnDrop.transform.GetChild(0).gameObject.transform.localScale;
    }
    public void UnEquipWeapon() {
        //if (Weapon_Slot.transform.GetChild(0).name == "placeholder") { return; }
        WeaponEquiped = false;
        Destroy(weapon.gameObject);
    }

    public void ScrollThroughInventory() {

        int InvSpace = Inventory_Slot.GetComponent<Drop_Inventory>().NumberOfSlotsFilled;
        if (InvSpace == 0) { return; }
        // InvSpace/2;

        if (Input.GetAxis("D-pad X") >= 00.2f || Input.GetAxis("Mouse ScrollWheel") >= 00.1f) { //right
            if (CurrentSlot == -1) { CurrentSlot = InvSpace/2; }
            if (CurrentSlot >= 0 && CurrentSlot < InvSpace - 1) { CurrentSlot += 1; }
            else if (CurrentSlot == InvSpace-1) { CurrentSlot = 0; }
            timer = timerValue;
            for (int i = 0; i < InvSpace; i++) { Inventory_Slot.transform.GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f); }
            Inventory_Slot.transform.GetChild(CurrentSlot).GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    
        if (Input.GetAxis("D-pad X") <= -0.2f || Input.GetAxis("Mouse ScrollWheel") <= -0.1f) { //left
            if (CurrentSlot == -1) { CurrentSlot = InvSpace/2; }
            if (CurrentSlot > 0 && CurrentSlot <= InvSpace) { CurrentSlot -= 1;}
            else if (CurrentSlot == 0) { CurrentSlot = InvSpace-1; }
            timer = timerValue;
            for (int i = 0; i < InvSpace; i++) { Inventory_Slot.transform.GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f); }
            Inventory_Slot.transform.GetChild(CurrentSlot).GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        if (Input.GetAxis("D-pad Y") >= 00.2f) {
            if (CurrentSlot == -1) { return; }
            Inventory_Slot.transform.GetChild(CurrentSlot).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            CurrentSlot = -1;
        }

        if (Input.GetAxis("D-pad Y") <= -0.2f) {
            if (CurrentSlot == -1) { return; }
            Inventory_Slot.transform.GetChild(CurrentSlot).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            CurrentSlot = -1;
        }


        if (PlayerManager.instance.MenuOpen) { return; }


        if (Input.GetButton("Fire2") || Input.GetButton("CB")) {
            if (CurrentSlot == -1) { return; }
            if (Inventory_Slot.transform.GetChild(CurrentSlot).GetComponent<Drag_Inventory>().typeOfItem == Drag_Inventory.Slot.Weapon) {
                if (Weapon_Slot.transform.childCount != 0) {
                    timer = timerValue;
                    UnEquipWeapon();
                    Weapon_Slot.transform.GetChild(0).SetParent(Inventory_Slot.transform);
                    Inventory_Slot.transform.GetChild(CurrentSlot).SetParent(Weapon_Slot.transform);
                    Weapon_Slot.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    Inventory_Slot.transform.GetChild(Inventory_Slot.transform.childCount-1).transform.SetSiblingIndex(CurrentSlot);
                    Inventory_Slot.transform.GetChild(CurrentSlot).GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);

                    EquipWeapon();
                    //Debug.Log("Swap");
                }
                else {
                    timer = timerValue;
                    Inventory_Slot.transform.GetChild(CurrentSlot).SetParent(Weapon_Slot.transform);
                    Weapon_Slot.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    EquipWeapon();
                    //Debug.Log("Push");
                    CurrentSlot -= 1;
                }
            }
        }

    }

    public void EnableGraphicRaycaster(bool enable) {
        Menu.GetComponent<GraphicRaycaster>().enabled = enable;
    }
}
