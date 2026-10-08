using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] TextMeshProUGUI dialogTMP;

    RectTransform parentRect;
    RectTransform textRect;

    void Awake()
    {
        parentRect = parent.GetComponent<RectTransform>();
        textRect = dialogTMP.rectTransform;
    }

    public void ClearDialog()
    {
        parent.SetActive(false);
        dialogTMP.text = "";
    }

    public void SetDialog(string text)
    {
        parent.SetActive(true);
        dialogTMP.text = text;
        RefreshLayout();
    }

    void RefreshLayout()
    {
        dialogTMP.ForceMeshUpdate();

        LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }
}