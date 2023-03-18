/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
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

using UnityEngine;

namespace QFramework
{
    public interface IToggle : IMGUIView,IHasText<IToggle>
    {
        Property<bool> ValueProperty { get; }

        IToggle IsOn(bool isOn);
    }
    
    internal class Toggle : View,IToggle
    {
        private string mText { get; set; }

        public Toggle()
        {
            ValueProperty = new Property<bool>(false);

            Style = new GUIStyleProperty(() => GUI.skin.toggle);
        }

        public Property<bool> ValueProperty { get; private set; }
        public IToggle IsOn(bool isOn)
        {
            ValueProperty.Value = isOn;
            return this;
        }

        protected override void OnGUI()
        {
            ValueProperty.Value = GUILayout.Toggle(ValueProperty.Value, mText ?? string.Empty, Style.Value, LayoutStyles);
        }

        public IToggle Text(string text)
        {
            mText = text;
            return this;
        }
    }
}