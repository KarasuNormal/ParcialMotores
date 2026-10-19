using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// El boton "encendido" es el seleccionado: solo uno a la vez.
// Pasar el mouse por un boton lo selecciona, y el anterior se apaga solo.
// Encendido: el texto se pone ambar, se corre un poco y aparece la franja de peligro
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private RectTransform tick;
    [SerializeField] private bool isPrimary;
    // Tildado en JUGAR: es el que arranca seleccionado

    [SerializeField] private Color normalColor = new Color32(0xCF, 0xC6, 0xB8, 0xFF);
    [SerializeField] private Color hoverColor = new Color32(0xF5, 0xA3, 0x00, 0xFF);
    [SerializeField] private float tickWidth = 21f;
    [SerializeField] private float slideDistance = 9f;
    [SerializeField] private float duration = 0.15f;

    private float labelBaseX;

    private void Awake()
    {
        labelBaseX = label.rectTransform.anchoredPosition.x;
        SetVisual(false, true);
    }

    private void Start()
    {
        if (isPrimary && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            // Al pasar el mouse, este boton pasa a ser el seleccionado
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // No hacemos nada: queda encendido hasta que se seleccione otro
    }

    public void OnSelect(BaseEventData eventData) { SetVisual(true, false); }
    public void OnDeselect(BaseEventData eventData) { SetVisual(false, false); }

    private void SetVisual(bool highlighted, bool instant)
    {
        float time = instant ? 0f : duration;
        DOTween.Kill(this);

        label.DOColor(highlighted ? hoverColor : normalColor, time).SetUpdate(true).SetId(this);
        label.rectTransform
            .DOAnchorPosX(labelBaseX + (highlighted ? slideDistance : 0f), time)
            .SetUpdate(true).SetId(this);

        if (tick != null)
        {
            tick.DOSizeDelta(new Vector2(highlighted ? tickWidth : 0f, tick.sizeDelta.y), time)
                .SetUpdate(true).SetId(this);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }
}