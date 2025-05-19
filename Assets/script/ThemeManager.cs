using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThemeManager
{
    private static bool isDarkMode = false;

    public static bool IsDarkMode
    {
        get { return isDarkMode; }
        set
        {
            if (isDarkMode != value)
            {
                isDarkMode = value;
                OnThemeChanged?.Invoke(isDarkMode);
            }
        }
    }

    public static event System.Action<bool> OnThemeChanged;
}
