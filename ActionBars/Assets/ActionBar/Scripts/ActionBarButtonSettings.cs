using UnityEngine;

[System.Serializable]
public class ActionBarButtonSettings
{
    public KeyCode PrimaryKey;
    public ActionBarModifierKeys PrimaryModifiers;

    public KeyCode SecondaryKey;
    public ActionBarModifierKeys SecondaryModifiers;

    public override string ToString()
    {
        string result = "";

        if (PrimaryKey != KeyCode.None)
        {
            result = (ToString(PrimaryModifiers) + ToString(PrimaryKey));
        }
        else if (SecondaryKey != KeyCode.None)
        {
            result = (ToString(SecondaryModifiers) + ToString(SecondaryKey));
        }

        if (result.Length > ActionBarSettings.Instance.MaxLabelCharacters)
        {
            if (PrimaryKey != KeyCode.None)
            {
                return "..." + ToString(PrimaryKey);
            }
            else if (SecondaryKey != KeyCode.None)
            {
                return "..." + ToString(SecondaryKey);
            }
        }

        return result;
    }

    string ToString(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Alpha0:
            case KeyCode.Alpha1:
            case KeyCode.Alpha2:
            case KeyCode.Alpha3:
            case KeyCode.Alpha4:
            case KeyCode.Alpha5:
            case KeyCode.Alpha6:
            case KeyCode.Alpha7:
            case KeyCode.Alpha8:
            case KeyCode.Alpha9:
                return key.ToString().Replace("Alpha", "");

            case KeyCode.LeftParen: return "(";
            case KeyCode.RightParen: return ")";

            case KeyCode.Keypad0:
            case KeyCode.Keypad1:
            case KeyCode.Keypad2:
            case KeyCode.Keypad3:
            case KeyCode.Keypad4:
            case KeyCode.Keypad5:
            case KeyCode.Keypad6:
            case KeyCode.Keypad7:
            case KeyCode.Keypad8:
            case KeyCode.Keypad9:
                return "Num" + key.ToString().Replace("Keypad", "");

            case KeyCode.KeypadMinus: return "Num-";
            case KeyCode.KeypadPlus: return "Num+";
            case KeyCode.Escape: return "Esc";
            case KeyCode.Minus: return "-";
            case KeyCode.Plus: return "+";

            default:
                return key.ToString();
        }
    }

    string ToString(ActionBarModifierKeys mod)
    {
        string result = "";

        if (((mod & ActionBarModifierKeys.LeftAlt) == ActionBarModifierKeys.LeftAlt) || ((mod & ActionBarModifierKeys.RightAlt) == ActionBarModifierKeys.RightAlt))
        {
            result += "A-";
        }

        if (((mod & ActionBarModifierKeys.LeftShift) == ActionBarModifierKeys.LeftShift) || ((mod & ActionBarModifierKeys.RightShift) == ActionBarModifierKeys.RightShift))
        {
            result += "S-";
        }

        if (((mod & ActionBarModifierKeys.LeftCtrl) == ActionBarModifierKeys.LeftCtrl) || ((mod & ActionBarModifierKeys.RightCtrl) == ActionBarModifierKeys.RightCtrl))
        {
            result += "C-";
        }

        if (((mod & ActionBarModifierKeys.LeftApple) == ActionBarModifierKeys.LeftApple) || ((mod & ActionBarModifierKeys.RightApple) == ActionBarModifierKeys.RightApple))
        {
            result += "Cm";
        }

        if (((mod & ActionBarModifierKeys.LeftWindows) == ActionBarModifierKeys.LeftWindows) || ((mod & ActionBarModifierKeys.RightWindows) == ActionBarModifierKeys.RightWindows))
        {
            result += "W-";
        }

        return result;
    }
}