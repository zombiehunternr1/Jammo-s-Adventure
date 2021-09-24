using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Text GameoverText;
    private Image RetryArrow;
    private Text RetryText;
    private Image QuitArrow;
    private Text QuitText;

    public Vector2 HorizontalNavigate;
    public Vector2 VerticalNavigate;
    public bool Confirm;

    [HideInInspector]
    public GameObject BreakableCrateContainer;
    [HideInInspector]
    public GameObject SpawnedItemsContainer;
    private GameObject FadeOutPanel;
    private PlayerScript Player;
    private float HoldNextFade = 1.5f;
    private float FadeSpeed = 1;
    private bool IsFadingToBlack = true;
    [HideInInspector]
    public bool Gameover;

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
        if(BreakableCrateContainer == null)
        {
            BreakableCrateContainer = GameObject.Find("Breakable Crates");
        }
        if (SpawnedItemsContainer == null)
        {
            SpawnedItemsContainer = GameObject.Find("SpawnedItems");
        }
        if(FadeOutPanel == null)
        {
            FadeOutPanel = GameObject.Find("FadeOutPanel");
        }
        if(Player == null)
        {
            Player = GameObject.Find("Player").GetComponent<PlayerScript>();
        }
        if(GameoverText == null)
        {
            GameoverText = GameObject.Find("Game Over").GetComponent<Text>();
        }
        if(RetryArrow == null)
        {
            RetryArrow = GameObject.Find("Retry Arrow").GetComponent<Image>();
        }
        if(RetryText == null)
        {
            RetryText = GameObject.Find("Retry Text").GetComponent<Text>();
        }
        if(QuitArrow == null)
        {
            QuitArrow = GameObject.Find("Quit Arrow").GetComponent<Image>();
        }
        if(QuitText == null)
        {
            QuitText = GameObject.Find("Quit Text").GetComponent<Text>();
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

    private void ChangeControlMap()
    {
        Debug.Log("I want to change my input map to UI");
    }

    IEnumerator FadeToBlack()
    {
        Image PanelImage = FadeOutPanel.GetComponent<Image>();
        Color ChangePanelColor = PanelImage.color;
        Color ChangeArrowColor = RetryArrow.color;
        Color ChangeOptionTextColor = RetryText.color;
        Color ChangeGameoverTextColor = GameoverText.color;

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
            IsFadingToBlack = false;
            yield return new WaitForSeconds(HoldNextFade);
            StartCoroutine(FadeToBlack());
        }
        else
        {
            if (Gameover)
            {
                while(GameoverText.color.a < 1)
                {
                    FadeAmount = ChangeGameoverTextColor.a + (FadeSpeed * Time.deltaTime);
                    ChangeGameoverTextColor = new Color(ChangeGameoverTextColor.r, ChangeGameoverTextColor.g, ChangeGameoverTextColor.b, FadeAmount);
                    GameoverText.color = ChangeGameoverTextColor;
                    yield return null;
                }
                yield return new WaitForSeconds(2);
                while(RetryArrow.color.a < 1)
                {
                    FadeAmount = ChangeArrowColor.a + (FadeSpeed * Time.deltaTime);
                    ChangeArrowColor = new Color(ChangeArrowColor.r, ChangeArrowColor.g, ChangeArrowColor.b, FadeAmount);
                    ChangeOptionTextColor = new Color(ChangeOptionTextColor.r, ChangeOptionTextColor.g, ChangeOptionTextColor.b, FadeAmount);
                    RetryArrow.color = ChangeArrowColor;
                    RetryText.color = ChangeOptionTextColor;
                    QuitText.color = ChangeOptionTextColor;
                    yield return null;
                }
                ChangeControlMap();
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
            }
            StopCoroutine(FadeToBlack());
        }
    }

    //Player Input
    private void OnNavigateHorizontal(InputAction.CallbackContext Context)
    {
        HorizontalNavigate = Context.ReadValue<Vector2>();
        Debug.Log(HorizontalNavigate);
    }

    private void OnNavigateVertical(InputAction.CallbackContext Context)
    {
        VerticalNavigate = Context.ReadValue<Vector2>();
        Debug.Log(VerticalNavigate);
    }

    private void OnConfirm(InputAction.CallbackContext Context)
    {
        Confirm = Context.ReadValueAsButton();
        Debug.Log(Confirm);
    }
}
