/*
	Copyright 2012, Jasper Flick
	http://catlikecoding.com/
	Version 1.0.1
	
	1.0.1: changed menu item placement and removed help item
	1.0.0: initial version
	
	Distance Map Generator is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.
	
	Distance Map Generator is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.
	
	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;
using UnityEditor;
using UnityEngine;

public class CCDistanceMapGeneratorWindow : EditorWindow {
	
	[MenuItem("Window/Distance Map Generator")]
	public static void OpenWindow () {
		EditorWindow.GetWindow<CCDistanceMapGeneratorWindow>(true, "Distance Map Generator");
	}
	
	private static string
		rgbModeKey = "CCDistanceMapGeneratorWindow.rgbMode",
		insideDistanceKey = "CCDistanceMapGeneratorWindow.insideDistance",
		outsideDistanceKey = "CCDistanceMapGeneratorWindow.outsideDistance",
		postProcessDistanceKey = "CCDistanceMapGeneratorWindow.postProcessDistance";
	
	private Texture2D source, destination;
	private float postProcessDistance = 0f;
	private float insideDistance = 3f, outsideDistance = 3f;
	private CCDistanceMapGenerator.RGBMode rgbMode;
	private bool allowSave;
	
	void OnEnable () {
		source = Selection.activeObject as Texture2D;
		rgbMode = (CCDistanceMapGenerator.RGBMode)EditorPrefs.GetInt(rgbModeKey);
		insideDistance = EditorPrefs.GetFloat(insideDistanceKey, 3f);
		outsideDistance = EditorPrefs.GetFloat(outsideDistanceKey, 3f);
		postProcessDistance = EditorPrefs.GetFloat(postProcessDistanceKey);
	}
	
	void OnGUI () {
		GUILayout.BeginArea(new Rect(2f, 2f, 220f, 200f));
		
		Texture2D newSource = (Texture2D)EditorGUILayout.ObjectField("Source Texture", source, typeof(Texture2D), false);
		if(newSource != source){
			source = newSource;
			DestroyImmediate(destination);
			allowSave = false;
		}
		if(source == null){
			GUILayout.EndArea();
			return;
		}
		
		CCDistanceMapGenerator.RGBMode oldMode = rgbMode;
		rgbMode = (CCDistanceMapGenerator.RGBMode)EditorGUILayout.EnumPopup("RGB Mode", rgbMode);
		if(rgbMode != oldMode){
			EditorPrefs.SetInt(rgbModeKey, (int)rgbMode);
			allowSave = false;
		}
		
		float oldValue = insideDistance;
		insideDistance = EditorGUILayout.FloatField("Inside Distance", insideDistance);
		if(insideDistance != oldValue){
			EditorPrefs.SetFloat(insideDistanceKey, insideDistance);
			allowSave = false;
		}
		oldValue = outsideDistance;
		outsideDistance = EditorGUILayout.FloatField("Outside Distance", outsideDistance);
		if(outsideDistance != oldValue){
			EditorPrefs.SetFloat(outsideDistanceKey, outsideDistance);
			allowSave = false;
		}
		oldValue = postProcessDistance;
		postProcessDistance = EditorGUILayout.FloatField("Post-process", postProcessDistance);
		if(postProcessDistance != oldValue){
			EditorPrefs.SetFloat(postProcessDistanceKey, postProcessDistance);
			allowSave = false;
		}
		
		if(GUILayout.Button("Generate")){
			if(destination == null){
				destination = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
				destination.hideFlags = HideFlags.HideAndDontSave;
			}
			CCDistanceMapGenerator.Generate(source, destination, insideDistance, outsideDistance, postProcessDistance, rgbMode);
			destination.Apply();
			allowSave = true;
		}
		
		if(allowSave && GUILayout.Button("Export PNG file")){
			string filePath = EditorUtility.SaveFilePanel(
				"Save Distance Map",
				new FileInfo(AssetDatabase.GetAssetPath(source)).DirectoryName,
				source.name + " distance map",
				"png");
			if(filePath.Length > 0){
				File.WriteAllBytes(filePath, destination.EncodeToPNG());
				AssetDatabase.Refresh();
			}
		}
		
		GUILayout.EndArea();
		
		if(destination != null){
			EditorGUI.DrawTextureAlpha(new Rect(220f, 2f, source.width, source.height), destination);
			Rect p = position;
			if(p.width < 222f + source.width){
				p.width = 222f + source.width;
			}
			if(p.height < 4f + source.height){
				p.height = 4f + source.height;
			}
			position = p;
		}
	}
}
