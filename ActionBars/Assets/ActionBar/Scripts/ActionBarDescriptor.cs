using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarDescriptor
{
    int stack = 0;
    bool disabled = false;
    float cooldown = 0f;
    float cooldownStart = 0f;

    public int Atlas = 0;
    public int Icon = 0;
    public int ItemGroup = 0;
    public int ItemType = 0;
    public bool Stackable = false;
    public bool DisableOnZeroStacks = true;
    public Action<ActionBarDescriptor> Callback = null;
    public HashSet<ActionBarButton> Buttons = new HashSet<ActionBarButton>();
    public UnityEngine.AudioClip PressAudioClip = null;

    public int Stack
    {
        get { return stack; }
        set
        {
            value = UnityEngine.Mathf.Clamp(value, 0, int.MaxValue);

            if (Stackable && stack != value)
            {
                stack = value;

                foreach (ActionBarButton button in Buttons)
                {
                    button.Stack = stack;
                }

                if (stack == 0 && !disabled)
                {
                    Disabled = true;
                }
                
                if(stack > 0 && disabled)
                {
                    Disabled = false;
                }
            }
        }
    }

    public bool Disabled
    {
        get { return disabled; }
        set
        {
            disabled = value;

            foreach (ActionBarButton button in Buttons)
            {
                button.SetGrayscale(disabled ? 1f : 0f);
            }
        }
    }

    public float Cooldown
    {
        get { return cooldown; }
        set
        {
            cooldown = value;
            cooldownStart = UnityEngine.Time.time;

            foreach (ActionBarButton button in Buttons)
            {
                button.SetCooldown(cooldownStart, cooldown);
            }
        }
    }

    public float CooldownStart
    {
        get { return cooldownStart; }
    }

    public float CooldownRemaining
    {
        get { return cooldown - (UnityEngine.Time.time - cooldownStart); }
    }

    public bool OnCooldown
    {
        get { return (UnityEngine.Time.time - cooldownStart) < cooldown; }
    }
}