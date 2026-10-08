using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to a button for sound effects
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonAudioPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] AudioClip hoverSFX;
    [SerializeField] AudioClip clickSFX;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (hoverSFX == null)
            hoverSFX = AudioBus.DefaultButtonHoverSFX;
        if (clickSFX == null)
            clickSFX = AudioBus.DefaultButtonClickSFX;

        Debug.Log(AudioBus.DefaultButtonClickSFX);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanPlay(hoverSFX)) return;
        AudioBus.Instance.PlayOneshotSFX(hoverSFX, 0.55f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanPlay(clickSFX)) return;
        AudioBus.Instance.PlayOneshotSFX(clickSFX, 1f);
    }

    private bool CanPlay(AudioClip clip)
    {
        return clip != null && button.interactable;
    }
}