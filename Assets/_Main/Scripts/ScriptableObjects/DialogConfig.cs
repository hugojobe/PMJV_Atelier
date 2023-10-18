using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Config File", fileName = "New Config File")]
public class DialogConfig : ScriptableObject
{
    public CharacterConfig characterConfig;

    public Color defautTextColor;
    public TMP_FontAsset defaultFont;
    public float defaultFontSize = 28;
}
