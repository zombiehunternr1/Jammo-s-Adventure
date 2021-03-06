using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EventListener : MonoBehaviour
{
    public PlayerInfo PlayerInfo;
    public ArtifactsCollected CollectedShards, TotalShards;
    public AudioSource AudioSource;
    public AudioClip ShardCollected;
    public Text BoltText;
    public Text LifeText;
    public Animator HUD;
    public GameObject Life;
    public GameObject ArtifactShards;

    private float StartTime = 0;
    private float CurrentTime = 0;
    public float EndInvulnerabiltyAfter = 5;
    public float EndInvincibilityAfter = 10;

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
        EventManager.OnPlayerHit += HandlePlayerHit;
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
        EventManager.OnPlayerHit -= HandlePlayerHit;
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
        HUD.IsInTransition(0);
        HUD.CrossFade("Display Life HUD", 0.25f);
    }

    private void HandleCollectedBoltDisplay()
    {
        HUD.IsInTransition(0);
        HUD.CrossFade("Display Bolt HUD", 0.25f);
    }

    private void HandleCollectedShardDisplay()
    {
        AudioSource.clip = ShardCollected;
        AudioSource.Play();
        HUD.IsInTransition(0);
        HUD.CrossFade("Display Shard HUD", 0.25f);
        for(int i = 0; i < CollectedShards.Artifacts.Count; i++)
        {
            for(int j = 0; j < TotalShards.Artifacts.Count; j++)
            {
                if (CollectedShards.Artifacts[i].Level == TotalShards.Artifacts[j].Level)
                {
                    Shards[Convert.ToInt32(CollectedShards.Artifacts[i].ActiveShardSelected)].GetComponent<Renderer>().enabled = true;
                    Shards[Convert.ToInt32(CollectedShards.Artifacts[i].ActiveShardSelected)].GetComponent<Renderer>().material.SetColor("_EmissionColor", CollectedShards.Artifacts[i].ArtifactColor);
                }
            }

        }
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
        PlayerInfo.ExtraHit = 0;
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
    
    private void CheckExtraHit()
    {
        PlayerInfo.ExtraHit--;
        if(PlayerInfo.ExtraHit < 0)
        {
            PlayerInfo.ExtraHit = 0;
            GameManager.Booleans.Invulnerable = false;
            CheckWithdrawLife();
        }
        else
        {
            GameManager.Booleans.Invulnerable = true;
            if (PlayerInfo.ExtraHit == 0)
            {
                Player.GetComponentInChildren<CompanionRobot>().GetComponent<Animator>().Play("Explode");
            }
            else
            {
                if (PlayerInfo.ExtraHit == 1)
                {
                    Player.GetComponentInChildren<CompanionRobot>().ShellColor.color = Color.red;
                    Player.GetComponentInChildren<CompanionRobot>().GetComponent<Animator>().Play("Damage");
                }
                else if (PlayerInfo.ExtraHit == 2)
                {
                    Player.GetComponentInChildren<CompanionRobot>().ShellColor.color = Color.yellow;
                    Player.GetComponentInChildren<CompanionRobot>().GetComponent<Animator>().Play("Damage");
                }
            }
            StartCoroutine(Invulnerability());
        }
    }

    private void HandlePlayerHit()
    {
        CheckExtraHit();
    }

    private void HandlePlayerMovement()
    {
        GameManager.Booleans.CanMove = !GameManager.Booleans.CanMove;
    }

    private void HandleClearItemContainer()
    {
        GameManager.Instance.ClearItemsContainer();
    }

    IEnumerator Invulnerability()
    {
        while(CurrentTime < EndInvulnerabiltyAfter)
        {
            if (Player.PlayerModel.gameObject.activeSelf)
            {
                Player.PlayerModel.gameObject.SetActive(false);
            }
            else
            {
                Player.PlayerModel.gameObject.SetActive(true);
            }
            new WaitForSeconds(0.1f);
            CurrentTime += Time.deltaTime;         
            if(CurrentTime >= EndInvulnerabiltyAfter)
            {
                StopAllCoroutines();
                CurrentTime = StartTime;
                GameManager.Booleans.Invulnerable = false;
                Player.PlayerModel.gameObject.SetActive(true);
            }
            yield return CurrentTime;
        }
        yield return null;
    }

    public IEnumerator Invincible()
    {
        Player.GetComponent<CharacterSkinController>().InvincibleEvent();
        GameManager.Booleans.Invulnerable = true;
        while(CurrentTime < EndInvincibilityAfter)
        {
            CurrentTime += Time.deltaTime;
            if(CurrentTime >= EndInvincibilityAfter)
            {
                StopAllCoroutines();
                CurrentTime = StartTime;
                GameManager.Booleans.Invulnerable = false;
                Player.GetComponent<CharacterSkinController>().ReturnToNormalEvent();
                Player.GetComponentInChildren<CompanionRobot>().ShellColor.color = Color.yellow;
                Player.GetComponentInChildren<CompanionRobot>().GetComponent<Animator>().Play("Damage");
                PlayerInfo.ExtraHit--;
            }
            yield return CurrentTime;
        }
        yield return null;
    }
}
