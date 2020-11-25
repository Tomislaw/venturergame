using TMPro;
using UnityEngine;

public class InteractionContentHud : MonoBehaviour
{
    public TMP_Text Text;
    public TMP_Text UseCharacter;

    public string LabelText { get => Text.text; set => Text.text = value; }
    public string CharText { get => UseCharacter.text; set => UseCharacter.text = value; }
}