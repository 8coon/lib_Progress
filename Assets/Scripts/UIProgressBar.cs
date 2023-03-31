using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIProgressBarAlign
{
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left,
    TopLeft
}

[System.Serializable]
public record UIProgressBarConstraints
{
    public UIProgressBarAlign align = UIProgressBarAlign.TopRight;

    public float top = 0.05f;
    public float left = 0.05f;
    public float bottom = 0.05f;
    public float right = 0.05f;

    public float width = 0.1f;
    public float height = 0.03f;
}

[System.Serializable]
public record UIProgressBarStyle
{
    public Color color = Color.cyan;
    public float borderWidth = 2;
    public float borderPadding = 2;
}

public class UIProgressBar : MonoBehaviour
{
    public float value = 0.3f;
    public UIProgressBarConstraints constraints;
    public UIProgressBarStyle style;

    private Texture2D texture;
    private GUIStyle guiStyle;
    private Color cachedColor = Color.clear;

    private void UpdateColor()
    {
        if (!cachedColor.Equals(style.color))
        {
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, style.color);
            texture.Apply();

            guiStyle = new GUIStyle();
            guiStyle.normal.background = texture;

            cachedColor = style.color;
        }
    }

    private Rect GetBorderRect()
    {
        var borderRect = new Rect(
            constraints.top * Screen.height,
            constraints.left * Screen.width,
            constraints.width * Screen.width,
            constraints.height * Screen.height
        );

        if (
            constraints.align == UIProgressBarAlign.Right ||
            constraints.align == UIProgressBarAlign.TopRight ||
            constraints.align == UIProgressBarAlign.BottomRight
        )
        {
            var padding = constraints.right * Screen.width;

            borderRect.xMin = Screen.width - padding - borderRect.width;
            borderRect.xMax = Screen.width - padding;
        }

        if (
            constraints.align == UIProgressBarAlign.Bottom ||
            constraints.align == UIProgressBarAlign.BottomLeft ||
            constraints.align == UIProgressBarAlign.BottomRight
        )
        {
            var padding = constraints.bottom * Screen.height;

            borderRect.yMin = Screen.height - padding - borderRect.height;
            borderRect.yMax = Screen.height - padding;
        }

        return borderRect;
    }

    void Start()
    {
        UpdateColor();
    }

    void OnGUI()
    {
        UpdateColor();

        var borderRect = GetBorderRect();
        var fillerRect = new Rect(borderRect);

        fillerRect.xMin += style.borderWidth + style.borderPadding;
        fillerRect.yMin += style.borderWidth + style.borderPadding;
        fillerRect.xMax -= style.borderWidth + style.borderPadding;
        fillerRect.yMax -= style.borderWidth + style.borderPadding;

        fillerRect.width *= Mathf.Clamp(value, 0, 1);

        // Drawing border

        //  top
        GUI.Box(new Rect(
            borderRect.xMin,
            borderRect.yMin,
            borderRect.width,
            style.borderWidth
        ), GUIContent.none, guiStyle);

        //  right
        GUI.Box(new Rect(
            borderRect.xMax - style.borderWidth,
            borderRect.yMin,
            style.borderWidth,
            borderRect.height
        ), GUIContent.none, guiStyle);

        //  bottom
        GUI.Box(new Rect(
            borderRect.xMin,
            borderRect.yMax - style.borderWidth,
            borderRect.width,
            style.borderWidth
        ), GUIContent.none, guiStyle);

        //  left
        GUI.Box(new Rect(
            borderRect.xMin,
            borderRect.yMin,
            style.borderWidth,
            borderRect.height
        ), GUIContent.none, guiStyle);

        // Drawing filler
        GUI.Box(fillerRect, GUIContent.none, guiStyle);
    }
}
