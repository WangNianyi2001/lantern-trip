namespace QFramework
{
    public static class LayoutExtension
    {
        public static T AddTo<T>(this T view, IMGUILayout parent) where T : IMGUIView
        {
            parent.AddChild(view);
            return view;
        }
    }
}