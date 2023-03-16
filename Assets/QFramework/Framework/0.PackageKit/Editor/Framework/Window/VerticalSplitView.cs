/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 切分方式
    /// </summary>
    public enum SplitType
    {
        /// <summary>
        /// 纵向
        /// </summary>
        Vertical,
        /// <summary>
        /// 横向
        /// </summary>
        Horizontal
    }
    public class VerticalSplitView
    {
        private SplitType _splitType = SplitType.Vertical;
        private float _split = 200;
        public Action<Rect> fistPan, secondPan;
        public event System.Action onBeginResize;
        public event System.Action onEndResize;

        public bool dragging
        {
            get { return _resizing; }
            private set
            {
                if (_resizing != value)
                {
                    _resizing = value;
                    if (value)
                    {
                        if (onBeginResize != null)
                        {
                            onBeginResize();
                        }
                    }
                    else
                    {
                        if (onEndResize != null)
                        {
                            onEndResize();
                        }
                    }
                }
            }
        }

        private bool _resizing;

        public void OnGUI(Rect position)
        {
            var rs = position.Split(_splitType, _split, 4);
            var mid = position.SplitRect(_splitType, _split, 4);
            if (fistPan != null)
            {
                fistPan(rs[0]);
            }

            if (secondPan != null)
            {
                secondPan(rs[1]);
            }

            GUI.Box(mid, "");
            Event e = Event.current;
            if (mid.Contains(e.mousePosition))
            {
                if (_splitType == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeVertical);
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (mid.Contains(Event.current.mousePosition))
                    {
                        dragging = true;
                    }

                    break;
                case EventType.MouseDrag:
                    if (dragging)
                    {
                        switch (_splitType)
                        {
                            case SplitType.Vertical:
                                _split += Event.current.delta.x;
                                break;
                            case SplitType.Horizontal:
                                _split += Event.current.delta.y;
                                break;
                        }

                        _split = Mathf.Clamp(_split, 100, position.width - 100);
                    }

                    break;
                case EventType.MouseUp:
                    if (dragging)
                    {
                        dragging = false;
                    }

                    break;
            }
        }
    }
}