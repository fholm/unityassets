
@script ExecuteInEditMode()

@CustomEditor (EdgeDetectEffectNormals)

class EdgeDetectEffectNormalsEditor extends Editor 
{	
	var serObj : SerializedObject;	
		
	var mode : SerializedProperty;
	var sensitivityDepth : SerializedProperty;
	var sensitivityNormals : SerializedProperty;

	var edgesOnly : SerializedProperty;
	var edgesOnlyBgColor : SerializedProperty;	
	

	function OnEnable () {
		serObj = new SerializedObject (target);
		
		mode = serObj.FindProperty("mode");
		
		sensitivityDepth = serObj.FindProperty("sensitivityDepth");
		sensitivityNormals = serObj.FindProperty("sensitivityNormals");

		edgesOnly = serObj.FindProperty("edgesOnly");
		edgesOnlyBgColor = serObj.FindProperty("edgesOnlyBgColor");	
	}
    		
    function OnInspectorGUI ()
    {         
    	serObj.Update ();
    	
    	EditorGUILayout.PropertyField (mode, new GUIContent("Mode"));
    	
    	GUILayout.Label ("Edge sensitivity");
   		EditorGUILayout.PropertyField (sensitivityDepth, new GUIContent("Depth"));
   		EditorGUILayout.PropertyField (sensitivityNormals, new GUIContent("Normals"));
   		    		
   		EditorGUILayout.Separator ();
   		
   		GUILayout.Label ("Background options");
   		edgesOnly.floatValue = EditorGUILayout.Slider ("Edges only", edgesOnly.floatValue, 0.0, 1.0);
   		EditorGUILayout.PropertyField (edgesOnlyBgColor, new GUIContent ("Background"));    		
    	    	
    	serObj.ApplyModifiedProperties();
    }
}
