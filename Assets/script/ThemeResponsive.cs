using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ThemeResponsive : MonoBehaviour
{
    public enum ResponsiveType { UI_Image, UI_Text, Directional_Light }
    public ResponsiveType responsiveType;

    // Optional colors for UI and light
    public Color lightModeColor = Color.white;
    public Color darkModeColor = Color.black;

    private Image uiImage;
    private Text uiText;
    private Light sceneLight;

    private void Awake()
    {
        switch (responsiveType)
        {
            case ResponsiveType.UI_Image:
                uiImage = GetComponent<Image>();
                break;
            case ResponsiveType.UI_Text:
                uiText = GetComponent<Text>();
                break;
            case ResponsiveType.Directional_Light:
                sceneLight = GetComponent<Light>();
                break;
        }
    }

    private void OnEnable()
    {
        ThemeManager.OnThemeChanged += ApplyTheme;
        ApplyTheme(ThemeManager.IsDarkMode);
    }

    private void OnDisable()
    {
        ThemeManager.OnThemeChanged -= ApplyTheme;
    }

    private void ApplyTheme(bool isDark)
    {
        Color selectedColor = isDark ? darkModeColor : lightModeColor;

        switch (responsiveType)
        {
            case ResponsiveType.UI_Image:
                if (uiImage != null)
                    uiImage.color = selectedColor;
                break;

            case ResponsiveType.UI_Text:
                if (uiText != null)
                    uiText.color = selectedColor;
                break;

            case ResponsiveType.Directional_Light:
                if (sceneLight != null && sceneLight.type == LightType.Directional)
                    sceneLight.color = selectedColor;
                break;
        }
    }
}
