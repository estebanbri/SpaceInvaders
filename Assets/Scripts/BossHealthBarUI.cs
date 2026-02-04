using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Enemigo boss;

    public void Bind(Enemigo enemigo)
    {
        boss = enemigo;

        slider.maxValue = enemigo.MaxHealth;
        slider.value = enemigo.CurrentHealth;

        gameObject.SetActive(true);

        enemigo.OnHealthChanged += UpdateHealth;
        enemigo.OnEnemyDied += Hide;
    }

    void UpdateHealth(int current, int max)
    {
        slider.value = current;
    }

    void Hide()
    {
        gameObject.SetActive(false);

        if (boss != null)
        {
            boss.OnHealthChanged -= UpdateHealth;
            boss.OnEnemyDied -= Hide;
        }
    }
}