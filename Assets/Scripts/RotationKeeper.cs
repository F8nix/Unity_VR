using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotationKeeper : MonoBehaviour
{
    // Skrypt nie jest ju� odpowiedzialny za weryfikowanie obracania przedmiotu, natomiast nie ma sensu psu� scen refaktoryzacj� dlatego zostaje z nazw� jaka jest
    // Je�eli zostanie czas: zmieni� nazw� na "ParentKeeper" albo "JointKeeper"

    public XRDirectInteractor directLeftRef;
    public XRDirectInteractor directRightRef;
    public Transform physicsLeftHand;
    public Transform physicsRightHand;
    public Transform handParentLeft;
    public Transform handParentRight;
    private Transform startParent;
    private bool leftSelecting;
    private bool rightSelecting;

    private void Start()
    {
        startParent = transform.parent;
    }
    public void AssignParent()
    {
        CheckSelection();
        StartCoroutine(Assign());
    }

    public void CheckSelection()
    {
        try
        {
            if (directLeftRef.interactablesSelected[0] != null && (directLeftRef.interactablesSelected[0] as XRGrabInteractable).name.Equals(gameObject.name))
            {
                leftSelecting = true;
            }
            else
            {
                leftSelecting = false;
            }
        }
        catch
        {
            leftSelecting = false;
        }

        try
        {
            if (directRightRef.interactablesSelected[0] != null && (directRightRef.interactablesSelected[0] as XRGrabInteractable).name.Equals(gameObject.name))
            {
                rightSelecting = true;
            }
            else
            {
                rightSelecting = false;
            }
        }
        catch
        {
            rightSelecting = false;
        }
    }

    public void ReturnParent()
    {
        transform.parent = startParent;
        Destroy(gameObject.GetComponent<FixedJoint>());
    }

    IEnumerator Assign()
    {
        yield return new WaitForSeconds(0.1f);

        if (leftSelecting)
        {
            transform.parent = handParentLeft;
            StartCoroutine(CreateJoint());
        }

        if (rightSelecting)
        {
            transform.parent = handParentRight;
            StartCoroutine(CreateJoint());
        }
    }

    IEnumerator CreateJoint()
    {
        yield return new WaitForSeconds(0.1f);

        if (leftSelecting)
        {
            FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = physicsLeftHand.GetComponent<Rigidbody>();
        }

        if (rightSelecting)
        {
            FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = physicsRightHand.GetComponent<Rigidbody>();
        }
    }
}
