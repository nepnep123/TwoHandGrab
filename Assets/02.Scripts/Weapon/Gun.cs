using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Gun : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;

    public GameObject bullet;
    public Transform barrelPivot;
    public float shootingSpeed = 10f;
    public GameObject muzzleFlash;

    private Animator animator;
    private Interactable interactable;

    private void Start()
    {
        animator = GetComponent<Animator>();
        muzzleFlash.SetActive(false);
        interactable = GetComponent<Interactable>();
    }

    private void Update()
    {
        if(interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            if (fireAction[source].stateDown)
            {
                Fire();
            }
        }
    }

    void Fire()
    {
        Rigidbody bulletRigid = Instantiate(bullet, barrelPivot.position, barrelPivot.rotation).GetComponent<Rigidbody>();
        bulletRigid.velocity = -barrelPivot.forward * shootingSpeed;
        muzzleFlash.SetActive(true);
    }
}
