using UnityEngine;
using UnityEditor;
using System.Linq;

public class ExplosionSpriteAssigner : EditorWindow
{
    [MenuItem("Tools/Assign Explosion Sprites to Enemies")]
    public static void AssignExplosionSpritesToEnemies()
    {
        string explosionFolderPath = "Assets/Graphics/Particle/Explosion_1";
        
        Sprite[] explosionSprites = LoadExplosionSprites(explosionFolderPath);
        
        if (explosionSprites == null || explosionSprites.Length == 0)
        {
            Debug.LogError("Explosion sprites not found in " + explosionFolderPath);
            return;
        }
        
        Debug.Log($"Loaded {explosionSprites.Length} explosion sprites");
        
        string enemyPrefabPath = "Assets/Prefab/Enemy";
        string[] enemyPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { enemyPrefabPath });
        
        int assignedCount = 0;
        
        foreach (string guid in enemyPrefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null) continue;
            
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                Undo.RecordObject(enemy, "Assign Explosion Sprites");
                SerializedObject serializedObject = new SerializedObject(enemy);
                SerializedProperty spritesProperty = serializedObject.FindProperty("explosionSprites");
                
                if (spritesProperty != null)
                {
                    spritesProperty.ClearArray();
                    
                    for (int i = 0; i < explosionSprites.Length; i++)
                    {
                        spritesProperty.InsertArrayElementAtIndex(i);
                        spritesProperty.GetArrayElementAtIndex(i).objectReferenceValue = explosionSprites[i];
                    }
                    
                    serializedObject.ApplyModifiedProperties();
                    assignedCount++;
                    Debug.Log($"Assigned explosion sprites to {prefab.name}");
                }
            }
        }
        
        Debug.Log($"Explosion sprites assigned to {assignedCount} enemy prefabs");
    }
    
    private static Sprite[] LoadExplosionSprites(string folderPath)
    {
        string[] spriteGUIDs = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
        
        if (spriteGUIDs.Length == 0)
        {
            Debug.LogWarning("No sprites found in " + folderPath);
            return null;
        }
        
        Sprite[] sprites = new Sprite[spriteGUIDs.Length];
        
        for (int i = 0; i < spriteGUIDs.Length; i++)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(spriteGUIDs[i]);
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }
        
        sprites = sprites.OrderBy(s => s.name).ToArray();
        
        return sprites;
    }
}
