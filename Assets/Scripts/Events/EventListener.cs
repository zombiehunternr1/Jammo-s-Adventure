using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventListener : MonoBehaviour
{
    public PlayerInfo PlayerInfo;
    public Text BoltText;
    public Text LifeText;
    public Animator HUD;

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnSubscribeEvents();
    }

    void SubscribeEvents()
    {
        EventManager.OnCollectedLife += HandleCollectedLife;
        EventManager.OnCollectedBolt += HandleCollectedBolt;
    }

    void UnSubscribeEvents()
    {
        EventManager.OnCollectedLife -= HandleCollectedLife;
        EventManager.OnCollectedBolt -= HandleCollectedBolt;
    }

    private void HandleCollectedLife()
    {
        PlayerInfo.Lives++;
        LifeText.text = PlayerInfo.Lives.ToString();
        HUD.SetTrigger("IsDisplayingLifeHUD");
    }

    private void HandleCollectedBolt()
    {
        PlayerInfo.Bolts++;
        BoltText.text = PlayerInfo.Bolts.ToString();
        HUD.SetTrigger("IsDisplayingBoltHUD");
    }
}
