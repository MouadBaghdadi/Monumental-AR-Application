using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeToggle : MonoBehaviour
{
    public void ToggleTheme()
    {
        ThemeManager.IsDarkMode = !ThemeManager.IsDarkMode;
    }
}
