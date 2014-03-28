using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projector))]
public class AoEMouseTarget : MonoBehaviour
{
    public Camera Camera;
    public float YOffset = 10;
    public string ButtonName = "Fire1";

    void Update()
    {
        if (Camera == null)
        {
            Camera = Camera.main;

            if (Camera == null)
            {
                Debug.LogError("AoETarget: Camera is null");
                return;
            }
        }

        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = default(RaycastHit);

        if (Physics.Raycast(ray, out hit, float.MaxValue, ~GetComponent<Projector>().ignoreLayers))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + YOffset, hit.point.z);

            if (Input.GetButtonUp(ButtonName))
            {
                gameObject.SendMessage("CastSpell", hit.point);
            }
        }
    }
}