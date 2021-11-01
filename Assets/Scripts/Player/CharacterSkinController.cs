using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinController : MonoBehaviour
{
    Renderer[] characterMaterials;

    public Texture2D[] SkinColorList;
    [ColorUsage(true,true)]
    public Color[] eyeColors;
    public enum EyePosition { Nomal, Happy, Angry, Dead}
    public EyePosition eyeState;

    //Player expressions
    private int Normal = 0;
    private int Happy = 1;
    private int Angry = 2;
    private int Dead = 3;

    // Start is called before the first frame update
    void Awake()
    {
        characterMaterials = GetComponentsInChildren<Renderer>();       
    }

    //AnimationEvents

    void DeathEvent()
    {
            ChangeMaterialSettings(Dead);
            ChangeEyeOffset(EyePosition.Dead);
    }

    void VictoryEvent()
    {
        ChangeMaterialSettings(Happy);
        ChangeEyeOffset(EyePosition.Happy);
        EventManager.EnablePlayerMovement();
    }

    void HappyIdleDancingEvent()
    {
        ChangeEyeOffset(EyePosition.Happy);
    }

    public void ReturnToNormalEvent()
    {
        ChangeMaterialSettings(Normal);
        ChangeEyeOffset(EyePosition.Nomal);
    }

    public void InvincibleEvent()
    {
        ChangeMaterialSettings(Happy);
    }

    void ChangeMaterialSettings(int index)
    {
        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetColor("_EmissionColor", eyeColors[index]);
            else
                characterMaterials[i].material.SetTexture("_BaseMap", SkinColorList[index]);
        }
    }

    void ChangeEyeOffset(EyePosition pos)
    {
        Vector2 offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.Nomal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.Happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.Angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.Dead:
                offset = new Vector2(0, .66f);
                break;
            default:
                break;
        }

        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetTextureOffset("_BaseMap", offset);
        }
    }
}
