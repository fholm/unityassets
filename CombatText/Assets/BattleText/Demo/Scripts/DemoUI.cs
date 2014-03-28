using UnityEngine;
using System.Collections;

public class DemoUI : MonoBehaviour
{
    [SerializeField]
    GameObject friendlyNamePlate;

    [SerializeField]
    GameObject neutralNamePlate;

    [SerializeField]
    GameObject enemyNamePlate;

    [SerializeField]
    BattleTextSource noticeSource;

    [SerializeField]
    BattleTextSource damageSource;

    [SerializeField]
    BattleTextAnimation critAnimation;

    [SerializeField]
    BattleTextAnimation hitAnimation;

    [SerializeField]
    MonoBehaviour vikingWalker;

    [SerializeField]
    MonoBehaviour cameraRotator;

    [SerializeField]
    BattleTextSource plumeText;

    [SerializeField]
    BattleTextSource[] combatText;

    Color[] combatColors = new Color[] { Color.red, Color.green, Color.cyan };
    string[] noticeTexts = new string[] { "Finish Him!", "Execute!", "Knock Out!" };

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, Screen.height - 20));

        if (GUILayout.Button("Show Friendly Nameplate"))
        {
            friendlyNamePlate.active = true;
            neutralNamePlate.active = false;
            enemyNamePlate.active = false;
        }

        if (GUILayout.Button("Show Neutral Nameplate"))
        {
            friendlyNamePlate.active = false;
            neutralNamePlate.active = true;
            enemyNamePlate.active = false;
        }

        if (GUILayout.Button("Show Enemy Nameplate"))
        {

            friendlyNamePlate.active = false;
            neutralNamePlate.active = false;
            enemyNamePlate.active = true;
        }

        if (GUILayout.Button("Trigger Notice"))
        {
            noticeSource.DisplayText(noticeTexts[Random.Range(0, noticeTexts.Length)]);
        }

        if (GUILayout.Button("Trigger Damage"))
        {
            bool isCrit = Random.Range(0, 10) < 3;
            BattleTextAnimation animation = isCrit ? critAnimation : hitAnimation;
            int damage = isCrit ? Random.Range(1000, 2000) : Random.Range(1, 1000);
            damageSource.DisplayText(damage.ToString(), animation);
        }

        if (GUILayout.Button("Trigger Combat Text"))
        {
            int type = Random.Range(0, 3);
            BattleTextSource source = combatText[Random.Range(0, combatText.Length)];
            string text = "";

            switch (type)
            {
                case 0:
                    text = "-" + Random.Range(0, 1000);
                    break;

                case 1:
                    text = "+" + Random.Range(0, 1000);
                    break;

                case 2:
                    text = "(Shielded)";
                    break;
            }

            source.DisplayText(text, combatColors[type]);
        }

        if (GUILayout.Button("Trigger Plume"))
        {
            int type = Random.Range(0, 2);
            string text = "";

            switch (type)
            {
                case 0:
                    text = "-" + Random.Range(0, 1000);
                    break;

                case 1:
                    text = "+" + Random.Range(0, 1000);
                    break;
            }

            plumeText.DisplayText(text, combatColors[type]);
        }


        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(Screen.width - 210, 10, 200, Screen.height - 20));

        if (GUILayout.Button("Rotate Camera"))
        {
            cameraRotator.enabled = !cameraRotator.enabled;
        }

        if (GUILayout.Button("Viking Walking"))
        {
            vikingWalker.enabled = !vikingWalker.enabled;

            if (vikingWalker.enabled)
            {
                vikingWalker.animation.CrossFade("Walk");
            }
            else
            {
                vikingWalker.animation.CrossFade("Idle");
            }
        }

        if (GUILayout.Button("Nameplates Look At Camera"))
        {
            friendlyNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera = !friendlyNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera;
            neutralNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera = !neutralNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera;
            enemyNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera = !enemyNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera;


            if (!friendlyNamePlate.GetComponent<BattleTextRenderer>().LookAtMainCamera)
            {
                friendlyNamePlate.transform.rotation = Quaternion.Euler(0, -90, 0);
                neutralNamePlate.transform.rotation = Quaternion.Euler(0, -90, 0);
                enemyNamePlate.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
        }
        if(GUILayout.Button("Damage Text Follows Viking"))
        {
            damageSource.FollowTargetPosition = !damageSource.FollowTargetPosition;
        }

        GUILayout.EndArea();
    }
}