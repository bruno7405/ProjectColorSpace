using TMPro;
using UnityEngine;

public class DialogUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogTMP;

    public void SetDialog(string text)
    {
        dialogTMP.text = text;
    }

}
