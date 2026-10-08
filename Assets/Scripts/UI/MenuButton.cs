using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text _buttonText;
    private Image _image;
    private Material _buttonMat;

    private string _normalText;
    private string _hoveredText => "<" + _normalText + ">";

    private Button _button;

    [SerializeField] UnityEvent OnButtonClicked;

    private float _targetS, _actualS;
    public float FadeSpeed = 0.1f;
    private bool _dirty = false;

    void Start()
    {
        _button = GetComponent<Button>();
        _buttonText = GetComponentInChildren<TMP_Text>();
    
        _button.onClick.AddListener(HandleClick);
        _normalText = _buttonText.text;

        _image = GetComponent<Image>();
        _buttonMat = new Material(_image.material);
        _buttonMat.name = "TESTING TESTING TESTING";
        _image.material = _buttonMat;
    }

    void OnDisable()
    {
        _buttonText.text = _normalText;
        _targetS = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _buttonText.text = _hoveredText;
        _targetS = 1;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _buttonText.text = _normalText;
        _targetS = 0;
    }

    public void HandleClick()
    {
        OnButtonClicked?.Invoke();
    }

    public void Update()
    {
        if (!Mathf.Approximately(_actualS, _targetS))
        {
            _actualS = Mathf.MoveTowards(_actualS, _targetS, FadeSpeed * Time.unscaledDeltaTime);
            _dirty = true;
        }

        if (_dirty)
        {
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        _buttonMat.SetFloat("_Strength", _actualS);
    }
}
