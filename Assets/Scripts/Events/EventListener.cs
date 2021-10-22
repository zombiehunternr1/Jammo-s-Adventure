using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EventListener : MonoBehaviour
{
    public PlayerInfo PlayerInfo;
    public ArtifactsCollected CollectedShards, TotalShards;
    public Text BoltText;
    public Text LifeText;
    public Animator HUD;
    public GameObject Life;
    public GameObject ArtifactShards;

    private List<Transform> Shards;
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
        EventManager.OnCollectedShardUpdate += HandleCollectedShardDisplay;
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
        EventManager.OnCollectedShardUpdate -= HandleCollectedShardDisplay;
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
        Shards = new List<Transform>();
        Transform[] Children = ArtifactShards.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Children.Length; i++)
        {
            Shards.Add(Children[i]);
        }
        foreach(Transform Shard in Shards)
        {
            Shard.GetComponent<Renderer>().enabled = false;
        }
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

    private void HandleCollectedShardDisplay()
    {
        HUD.SetTrigger("IsDisplayingShardHUD");
        for(int i = 0; i < CollectedShards.Artifacts.Count; i++)
        {
            for(int j = 0; j < TotalShards.Artifacts.Count; j++)
            {
                if (CollectedShards.Artifacts[i].Level == TotalShards.Artifacts[j].Level)
                {
                    Shards[Convert.ToInt32(CollectedShards.Artifacts[i].ActiveShardSelected)].GetComponent<Renderer>().enabled = true;
                }
            }

        }

        StartCoroutine(DisableShardTrigger());
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
        PlayerInfo.Bolts = 0;
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

    private IEnumerator DisableShardTrigger()
    {
        yield return new WaitForSeconds(5);
        HUD.ResetTrigger("IsDisplayingShardHUD");
        HUD.SetTrigger("IsDisplayingShardHUD");
    }
}
