using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text CrateCount;
    public Text GameoverText;
    public Image RetryArrow;
    public Text RetryText;
    public Image QuitArrow;
    public Text QuitText;
    public PlayerScript Player;
    public GameObject BreakableCrateContainer;
    public GameObject SpawnedItemsContainer;
    public GameObject FadeOutPanel;

    private List<ICrateBase> TotalBrokenCrates = new List<ICrateBase>();
    private List<ICrateBase> CurrentlyBrokenCrates = new List<ICrateBase>();
    private Vector2 HorizontalNavigate;
    private Vector2 VerticalNavigate;
    private float HoldNextFade = 1.5f;
    private float FadeSpeed = 1;
    private bool IsFadingToBlack = true;
    private bool FirstTime = true;

    public static class Booleans
    {
        public static bool GameOver { get; set; }
        public static bool IsResetGame { get; set; }

        public static bool CanMove { get; set; }
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        StartCoroutine(FadeEffect());
    }

    public void UpdateItemContainerList(GameObject Item)
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().Items.Add(Item);
    }

    public void UpdateCrateCount(ICrateBase Crate)
    {
        CurrentlyBrokenCrates.Add(Crate);
        CrateCount.text = CurrentlyBrokenCrates.Count + "/" + TotalBrokenCrates.Count;
    }

    public void ClearItemsContainer()
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().DestroyItems();
    }

    private void GetTotalCrateCount()
    {
        ICrateBase[] TempCrates = BreakableCrateContainer.GetComponentsInChildren<ICrateBase>();
        foreach (ICrateBase Crate in TempCrates)
        {
            TotalBrokenCrates.Add(Crate);
        }
        CrateCount.text = CurrentlyBrokenCrates.Count + "/" + TotalBrokenCrates.Count;
    }

    private void ResetTillCheckpoint()
    {
        Animator PlayerAnim = Player.GetComponent<Animator>();
        Player.GetComponent<CharacterSkinController>().ReturnToNormalEvent();
        PlayerAnim.ResetTrigger("IsDead");
        PlayerAnim.SetTrigger("IsDead");
        Player.ResetCheckpointPosition();
    }

    private void ResetToStartLevel()
    {
        Animator PlayerAnim = Player.GetComponent<Animator>();
        Player.GetComponent<CharacterSkinController>().ReturnToNormalEvent();
        PlayerAnim.Play("Movement");
        Player.ResetGameoverPosition();
    }

    public void PlayerDied()
    {
        StartCoroutine(FadeEffect());
    }

    public void ResetGame()
    {
        StartCoroutine(FadeEffect());
    }

    IEnumerator FadeEffect()
    {
        Image PanelImage = FadeOutPanel.GetComponent<Image>();
        Color ChangePanelColor = PanelImage.color;
        Color ChangeArrowColor = RetryArrow.color;
        Color ChangeOptionTextColor = RetryText.color;
        Color ChangeGameoverTextColor = GameoverText.color;

        float FadeAmount = 1;

        if (IsFadingToBlack)
        {
            if (FirstTime)
            {
                ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                PanelImage.color = ChangePanelColor;
                IsFadingToBlack = false;
                yield return new WaitForSeconds(HoldNextFade);
                GetTotalCrateCount();
                StartCoroutine(FadeEffect());
            }
            while (PanelImage.color.a < 1)
            {
                FadeAmount = ChangePanelColor.a + (FadeSpeed * Time.deltaTime);
                ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                PanelImage.color = ChangePanelColor;
                yield return null;
            }
            IsFadingToBlack = false;
            StartCoroutine(FadeEffect());
        }
        else
        {
            if (Booleans.GameOver)
            {
                while(GameoverText.color.a < 1)
                {
                    FadeAmount = ChangeGameoverTextColor.a + (FadeSpeed * Time.deltaTime);
                    ChangeGameoverTextColor = new Color(ChangeGameoverTextColor.r, ChangeGameoverTextColor.g, ChangeGameoverTextColor.b, FadeAmount);
                    GameoverText.color = ChangeGameoverTextColor;
                    yield return null;
                }
                yield return new WaitForSeconds(HoldNextFade);
                while(RetryArrow.color.a < 1)
                {
                    FadeAmount = ChangeArrowColor.a + (FadeSpeed * Time.deltaTime);
                    ChangeArrowColor = new Color(ChangeArrowColor.r, ChangeArrowColor.g, ChangeArrowColor.b, FadeAmount);
                    ChangeOptionTextColor = new Color(ChangeOptionTextColor.r, ChangeOptionTextColor.g, ChangeOptionTextColor.b, FadeAmount);
                    RetryArrow.color = ChangeArrowColor;
                    RetryText.color = ChangeOptionTextColor;
                    QuitArrow.color = ChangeArrowColor;
                    QuitText.color = ChangeOptionTextColor;
                    yield return null;
                }
                yield return new WaitForSeconds(HoldNextFade);
                Player.PlayerInput.SwitchCurrentActionMap("UI");
                Booleans.GameOver = false;
                StopAllCoroutines();
            }
            else if (Booleans.IsResetGame)
            {
                Booleans.IsResetGame = false;
                ResetToStartLevel();
                while (GameoverText.color.a > 0)
                {
                    FadeAmount = ChangeGameoverTextColor.a - (FadeSpeed * Time.deltaTime);
                    ChangeGameoverTextColor = new Color(ChangeGameoverTextColor.r, ChangeGameoverTextColor.g, ChangeGameoverTextColor.b, FadeAmount);
                    ChangeArrowColor = new Color(ChangeArrowColor.r, ChangeArrowColor.g, ChangeArrowColor.b, FadeAmount);
                    ChangeOptionTextColor = new Color(ChangeOptionTextColor.r, ChangeOptionTextColor.g, ChangeOptionTextColor.b, FadeAmount);
                    GameoverText.color = ChangeGameoverTextColor;
                    RetryArrow.color = ChangeArrowColor;
                    RetryText.color = ChangeOptionTextColor;
                    QuitArrow.color = ChangeArrowColor;
                    QuitText.color = ChangeOptionTextColor;
                    yield return null;
                }
                yield return new WaitForSeconds(HoldNextFade);
                while (PanelImage.color.a > 0)
                {
                    ChangePanelColor = PanelImage.color;
                    FadeAmount = ChangePanelColor.a - (FadeSpeed * Time.deltaTime);
                    ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                    PanelImage.color = ChangePanelColor;
                    yield return null;
                }
                yield return new WaitForSeconds(HoldNextFade);
                Player.PlayerInput.SwitchCurrentActionMap("CharacterControls");
                EventManager.EnablePlayerMovement();
            }
            else
            {
                if (!FirstTime)
                {
                    ClearItemsContainer();
                    ResetTillCheckpoint();
                }
                yield return new WaitForSeconds(HoldNextFade);
                while (PanelImage.color.a > 0)
                {
                    ChangePanelColor = PanelImage.color;
                    FadeAmount = ChangePanelColor.a - (FadeSpeed * Time.deltaTime);

                    ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                    PanelImage.color = ChangePanelColor;
                    yield return null;
                }
                FirstTime = false;
                IsFadingToBlack = true;
                EventManager.EnablePlayerMovement();
                StopAllCoroutines();
            }
        }
    }
    public void OnNavigateHorizontal(InputAction.CallbackContext Context)
    {
        HorizontalNavigate = Context.ReadValue<Vector2>();
    }

    public void OnNavigateVertical(InputAction.CallbackContext Context)
    {
        VerticalNavigate = Context.ReadValue<Vector2>();
        if (RetryArrow.enabled)
        {
            RetryArrow.enabled = false;
            QuitArrow.enabled = true;
        }
        else if (QuitArrow.enabled)
        {
            QuitArrow.enabled = false;
            RetryArrow.enabled = true;
        }
    }

    public void OnConfirm(InputAction.CallbackContext Context)
    {
        if (RetryArrow.enabled)
        {
            EventManager.ResetGameOver();
        }
        else if (QuitArrow.enabled)
        {
            Debug.Log("Quit game");
        }
    }
}