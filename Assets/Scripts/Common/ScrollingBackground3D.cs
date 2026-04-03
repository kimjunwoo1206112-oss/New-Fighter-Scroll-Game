using UnityEngine;

public class ScrollingBackground3D : MonoBehaviour
{
    [SerializeField] private float scrollSpeedX = 0f;
    [SerializeField] private float scrollSpeedY = 0.1f;

    private Renderer meshRenderer;
    private Vector2 offset;
    private MaterialPropertyBlock materialPropertyBlock;
    private static readonly int OffsetID = Shader.PropertyToID("_Offset");

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        meshRenderer.sortingOrder = -100;
    }

    private void Update()
    {
        if (meshRenderer != null)
        {
            offset.x += scrollSpeedX * Time.deltaTime;
            offset.y += scrollSpeedY * Time.deltaTime;
            materialPropertyBlock.SetVector(OffsetID, offset);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
