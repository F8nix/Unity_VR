using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotationKeeper : MonoBehaviour
{
    public XRDirectInteractor directLeftRef;
    public XRDirectInteractor directRightRef;
    public Transform leftHand;
    public Transform rightHand;
    public Transform playerParent;

    private Transform attachPoint;
    private Transform startParent;
    private bool leftSelecting;
    private bool rightSelecting;

    private void Start()
    {
        startParent = transform.parent;
    }
    void Update()
    {
        try
        {
            if (directLeftRef.interactablesSelected[0] != null && (directLeftRef.interactablesSelected[0] as XRGrabInteractable).name.Equals(gameObject.name))
            {
                transform.rotation = leftHand.rotation;
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
                transform.rotation = rightHand.rotation;
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            if(leftSelecting)
            {
                //Debug.Log("boing");
                //leftHand.GetComponent<Rigidbody>().velocity = leftHand.GetComponent<Rigidbody>().velocity * -10;
            }    

            if(rightSelecting)
            {
                //Debug.Log("boing");
                //rightHand.GetComponent<Rigidbody>().velocity = rightHand.GetComponent<Rigidbody>().velocity * -10;
            }
        }
    }

    public void AssignParent()
    {
        transform.parent = playerParent;
        StartCoroutine(CreateJoint());
    }

    public void ReturnParent()
    {
        transform.parent = startParent;
        Destroy(gameObject.GetComponent<FixedJoint>());
    }

    IEnumerator CreateJoint()
    {
        yield return new WaitForSeconds(0.1f);

        if (leftSelecting)
        {
            FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = leftHand.GetComponent<Rigidbody>();
        }

        if (rightSelecting)
        {
            FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = rightHand.GetComponent<Rigidbody>();
        }
    }
}