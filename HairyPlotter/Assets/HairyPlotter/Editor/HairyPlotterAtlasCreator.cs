/**
 * Copyright (C) 2012 Fredrik Holmstrom
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 **/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

public class HairyPlotterAtlasCreator : EditorWindow
{
    string assetName = "";
    int textureSize = 1024;
    int padding = 1;
    Texture2D[] textures;

    [MenuItem("Window/HairyPlotter Atlas Creator")]
    static void Init()
    {
        HairyPlotterAtlasCreator window = (HairyPlotterAtlasCreator)EditorWindow.GetWindow(typeof(HairyPlotterAtlasCreator));
        window.title = "Atlas Creator";
    }

    void OnGUI()
    {
        GUILayout.Label("HairyPlotter Atlas Creator", EditorStyles.boldLabel);

        if (textures == null)
            textures = new Texture2D[0];

        assetName = EditorGUILayout.TextField("Asset Name", assetName);
        textureSize = EditorGUILayout.IntField("Max Size", textureSize);
        padding = EditorGUILayout.IntField("Padding", padding);

        System.Array.Resize(ref textures, EditorGUILayout.IntField("Textures", textures.Length));

        for (int i = 0; i < textures.Length; ++i)
        {
            textures[i] = (Texture2D)EditorGUILayout.ObjectField(textures[i], typeof(Texture2D), false);
        }

        if (assetName != "" && textures.Where(x => x != null).Count() > 0)
        {
            if (GUILayout.Button("Create Atlas", EditorStyles.miniButton))
            {
                Texture2D atlas = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);
                atlas.PackTextures(textures.Where(x => x != null).ToArray(), padding, textureSize);

                char dirSep = System.IO.Path.DirectorySeparatorChar;
                string fileName = "Assets" + dirSep + assetName + ".png";

                System.IO.File.WriteAllBytes(fileName, atlas.EncodeToPNG());
                AssetDatabase.ImportAsset(fileName);
            }
        }
    }
}