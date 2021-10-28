﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;
    public Animator anim;
    public NetworkAnimator networkAnimator;
    
    public bool zoomToggle = false;
    public GameObject crosshair;
    public GameObject crosshair2;
    public GameObject deadText;

    public GameObject leaderboard;
    public bool leaderOn = false;

    public bool isActive;

    [AddComponentMenu("")]

    public override void OnStartAuthority() {

        //Configure crosshair
        crosshair = GameObject.Find("ScopedCrosshairImage");
        crosshair2 = GameObject.Find("DefaultCrosshairImage");
        crosshair.SetActive(false);

        //Configure leaderboard
        leaderboard = GameObject.Find("LeaderBoard");
        leaderboard.SetActive(false);

        //Configure dead text
        deadText = GameObject.Find("Died");
        deadText.SetActive(false);

        //Enable camera & audio listener
        Transform fpc = transform.Find("FirstPersonCharacter");
        fpc.GetComponent<Camera>().enabled = true;
        fpc.GetComponent<AudioListener>().enabled = true;
        isActive = true;
    }

    void Update() {
        if (!hasAuthority) return;

        ScopeToggle();              // Bound to Q
        Shoot();                    // Bound to F
        ViewBoard();                // Bound to Tab
        DieOutOfBounds();
    }

    public void ScopeToggle() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (zoomToggle) {
                Camera.main.fieldOfView += 35;
                Camera.main.orthographicSize += 0.5f;
                zoomToggle = false;
                crosshair.SetActive(false);
                crosshair2.SetActive(true);
            }
            else {
                Camera.main.fieldOfView -= 35;
                Camera.main.orthographicSize -= 0.5f;
                zoomToggle = true;
                crosshair2.SetActive(false);
                crosshair.SetActive(true);
            }
        }
    }

    public void Shoot() {
        if (Input.GetKeyDown(KeyCode.F)) {

            //Start animation
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");

            // Draw Raycast
            RaycastHit hit;
            Ray fromCamera =  Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(fromCamera, out hit, Mathf.Infinity)) {
                if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)"
                    || hit.transform.gameObject.name == "FPSNetworkBotController(Clone)") {
                    Debug.Log("Fired at object named " + hit.transform.gameObject.name);
                    CmdDealDamage(hit.transform);
                }
            }
        }
    }

    public void ViewBoard(){
         if (Input.GetKey(KeyCode.Tab)) {
            leaderboard.SetActive(true);
         } else {
            leaderboard.SetActive(false);
         }
    }

    public void DieOutOfBounds() {
        if (this.transform.position.y < -10) {
            Destroy(this.gameObject);
            deadText.SetActive(true);
        }
    }

    [Command]
    public void CmdDealDamage(Transform target) {
        Debug.Log("Asked server to deal damage");
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        if (target.gameObject.name == "FPSNetworkPlayerController(Clone)") {
            Debug.Log("player target");
            Damageable playerDamage = target.gameObject.GetComponent<Damageable>();
            playerDamage.dealDamage(20);
        } else {
            BotDamageable playerDamage = target.gameObject.GetComponent<BotDamageable>();
            playerDamage.dealDamage(20);

            BotHealth botHealth = target.gameObject.GetComponent<BotHealth>();
            Debug.Log("bot has " + botHealth.getHealth() + " health left");
        }
        
    }

}