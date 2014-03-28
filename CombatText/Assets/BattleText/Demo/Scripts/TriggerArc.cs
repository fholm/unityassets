using UnityEngine;

public class TriggerArc : MonoBehaviour
{
    Color[] colors = new Color[] { Color.red, Color.green };
    BattleTextSource source;

    void Start()
    {
        source = GetComponent<BattleTextSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            source.DisplayText("-" + Random.Range(1, 1000).ToString(), colors[Random.Range(0, colors.Length)]);
        }
    }
}
