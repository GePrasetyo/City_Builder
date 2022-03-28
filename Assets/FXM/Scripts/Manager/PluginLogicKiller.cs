using CityBuilderCore;
using UnityEngine;

public class PluginLogicKiller : MonoBehaviour
{
    public DefaultGameManager gameManager;

    void Awake()
    {
        gameManager.DisableEfficiency = true;
        gameManager.DisableEmployment = true;
        gameManager.RiskMultiplier = 0;
        gameManager.ServiceMultiplier = 0;
        gameManager.ItemsMultiplier = 0;



    }


}
