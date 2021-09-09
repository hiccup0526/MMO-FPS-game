﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;
    public Animator anim;
    public NetworkAnimator networkAnimator;

    [AddComponentMenu("")]
    void FixedUpdate()
    {
        anim.SetFloat("Vertical", Mathf.Abs(Input.GetAxis("Vertical")));
        if (hasAuthority)
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime);
    }

    void Update() {
        /*if (Input.GetKeyDown(KeyCode.E)) {
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            Debug.Log("Shots fired");

            RaycastHit hit;
            Ray fromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(fromCamera, out hit)) {
                Debug.LogFormat("Raycast hit collider: {0}", hit.transform.gameObject.name);
                Debug.DrawRay(fromCamera.origin, fromCamera.direction, Color.red, 20, true);


                Transform target = hit.transform;
                SkinnedMeshRenderer smr = target.GetChild(1).GetComponent<SkinnedMeshRenderer>();

                if (smr != null) {
                    Debug.LogFormat("SKinned mesh renderer not null");
                    Material[] mats = smr.materials;
                    mats[0] = (Material)Resources.Load("Prefabs/YellowSmooth");
                    smr.materials = mats;
                }
            } else {
                Debug.Log("Nothing was shot");
            }
        }*/

        //shot debug
        if (Input.GetKeyDown(KeyCode.F)) {
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            Debug.Log("Shots fired");

            RaycastHit hit;
            Ray fromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(fromCamera.origin, fromCamera.direction, out hit, Mathf.Infinity)) {
                Debug.LogFormat("hit registered {0} of origin {1} and direction/length {2}", hit.transform.gameObject.name, fromCamera.origin, fromCamera.direction);
                Debug.DrawRay(fromCamera.origin, fromCamera.direction*1000000, Color.red, 1000, true);
            } else {
                Debug.Log("Nothing was shot");
            }
        }
    }

    public override void OnStartAuthority() {
        Camera camera = transform.Find("FirstPersonCharacter").GetComponent<Camera>();
        camera.enabled = true;
        
    }
}
