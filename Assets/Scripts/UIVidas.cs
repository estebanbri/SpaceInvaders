using System.Collections.Generic; 
using UnityEngine; 

public class UIVidas : MonoBehaviour { 
    
    public static UIVidas Instance { get; private set; } 
    
    [SerializeField] private GameObject iconFilledPrefab; 
    [SerializeField] private GameObject iconEmptyPrefab; 
    
    private List<GameObject> currentIcons = new List<GameObject>(); 
    private int livesMax; 

    private void Awake() { 
        Instance = this; 
    }
    public void Initialize(int livesMax)
    {
        this.livesMax = livesMax;
        ClearIcons();

        for (int i = 0; i < livesMax; i++)
        {
            GameObject icon = Instantiate(iconEmptyPrefab, transform);
            currentIcons.Add(icon);
        }

        SetLivesIcons(livesMax);
    }

    public void SetLivesIcons(int livesCurrent)
    {
        for (int i = 0; i < currentIcons.Count; i++)
        {
            var image = currentIcons[i].GetComponent<UnityEngine.UI.Image>();
            image.sprite = i < livesCurrent
                ? iconFilledPrefab.GetComponent<UnityEngine.UI.Image>().sprite
                : iconEmptyPrefab.GetComponent<UnityEngine.UI.Image>().sprite;
        }
    }

    private void ClearIcons()
    {
        foreach (var icon in currentIcons)
            Destroy(icon);

        currentIcons.Clear();
    }
}