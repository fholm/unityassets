using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ActionBarInput : MonoBehaviour
{
    static ActionBarInput instance = null;

    public static ActionBarInput Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ActionBarInput)) as ActionBarInput;
                instance.Start();
            }

            return instance;
        }
    }

    public static bool LeftAlt_Up;
    public static bool LeftCtrl_Up;
    public static bool LeftShift_Up;
    public static bool LeftApple_Up;
    public static bool LeftWindows_Up;

    public static bool RightAlt_Up;
    public static bool RightCtrl_Up;
    public static bool RightShift_Up;
    public static bool RightApple_Up;
    public static bool RightWindows_Up;

    public static bool LeftAlt_Down;
    public static bool LeftCtrl_Down;
    public static bool LeftShift_Down;
    public static bool LeftApple_Down;
    public static bool LeftWindows_Down;

    public static bool RightAlt_Down;
    public static bool RightCtrl_Down;
    public static bool RightShift_Down;
    public static bool RightApple_Down;
    public static bool RightWindows_Down;

    Mesh mesh = null;
    Color[] colors = new Color[4];
    Vector3 pickupPosition = Vector3.zero;
    ActionBarButton pickupButton = null;
    ActionBarDescriptor pickupDescriptor = null;

    void Start()
    {
        if (mesh == null)
        {
            Mesh quad = Resources.Load("ActionBarButtonQuad") as Mesh;

            mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = quad.vertices;
            mesh.triangles = quad.triangles;
            mesh.uv = quad.uv;
            mesh.colors = colors;

            renderer.material = new Material(Resources.Load("ActionBarShaderPickup") as Shader);
            renderer.material.SetTexture("_Button", ActionBarSettings.Instance.ButtonTexture);
            renderer.enabled = false;
        }
    }

    void Update()
    {
        LeftAlt_Up = Input.GetKey(KeyCode.LeftAlt);
        LeftShift_Up = Input.GetKey(KeyCode.LeftShift);
        LeftCtrl_Up = Input.GetKey(KeyCode.LeftControl);
        LeftApple_Up = Input.GetKey(KeyCode.LeftApple);
        LeftWindows_Up = Input.GetKey(KeyCode.LeftWindows);

        RightAlt_Up = Input.GetKey(KeyCode.RightAlt);
        RightShift_Up = Input.GetKey(KeyCode.RightShift);
        RightCtrl_Up = Input.GetKey(KeyCode.RightControl);
        RightApple_Up = Input.GetKey(KeyCode.RightApple);
        RightWindows_Up = Input.GetKey(KeyCode.RightWindows);

        LeftAlt_Down = Input.GetKey(KeyCode.LeftAlt);
        LeftShift_Down = Input.GetKey(KeyCode.LeftShift);
        LeftCtrl_Down = Input.GetKey(KeyCode.LeftControl);
        LeftApple_Down = Input.GetKey(KeyCode.LeftApple);
        LeftWindows_Down = Input.GetKey(KeyCode.LeftWindows);

        RightAlt_Down = Input.GetKey(KeyCode.RightAlt);
        RightShift_Down = Input.GetKey(KeyCode.RightShift);
        RightCtrl_Down = Input.GetKey(KeyCode.RightControl);
        RightApple_Down = Input.GetKey(KeyCode.RightApple);
        RightWindows_Down = Input.GetKey(KeyCode.RightWindows);

        PickupInput();
        PositionInput();
    }

    void PositionInput()
    {
        // Mouse position
        Vector3 mpos = Input.mousePosition;

        // Follow mouse position around
        transform.position = new Vector3(
            (-Screen.width / 2) + mpos.x - (transform.localScale.x / 2),
            (-Screen.height / 2) + mpos.y + (transform.localScale.y / 2),
            0
        );
    }

    void PickupInput()
    {
        if (Input.GetMouseButtonDown(0) && pickupDescriptor == null)
        {
            // Try to find a button
            if (RaycastButton(out pickupButton))
            {
                if (!pickupButton.Empty)
                {
                    if (!pickupButton.Locked || pickupButton.CloneOnPickup)
                    {
                        pickupPosition = Input.mousePosition;
                    }
                }

                // Toggle that we're pressing this button and an overlay
                pickupButton.Pressed = true;
                pickupButton.Overlay = true;
            }
            else
            {
                ClearPickup();
            }
        }

        if (Input.GetMouseButton(0) && pickupButton != null && pickupButton.Descriptor != null)
        {
            if (Vector3.Distance(Input.mousePosition, pickupPosition) >= ActionBarSettings.Instance.ButtonPickupDistance)
            {
                // We're not pressing or need an overlay anymore
                pickupButton.Overlay = false;
                pickupButton.Pressed = false;

                // Pickup descriptor
                if (pickupButton.CloneOnPickup)
                {
                    Pickup(pickupButton.Descriptor);
                }
                else
                {
                    Pickup(pickupButton.RemoveDescriptor());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Click
            if (pickupDescriptor == null && pickupButton != null)
            {
                ActionBarButton button;

                if (RaycastButton(out button))
                {
                    if (button == pickupButton)
                    {
                        pickupButton.Press();
                    }
                }

                // Disable overlay and pressed state
                pickupButton.Pressed = false;
                pickupButton.Overlay = false;

                // Clear all pickup info
                ClearPickup();
            }

            // Picked up
            else
            {
                PlacePickedup();
            }
        }
    }

    void PlacePickedup()
    {
        ActionBarButton button;

        if (pickupDescriptor != null && RaycastButton(out button))
        {
            if (button.Locked || button.Row.Excludes(pickupDescriptor.ItemGroup) || (pickupButton != null && button.Row.Excludes(pickupButton.ItemGroup)))
            {
                // Return button
                if (pickupButton != null)
                {
                    pickupButton.SetDescriptor(pickupDescriptor);
                }

                // Clear
                ClearPickup();
            }
            else
            {
                if (button.Empty)
                {
                    // Check if we need to clone this
                    DecloneWithinGroup(pickupButton, button, pickupDescriptor);

                    // Set buttons descriptor
                    button.SetDescriptor(pickupDescriptor);

                    // Clear
                    ClearPickup();
                }
                else
                {
                    // Descriptor already on the button
                    if (ReferenceEquals(button.Descriptor, pickupDescriptor))
                    {
                        ClearPickup();
                    }

                    // Only if we're not putting a one back in the same place
                    else
                    {
                        // Check if we need to clone this
                        DecloneWithinGroup(pickupButton, button, pickupDescriptor);

                        // Check if we can stack this shit!
                        if (button.Descriptor.Stackable && pickupDescriptor.Stackable && button.Descriptor.ItemGroup == pickupDescriptor.ItemGroup && button.Descriptor.ItemType == pickupDescriptor.ItemType)
                        {
                            // Add one to stack
                            button.Descriptor.Stack += pickupDescriptor.Stack;

                            // Remove
                            ClearPickup();
                        }
                        else
                        {
                            // Switch the descriptor and pickup the old one
                            Pickup(button.SetDescriptor(pickupDescriptor));

                            // We need to clear this, since we already placed something in this button
                            // We cant revert if users places it somewhere else
                            pickupButton = null;
                        }
                    }
                }
            }
        }
        else
        {
            // Clear
            ClearPickup();
        }
    }

    bool RaycastButton(out ActionBarButton button)
    {
        Ray ray = ActionBarCamera.Instance.camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << gameObject.layer))
        {
            button = hit.transform.GetComponent<ActionBarButton>();
            return button != null;
        }

        button = null;
        return false;
    }

    void SetIcon(int atlasIndex, int iconIndex, int size)
    {
        float iconScale = 1f / ActionBarSettings.Instance.AtlasSize;
        int atlasSize = ActionBarSettings.Instance.AtlasSize;

        // Set scale
        transform.localScale = new Vector3(size, size, 0);

        // Prepare renderer and material
        renderer.enabled = true;
        renderer.material.SetTexture("_Atlas", ActionBarSettings.Instance.GetAtlasMaterial(atlasIndex).GetTexture("_Atlas"));
        renderer.material.SetFloat("_IconScale", iconScale);

        // Set icon data
        for (int i = 0; i < 4; ++i)
        {
            colors[i].a = (1f / atlasSize) * (iconIndex / atlasSize);
            colors[i].g = (1f / atlasSize) * (iconIndex % atlasSize);
        }

        mesh.colors = colors;
    }

    void ClearPickup()
    {
        //pickupIn = 0;
        pickupButton = null;
        pickupDescriptor = null;
        pickupPosition = Vector3.zero;
        renderer.enabled = false;
    }

    void DecloneWithinGroup(ActionBarButton from, ActionBarButton to, ActionBarDescriptor descriptor)
    {
        if (to.Row.RemoveCloneWithinGroup && descriptor.Buttons.Count > 0 && descriptor.ItemGroup == to.ItemGroup)
        {
            foreach (ActionBarButton button in descriptor.Buttons.ToArray())
            {
                if (button.ItemGroup == to.ItemGroup)
                {
                    button.RemoveDescriptor();
                }
            }
        }
    }

    public void Pickup(ActionBarDescriptor descriptor)
    {
        pickupDescriptor = descriptor;
        SetIcon(descriptor.Atlas, descriptor.Icon, 64);
    }

    public static bool CheckModifierKeys_Up(ActionBarModifierKeys mod)
    {
        return
            ((mod & ActionBarModifierKeys.LeftAlt) == ActionBarModifierKeys.LeftAlt) == LeftAlt_Up &&
            ((mod & ActionBarModifierKeys.LeftShift) == ActionBarModifierKeys.LeftShift) == LeftShift_Up &&
            ((mod & ActionBarModifierKeys.LeftCtrl) == ActionBarModifierKeys.LeftCtrl) == LeftCtrl_Up &&
            ((mod & ActionBarModifierKeys.LeftApple) == ActionBarModifierKeys.LeftApple) == LeftApple_Up &&
            ((mod & ActionBarModifierKeys.LeftWindows) == ActionBarModifierKeys.LeftWindows) == LeftWindows_Up &
            ((mod & ActionBarModifierKeys.RightAlt) == ActionBarModifierKeys.RightAlt) == RightAlt_Up &&
            ((mod & ActionBarModifierKeys.RightShift) == ActionBarModifierKeys.RightShift) == RightShift_Up &&
            ((mod & ActionBarModifierKeys.RightCtrl) == ActionBarModifierKeys.RightCtrl) == RightCtrl_Up &&
            ((mod & ActionBarModifierKeys.RightApple) == ActionBarModifierKeys.RightApple) == RightApple_Up &&
            ((mod & ActionBarModifierKeys.RightWindows) == ActionBarModifierKeys.RightWindows) == RightWindows_Up;
    }

    public static bool CheckModifierKeys_Down(ActionBarModifierKeys mod)
    {
        return
            ((mod & ActionBarModifierKeys.LeftAlt) == ActionBarModifierKeys.LeftAlt) == LeftAlt_Down &&
            ((mod & ActionBarModifierKeys.LeftShift) == ActionBarModifierKeys.LeftShift) == LeftShift_Down &&
            ((mod & ActionBarModifierKeys.LeftCtrl) == ActionBarModifierKeys.LeftCtrl) == LeftCtrl_Down &&
            ((mod & ActionBarModifierKeys.LeftApple) == ActionBarModifierKeys.LeftApple) == LeftApple_Down &&
            ((mod & ActionBarModifierKeys.LeftWindows) == ActionBarModifierKeys.LeftWindows) == LeftWindows_Down &
            ((mod & ActionBarModifierKeys.RightAlt) == ActionBarModifierKeys.RightAlt) == RightAlt_Down &&
            ((mod & ActionBarModifierKeys.RightShift) == ActionBarModifierKeys.RightShift) == RightShift_Down &&
            ((mod & ActionBarModifierKeys.RightCtrl) == ActionBarModifierKeys.RightCtrl) == RightCtrl_Down &&
            ((mod & ActionBarModifierKeys.RightApple) == ActionBarModifierKeys.RightApple) == RightApple_Down &&
            ((mod & ActionBarModifierKeys.RightWindows) == ActionBarModifierKeys.RightWindows) == RightWindows_Down;
    }
}