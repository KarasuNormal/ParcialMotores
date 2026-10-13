using System;

// El "tablero de anuncios" del juego.
// Los scripts del juego avisan aca que paso algo (victoria o derrota),
// y los que quieran enterarse (la UI, el timer) se suscriben.
public static class GameEvents
{
    public static event Action OnVictory;
    public static event Action OnDefeat;

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
}
