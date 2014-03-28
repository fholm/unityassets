using UnityEngine;

public class QuestGiverMapItem : MapItem
{
    protected override void drawLabel(float left, float top, float size, string label)
    {
        Rect boxR = new Rect(left + size, top + (size / 4), 155, 55);
        Rect nameR = new Rect(left + size + 3, top + (size / 4), 150, 20);
        Rect q1R = new Rect(left + size + 3, top + (size / 4) + 15, 150, 24);
        Rect q2R = new Rect(left + size + 3, top + (size / 4) + 30, 150, 24);

        GUIStyle q1Style = new GUIStyle("Label");
        q1Style.normal.textColor = Color.green;

        GUIStyle q2Style = new GUIStyle("Label");
        q2Style.normal.textColor = Color.red;

        GUI.Box(boxR, "");
        GUI.Label(nameR, label ?? name);
        GUI.Label(q1R, " - Deliver pigs in blankets", q1Style);
        GUI.Label(q2R, " - Fight dragon", q2Style);
    }
}
