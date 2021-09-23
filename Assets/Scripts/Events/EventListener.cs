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
    public static bool GameOver;

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
        EventManager.OnPlayerDied += HandlePlayerDied;
    }

    void UnSubscribeEvents()
    {
        EventManager.OnCollectedLifeDisplay -= HandleCollectedLifeDisplay;
        EventManager.OnCollectedBoltDisplay -= HandleCollectedBoltDisplay;
        EventManager.OnCollectedLifeUpdate -= HandleCollectedLifeUpdate;
        EventManager.OnCollectedBoltUpdate -= HandleCollectedBoltUpdate;
        EventManager.OnPlayerDied -= HandlePlayerDied;
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
        AddLife();
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
            HandleCollectedLifeDisplay();
            SpawnedLife = Instantiate(Life, Player.transform.position, Quaternion.identity);
            SpawnedLife.GetComponent<Life>().GoToHover();
        }
        BoltText.text = PlayerInfo.Bolts.ToString();
    }

    private void AddLife()
    {
        if (PlayerInfo.Lives >= 99)
        {
            PlayerInfo.Lives = 99;
        }
        else
        {
            PlayerInfo.Lives++;
        }
        LifeText.text = PlayerInfo.Lives.ToString();
    }

    private void RemoveLife()
    {
        if (PlayerInfo.Lives <= 0)
        {
            PlayerInfo.Lives = 0;
            LifeText.text = PlayerInfo.Lives.ToString();
            GameOver = true;
        }
        else
        {
            PlayerInfo.Lives--;
            LifeText.text = PlayerInfo.Lives.ToString();
        }
    }

    public void HandlePlayerDied()
    {
        RemoveLife();
        GameManager.Instance.PlayerDied();
    }

    private void HandleClearItemContainer()
    {
        GameManager.Instance.ClearItemsContainer();
    }

    private IEnumerator DisableLifeTrigger()
    {
        yield return new WaitForSeconds(1);
        HUD.ResetTrigger("IsDisplayingLifeHUD");
    }

    private IEnumerator DisableBoltTrigger()
    {
        yield return new WaitForSeconds(1);
        HUD.ResetTrigger("IsDisplayingBoltHUD");
    }
}
