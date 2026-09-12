using UnityEditor;
using UnityEngine;

public static class TemporaryPlatformTest
{
    [MenuItem("Tools/Prueba temporal/Inclinacion en Buildings-Cube (6)")]
    private static void Tilt() => Attach<TiltingPlatform>();

    [MenuItem("Tools/Prueba temporal/Rotura en Buildings-Cube (6)")]
    private static void Break() => Attach<BreakablePlatform>();

    private static void Attach<T>() where T : DynamicPlatform
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("Entra en Play y vuelve a elegir la prueba. No se modifica la escena guardada.");
            return;
        }
        var players = Object.FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
        CharacterController player = null;
        foreach (var candidate in players)
            if (candidate.CompareTag("Player"))
            {
                if (player != null) { Debug.LogWarning("Hay varios personajes Player; no se aplico la prueba."); return; }
                player = candidate;
            }
        if (player == null) { Debug.LogWarning("No se encontro un CharacterController con tag Player."); return; }
        GameObject target = null;
        foreach (var candidate in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            if (candidate.name != "Cube (6)" || candidate.parent == null ||
                candidate.parent.name != "Buildings" || candidate.gameObject.scene.name != "Mapa") continue;
            if (target != null) { Debug.LogWarning("Hay mas de un Buildings/Cube (6); no se aplico la prueba."); return; }
            target = candidate.gameObject;
        }
        if (target == null) { Debug.LogWarning("No se encontro Mapa: Buildings/Cube (6)."); return; }
        var existing = target.GetComponents<DynamicPlatform>();
        foreach (var platform in existing)
            if (!(platform is T))
            { Debug.LogWarning("Cube (6) ya tiene otro comportamiento; no se reemplazo."); return; }
        if (existing.Length > 1) { Debug.LogWarning("Cube (6) tiene plataformas duplicadas; no se modifico."); return; }
        if (existing.Length == 1 && !existing[0].enabled)
        { Debug.LogWarning("La plataforma esta desactivada; no se modifico."); return; }
        // Non-convex mesh colliders cannot become dynamic rigidbodies safely.
        if (typeof(T) == typeof(BreakablePlatform))
            foreach (var mesh in target.GetComponentsInChildren<MeshCollider>())
                if (!mesh.convex)
                { Debug.LogWarning("La rotura necesita colliders convexos. No se modifico el edificio."); return; }
        // Disable colliders while AddComponent creates its initially dynamic body.
        // All changes happen in Play Mode and are discarded when Play stops.
        var colliders = target.GetComponentsInChildren<Collider>();
        var enabledStates = new bool[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            enabledStates[i] = colliders[i].enabled;
            colliders[i].enabled = false;
        }
        try
        {
            var body = target.GetComponent<Rigidbody>();
            if (body == null) body = target.AddComponent<Rigidbody>();
            body.isKinematic = true;
            if (target.GetComponent<T>() == null) target.AddComponent<T>();
        }
        finally
        {
            for (int i = 0; i < colliders.Length; i++)
                if (colliders[i] != null) colliders[i].enabled = enabledStates[i];
        }
        var contact = player.GetComponent<PlatformTestContact>();
        if (contact == null) contact = player.gameObject.AddComponent<PlatformTestContact>();
        contact.Target = target.GetComponent<T>();
        Selection.activeGameObject = target;
        Debug.Log("Prueba temporal " + typeof(T).Name + " en " + target.name +
            ". Se activa al pisarlo y se revierte al detener Play.", target);
    }
}
