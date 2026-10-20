using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenUI : MonoBehaviour
{
    [System.Serializable]
    private class EndScreen
    {
        public CanvasGroup panel;
        // El panel entero (necesita un componente Canvas Group)
        public RectTransform background;
        // Opcional: la imagen de fondo, para el zoom
        public RectTransform title;
        // Opcional: el titulo ("HAS MUERTO" / "LO LOGRASTE")
        public CanvasGroup[] details;
        // Opcional: subtitulo, consejo, boton... en el orden en que aparecen
    }

    [SerializeField] private EndScreen victory;
    [SerializeField] private EndScreen defeat;
    [SerializeField] private Image defeatFlash;
    // Opcional: una imagen roja a pantalla completa para el "golpe" de la derrota
    [SerializeField] private CanvasGroup[] hudElements;
    // Lo que se esconde al terminar: el tutorial y el cronometro

    [Header("Despues de ganar")]
    [SerializeField] private string creditsScene = "Credits";
    [SerializeField] private float victoryToCreditsDelay = 4f;
    // Segundos que se ve "Lo lograste" antes de pasar a los creditos

    private bool shown;
    private Vector2 victoryTitleBase;

    private void Awake()
    {
        Hide(victory);
        Hide(defeat);

        if (victory.title != null)
        {
            victoryTitleBase = victory.title.anchoredPosition;
        }

        if (defeatFlash != null)
        {
            defeatFlash.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnVictory += ShowVictory;
        GameEvents.OnDefeat += ShowDefeat;
        // Nos suscribimos a los avisos de fin de juego
    }

    private void OnDisable()
    {
        GameEvents.OnVictory -= ShowVictory;
        GameEvents.OnDefeat -= ShowDefeat;
        DOTween.Kill(this);
    }

    private void Hide(EndScreen screen)
    {
        if (screen.panel == null) return;

        screen.panel.alpha = 0f;
        screen.panel.interactable = false;
        screen.panel.blocksRaycasts = false;
        screen.panel.gameObject.SetActive(false);
    }

    private void HideHud()
    {
        foreach (CanvasGroup hud in hudElements)
        {
            hud.blocksRaycasts = false;
            hud.DOFade(0f, 0.25f).SetUpdate(true).SetId(this);
        }
    }

    private void Open(EndScreen screen)
    {
        screen.panel.gameObject.SetActive(true);
        screen.panel.transform.SetAsLastSibling();
        // Lo mandamos al final del Canvas, asi se dibuja arriba de todo

        foreach (CanvasGroup detail in screen.details)
        {
            detail.alpha = 0f;
        }
    }

    private void AppendDetails(Sequence sequence, EndScreen screen)
    {
        // Los detalles aparecen uno por uno
        foreach (CanvasGroup detail in screen.details)
        {
            sequence.Append(detail.DOFade(1f, 0.25f));
        }

        sequence.AppendCallback(() =>
        {
            screen.panel.interactable = true;
            screen.panel.blocksRaycasts = true;
            // Recien ahora se pueden apretar los botones
        });
    }

    private void ShowDefeat()
    {
        if (shown) return;
        shown = true;

        HideHud();
        Open(defeat);

        Sequence sequence = DOTween.Sequence().SetUpdate(true).SetId(this);
        // SetUpdate(true): la animacion corre aunque el juego este pausado

        // 1. Destello rojo, como un impacto
        if (defeatFlash != null)
        {
            defeatFlash.gameObject.SetActive(true);
            Color flash = defeatFlash.color;
            flash.a = 0f;
            defeatFlash.color = flash;
            sequence.Append(defeatFlash.DOFade(0.85f, 0.08f));
        }

        // 2. El panel aparece mientras el destello se va
        sequence.Append(defeat.panel.DOFade(1f, 0.3f));
        if (defeatFlash != null)
        {
            sequence.Join(defeatFlash.DOFade(0f, 0.5f));
        }

        // 3. El titulo cae de golpe y tiembla
        if (defeat.title != null)
        {
            defeat.title.localScale = Vector3.one * 1.6f;
            sequence.Append(defeat.title.DOScale(1f, 0.25f).SetEase(Ease.InQuad));
            sequence.Append(defeat.title.DOShakeAnchorPos(0.3f, 10f, 25));
        }

        // 4. Subtitulo, consejo y boton, de a uno
        AppendDetails(sequence, defeat);
    }

    private void ShowVictory()
    {
        if (shown) return;
        shown = true;

        HideHud();
        Open(victory);

        Sequence sequence = DOTween.Sequence().SetUpdate(true).SetId(this);

        // 1. Fundido suave y lento
        sequence.Append(victory.panel.DOFade(1f, 0.8f));

        // 2. El fondo se aleja un poco, como una camara que se abre
        if (victory.background != null)
        {
            victory.background.localScale = Vector3.one * 1.12f;
            sequence.Join(victory.background.DOScale(1f, 1.4f).SetEase(Ease.OutCubic));
        }

        // 3. El titulo sube con calma
        if (victory.title != null)
        {
            victory.title.anchoredPosition = victoryTitleBase + Vector2.down * 40f;
            sequence.Insert(0.4f, victory.title.DOAnchorPos(victoryTitleBase, 0.7f).SetEase(Ease.OutCubic));
        }

        // 4. Subtitulo y boton, de a uno
        AppendDetails(sequence, victory);

        // 5. Despues de unos segundos, pasamos a los creditos
        sequence.AppendInterval(victoryToCreditsDelay);
        sequence.AppendCallback(() => SceneManager.LoadScene(creditsScene));
    }
}