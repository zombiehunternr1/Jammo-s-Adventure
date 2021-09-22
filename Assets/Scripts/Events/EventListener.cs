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
    public GameObject Life;

    private Transform Player;
    private GameObject SpawnedLife;

    void OnEnable()
    {
        SetupUIValues();
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnSubscribeEvents();
    }

    void SubscribeEvents()
    {
        EventManager.OnCollectedLifeDisplay += HandleCollectedLifeDisplay;
        EventManager.OnCollectedBoltDisplay += HandleCollectedBoltDisplay;
        EventManager.OnCollectedLifeUpdate += HandleCollectedLifeUpdate;
        EventManager.OnCollectedBoltUpdate += HandleCollectedBoltUpdate;
    }

    void UnSubscribeEvents()
    {
        EventManager.OnCollectedLifeDisplay -= HandleCollectedLifeDisplay;
        EventManager.OnCollectedBoltDisplay -= HandleCollectedBoltDisplay;
        EventManager.OnCollectedLifeUpdate -= HandleCollectedLifeUpdate;
        EventManager.OnCollectedBoltUpdate -= HandleCollectedBoltUpdate;
    }

    private void SetupUIValues()
    {
        LifeText.text = PlayerInfo.Lives.ToString();
        BoltText.text = PlayerInfo.Bolts.ToString();
        Player = (Transform)GameObject.Find("Player").gameObject.GetComponent(typeof(Transform));
    }

    private void HandleCollectedLifeDisplay()
    {
        HUD.SetTrigger("IsDisplayingLifeHUD");
        StartCoroutine(DisableLifeTrigger());
    }

    private void HandleCollectedBoltDisplay()
    {
        HUD.SetTrigger("IsDisplayingBoltHUD");
        StartCoroutine(DisableBoltTrigger());
    }

    private void HandleCollectedLifeUpdate()
    {
        PlayerInfo.Lives++;
        LifeText.text = PlayerInfo.Lives.ToString();
    }

    private void HandleCollectedBoltUpdate()
    {
        PlayerInfo.Bolts++;
        CheckBoltCount();
    }

    private void CheckBoltCount()
    {
        if (PlayerInfo.Bolts > 99)
        {
            PlayerInfo.Bolts = 0;
            SpawnedLife = Instantiate(Life, Player.transform.position, Quaternion.identity);
            SpawnedLife.GetComponent<Animator>().SetTrigger("SkipSpawning");
        }
        BoltText.text = PlayerInfo.Bolts.ToString();
    }

    private IEnumerator DisableLifeTrigger()
    {
        SpawnedLife.GetComponent<Animator>().ResetTrigger("SkipSpawning");
        yield return new WaitForSeconds(1);
        HUD.ResetTrigger("IsDisplayingLifeHUD");
    }

    private IEnumerator DisableBoltTrigger()
    {
        yield return new WaitForSeconds(1);
        HUD.ResetTrigger("IsDisplayingBoltHUD");
    }
}
