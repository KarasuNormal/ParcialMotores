using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string gameScene = "Mapa";
    [SerializeField] private string creditsScene = "Credits";

    [Header("Paneles")]
    [SerializeField] private GameObject controlsPanel;
    // Opcional: el panel de Controles (se arma despues)

    private enum LogoIntro { Cinematic, Impact }

    [Header("Entrada animada")]
    [SerializeField] private LogoIntro logoIntro = LogoIntro.Cinematic;
    // Cinematic: el logo emerge lento y tiembla la ciudad. Impact: el logo cae de golpe
    [SerializeField] private RectTransform background;
    // La imagen de fondo: es la que tiembla
    [SerializeField] private RectTransform logo;
    [SerializeField] private CanvasGroup logoGroup;
    [SerializeField] private CanvasGroup[] buttons;
    // Los botones en orden (cada uno con un Canvas Group)
    [SerializeField] private CanvasGroup[] extras;
    // Opcional: lo que aparece al final (el texto del estudio, la onda sismica)

    [Header("Salir")]
    [SerializeField] private GameObject quitButton;
    // En la version web (itch.io) "Salir" no puede cerrar la pestana, asi que lo escondemos

    private void Start()
    {
        Time.timeScale = 1f;
        // Por si venimos de un juego pausado

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // En el juego el cursor queda bloqueado: en el menu lo liberamos

#if UNITY_WEBGL
        if (quitButton != null)
        {
            quitButton.SetActive(false);
        }
#endif

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        PlayIntro();
    }

    private void PlayIntro()
    {
        Sequence intro = DOTween.Sequence().SetId(this);

        float buttonsStart = 0.9f;

        if (background != null)
        {
            background.localScale = Vector3.one * 1.04f;
            // Un poquito mas grande, asi al temblar no se ven los bordes
        }

        // 1. La entrada del logo
        if (logo != null && logoGroup != null)
        {
            logoGroup.alpha = 0f;

            if (logoIntro == LogoIntro.Cinematic)
            {
                // Emerge lento de la oscuridad, y despues tiembla la ciudad
                logo.localScale = Vector3.one * 0.94f;
                intro.Append(logoGroup.DOFade(1f, 1.6f).SetEase(Ease.InOutSine));
                intro.Join(logo.DOScale(1f, 2.2f).SetEase(Ease.OutCubic));

                if (background != null)
                {
                    intro.Insert(1.2f, background.DOShakeAnchorPos(0.6f, 14f, 18));
                }

                buttonsStart = 1.5f;
            }
            else
            {
                // Cae de golpe, como un bloque de piedra, y el golpe sacude la ciudad
                logo.localScale = Vector3.one * 1.7f;
                intro.Append(logoGroup.DOFade(1f, 0.12f));
                intro.Join(logo.DOScale(1f, 0.32f).SetEase(Ease.InExpo));

                if (background != null)
                {
                    intro.Append(background.DOShakeAnchorPos(0.5f, 22f, 25));
                }

                buttonsStart = 0.9f;
            }
        }

        // 2. Los botones entran de a uno desde la izquierda
        for (int i = 0; i < buttons.Length; i++)
        {
            CanvasGroup button = buttons[i];
            RectTransform rect = (RectTransform)button.transform;
            Vector2 basePosition = rect.anchoredPosition;

            button.alpha = 0f;
            button.interactable = false;
            rect.anchoredPosition = basePosition + Vector2.left * 45f;

            float start = buttonsStart + i * 0.1f;
            intro.Insert(start, button.DOFade(1f, 0.45f));
            intro.Insert(start, rect.DOAnchorPos(basePosition, 0.45f).SetEase(Ease.OutCubic));
        }

        // 3. Al final, los detalles
        foreach (CanvasGroup extra in extras)
        {
            extra.alpha = 0f;
            intro.Insert(buttonsStart + 0.7f, extra.DOFade(1f, 0.8f));
        }

        intro.OnComplete(() =>
        {
            foreach (CanvasGroup button in buttons)
            {
                button.interactable = true;
            }
        });
    }

    // --- Lo que hace cada boton (se conectan en el OnClick de cada uno) ---

    public void Play()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void OpenControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    public void CloseControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    public void OpenCredits()
    {
        if (Application.CanStreamedLevelBeLoaded(creditsScene))
        {
            SceneManager.LoadScene(creditsScene);
        }
        else
        {
            Debug.Log("La escena de creditos todavia no existe o no esta en Build Profiles.");
        }
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit: en el editor no se cierra, pero en el juego armado si.");
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }
}