using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class MaskGenerator : EditorWindow
{
    // Input textures
    private Texture2D _redChannel;
    private Texture2D _greenChannel;
    private Texture2D _blueChannel;
    private Texture2D _alphaChannel;
    private Texture2D _normalMap;
    private Texture2D _rawColorMap;
    private Texture2D _curvatureMap;
    private bool _normalMapInvert;
    private bool _heightMapInvert;

    private readonly List<Texture2D> _textures = new();

    private float _scale = 0.5f;
    private float _normalStrength = 1;

    private Vector2 scrollPosition;

    // Output texture
    private Texture2D _combinedTexture;
    private string _selectedPath;

    [MenuItem("Tools/Nature Manufacture/Mask Generator")]
    public static void ShowWindow()
    {
        GetWindow<MaskGenerator>("Mask Generator");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("Combine Textures into Channels", EditorStyles.boldLabel);
        GUILayout.Label("Select the folder containing the textures you want to combine.");
        GUILayout.Label($"Currently selected folder: {_selectedPath}");

        _redChannel = (Texture2D)EditorGUILayout.ObjectField("MT Channel", _redChannel, typeof(Texture2D), false);
        _greenChannel = (Texture2D)EditorGUILayout.ObjectField("AO Channel", _greenChannel, typeof(Texture2D), false);
        _blueChannel = (Texture2D)EditorGUILayout.ObjectField("H Channel", _blueChannel, typeof(Texture2D), false);
        _alphaChannel = (Texture2D)EditorGUILayout.ObjectField("SM Channel", _alphaChannel, typeof(Texture2D), false);
        _heightMapInvert = EditorGUILayout.Toggle("Invert Height Map", _heightMapInvert);
        _normalMap = (Texture2D)EditorGUILayout.ObjectField("Normal Map", _normalMap, typeof(Texture2D), false);
        _normalMapInvert = EditorGUILayout.Toggle("Invert Normal Map", _normalMapInvert);
        _rawColorMap = (Texture2D)EditorGUILayout.ObjectField("Raw Color Map", _rawColorMap, typeof(Texture2D), false);
        _curvatureMap = (Texture2D)EditorGUILayout.ObjectField("Curvature Map", _curvatureMap, typeof(Texture2D), false);


        if (GUILayout.Button("Get Textures in Selected Folder"))
        {
            ClearAllMaps();
            GetTexturesInSelectedFolder();
        }

        if (GUILayout.Button("Process Textures"))
        {
            ProcessTextures();
        }

        _scale = EditorGUILayout.Slider("Scale", _scale, 0, 10);
        _normalStrength = EditorGUILayout.Slider("Normal Strength", _normalStrength, 0, 10);

        if (GUILayout.Button("Generate Curvature Map"))
        {
            GenerateCurvatureMap();
        }


        if (GUILayout.Button("Generate Shape Texture"))
        {
            GenerateShapeTexture(_curvatureMap, _greenChannel, _blueChannel);
        }

        EditorGUILayout.EndScrollView();
    }

    private void GetTexturesInSelectedFolder()
    {
        _textures.Clear();

        // Get the selected folder path
        _selectedPath = "Assets";
        Object[] selectedObjects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        if (selectedObjects.Length > 0)
        {
            _selectedPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
            // Ensure the selected path is a folder if not find folder of the selected object
            if (!AssetDatabase.IsValidFolder(_selectedPath) && File.Exists(_selectedPath))
            {
                _selectedPath = Path.GetDirectoryName(_selectedPath);
            }
        }


        // Find all Texture2D assets in the selected folder
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { _selectedPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture == null) continue;

            _textures.Add(texture);
            switch (texture.name)
            {
                case "metallicMap":
                    _redChannel = texture;
                    break;
                case "ambientOcclusionMap":
                    _greenChannel = texture;
                    break;
                case "heightMap":
                    _blueChannel = texture;
                    break;
                case "smoothnessMap":
                    _alphaChannel = texture;
                    break;
                case "normalMap":
                    _normalMap = texture;
                    break;
                case "rawColorMap":
                    _rawColorMap = texture;
                    break;
                case "curvatureMap":
                    _curvatureMap = texture;
                    break;
            }
        }

        Repaint();
    }

    private void ClearAllMaps()
    {
        _redChannel = null;
        _greenChannel = null;
        _blueChannel = null;
        _alphaChannel = null;
        _normalMap = null;
        _rawColorMap = null;
        _curvatureMap = null;
    }

    private void GenerateShapeTexture(Texture2D curvatureTexture, Texture2D aoTexture, Texture2D heightTexture)
    {
        int width = curvatureTexture.width;
        int height = curvatureTexture.height;

        // Ensure all input textures have the same dimensions
        if (width != aoTexture.width || width != heightTexture.width || height != aoTexture.height || height != heightTexture.height)
        {
            Debug.LogError("Input textures must have the same dimensions.");
            return;
        }

        // Create the combined texture
        Texture2D shapeTexture = new Texture2D(width, height);

        // Combine textures into RGB channels and set alpha channel to black
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color rColor = curvatureTexture.GetPixel(x, y);
                Color gColor = aoTexture.GetPixel(x, y);
                Color bColor = heightTexture.GetPixel(x, y);

                var combinedColor = new Color(rColor.r, gColor.r, bColor.r, 0);

                shapeTexture.SetPixel(x, y, combinedColor);
            }
        }

        // Apply changes and refresh
        shapeTexture.Apply();

        SaveTexture(shapeTexture, "shapeTexture");

        Debug.Log("Shape texture generated and saved as shapeTexture.png");
    }

    private void GenerateCurvatureMap()
    {
        Shader curvatureShader = Shader.Find("NatureManufacture Shaders/Debug/Curvature2");


        Material material = new Material(curvatureShader);
        material.SetTexture("_MainTex", _normalMap);
        material.SetFloat("_Scale", _scale);
        material.SetFloat("_NormalStrength", _normalStrength);

        RenderTexture curvatureMap = new RenderTexture(_normalMap.width, _normalMap.height, 0);
        Graphics.Blit(_normalMap, curvatureMap, material);


        Texture2D curvatureTexture = RenderTextureToTexture(curvatureMap);
        curvatureMap.Release();
        DestroyImmediate(material);

        SaveTexture(curvatureTexture, "curvatureMap");
    }

    private static Texture2D RenderTextureToTexture(RenderTexture curvatureMap)
    {
        // Convert RenderTexture to Texture2D
        Texture2D outputTexture = new Texture2D(curvatureMap.width, curvatureMap.height);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = curvatureMap;

        outputTexture.ReadPixels(new Rect(0, 0, curvatureMap.width, curvatureMap.height), 0, 0);
        outputTexture.Apply();

        RenderTexture.active = previous;
        return outputTexture;
    }

    private void ProcessTextures()
    {
        if (_redChannel == null || _greenChannel == null || _blueChannel == null || _alphaChannel == null)
        {
            Debug.LogError("Please assign all three input textures.");
            return;
        }


        int width = _redChannel.width;
        int height = _redChannel.height;

        // Ensure all input textures have the same dimensions
        if (width != _greenChannel.width || width != _blueChannel.width || height != _greenChannel.height || height != _blueChannel.height
            || height != _alphaChannel.height || height != _alphaChannel.height)
        {
            Debug.LogError("Input textures must have the same dimensions.");
            return;
        }

        MakeTextureReadable(_redChannel);
        MakeTextureReadable(_greenChannel);
        MakeTextureReadable(_blueChannel, _heightMapInvert);
        MakeTextureReadable(_alphaChannel);
        MakeTextureReadable(_normalMap, _normalMapInvert, true);
        MakeTextureReadable(_rawColorMap);


        // Create the combined texture
        _combinedTexture = new Texture2D(width, height);

        // Combine textures into RGB channels
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color redColor = _redChannel.GetPixel(x, y);
                Color greenColor = _greenChannel.GetPixel(x, y);
                Color blueColor = _blueChannel.GetPixel(x, y);
                Color alphaColor = _alphaChannel.GetPixel(x, y);

                var combinedColor = new Color(redColor.r, greenColor.r, blueColor.r, alphaColor.r);

                _combinedTexture.SetPixel(x, y, combinedColor);
            }
        }

        // Apply changes and refresh
        _combinedTexture.Apply();


        SaveTexture(_combinedTexture, "mask");

        Debug.Log("Textures combined and saved as mask.png");
    }

    private void SaveTexture(Texture2D texture, string textureName)
    {
        byte[] pngBytes = texture.EncodeToPNG();
        string path = $"{_selectedPath}/{textureName}.png";

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, pngBytes);
        }


        DestroyImmediate(texture);
        AssetDatabase.Refresh();

        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null)
        {
            Debug.LogError($"Selected object is not a texture. {path}");
            return;
        }

        // Set the "Read/Write Enabled" flag to true
        textureImporter.isReadable = true;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        Resources.UnloadUnusedAssets();
        GC.Collect();
    }


    private void MakeTextureReadable(Texture2D texture, bool invert = false, bool normalMap = false)
    {
        if (texture == null)
        {
            Debug.LogError("Please assign a valid texture.");
            return;
        }

        // Get the asset path of the target texture
        string assetPath = AssetDatabase.GetAssetPath(texture);

        // Get the TextureImporter for the target texture
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (textureImporter == null)
        {
            Debug.LogError("Selected object is not a texture.");
            return;
        }

        // Set the "Read/Write Enabled" flag to true
        textureImporter.isReadable = true;
        if (invert)
        {
            textureImporter.swizzleR = TextureImporterSwizzle.OneMinusR;
            textureImporter.swizzleG = TextureImporterSwizzle.OneMinusG;
            textureImporter.swizzleB = !normalMap ? TextureImporterSwizzle.OneMinusB : TextureImporterSwizzle.B;
        }
        else
        {
            textureImporter.swizzleR = TextureImporterSwizzle.R;
            textureImporter.swizzleG = TextureImporterSwizzle.G;
            textureImporter.swizzleB = TextureImporterSwizzle.B;
        }

        // Reimport the texture to apply the changes
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        Debug.Log("Texture is now readable: " + assetPath);
    }
}