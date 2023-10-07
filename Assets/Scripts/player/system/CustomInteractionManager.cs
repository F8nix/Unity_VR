using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomInteractionManager : XRInteractionManager
{
    private void Start()
    {
        Application.targetFrameRate = 120;
    }
    // Poniewa� zwyk�y interaction manager jest read-only, tworz� sw�j w�asny dziedzicz�c po nim dodaj�c potrzebn� mi do kontrolera interakcji funkcj�
    // Reczne wylaczenie zaznaczenia przedmiotu pozwala pozniej na stworzenie naszej wlasnej logiki interaktowania z przedmiotem na dystans
    public void ForceDeselect(XRBaseInteractor interactor)
    {
        while (interactor.interactablesSelected.Count > 0)
        {
            SelectExit(interactor, interactor.interactablesSelected[0]);
        }
    }
}
