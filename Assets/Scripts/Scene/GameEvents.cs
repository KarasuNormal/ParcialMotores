using System;

// El "tablero de anuncios" del juego.
// Los scripts del juego avisan aca que paso algo (victoria, derrota,
// que el jugador puede interactuar), y los que quieran enterarse
// (la UI, el timer, las voces) se suscriben.
public static class GameEvents
{
    public static event Action OnVictory;
    public static event Action OnDefeat;

    // Cartel de INTERACTUAR [E]:
    // se muestra cuando el jugador entra a una zona interactuable
    // y se oculta cuando sale o cuando ya interactuo.
    public static event Action OnInteractPromptShow;
    public static event Action OnInteractPromptHide;

    // Generadores y puerta final:
    // (generadores activados, generadores necesarios)
    public static event Action<int, int> OnGeneratorActivated;
    public static event Action OnDoorOpened;

    // En C#, un evento solo se puede disparar desde la clase donde se declara.
    // Por eso los demas scripts usan estos metodos para avisar.
    public static void RaiseVictory()
    {
        OnVictory?.Invoke();
    }

    public static void RaiseDefeat()
    {
        OnDefeat?.Invoke();
    }

    public static void RaiseInteractPromptShow()
    {
        OnInteractPromptShow?.Invoke();
    }

    public static void RaiseInteractPromptHide()
    {
        OnInteractPromptHide?.Invoke();
    }

    public static void RaiseGeneratorActivated(int activated, int required)
    {
        OnGeneratorActivated?.Invoke(activated, required);
    }

    public static void RaiseDoorOpened()
    {
        OnDoorOpened?.Invoke();
    }
}
