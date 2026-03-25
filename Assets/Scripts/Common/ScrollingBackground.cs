using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeedX = 0f;
    [SerializeField] private float scrollSpeedY = 0.1f;

    private RawImage rawImage;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (rawImage != null)
        {
            float x = rawImage.uvRect.x + scrollSpeedX * Time.deltaTime;
            float y = rawImage.uvRect.y + scrollSpeedY * Time.deltaTime;
            rawImage.uvRect = new Rect(x, y, rawImage.uvRect.width, rawImage.uvRect.height);
        }
    }
}
