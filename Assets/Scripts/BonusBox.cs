using UnityEngine;

public class BonusBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Nave>(out Nave nave))
        {
            BonusPickerManager.Instance.OpenPicker();
            Destroy(gameObject);
        }
    }
}