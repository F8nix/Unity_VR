using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    float fadeTime = 1f;

    // sure
    private Renderer rendererRef;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // komponent zamkni�ty w sobie, wi�c nie trzeba przypisania
        rendererRef = GetComponent<MeshRenderer>();
        rendererRef.material.SetColor("_Color", new Color(0f, 0f, 0f, 1f));
        FadeIn(fadeTime);
    }

    private IEnumerator Fade(float targetAlpha, Action? doneCallback = null)
    {
        var currentColor = rendererRef.material.GetColor("_Color");
        var currentAlpha = currentColor.a;
        float time = 0f;

        while (time <= fadeTime)
        {
            time += Time.deltaTime;
            var alpha = Mathf.Lerp(currentAlpha, targetAlpha, time / fadeTime);
            rendererRef.material.SetColor("_Color", new Color(0f, 0f, 0f, alpha));
            yield return null;
        }

        if (doneCallback != null)
        {
            doneCallback.Invoke();
        }
    }

    public void FadeOut(float fadeTime, Action onDone = null)
    {
        this.fadeTime = fadeTime;
        StartCoroutine(Fade(1, onDone));
    }

    public void FadeIn(float fadeTime, Action onDone = null)
    {
        this.fadeTime = fadeTime;
        StartCoroutine(Fade(0, onDone));
    }
}


// Do �ciany
// b�dzie trzeba importy
// jaki� gameObject w okolicy g�owy kt�ry ma collider trigger jest potrzebny tutaj
public class WallFadeDetector : MonoBehaviour
{
    public float wallFadeOut = 0.2f;
    public float wallFadeIn = 0.2f;
    private void OnTriggerEnter(Collider other)
    {
        // Przyda�oby si� sprawdzi� czy other jest �cian� w jaki� spos�b. Nie jestem pewien jak to zrobi� dok�adnie tbh. Ale wywo�anie fadera w ten spos�b
        ScreenFader.Instance.FadeOut(wallFadeOut);
    }
    private void OnTriggerExit(Collider other)
    {
        ScreenFader.Instance.FadeIn(wallFadeIn);
    }

}