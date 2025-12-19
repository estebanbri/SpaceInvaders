using UnityEngine;

public class EscudoVisual : MonoBehaviour
{
    public void HideEscudo()
    {
        gameObject.SetActive(false);
    }

    public void ShowEscudo()
    {
        gameObject.SetActive(true);
    }

}
