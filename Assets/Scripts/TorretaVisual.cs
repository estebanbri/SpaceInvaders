using UnityEngine;

public class TorretaVisual : MonoBehaviour
{
    private Torreta torreta;
    

    private void Awake()
    {
        torreta = GetComponentInParent<Torreta>();
    }

    private void OnBecameVisible()
    {
        torreta.SetVisible(true);
    }

    private void OnBecameInvisible()
    {
        torreta.SetVisible(false);
    }

   
}