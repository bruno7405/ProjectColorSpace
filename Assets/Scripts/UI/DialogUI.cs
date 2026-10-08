using TMPro;
using UnityEngine;

public class DialogUI : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] TextMeshProUGUI dialogTMP;

    public void ClearDialog()
    {
        parent.SetActive(false);
        dialogTMP.text = "";
    }

    public void SetDialog(string text)
    {
        parent.SetActive(true);
        dialogTMP.text = text;
    }

}
