using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Linq;

public class RemotePickupBehaviour : XRBaseInteractor
{
    private static RemotePickupBehaviour _instance;
    public static RemotePickupBehaviour Instance { get { return _instance; } }

    private bool isRecalled = false;
    private bool gripPressedRight = false;
    private bool gripPressedLeft = false;
    public XRGrabInteractable grabbedObject;
    public XRRayInteractor usedInteractor;
    public XRRayInteractor interactorRefLeft;
    public XRRayInteractor interactorRefRight;
    public HandPhysics physicsRefLeft;
    public HandPhysics physicsRefRight;
    [SerializeField] private float gripSensitivity = 0.3f;

    public GameObject controllerLeft;
    public GameObject controllerRight;

    // Poniewa� jest to gra jednoosobowa i zestaw kontroler�w jest zawsze jeden, to z mo�emy zrobi� z managera interakcji singleton
    private new void Awake()
    {
        base.Awake();

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public void Update()
    {
        CheckForInput();
        CheckForSelection();
    }

    // Podczas pracy bezpo�redniego interactora, wy��czamy interakcj� promieniem aby nie da�o si� rusza� r�k� dw�ch obiekt�w naraz (b�d� bugowa� te obecnie z�apane)
    public void CheckForSelection()
    {
        if (controllerLeft.GetComponent<XRDirectInteractor>().hasSelection)
        {
            interactorRefLeft.enabled = false;
            interactorRefLeft.gameObject.GetComponent<XRInteractorLineVisual>().enabled = false;
        }
        else if (!controllerLeft.GetComponent<XRDirectInteractor>().hasSelection)
        {
            interactorRefLeft.enabled = true;
            interactorRefLeft.gameObject.GetComponent<XRInteractorLineVisual>().enabled = true;
        }

        if (controllerRight.GetComponent<XRDirectInteractor>().hasSelection)
        {
            interactorRefRight.enabled = false;
            interactorRefRight.gameObject.GetComponent<XRInteractorLineVisual>().enabled = false;
        }
        else if (!controllerRight.GetComponent<XRDirectInteractor>().hasSelection)
        {
            interactorRefRight.enabled = true;
            interactorRefRight.gameObject.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
    }

    // Sprawd�, czy klikane s� obecnie jakiekolwiek guziki
    // Teoretycznie lepiej by by�o zrobi� jaki� event przy guzikach kt�ry wywyo�a funkcj� ni� wciska� to w update, ale p�ki co jest git
    public void CheckForInput()
    {
        if (isRecalled)
        {
            return;
        }

        gripPressedRight = Input.GetAxis("XRI_Right_Grip") > gripSensitivity;
        gripPressedLeft = Input.GetAxis("XRI_Left_Grip") > gripSensitivity;

        if (gripPressedRight)
        {
            physicsRefRight.DisableColliders();
            RecallObject(controllerRight, interactorRefRight);
        }
        else if (!gripPressedRight)
        {
            ReleaseObject(interactorRefRight);
        }

        if (gripPressedLeft)
        {
            physicsRefLeft.DisableColliders();
            RecallObject(controllerLeft, interactorRefLeft);
        }
        else if (!gripPressedLeft)
        {
            ReleaseObject(interactorRefLeft);
        }
    }

    // Przywr�� zaznaczony (promieniem) przedmiot do r�ki
    public void RecallObject(GameObject currentController, XRRayInteractor currentInteractor)
    {
        if (gripPressedRight && currentController == controllerRight || gripPressedLeft && currentController == controllerLeft)
        {
            isRecalled = true;

            try
            {
                // Funkcj� mo�na wywo�a� bez trzymania jakiegokolwiek przedmiotu i mo�e ona sypn�� b��dem, w przypadku takiego wyj�tku po prostu z niej wychodzimy 
                grabbedObject = currentInteractor.interactablesSelected[0] as XRGrabInteractable;
            }
            catch 
            {
                isRecalled = false;
                return;
            }

            // Nie trzeba kombinowa� z r�cznym przenoszeniem obiektu do r�ki zmian� pozycji poniewa� XR Toolkit daje nam ju� tak� funckj�
            // Jedyne co trzeba zrobi� to j� w��czy� podczas "zrestartowania" zaznaczenia
            ForceDeselect(currentInteractor);
            currentInteractor.useForceGrab = true;
            ForceSelect(currentInteractor,grabbedObject);
        }
        StartCoroutine(WaitForRelease(currentInteractor));
    }

    // Ko�cz�c interakcj� wy��czamy ForceGrab aby nast�pny podniesiony przedmiot nie by� natychmiastowo z�apany do d�oni
    public void ReleaseObject(XRRayInteractor currentInteractorRef)
    {
        currentInteractorRef.useForceGrab = false;
        grabbedObject = null;
    }

    // Wymu� wypuszczenie obiektu przez interactor (tak, jak przy puszczeniu triggera)
    public void ForceDeselect(XRBaseInteractor interactor)
    {
        gameObject.GetComponent<CustomInteractionManager>().ForceDeselect(interactor);
    }

    // Wymu� zaznaczenie obiektu przez interactor (tak, jakby r�cznie klikni�to trigger)
    public void ForceSelect(XRBaseInteractor interactor, IXRSelectInteractable interactable)
    {
        gameObject.GetComponent<CustomInteractionManager>().SelectEnter(interactor, interactable);
    }

    // Op�niamy zmian� zmiennej aby funkcja CheckInput() nie pr�bowa�a zbyt szybko egzekwowa� swoich cz�ci kodu
    // W przeciwnym wypadku kostka driftuje po promieniu tak d�ugo, jak wci�ni�ty jest grip
    IEnumerator WaitForRelease(XRRayInteractor currentInteractorRef)
    {
        yield return new WaitForSeconds(0.1f);
        isRecalled = false;

        if (currentInteractorRef.Equals(interactorRefRight))
        {
            physicsRefRight.Delay(0.2f);
        }

        if (currentInteractorRef.Equals(interactorRefLeft))
        {
            physicsRefLeft.Delay(0.2f);
        }
    }
}



