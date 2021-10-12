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

    private GameObject SpawnedLife;
    [SerializeField] private PlayerScript Player;

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
        EventManager.OnEnablePlayerMovement += HandlePlayerMovement;
        EventManager.OnClearItemsContainer += HandleClearItemContainer;
        EventManager.OnResetGameOverPlayer += HandleResetGameOverPlayer;
    }

    void UnSubscribeEvents()
    {
        EventManager.OnCollectedLifeDisplay -= HandleCollectedLifeDisplay;
        EventManager.OnCollectedBoltDisplay -= HandleCollectedBoltDisplay;
        EventManager.OnCollectedLifeUpdate -= HandleCollectedLifeUpdate;
        EventManager.OnCollectedBoltUpdate -= HandleCollectedBoltUpdate;
        EventManager.OnPlayerDied -= HandlePlayerDied;
        EventManager.OnEnablePlayerMovement -= HandlePlayerMovement;
        EventManager.OnClearItemsContainer -= HandleClearItemContainer;
        EventManager.OnResetGameOverPlayer -= HandleResetGameOverPlayer;
    }

    private void SetupUIValues()
    {
        LifeText.text = PlayerInfo.Lives.ToString();
        BoltText.text = PlayerInfo.Bolts.ToString();
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
        CheckAddLife();
    }

    private void HandleCollectedBoltUpdate()
    {
        CheckBoltCount();
    }
    
    private void HandleResetGameOverPlayer()
    {
        PlayerInfo.Lives = 5;
        GameManager.Booleans.GameOver = false;
        GameManager.Booleans.IsResetGame = true;
        GameManager.Instance.ResetGame();
    }

    private void CheckBoltCount()
    {
        if (PlayerInfo.Bolts >= 99)
        {
            PlayerInfo.Bolts = 0;
            HandleCollectedLifeDisplay();
            SpawnedLife = Instantiate(Life, Player.transform.position, Quaternion.identity);
            SpawnedLife.GetComponent<Life>().GoToHover();
        }
        else
        {
            PlayerInfo.Bolts++;
        }
        BoltText.text = PlayerInfo.Bolts.ToString();
    }

    private void CheckAddLife()
    {
        PlayerInfo.Lives++;
        if (PlayerInfo.Lives >= 99)
        {
            PlayerInfo.Lives = 99;
        }
        LifeText.text = PlayerInfo.Lives.ToString();
    }

    private void CheckWithdrawLife()
    {
        PlayerInfo.Lives--;
        if (PlayerInfo.Lives < 0)
        {
            PlayerInfo.Lives = 0;
            GameManager.Booleans.GameOver = true;
        }
        LifeText.text = PlayerInfo.Lives.ToString();
        GameManager.Instance.PlayerDied();
    }

    private void HandlePlayerDied()
    {
        CheckWithdrawLife();
    }

    private void HandlePlayerMovement()
    {
        GameManager.Booleans.CanMove = !GameManager.Booleans.CanMove;
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
