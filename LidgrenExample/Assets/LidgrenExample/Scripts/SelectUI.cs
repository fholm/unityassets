using UnityEngine;
using System.Collections;

public class SelectUI : MonoBehaviour
{
    void OnGUI()
    {
        float halfWidth = Screen.width / 2f;
        float halfHeight = Screen.height / 2f;

        if (GUI.Button(new Rect(halfWidth - 50, halfHeight - 12, 100, 20), "Start Server"))
        {
            LidgrenServer server = (LidgrenServer)FindObjectOfType(typeof(LidgrenServer));
            server.enabled = true;
            Application.LoadLevel("Level");
        }

        if (GUI.Button(new Rect(halfWidth - 50, halfHeight + 12, 100, 20), "Start Client"))
        {
            LidgrenClient client = (LidgrenClient)FindObjectOfType(typeof(LidgrenClient));
            client.enabled = true;
            Application.LoadLevel("Level");
        }
    }

}
