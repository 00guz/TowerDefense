using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SpriteExporter : EditorWindow
{
    [MenuItem("Window/Sprite Exporter")]
    public static void ShowWindow()
    {
        GetWindow<SpriteExporter>("Sprite Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite Export Settings", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Export Selected Sprites as PNG"))
        {
            ExportSelectedSprites();
        }
        
        if (GUILayout.Button("Export All Sprites in Selected Texture"))
        {
            ExportAllSpritesInTexture();
        }
    }

    static void ExportSelectedSprites()
    {
        if (Selection.objects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please select one or more sprites in the Project window.", "OK");
            return;
        }

        string exportPath = EditorUtility.SaveFolderPanel("Select Export Folder", "", "");
        if (string.IsNullOrEmpty(exportPath)) return;

        int exportedCount = 0;
        
        foreach (Object obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                ExportSprite(sprite, exportPath);
                exportedCount++;
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Export Complete", $"Exported {exportedCount} sprites to PNG files.", "OK");
    }

    static void ExportAllSpritesInTexture()
    {
        if (Selection.activeObject is Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .ToArray();

            if (sprites.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No sprites found in the selected texture.", "OK");
                return;
            }

            string exportPath = EditorUtility.SaveFolderPanel("Select Export Folder", "", "");
            if (string.IsNullOrEmpty(exportPath)) return;

            foreach (Sprite sprite in sprites)
            {
                ExportSprite(sprite, exportPath);
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Export Complete", $"Exported {sprites.Length} sprites to PNG files.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Please select a texture in the Project window.", "OK");
        }
    }

    static void ExportSprite(Sprite sprite, string exportPath)
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;
        
        Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        
        Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        newTex.SetPixels(pixels);
        newTex.Apply();
        
        byte[] bytes = newTex.EncodeToPNG();
        string filePath = System.IO.Path.Combine(exportPath, $"{sprite.name}.png");
        File.WriteAllBytes(filePath, bytes);
        
        DestroyImmediate(newTex);
    }
}