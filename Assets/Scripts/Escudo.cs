using System;
using System.Collections;
using UnityEngine;

public class Escudo : MonoBehaviour
{
    public static Escudo Instance { get;  set; }

    private EscudoVisual escudoVisualComponent;
    private bool isActive;
    private void Awake()
    {
        Instance = this;
        escudoVisualComponent = GetComponentInChildren<EscudoVisual>();
        RemoveEscudo();
    }

    public void AddEscudo()
    {
        isActive = true;
        escudoVisualComponent.ShowEscudo();
    }

    public void RemoveEscudo()
    {
        escudoVisualComponent.HideEscudo();
        StartCoroutine(AddInvulnerability());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemigo>(out Enemigo enemigo) ||  collision.TryGetComponent<Torreta>(out Torreta torreta)) {
            RemoveEscudo();
        }
    }

    public bool IsActive() { 
        return isActive == true; 
    }

    private IEnumerator AddInvulnerability() {
        yield return new WaitForSeconds(1f);
        isActive = false;
    }

    public void PlayEscudoDestroyedEffect()
    {
        escudoVisualComponent.OnDestroyedEffect();
    }
}
