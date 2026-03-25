using UnityEngine;

public class ScrollingBackground3D : MonoBehaviour
{
    [SerializeField] private float scrollSpeedX = 0f;
    [SerializeField] private float scrollSpeedY = 0.1f;

    private Renderer meshRenderer;
    private Vector2 offset;

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (meshRenderer != null)
        {
            offset.x += scrollSpeedX * Time.deltaTime;
            offset.y += scrollSpeedY * Time.deltaTime;
            meshRenderer.material.mainTextureOffset = offset;
        }
    }
}
