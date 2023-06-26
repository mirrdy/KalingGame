using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SynchronizeMeshCollider : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;

    private void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void LateUpdate()
    {
        meshCollider.sharedMesh = skinnedMeshRenderer.sharedMesh;
    }
}