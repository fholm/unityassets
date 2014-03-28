public static class ActionBarExtensions
{
    public static void Invoke(this ActionBarDescriptor descriptor)
    {
        if (descriptor == null || descriptor.Callback == null)
        {
            return;
        }

        if(descriptor.Disabled)
        {
            ActionBarSettings.Instance.PlayDisabledSound();
            return;
        }

        if(descriptor.OnCooldown)
        {
            ActionBarSettings.Instance.PlayCooldownSound();
            return;
        }

        if (descriptor.PressAudioClip != null && ActionBarSettings.Instance.ButtonAudioSource != null)
        {
            ActionBarSettings.Instance.ButtonAudioSource.PlayOneShot(descriptor.PressAudioClip);
        }
        else
        {
            ActionBarSettings.Instance.PlayPressSound();
        }

        descriptor.Callback(descriptor);
    }
}
