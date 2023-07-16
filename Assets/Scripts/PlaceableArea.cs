using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaceableArea : MonoBehaviour
{
    public Vector3 areaPosition;
    public bool disableOnPlacement;
    public bool mustBeDropped;
    public GameObject neededObject;
    public GameObject leftControllerRef;
    public GameObject rightControllerRef;
    public RemotePickupBehaviour pickupRef;
    [SerializeField] private enum InteractionType { Stay = 1, Jump = 2, Spin = 3 }
    [SerializeField] private InteractionType chosenInteraction;

    // Ten skrypt przewidywany jest dla stref, w kt�rych nale�y b�dzie od�o�y� przedmiot na miejsce (np: klucz w drzwiach, brakuj�cy element uk�adanki, puste miejsce na szafce itp)
    // Je�eli przedmiot zosta� rzucony lu�no, pomijamy pierwszy cz�on warunkowy, odk�adamy go na miejsce a nast�pnie nakazujemy mu wykona� jak�� akcj�
    // Je�eli przedmiot jest podsuni�ty przez aktywnie trzymaj�c� go r�ke, na pocz�tku trzeba go jeszcze z tej r�ki wytr�ci�
    private void OnTriggerEnter(Collider enteredCollider)
    {
        if (!mustBeDropped)
        {
            if (enteredCollider.gameObject.name.Equals(neededObject.name))
            {
                try
                {
                    DeselectBoth(enteredCollider);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                PlaceInArea(enteredCollider.gameObject);
            }

            return;
        }

        if (enteredCollider.gameObject.name.Equals(neededObject.name))
        {
            PlaceInArea(enteredCollider.gameObject);
        }

    }

    // Od�� przedmiot na miejsce, nast�pnie (opcjonalnie) wykonaj akcj�
    public void PlaceInArea(GameObject givenObject)
    {
        // Mam dziwne wrazenie ze przy bezposrednim trzymaniu przedmiotu w rece czasami obiekt teleportuje sie do miejsca docelowego razem z graczem
        // Nie mam na razie pomyslu na to jak to zbadac ani co z tym zrobic

        switch ((int)chosenInteraction)
        {
            case 1:
                givenObject.transform.position = areaPosition;
                break;
        }

        if (disableOnPlacement)
        {
            givenObject.GetComponent<XRGrabInteractable>().enabled = false;
        }
    }

    public void DeselectBoth(Collider givenCollider)
    {
        bool leftHandHasSelection = leftControllerRef.GetComponent<XRDirectInteractor>().hasSelection || pickupRef.interactorRefLeft.hasSelection;
        bool rightHandHasSelection = rightControllerRef.GetComponent<XRDirectInteractor>().hasSelection || pickupRef.interactorRefRight.hasSelection;

        bool firstLeftCheck;
        bool secondLeftCheck;
        bool firstRightCheck;
        bool secondRightCheck;

        // Podobnie jak w RemotePickupBehaviour, nie ka�dy z tych check�w jest zawsze potrzebny, a ka�dy sypnie b��dem je�eli nie b�dzie si� zgadza�, dlatego robimy tu �cian� try/catchy
        // Ten kawa�ek kodu sprawdza, czy w kt�rymkolwiek z dost�pnych interactor�w jest obecnie z�apany przedmiot, a dost�pne s�: Lewa r�ka promie�, lewa r�ka bezpo�rednio, prawa r�ka promie� oraz prawa r�ka bezpo�rednio
        // Obiekty s� sprawdzane po nazwie, a nie przy pomocy GameObject.referenceEquals poniewa� 1) kostka na ziemii jest typu GameObject a z�apana XRGrabInteractable, a 2) z�apana kostka ma jeszcze doczepiane dziecko z DynamicAttacha i te sprawdzenia nie przechodz�
        // Prawdopodobnie da�oby si� to omin�� gdyby da�o si� przerzutowa� XRGrabInteractable > GameObject, ale nie znalaz�em takiego rozwi�zania w internecie

        try
        {
            firstLeftCheck = (givenCollider.gameObject.name.Equals((pickupRef.interactorRefLeft.interactablesSelected[0] as XRGrabInteractable).name));
        }
        catch
        {
            firstLeftCheck = false;
        }

        try
        {
            secondLeftCheck = (givenCollider.gameObject.name.Equals((leftControllerRef.GetComponent<XRDirectInteractor>().interactablesSelected[0] as XRGrabInteractable).name));
        }
        catch
        {
            secondLeftCheck = false;
        }

        try
        {
            firstRightCheck = (givenCollider.gameObject.name.Equals((pickupRef.interactorRefRight.interactablesSelected[0] as XRGrabInteractable).name));
        }
        catch
        {
            firstRightCheck = false;
        }

        try
        {
            secondRightCheck = (givenCollider.gameObject.name.Equals((rightControllerRef.GetComponent<XRDirectInteractor>().interactablesSelected[0] as XRGrabInteractable).name));
        }
        catch
        {
            secondRightCheck = false;
        }

        // Je�eli potrzebny obiekt jest trzymany przez kt�r�kolwiek z r�k, wyrzu� go z aktywnego zaznaczenia przez interactor (inaczej nie mo�na zmieni� mu pozycji)

        if (leftHandHasSelection && (givenCollider.gameObject.name.Equals(neededObject.name)) && (firstLeftCheck || secondLeftCheck))
        { 
            pickupRef.ForceDeselect(pickupRef.interactorRefLeft);
            pickupRef.ForceDeselect(leftControllerRef.GetComponent<XRDirectInteractor>());
        }

        if (rightHandHasSelection && (givenCollider.gameObject.name.Equals(neededObject.name)) && (firstRightCheck || secondRightCheck))
        {
            pickupRef.ForceDeselect(pickupRef.interactorRefRight);
            pickupRef.ForceDeselect(rightControllerRef.GetComponent<XRDirectInteractor>());
        }
    }
}
