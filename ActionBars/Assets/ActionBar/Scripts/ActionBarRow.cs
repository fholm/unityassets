using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class ActionBarRow : MonoBehaviour
{
    Mesh quad = null;
    ActionBarButton[] buttons = new ActionBarButton[0];
    List<Action<ActionBarRow>> initCallbacks = new List<Action<ActionBarRow>>();

    [SerializeField]
    int buttonSize = 64;

    [SerializeField]
    int buttonRows = 1;

    [SerializeField]
    int buttonColumns = 10;

    [SerializeField]
    int buttonRowSpacing = 0;

    [SerializeField]
    int buttonColumnSpacing = 0;

    [SerializeField]
    ActionBarButtonSettings[] buttonSettings = new ActionBarButtonSettings[0];

    [SerializeField]
    bool labelOverrideFont = false;

    [SerializeField]
    float labelFontSize = 0.2f;

    [SerializeField]
    Vector2 labelFontPosition = Vector2.zero;

    [SerializeField]
    bool stackOverrideFont = false;

    [SerializeField]
    float stackFontSize = 0.2f;

    [SerializeField]
    Vector2 stackFontPosition = Vector2.zero;

    [SerializeField]
    bool anchor = true;

    [SerializeField]
    ActionBarRowAnchorPoint anchorPoint = ActionBarRowAnchorPoint.BottomCenter;

    [SerializeField]
    Vector2 anchorOffset = Vector2.zero;

    [SerializeField]
    bool isLocked = false;

    [SerializeField]
    bool cloneOnPickup = false;

    [SerializeField]
    bool removeCloneWithinGroup = false;

    [SerializeField]
    int itemGroup = 0;

    [SerializeField]
    int[] excludeGroups = new int[0];

    public Mesh Quad
    {
        get { return quad; }
    }

    public int ItemGroup
    {
        get { return itemGroup; }
    }

    public int ButtonSize
    {
        get { return buttonSize; }
    }

    public int Layer
    {
        get { return gameObject.layer; }
    }

    public float LabelFontSize
    {
        get { return labelOverrideFont ? labelFontSize : ActionBarSettings.Instance.LabelFontSize; }
    }

    public Vector3 LabelFontPosition
    {
        get { return labelOverrideFont ? labelFontPosition : ActionBarSettings.Instance.LabelFontPosition; }
    }

    public float StackFontSize
    {
        get { return stackOverrideFont ? stackFontSize : ActionBarSettings.Instance.StackFontSize; }
    }

    public Vector3 StackFontPosition
    {
        get { return stackOverrideFont ? stackFontPosition : ActionBarSettings.Instance.StackFontPosition; }
    }

    public bool IsLocked
    {
        get { return isLocked; }
        set { isLocked = value; }
    }

    public bool CloneOnPickup
    {
        get { return cloneOnPickup; }
    }

    public bool RemoveCloneWithinGroup
    {
        get { return removeCloneWithinGroup; }
    }

    public ActionBarDescriptor SetButton(int buttonIndex, ActionBarDescriptor descriptor)
    {
        if (buttonIndex >= buttons.Length)
        {
            Debug.LogError("buttonIndex out of range");
            return null;
        }

        return buttons[buttonIndex].SetDescriptor(descriptor);
    }

    public void AddInitCallback(System.Action<ActionBarRow> callback)
    {
        if (buttons.Length != buttonSettings.Length)
        {
            initCallbacks.Add(callback);
        }
        else
        {
            callback(this);
        }
    }

    public bool Excludes(int group)
    {
        return excludeGroups.Contains(group);
    }

    int Width
    {
        get
        {
            return (buttonSize * buttonColumns) + Mathf.Clamp((buttonColumnSpacing * (buttonColumns - 1)), 0, int.MaxValue);
        }
    }

    int Height
    {
        get
        {
            return (buttonSize * buttonRows) + Mathf.Clamp((buttonColumnSpacing * (buttonRows - 1)), 0, int.MaxValue);
        }
    }

    void Start()
    {
        quad = Resources.Load("ActionBarButtonQuad", typeof(Mesh)) as Mesh;

        InitButtons();
        InitButtonPositions();

        Update();

        // Call on-init callbacks
        foreach (System.Action<ActionBarRow> callback in initCallbacks)
        {
            callback(this);
        }

        initCallbacks.Clear();
    }

    void Update()
    {
        Vector2 position = Vector2.zero;

        if (anchor)
        {
            position = CalculateAnchorScreenPosition();
        }
        else
        {
            position = transform.position;
        }

        SetPosition(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

        // Check button presses

        for (int i = 0; i < Mathf.Min(buttonSettings.Length, buttons.Length); ++i)
        {
            ActionBarButton b = buttons[i];
            ActionBarButtonSettings k = buttonSettings[i];

            if (k != null && b != null)
            {
                if (Input.GetKeyDown(k.PrimaryKey) && ActionBarInput.CheckModifierKeys_Down(k.PrimaryModifiers))
                {
                    b.Overlay = true;
                }
                else if (Input.GetKeyDown(k.SecondaryKey) && ActionBarInput.CheckModifierKeys_Down(k.SecondaryModifiers))
                {
                    b.Overlay = true;
                }
                else if (Input.GetKeyUp(k.PrimaryKey) && ActionBarInput.CheckModifierKeys_Up(k.PrimaryModifiers))
                {
                    b.Press();
                }
                else if (Input.GetKeyUp(k.SecondaryKey) && ActionBarInput.CheckModifierKeys_Up(k.SecondaryModifiers))
                {
                    b.Press();
                }

                if (!b.Pressed && !Input.GetKey(k.PrimaryKey) && !Input.GetKey(k.SecondaryKey))
                {
                    b.Overlay = false;
                }
            }
        }
    }

    void SetPosition(int x, int y)
    {
        float fx = x;
        float fy = y;

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.XBOX360:
                fx += 0.5f;
                fy += 0.5f;
                break;
        }

        transform.position = new Vector3(fx, fy, 1);
    }

    void InitButtons()
    {
        ActionBarButton[] newButtons = new ActionBarButton[buttonSettings.Length];

        if (newButtons.Length > buttons.Length)
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                newButtons[i] = buttons[i];
            }

            for (int i = buttons.Length; i < newButtons.Length; ++i)
            {
                newButtons[i] = CreateButton();
            }
        }
        else
        {
            for (int i = 0; i < newButtons.Length; ++i)
            {
                newButtons[i] = buttons[i];
            }

            for (int i = newButtons.Length; i < buttons.Length; ++i)
            {
                GameObject.Destroy(buttons[i]);
            }
        }

        buttons = newButtons;

        if (ActionBarSettings.Instance.DisplayKeybindings)
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Label = buttonSettings[i].ToString();
            }
        }
    }

    void InitButtonPositions()
    {
        int i = 0;
        int xPos = 0;
        int yPos = 0;

        for (int r = 0; r < buttonRows; ++r)
        {
            xPos = 0;

            for (int c = 0; c < buttonColumns; ++c)
            {
                buttons[i].transform.localScale = new Vector3(buttonSize, buttonSize, buttonSize);
                buttons[i].transform.localPosition = new Vector3(xPos, yPos, 0);

                i++;
                xPos += buttonSize + buttonColumnSpacing;
            }

            yPos -= buttonSize + buttonRowSpacing;
        }
    }

    ActionBarButton CreateButton()
    {
        // Create our new game object
        GameObject go = new GameObject("ActionBarButton");
            
        // Add components
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.transform.parent = transform;

        // Init
        ActionBarButton button = go.AddComponent<ActionBarButton>();
        button.Init(this);

        return button;
    }

    Vector2 CalculateAnchorScreenPosition()
    {
        Vector2 position = Vector2.zero;

        switch (anchorPoint)
        {
            case ActionBarRowAnchorPoint.BottomLeft:
                position.y = -(Screen.height / 2) + Height;
                position.x = -(Screen.width / 2);
                break;

            case ActionBarRowAnchorPoint.BottomCenter:
                position.y = -(Screen.height / 2) + Height;
                position.x = -(Width / 2);
                break;

            case ActionBarRowAnchorPoint.BottomRight:
                position.y = -(Screen.height / 2) + Height;
                position.x = (Screen.width / 2) - Width;
                break;

            case ActionBarRowAnchorPoint.MiddleLeft:
                position.y = (Height / 2);
                position.x = -(Screen.width / 2);
                break;

            case ActionBarRowAnchorPoint.MiddleRight:
                position.y = (Height / 2);
                position.x = (Screen.width / 2) - Width;
                break;
            case ActionBarRowAnchorPoint.TopLeft:
                position.y = (Screen.height / 2);
                position.x = -(Screen.width / 2);
                break;

            case ActionBarRowAnchorPoint.TopCenter:
                position.y = (Screen.height / 2);
                position.x = -(Width / 2);
                break;

            case ActionBarRowAnchorPoint.TopRight:
                position.y = (Screen.height / 2);
                position.x = (Screen.width / 2) - Width;
                break;
        }

        return anchorOffset + position;
    }
}