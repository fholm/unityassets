using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ActionBarRow))]
public class ActionBarRowEditor : Editor
{
    static ActionBarRow previous;
    static bool[] keysToggled;

    public override void OnInspectorGUI()
    {
        ActionBarRow row = target as ActionBarRow;

        if (row == null)
        {
            return;
        }

        var buttonSize = ActionBarUtils.GetField<int>(row, "buttonSize");
        var buttonRows = ActionBarUtils.GetField<int>(row, "buttonRows");
        var buttonColumns = ActionBarUtils.GetField<int>(row, "buttonColumns");
        var buttonRowSpacing = ActionBarUtils.GetField<int>(row, "buttonRowSpacing");
        var buttonColumnSpacing = ActionBarUtils.GetField<int>(row, "buttonColumnSpacing");
        var buttonSettings = ActionBarUtils.GetField<ActionBarButtonSettings[]>(row, "buttonSettings");
        var isLocked = ActionBarUtils.GetField<bool>(row, "isLocked");
        var cloneOnPickup = ActionBarUtils.GetField<bool>(row, "cloneOnPickup");
        var removeCloneWithinGroup = ActionBarUtils.GetField<bool>(row, "removeCloneWithinGroup");
        var itemGroup = ActionBarUtils.GetField<int>(row, "itemGroup");
        var excludeGroups = ActionBarUtils.GetField<int[]>(row, "excludeGroups");
        var labelOverrideFont = ActionBarUtils.GetField<bool>(row, "labelOverrideFont");
        var labelFontSize = ActionBarUtils.GetField<float>(row, "labelFontSize");
        var labelFontPosition = ActionBarUtils.GetField<Vector2>(row, "labelFontPosition");
        var stackOverrideFont = ActionBarUtils.GetField<bool>(row, "stackOverrideFont");
        var stackFontSize = ActionBarUtils.GetField<float>(row, "stackFontSize");
        var stackFontPosition = ActionBarUtils.GetField<Vector2>(row, "stackFontPosition");
        var anchor = ActionBarUtils.GetField<bool>(row, "anchor");
        var anchorPoint = ActionBarUtils.GetField<ActionBarRowAnchorPoint>(row, "anchorPoint");
        var anchorOffset = ActionBarUtils.GetField<Vector2>(row, "anchorOffset");

        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Buttons");
        int buttonSettingsLength = EditorGUILayout.IntField(buttonSettings.Value.Length);
        EditorGUILayout.EndHorizontal();
        */

        buttonRows.Value = Mathf.Clamp(EditorGUILayout.IntField("Rows", buttonRows.Value), 0, 64);
        buttonColumns.Value = Mathf.Clamp(EditorGUILayout.IntField("Columns", buttonColumns.Value), 0, 64);
        int buttonSettingsLength = buttonRows.Value * buttonColumns.Value;

        // If we're playing, don't allow editing button settings length
        if (Application.isPlaying)
        {
            if (buttonSettingsLength != buttonSettings.Value.Length)
            {
                Debug.LogWarning("Can't edit button count while playing");
            }

            buttonSettingsLength = buttonSettings.Value.Length;
        }

        isLocked.Value = EditorGUILayout.Toggle("Locked", isLocked.Value);
        cloneOnPickup.Value = EditorGUILayout.Toggle("Clone on pickup", cloneOnPickup.Value);

        if (cloneOnPickup.Value)
        {
            removeCloneWithinGroup.Value = EditorGUILayout.Toggle("De-Clone Within Group", removeCloneWithinGroup.Value);
        }

        itemGroup.Value = EditorGUILayout.IntField("Item Group", itemGroup.Value);
        buttonSize.Value = EditorGUILayout.IntField("Size", buttonSize.Value);
        buttonRowSpacing.Value = EditorGUILayout.IntField("Row Spacing", buttonRowSpacing.Value);
        buttonColumnSpacing.Value = EditorGUILayout.IntField("Column Spacing", buttonColumnSpacing.Value);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Exclude Groups");
        int excludeGroupsLength = EditorGUILayout.IntField(excludeGroups.Value.Length);
        EditorGUILayout.EndHorizontal();

        if (excludeGroups.Value.Length != excludeGroupsLength)
        {
            int[] excludeArray = new int[excludeGroupsLength];
            System.Array.Copy(excludeGroups.Value, excludeArray, Mathf.Min(excludeGroups.Value.Length, excludeArray.Length));

            if (excludeArray.Length > excludeGroups.Value.Length)
            {
                for (int i = excludeGroups.Value.Length; i < excludeArray.Length; ++i)
                {
                    excludeArray[i] = -1;
                }
            }

            excludeGroups.Value = excludeArray;
        }

        for (int i = 0; i < excludeGroups.Value.Length; ++i)
        {
            excludeGroups.Value[i] = EditorGUILayout.IntField(excludeGroups.Value[i]);
        }

        // Button settings

        if (buttonSettingsLength != buttonSettings.Value.Length)
        {
            var settingsArray = buttonSettings.Value;

            // Resize array
            System.Array.Resize(ref settingsArray, buttonSettingsLength);

            // Initialize all values
            for (int i = 0; i < buttonSettingsLength; ++i)
            {
                if (settingsArray[i] == null)
                {
                    settingsArray[i] = new ActionBarButtonSettings();
                }
            }

            // Set new array
            buttonSettings.Value = settingsArray;

            // Modify toggled array
            System.Array.Resize(ref keysToggled, buttonSettingsLength);
        }

        if (row != previous)
        {
            previous = row;
            keysToggled = new bool[buttonSettingsLength];
        }

        for (int i = 0; i < buttonSettingsLength; ++i)
        {
            var keyCombo = buttonSettings.Value[i];

            if (keysToggled[i] = EditorGUILayout.Foldout(keysToggled[i], "Button " + i + " key bindings"))
            {
                //EditorGUILayout.LabelField("Button #" + i, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Primary", EditorStyles.miniBoldLabel);
                keyCombo.PrimaryKey = (KeyCode)EditorGUILayout.EnumPopup("Key", keyCombo.PrimaryKey);
                keyCombo.PrimaryModifiers = (ActionBarModifierKeys)EditorGUILayout.EnumMaskField("Modifiers", keyCombo.PrimaryModifiers);

                EditorGUILayout.LabelField("Secondary", EditorStyles.miniBoldLabel);
                keyCombo.SecondaryKey = (KeyCode)EditorGUILayout.EnumPopup("Key", keyCombo.SecondaryKey);
                keyCombo.SecondaryModifiers = (ActionBarModifierKeys)EditorGUILayout.EnumMaskField("Modifiers", keyCombo.SecondaryModifiers);
            }
        }

        // Override label font settings

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Override Label Font");
        labelOverrideFont.Value = EditorGUILayout.Toggle(labelOverrideFont.Value);
        EditorGUILayout.EndHorizontal();

        if (labelOverrideFont.Value)
        {
            labelFontSize.Value = EditorGUILayout.FloatField("Label Font Size", labelFontSize.Value);
            labelFontPosition.Value = EditorGUILayout.Vector2Field("Label Font Position", labelFontPosition.Value);
        }

        // Override stack font settings

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Override Stack Font");
        stackOverrideFont.Value = EditorGUILayout.Toggle(stackOverrideFont.Value);
        EditorGUILayout.EndHorizontal();

        if (stackOverrideFont.Value)
        {
            stackFontSize.Value = EditorGUILayout.FloatField("Stack Font Size", stackFontSize.Value);
            stackFontPosition.Value = EditorGUILayout.Vector2Field("Stack Font Position", stackFontPosition.Value);
        }

        // Anchor

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Anchor");
        anchor.Value = EditorGUILayout.Toggle(anchor.Value);
        EditorGUILayout.EndHorizontal();

        if (anchor.Value)
        {
            anchorPoint.Value = (ActionBarRowAnchorPoint)EditorGUILayout.EnumPopup("Point", anchorPoint.Value);
            anchorOffset.Value = EditorGUILayout.Vector2Field("Offset", anchorOffset.Value);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(row);
        }
    }
}
