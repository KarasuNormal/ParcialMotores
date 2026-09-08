using UnityEngine;


[RequireComponent(typeof(Rigidbody))]

public class BreakablePlatform : DynamicPlatform
{
    
        [Header("Configuración de rotura")]
        [SerializeField] private float downwardForce = 2f;
        [SerializeField] private float torqueForce = 1f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = true;
        }

        public override void Activate()
        {
            foreach (var mesh in GetComponentsInChildren<MeshCollider>())
            {
                if (mesh.enabled && mesh.gameObject.activeInHierarchy &&
                    mesh.attachedRigidbody == rb && !mesh.convex)
                {
                    Debug.LogWarning("La rotura necesita un collider convexo o colliders simples. La plataforma permanece cinematica.", this);
                    return;
                }
            }
            rb.isKinematic = false;

            rb.AddForce(Vector3.down * downwardForce, ForceMode.Impulse);

            rb.AddTorque(
                Random.insideUnitSphere * torqueForce,
                ForceMode.Impulse
            );
        }
}
