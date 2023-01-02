using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class TextureUtilWindow : EditorWindow
{
    private static List<string> _texPaths = new List<string>();
    private static int _countParsed = 0;
    private static bool _go = false;

    private static int _size = 512;
    private static int _sizeIndex = 0;

    private static TextureImporterType _type = TextureImporterType.Default;
    private static int _typeIndex = 0;

    [MenuItem("SKCell/Tools/Texture Util")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(TextureUtilWindow));
        window.Show();
        
        GetTextures();
    }

    static void GetTextures()
    {
        var texGuids
            = AssetDatabase.FindAssets("t:texture2D");

        _texPaths = texGuids.Select(
            guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();
    }

    void OnGUI()
    {
        var c = _texPaths?.Count ?? 0;
        GUILayout.TextField("Texture Count: " + c);
        
        GUILayout.TextField("Parsed Count: " + _countParsed);
        if (GUILayout.Button("Reset"))
            _countParsed = 0;

        if (!_go && _texPaths != null && _texPaths.Count != 0
            && GUILayout.Button("Set All Texture Size"))
        {
            _go = true;
        }

        _sizeIndex = GUILayout.SelectionGrid(
            _sizeIndex, new[] {"256", "512", "1024"}, 3);

        switch (_sizeIndex)
        {
            case 0:
                _size = 256;
                break;
            case 1:
                _size = 512;
                break;
            case 2:
                _size = 1024;
                break;
        }

        _typeIndex = GUILayout.SelectionGrid(
            _typeIndex, new[] {"Default", "Sprite", "Normal"}, 3);

        switch (_typeIndex)
        {
            case 0:
                _type = TextureImporterType.Default;
                break;
            case 1:
                _type = TextureImporterType.Sprite;
                break;
            case 2:
                _type = TextureImporterType.NormalMap;
                break;
        }

        if (_go)
            SetTextureMaxSize(_countParsed++, _size);

        if (_countParsed >= _texPaths.Count)
            _go = false;

        if (_go)
        {
            EditorUtility.DisplayProgressBar("Progress: ", $"{_countParsed} / {_texPaths.Count}",
                (float) _countParsed / _texPaths.Count);
        }
        else
            EditorUtility.ClearProgressBar();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }


    private static void SetTextureMaxSize(int i, int size = 512)
    {
        var texPath = _texPaths[i];
        var imp = AssetImporter.GetAtPath(texPath) as TextureImporter;

        if (imp != null)
        {
            if (imp.textureType == _type)
            {
                imp.maxTextureSize = size;
                imp.streamingMipmaps = false;
                imp.SaveAndReimport();
            }

            Debug.Log($"{i} / {_texPaths.Count}");
        }
    }
}