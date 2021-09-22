using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
            FadeOutPanel = GameObject.Find("FadeOut");
        }
        if(Player == null)
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
        Color ChangeColor = PanelImage.color;
        float FadeAmount;

        if (IsFadingToBlack)
        {
            while (PanelImage.color.a < 1)
            {
                FadeAmount = ChangeColor.a + (FadeSpeed * Time.deltaTime);
                ChangeColor = new Color(ChangeColor.r, ChangeColor.g, ChangeColor.b, FadeAmount);
                PanelImage.color = ChangeColor;
                yield return null;
            }
            IsFadingToBlack = false;
            yield return new WaitForSeconds(HoldNextFade);
            StartCoroutine(FadeToBlack());
        }
        else
        {
            ClearItemsContainer();
            ResetPlayer();
            yield return new WaitForSeconds(HoldNextFade);
            while (PanelImage.color.a > 0)
            {
                ChangeColor = PanelImage.color;
                FadeAmount = ChangeColor.a - (FadeSpeed * Time.deltaTime);

                ChangeColor = new Color(ChangeColor.r, ChangeColor.g, ChangeColor.b, FadeAmount);
                PanelImage.color = ChangeColor;
                yield return null;
            }
            Player.CanMove = true;
            IsFadingToBlack = true;
            StopCoroutine(FadeToBlack());
        }
    }
}
