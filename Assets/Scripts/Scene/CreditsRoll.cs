using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{
    [SerializeField] private RectTransform roll;
    // La columna con todos los creditos (anclada arriba, al centro)
    [SerializeField] private float speed = 90f;
    // Velocidad de subida, en pixeles por segundo
    [SerializeField] private float startDelay = 0.8f;
    [SerializeField] private float endDelay = 2.5f;
    // Cuanto espera al terminar antes de volver al menu
    [SerializeField] private string menuScene = "MainMenu";
    [SerializeField] private CanvasGroup fade;
    // Opcional: una imagen negra a pantalla completa para el fundido de entrada y salida

    private bool leaving;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pedimos a Unity que calcule ya el alto de la columna
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(roll);

        float screenHeight = ((RectTransform)roll.parent).rect.height;
        float rollHeight = roll.rect.height;

        // Arranca justo debajo de la pantalla
        roll.anchoredPosition = new Vector2(roll.anchoredPosition.x, -screenHeight);

        if (fade != null)
        {
            fade.alpha = 1f;
            fade.blocksRaycasts = false;
            fade.DOFade(0f, 1f).SetId(this);
        }

        // Sube hasta que la ultima linea sale por arriba
        float distance = screenHeight + rollHeight;
        roll.DOAnchorPosY(rollHeight, distance / speed)
            .SetEase(Ease.Linear)
            .SetDelay(startDelay)
            .SetId(this)
            .OnComplete(() => DOVirtual.DelayedCall(endDelay, BackToMenu).SetId(this));
    }

    // Se conecta al OnClick del boton "Volver al menu"
    public void BackToMenu()
    {
        if (leaving) return;
        leaving = true;
        DOTween.Kill(this);

        if (fade != null)
        {
            fade.DOFade(1f, 0.6f).OnComplete(() => SceneManager.LoadScene(menuScene));
        }
        else
        {
            SceneManager.LoadScene(menuScene);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }
}
