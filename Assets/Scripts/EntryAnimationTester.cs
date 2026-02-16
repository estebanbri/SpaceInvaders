using UnityEngine;

public class EntryAnimationTester : MonoBehaviour
{
    public EntryAnimation entryAnimation;
    public Transform targetTransform;
    public float duration = 2f;

    void Start()
    {
        if (entryAnimation != null && targetTransform != null)
        {
            // entryAnimation.Initialize(entryAnimation.transform.position, targetTransform.position, duration);
        }
    }
}