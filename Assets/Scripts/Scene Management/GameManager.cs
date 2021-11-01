using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AudioClip Ambient;
    public TextMeshPro CrateCountProText;
    public Text CrateCountText;
    public Text GameoverText;
    public Image RetryArrow;
    public Text RetryText;
    public Image QuitArrow;
    public Text QuitText;
    public PlayerScript Player;
    public BarrierManager BarrierManager;
    public GameObject AllCrateTypes;
    public GameObject SpawnedItemsContainer;
    public GameObject StaticItemsContainer;
    public GameObject FadeOutPanel;

    private List<ICrateBase> TotalBrokenCrates = new List<ICrateBase>();
    private List<ICrateBase> CurrentlyBrokenCrates = new List<ICrateBase>();
    private List<IInteractable> InteractableCrates = new List<IInteractable>();
    private Vector2 HorizontalNavigate;
    private Vector2 VerticalNavigate;
    private float HoldNextFade = 1.5f;
    private float FadeSpeed = 1;
    private bool IsFadingToBlack = true;
    private bool FirstTime = true;
    private SpawnStaticItem StaticItems;
    private AudioSource AudioSource;

    public static class Booleans
    {
        public static bool GameOver { get; set; }
        public static bool IsResetGame { get; set; }
        public static bool CanMove { get; set; }
        public static bool CameraMove { get; set; }
        public static bool Invulnerable { get; set; }
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        if(StaticItems == null)
        {
            StaticItems = StaticItemsContainer.GetComponent<SpawnStaticItem>();
        }
        if(AudioSource == null)
        {
            AudioSource = GetComponent<AudioSource>();
        }
        AudioSource.clip = Ambient;
        AudioSource.Play();
        StartCoroutine(FadeEffect());
    }

    public void UpdateItemContainerList(GameObject Item)
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().Items.Add(Item);
        Item.transform.parent = SpawnedItemsContainer.transform;
    }

    public void UpdateCrateCount(ICrateBase Crate)
    {
        CurrentlyBrokenCrates.Add(Crate);
        GetTotalCrateCount();
    }

    public void ClearItemsContainer()
    {
        SpawnedItemsContainer.GetComponent<ClearSpawnedItems>().DestroyItems();
    }

    private void GetTotalCrateCount()
    {
        ICrateBase[] TempCrates = AllCrateTypes.GetComponentsInChildren<ICrateBase>(true);
        foreach (ICrateBase Crate in TempCrates)
        {
            if (!TotalBrokenCrates.Contains(Crate))
            {
                TotalBrokenCrates.Add(Crate);
            }
            if (Crate.Checkpoint != null)
            {
                if (Crate is Checkpoint)
                {
                    if (!CurrentlyBrokenCrates.Contains(Crate))
                    {
                        if (!Crate.gameObject.activeSelf)
                        {
                            CurrentlyBrokenCrates.Add(Crate);
                        }
                    }
                }
                else
                {
                    if (!CurrentlyBrokenCrates.Contains(Crate))
                    {
                        if (!Crate.gameObject.activeSelf)
                        {
                            CurrentlyBrokenCrates.Add(Crate);
                        }
                    }
                }
            }
        }
        CrateCountText.text = CurrentlyBrokenCrates.Count + "/" + TotalBrokenCrates.Count;
        CrateCountProText.text = CurrentlyBrokenCrates.Count + " / " + TotalBrokenCrates.Count;
    }

    private void GetTotalInteractables()
    {
        IInteractable[] TempCrates = AllCrateTypes.GetComponentsInChildren<IInteractable>();
        foreach(IInteractable Crate in TempCrates)
        {
            if (!InteractableCrates.Contains(Crate))
            {
                InteractableCrates.Add(Crate);
            }
            if(Crate is NitroDetonator)
            {
                Crate.gameObject.GetComponent<NitroDetonator>().GetAllNitros();
            }
        }
    }

    private void ResetTillCheckpoint()
    {
        Player.ResetPlayerMovement();
        ClearItemsContainer();
        ResetCrates();
        if (Player.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dying"))
        {
            Player.SkinController.ReturnToNormalEvent();
        }
        Player.ResetCheckpointPosition();
    }
    private void ResetToStartLevel()
    {
        Player.ResetPlayerMovement();
        ClearItemsContainer();
        ResetCrates();
        StaticItems.SpawnBoltType();
        Player.PlayerModel.SetActive(true);
        IsFadingToBlack = true;
        Player.SkinController.ReturnToNormalEvent();
        Player.ResetGameoverPosition();
    }
    public void SetCheckpoint(Checkpoint CheckPointPos)
    {
        for (int i = 0; i < CurrentlyBrokenCrates.Count; i++)
        {
            CurrentlyBrokenCrates[i].Checkpoint = CheckPointPos;
            Player.SetCheckPoint(CheckPointPos);
        }
    }

    private void ResetCrates()
    {
        List<ICrateBase> TempList = new List<ICrateBase>();

        if (Booleans.IsResetGame)
        {
            Booleans.IsResetGame = false;
            foreach(ICrateBase Crate in TotalBrokenCrates)
            {
                Crate.ResetCrate();
            }
            foreach(IInteractable Crate in InteractableCrates)
            {
                Crate.ResetCrate();
            }
        }
        else
        {
            for (int i = 0; i < CurrentlyBrokenCrates.Count; i++)
            {
                if (CurrentlyBrokenCrates[i].Checkpoint == null)
                {
                    TempList.Add(CurrentlyBrokenCrates[i]);
                }
            }
            CurrentlyBrokenCrates = new List<ICrateBase>();
            CurrentlyBrokenCrates = TempList;

            foreach (ICrateBase Crate in CurrentlyBrokenCrates)
            {
                Crate.ResetCrate();
            }
            foreach(IInteractable Crate in InteractableCrates)
            {
                Crate.ResetCrate();
            }
        }
        CurrentlyBrokenCrates = new List<ICrateBase>();
        GetTotalCrateCount();
        if (!CheckCrateTotal())
        {
            BarrierManager.EnableLasers();
        }
    }

    public void PlayerDied()
    {
        StartCoroutine(FadeEffect());
    }

    public void ResetGame()
    {
        StartCoroutine(FadeEffect());
    }

    public bool CheckCrateTotal()
    {
        return CurrentlyBrokenCrates.Count == TotalBrokenCrates.Count ? true : false;
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
                GetTotalInteractables();
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
                Player.playerInput.SwitchCurrentActionMap("UI");
                Booleans.GameOver = false;
                StopAllCoroutines();
            }
            else if (Booleans.IsResetGame)
            {
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
                if (Player.HasExploded)
                {
                    Player.HasExploded = false;
                }
                Player.playerInput.SwitchCurrentActionMap("CharacterControls");
                EventManager.EnablePlayerMovement();
            }
            else
            {
                if (!FirstTime)
                {
                    ResetTillCheckpoint();
                }
                Booleans.CameraMove = true;
                Player.PlayerModel.SetActive(true);
                yield return new WaitForSeconds(HoldNextFade);
                while (PanelImage.color.a > 0)
                {
                    ChangePanelColor = PanelImage.color;
                    FadeAmount = ChangePanelColor.a - (FadeSpeed * Time.deltaTime);

                    ChangePanelColor = new Color(ChangePanelColor.r, ChangePanelColor.g, ChangePanelColor.b, FadeAmount);
                    PanelImage.color = ChangePanelColor;
                    yield return null;
                }
                if (Player.HasExploded)
                {
                    Player.HasExploded = false;
                }
                FirstTime = false;
                IsFadingToBlack = true;
                EventManager.EnablePlayerMovement();
                Player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                Player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                StopAllCoroutines();
            }
        }
    }

    #region Inputsystem
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
    #endregion
}
