using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TwoHandedGunGuardHold : MonoBehaviour
{
    public Interactable mainInteractable; //첫번째로 잡은 손 

    private Interactable interactable;
    private Quaternion secondRotationOffset;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
    }


    public void ForceDetach()
    {
        if (interactable.attachedToHand)
        {
            interactable.attachedToHand.HoverUnlock(interactable);
            interactable.attachedToHand.DetachObject(gameObject);
        }
    }

    private Quaternion GetTargetRotation()
    {
        Vector3 mainHandUp = mainInteractable.attachedToHand.objectAttachmentPoint.up;
        Vector3 secondHandUp = interactable.attachedToHand.objectAttachmentPoint.up;

        return Quaternion.LookRotation(interactable.attachedToHand.transform.position - mainInteractable.attachedToHand.transform.position, mainHandUp);
    }

    private void OnHandHoverBegin(Hand hand)
    {
        hand.ShowGrabHint();
    }

    private void OnHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();
    }

    private void HandAttachedUpdate(Hand hand)
    {
        if (mainInteractable.attachedToHand)
        {
            if (mainInteractable.skeletonPoser)
            {
                Quaternion customHandPoseRotation = mainInteractable.skeletonPoser.GetBlendedPose(mainInteractable.attachedToHand.skeleton).rotation;
                mainInteractable.transform.rotation = GetTargetRotation() * secondRotationOffset * customHandPoseRotation;
            }
            else
            {
                mainInteractable.attachedToHand.objectAttachmentPoint.rotation = GetTargetRotation() * secondRotationOffset;
            }
        }
    }



    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes grabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(gameObject);

        //Grab
        if (interactable.attachedToHand == null && grabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, grabType, 0);
            hand.HoverLock(interactable);
            hand.HideGrabHint();
            secondRotationOffset = Quaternion.Inverse(GetTargetRotation()) * mainInteractable.attachedToHand.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation;
        }

        // Release
        else if (isGrabEnding)
        {
            DebugUI.instance.debug_txt.text = "Release";
            hand.DetachObject(gameObject);
            hand.HoverUnlock(interactable);
        }

    }
}
