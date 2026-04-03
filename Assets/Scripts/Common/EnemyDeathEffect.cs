using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private float destroyDelay = 2f;

    private void Start()
    {
        if (deathParticle != null)
        {
            deathParticle.Play();
        }
        
        Destroy(gameObject, destroyDelay);
    }

    public static void SpawnDeathEffect(Vector3 position, Material particleMaterial = null)
    {
        GameObject effectObj = new GameObject("EnemyDeathEffect");
        effectObj.transform.position = position;
        
        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.playOnAwake = true;
        main.startLifetime = 0.5f;
        main.startSpeed = 5f;
        main.startSize = 0.5f;
        main.startColor = Color.white;
        main.maxParticles = 30;
        
        var emission = ps.emission;
        emission.rateOverTime = 60;
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;
        
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-4f, 4f);
        velocity.y = new ParticleSystem.MinMaxCurve(-4f, 4f);
        velocity.z = new ParticleSystem.MinMaxCurve(-2f, 2f);
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient colorGradient = new Gradient();
        colorGradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 0.8f, 0f), 0f),
                new GradientColorKey(new Color(1f, 0.3f, 0f), 1f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorGradient.mode = GradientMode.Blend;
        colorOverLifetime.color = colorGradient;
        
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(0.5f, 0.2f);
        
        var renderer = effectObj.AddComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 10;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        if (particleMaterial != null)
        {
            renderer.material = particleMaterial;
        }
        else
        {
            Material mat = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
            if (mat != null && mat.shader != null)
            {
                mat.color = new Color(1f, 0.8f, 0f, 1f);
                renderer.material = mat;
            }
            else
            {
                renderer.material = new Material(Shader.Find("Unlit/Color"));
            }
        }
        
        EnemyDeathEffect effect = effectObj.AddComponent<EnemyDeathEffect>();
        effect.deathParticle = ps;
    }
}
