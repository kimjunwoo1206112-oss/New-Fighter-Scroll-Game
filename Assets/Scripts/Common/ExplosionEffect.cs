using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private Sprite[] explosionSprites;
    [SerializeField] private float frameRate = 30f;
    [SerializeField] private float destroyAfter = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float frameTimer;
    private int currentFrame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (explosionSprites != null && explosionSprites.Length > 0)
        {
            spriteRenderer.sprite = explosionSprites[0];
            currentFrame = 0;
            frameTimer = 0f;
        }
        
        Destroy(gameObject, destroyAfter);
    }

    private void Update()
    {
        if (explosionSprites == null || explosionSprites.Length == 0)
            return;

        frameTimer += Time.deltaTime;
        float frameInterval = 1f / frameRate;

        if (frameTimer >= frameInterval)
        {
            frameTimer -= frameInterval;
            currentFrame++;

            if (currentFrame >= explosionSprites.Length)
            {
                currentFrame = explosionSprites.Length - 1;
            }

            spriteRenderer.sprite = explosionSprites[currentFrame];
        }
    }

    public void SetSprites(Sprite[] sprites)
    {
        explosionSprites = sprites;
    }
}
