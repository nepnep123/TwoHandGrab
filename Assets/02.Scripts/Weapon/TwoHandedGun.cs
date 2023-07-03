using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TwoHandedGun : MonoBehaviour
{
    public bool automatic = true;
    
    public float shootDelay = 0.15f;
    public float shootSpeed = 10f;
    public GameObject bullet;
    public Transform barrelPivot;
    public GameObject muzzleFlash;

    public float recoilAmount = 5f; //체크 필요 

    public float rayCastRange = 800f;
    public float rayCastImpactForce = 1000f;
    public Transform rayCastSource;

    //public AudioClip gunShotAudioClip;

    public SteamVR_Action_Boolean fireAction;

    private Interactable interactable;
    public Interactable guardInteractable; 
    
    private AudioSource audioSource;
    private new Rigidbody rigidbody = null;

    private Coroutine firingRoutine = null;
    private bool isFireingRoutineRunning = false;
    private WaitForSeconds waitTime = null;

    private void Awake()
    {
        waitTime = new WaitForSeconds(shootDelay);
    }

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        muzzleFlash.SetActive(false);

        SetupAudio();
    }

    private void LateUpdate()
    {
        //서브 손잡이를 잡고있는 상태에서 메인 손잡이를 놨을때 
        if (!interactable.attachedToHand && guardInteractable.attachedToHand)
        {
            guardInteractable.DetachFromHand();
            guardInteractable.attachedToHand.HoverUnlock(guardInteractable);
            guardInteractable.attachedToHand.DetachObject(guardInteractable.gameObject);
            // -> HandHoverUpdate
        }

        //check if grabbed
        if (interactable.attachedToHand != null)
        {
            // Get the hand source
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            // Check button is down
            if (!automatic && fireAction[source].stateDown)
            {
                PullTrigger();
                ReleaseTrigger();
            }
            else if (automatic && fireAction[source].stateDown)
            {
                PullTrigger();
            }
            else if (fireAction[source].stateUp)
            {
                ReleaseTrigger();
            }
        }
    }                    

    private void HandHoverUpdate(Hand hand)
    {
        //서브 손잡이를 잡고있는 상태에서 메인 손잡이를 놨을때 
        if (!interactable.attachedToHand)
        {
            DebugUI.instance.debug_txt.text = "Main Hand Release";
            hand.DetachObject(guardInteractable.gameObject);
            hand.HoverUnlock(guardInteractable);
        }
    }


    private void SetupAudio()
    {
        //audioSource.clip = gunShotAudioClip;
    }

    private void PullTrigger()
    {
        if (!isFireingRoutineRunning)
        {
            firingRoutine = StartCoroutine(ShootSequence());
            isFireingRoutineRunning = true;
        }
    }

    private void ReleaseTrigger()
    {
        if(isFireingRoutineRunning && (firingRoutine != null))
        {
            StopCoroutine(firingRoutine);
            isFireingRoutineRunning = false;
        }
    }

    private IEnumerator ShootSequence()
    {
        while (gameObject.activeSelf)
        {
            RayCastShoot();
            audioSource.Play();

            ApplyRecoil();
            yield return waitTime;
        }
    }

    private void RayCastShoot()
    {
        RaycastHit hitInformation;

        bool rayCastDidHit = Physics.Raycast(rayCastSource.transform.position, rayCastSource.transform.forward, out hitInformation, rayCastRange);
        if (rayCastDidHit)
        {
            Transform objectHit = hitInformation.transform;

            if (hitInformation.rigidbody)
            {
                print("Trigger");
                hitInformation.rigidbody.AddForce(-hitInformation.normal * rayCastImpactForce);
            }
            Fire();
        }
    }

    //사격 
    private void ApplyRecoil()
    {
        rigidbody.AddRelativeForce(Vector3.back * recoilAmount, ForceMode.Impulse);
    }

    private void Fire()
    {
        Rigidbody bulletRigid = Instantiate(bullet, barrelPivot.position, barrelPivot.rotation).GetComponent<Rigidbody>();
        bulletRigid.velocity = barrelPivot.forward * shootSpeed;
        muzzleFlash.SetActive(true);
    }
}
