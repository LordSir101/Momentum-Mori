using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DamageShrine : Shrine
{
    PlayerImpact playerImpactScript;
    private float[] dmgUpgradeValues;
    [SerializeField] Sprite cleanseIcon;
    [SerializeField] Material cleanseIconMaterial;

    // Start is called before the first frame update
    void Start()
    {
        playerImpactScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerImpact>();
        CleanseIcon = cleanseIcon;
        CleanseIconMaterial = cleanseIconMaterial;

        dmgUpgradeValues = new float[] {0.10f, 0.10f, 0.15f};
    }

    protected override void ShowUpgradeText()
    {
        interactText.enabled = true;
        interactText.text = $"Increase damage dealt +{dmgUpgradeValues[numUpgrades]} ({upgradeCosts[numUpgrades]})";
    }

    protected override void Upgrade(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= upgradeCosts[numUpgrades])
        {
            rm.Essence -= upgradeCosts[numUpgrades];
            playerImpactScript.DamageBonusPercent += dmgUpgradeValues[numUpgrades];
            numUpgrades++;

        }
       
        if(numUpgrades == maxUpgrades)
        {
            CloseShrine();
        }
        else
        {
            ShowUpgradeText();
        }

        LightCandle();
    }
}
