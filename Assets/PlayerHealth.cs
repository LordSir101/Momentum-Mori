using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerHealth: MonoBehaviour
{
    [SerializeField] ScriptableRendererFeature frenzyEffect;
    public GameOverPanel gameOverPanel;
    public HealthBar healthBar;
    private int health = 100;
    public int MaxHealth { get; set; } = 100;
    public int Armor {get; set;} = 0;
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken - Armor;
        healthBar.SetHealth(health);

        if(health <= 0)
        {
            frenzyEffect.SetActive(false);
            gameOverPanel.SetWin(false);
        }
    }

    public void Heal(int healing)
    {
        health += healing;
        healthBar.SetHealth(health);
    }

    public void HealToFull()
    {
        health = MaxHealth;
        healthBar.SetHealth(health);
    }
    
}
