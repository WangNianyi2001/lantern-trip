using System;
using UnityEngine;

namespace QFramework
{
    public static class ViewExtension
    {
        public static TView Do<TView>(this TView self, Action<TView> onDo) where TView : IMGUIView
        {
            onDo(self);
            return self;
        }

        public static T Width<T>(this T view, float width) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.Width(width));
            return view;
        }

        public static T Height<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.Height(height));
            return view;
        }

        public static T MaxHeight<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.MaxHeight(height));
            return view;
        }

        public static T MinHeight<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.MinHeight(height));
            return view;
        }

        public static T ExpandHeight<T>(this T view) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.ExpandHeight(true));
            return view;
        }


        public static T TextMiddleLeft<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleLeft);
            return view;
        }

        public static T TextMiddleRight<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleRight);
            return view;
        }

        public static T TextLowerRight<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.LowerRight);
            return view;
        }

        public static T TextMiddleCenter<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleCenter);
            return view;
        }

        public static T TextLowerCenter<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.LowerCenter);
            return view;
        }

        public static T Color<T>(this T view, Color color) where T : IMGUIView
        {
            view.BackgroundColor = color;
            return view;
        }

        public static T FontColor<T>(this T view, Color color) where T : IMGUIView
        {
            view.Style.Set(style => style.normal.textColor = color);
            return view;
        }

        public static T FontBold<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.fontStyle = FontStyle.Bold);
            return view;
        }

        public static T FontNormal<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.fontStyle = FontStyle.Normal);
            return view;
        }

        public static T FontSize<T>(this T view, int fontSize) where T : IMGUIView
        {
            view.Style.Set(style => style.fontSize = fontSize);
            return view;
        }

        public static T Visible<T>(this T view, bool visible) where T : IMGUIView
        {
            view.Visible = visible;
            return view;
        }
    }
}