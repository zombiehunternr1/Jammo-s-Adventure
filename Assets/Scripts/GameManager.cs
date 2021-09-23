using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text GameOverText;
    public Text RetryText;
    public Text QuitText;
    public Image RetryImage;
    public Image QuitImage;

    [HideInInspector]
    public GameObject BreakableCrateContainer;
    [HideInInspector]
    public GameObject SpawnedItemsContainer;
    private GameObject FadeOutPanel;
    private PlayerScript Player;
    private float HoldNextFade = 1.5f;
    private float FadeSpeed = 1;
    private bool IsFadingToBlack = true;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        SetupSceneElements();
    }

    private void SetupSceneElements()
    {
        if (BreakableCrateContainer == null)
        {
            BreakableCrateContainer = GameObject.Find("Breakable Crates");
        }
        if (SpawnedItemsContainer == null)
        {
            SpawnedItemsContainer = GameObject.Find("SpawnedItems");
        }
        if (FadeOutPanel == null)
        {
            FadeOutPanel = GameObject.Find("FadeOutPanel");
        }
        if (Player == null)
        {
            Player = GameObject.Find("Player").GetComponent<PlayerScript>();
        }
    }

    public void UpdateItemContainerList(GameObject Item)
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().Items.Add(Item);
    }

    public void ClearItemsContainer()
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().DestroyItems();
    }

    private void ResetPlayer()
    {
        Animator PlayerAnim = Player.GetComponent<Animator>();
        Player.ResetPlayerPosition();
        Player.GetComponent<CharacterSkinController>().ReturnToNormalEvent();
        PlayerAnim.ResetTrigger("IsDead");
        PlayerAnim.SetTrigger("IsDead");
    }

    public void PlayerDied()
    {
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        Image PanelImage = FadeOutPanel.GetComponent<Image>();
        Color ChangePanelColor = PanelImage.color;
        Color ChangeTextAlphaGameOver = GameOverText.color;
        Color ChangeTextAlphaRetry = RetryText.color;
        Color ChangeTextAlphaQuit = QuitText.color;
        Color ChangeRetryImage = RetryImage.color;
        Color ChangeQuitImage = QuitImage.color;
        float FadeAmount;

        if (IsFadingToBlack)
        {
            while (PanelImage.color.a < 1)
            {
                FadeAmount = ChangePanelColor.a + (FadeSpeed * Time.deltaTime);
                ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                PanelImage.color = ChangePanelColor;
                yield return null;
            }
            while (RetryText.color.a > 0 || QuitText.color.a > 0 || RetryImage.color.a > 0 || QuitImage.color.a > 0)
            {
                ChangeTextAlphaRetry = RetryText.color;
                ChangeTextAlphaQuit = QuitText.color;
                ChangeRetryImage = RetryImage.color;
                ChangeQuitImage = QuitImage.color;
                FadeAmount = ChangeTextAlphaRetry.a + (FadeSpeed * Time.deltaTime);
                ChangeTextAlphaRetry = new Color(ChangeTextAlphaRetry.r, ChangeTextAlphaRetry.g, ChangeTextAlphaRetry.b, FadeAmount);
                ChangeTextAlphaQuit = new Color(ChangeTextAlphaQuit.r, ChangeTextAlphaQuit.g, ChangeTextAlphaQuit.b, FadeAmount);
                ChangeRetryImage = new Color(ChangeRetryImage.r, ChangeRetryImage.g, ChangeRetryImage.b, FadeAmount);
                ChangeQuitImage = new Color(ChangeQuitImage.r, ChangeQuitImage.g, ChangeQuitImage.b, FadeAmount);
                RetryText.color = ChangeTextAlphaRetry;
                QuitText.color = ChangeTextAlphaQuit;
                RetryImage.color = ChangeRetryImage;
                QuitImage.color = ChangeQuitImage;
                yield return null;
            }
            IsFadingToBlack = false;
            yield return new WaitForSeconds(HoldNextFade);
            StartCoroutine(FadeToBlack());
        }
        else
        {
            if (EventListener.GameOver)
            {
                EventListener.GameOver = false;
                while (GameOverText.color.a < 1)
                {
                    ChangeTextAlphaGameOver = GameOverText.color;
                    FadeAmount = ChangeTextAlphaGameOver.a + (FadeSpeed * Time.deltaTime);
                    ChangeTextAlphaGameOver = new Color(ChangeTextAlphaGameOver.r, ChangeTextAlphaGameOver.g, ChangeTextAlphaGameOver.b, FadeAmount);
                    GameOverText.color = ChangeTextAlphaGameOver;
                    yield return null;
                }
                yield return new WaitForSeconds(HoldNextFade);
                while (RetryText.color.a < 1)
                {
                    ChangeTextAlphaRetry = RetryText.color;
                    ChangeTextAlphaQuit = QuitText.color;
                    ChangeRetryImage = RetryImage.color;
                    FadeAmount = ChangeTextAlphaRetry.a + (FadeSpeed * Time.deltaTime);
                    ChangeTextAlphaRetry = new Color(ChangeTextAlphaRetry.r, ChangeTextAlphaRetry.g, ChangeTextAlphaRetry.b, FadeAmount);
                    ChangeTextAlphaQuit = new Color(ChangeTextAlphaQuit.r, ChangeTextAlphaQuit.g, ChangeTextAlphaQuit.b, FadeAmount);
                    ChangeRetryImage = new Color(ChangeRetryImage.r, ChangeRetryImage.g, ChangeRetryImage.b, FadeAmount);
                    RetryText.color = ChangeTextAlphaRetry;
                    QuitText.color = ChangeTextAlphaQuit;
                    RetryImage.color = ChangeRetryImage;
                }
                yield return null;
            }
            else
            {
                ClearItemsContainer();
                ResetPlayer();
                yield return new WaitForSeconds(HoldNextFade);
                while (PanelImage.color.a > 0)
                {
                    ChangePanelColor = PanelImage.color;
                    FadeAmount = ChangePanelColor.a - (FadeSpeed * Time.deltaTime);
                    ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                    PanelImage.color = ChangePanelColor;
                    yield return null;
                }
                Player.CanMove = true;
                IsFadingToBlack = true;
                EventManager.CollectLifeDisplay();
                yield return new WaitForSeconds(1);
                EventManager.CollectLifeUpdate();
                StopCoroutine(FadeToBlack());
            }
        }
    }
}
