using UnityEngine;

// Assigned only in Play Mode by the temporary test menu.
public sealed class PlatformTestContact : MonoBehaviour
{
    public DynamicPlatform Target { get; set; }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Target == null || hit.normal.y < 0.5f) return;
        var platform = hit.collider.GetComponentInParent<DynamicPlatform>();
        if (platform == Target) Target.RequestActivation();
    }
}
