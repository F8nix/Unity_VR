using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandParticles : MonoBehaviour
{
    public Animator handAnimator;


    [SerializeField] private InputActionReference shootTrail;

    //public ParticleSystem trailSystem;
    //public ParticleSystem impactSystem;
    public Transform tracerSpawnPoint;
    public TrailRenderer tracerTrail;
    public LayerMask maskRef;
    public Animator animatorRef;
    public XRRayInteractor rayInteractorRef;
    public XRDirectInteractor directInteractorRef;

    
    // W ten spos�b da si� sprawdza� eventy bez wciskania guzik�w do update'u
    // RemotePickupBehaviour.cs - CheckForInput()
    private void OnEnable()
    {
        shootTrail.action.performed += Shoot;
        shootTrail.action.Enable();
    }
    private void OnDisable()
    {
        shootTrail.action.performed -= Shoot;
        shootTrail.action.Disable();
    }
    public void Shoot(InputAction.CallbackContext obj)
    {
        StartCoroutine(ShootLogic());
    }

    private Vector3 GetDirection()
    {
        //Transform.forward, nie Vector3.forward
        Vector3 direction = transform.forward;
        return direction;
    }

    // Poniewa� smugi docieraj� wolniej ni� da si� podnie�� obiekt, w przypadku trafienia w niego nie wystrzeliwujemy niczego
    // Zamiast smug zrobi si� osobny efekt podnoszenia, oparty na �wietle lub outline'ach
    private IEnumerator ShootLogic()
    {
        yield return new WaitForSeconds(0.05f);
        if (rayInteractorRef.hasSelection || directInteractorRef.hasSelection)
        {
            handAnimator.SetBool("telekinesis", true);
            yield break;
        }

        //trailSystem.Play();
        Vector3 direction = GetDirection();

        // Sprawd�, czy cokolwiek zosta�o trafione przez raycast; w przypadku strzelenia "w powietrze" nie r�b niczego
        if (Physics.Raycast(tracerSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, maskRef))
        {
            TrailRenderer trail = Instantiate(tracerTrail, tracerSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
        }
    }

    // Wystrzel smug� w kierunku wskazanym przez kontroler
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        //Instantiate(impactSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }
    
}
