using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text _buttonText;

    private string _normalText;
    private string _hoveredText => "<" + _normalText + ">";

    private Button _button;

    [SerializeField] UnityEvent OnButtonClicked;

    void Start()
    {
        _button = GetComponent<Button>();
        _buttonText = GetComponentInChildren<TMP_Text>();
    
        _button.onClick.AddListener(HandleClick);
        _normalText = _buttonText.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _buttonText.text = _hoveredText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _buttonText.text = _normalText;
    }

    public void HandleClick()
    {
        OnButtonClicked?.Invoke();
    }
}
