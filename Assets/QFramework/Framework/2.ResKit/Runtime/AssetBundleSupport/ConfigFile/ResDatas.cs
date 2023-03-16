﻿/****************************************************************************
 * Copyright (c) 2018.3 ~ 2020.1 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 默认的 ResData 支持
    /// </summary>
    public class ResDatas : IResDatas
    {
        [Serializable]
        public class SerializeData
        {
            private AssetDataGroup.SerializeData[] mAssetDataGroup;

            public AssetDataGroup.SerializeData[] AssetDataGroup
            {
                get { return mAssetDataGroup; }
                set { mAssetDataGroup = value; }
            }
        }

        public virtual string FileName
        {
            get { return "asset_bindle_config.bin"; }
        }

        public IList<AssetDataGroup> AllAssetDataGroups
        {
            get { return mAllAssetDataGroup; }
        }

        protected readonly List<AssetDataGroup> mAllAssetDataGroup = new List<AssetDataGroup>();

        private AssetDataTable mAssetDataTable = null;

        public ResDatas()
        {
        }

        public void Reset()
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                mAllAssetDataGroup[i].Reset();
            }

            mAllAssetDataGroup.Clear();

            if (mAssetDataTable != null)
            {
                mAssetDataTable.Dispose();
            }

            mAssetDataTable = null;
        }

        public int AddAssetBundleName(string name, string[] depends, out AssetDataGroup group)
        {
            group = null;

            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            var key = GetKeyFromABName(name);

            if (key == null)
            {
                return -1;
            }

            group = GetAssetDataGroup(key);

            if (group == null)
            {
                group = new AssetDataGroup(key);
                Log.I("#Create Config Group:" + key);
                mAllAssetDataGroup.Add(group);
            }

            return group.AddAssetBundleName(name, depends);
        }

        public string[] GetAllDependenciesByUrl(string url)
        {
            var abName = AssetBundleSettings.AssetBundleUrl2Name(url);

            for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string[] depends;
                if (!mAllAssetDataGroup[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }

                return depends;
            }

            return null;
        }


        public AssetData GetAssetData(ResSearchKeys resSearchKeys)
        {
            if (mAssetDataTable == null)
            {
                mAssetDataTable = new AssetDataTable();

                for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
                {
                    foreach (var assetData in mAllAssetDataGroup[i].AssetDatas)
                    {
                        mAssetDataTable.Add(assetData);
                    }
                }
            }

            return mAssetDataTable.GetAssetDataByResSearchKeys(resSearchKeys);
        }

        public virtual void LoadFromFile(string path)
        {
            var binarySerializer = ResKit.Interface.GetUtility<IBinarySerializer>();
            var zipFileHelper = ResKit.Interface.GetUtility<IZipFileHelper>();

            var data = binarySerializer
                .DeserializeBinary(zipFileHelper.OpenReadStream(path));

            if (data == null)
            {
                Log.E("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            var sd = data as SerializeData;

            if (sd == null)
            {
                Log.E("Failed Load AssetDataTable:" + path);
                return;
            }

            Log.I("Load AssetConfig From File:" + path);
            SetSerizlizeData(sd);
        }


        public virtual IEnumerator LoadFromFileAsync(string path)
        {
            using (var www = new WWW(path))
            {
                yield return www;

                if (www.error != null)
                {
                    Log.E("Failed Deserialize AssetDataTable:" + path + " Error:" + www.error);
                    yield break;
                }

                var stream = new MemoryStream(www.bytes);

                var data = ResKit.Interface.GetUtility<IBinarySerializer>()
                    .DeserializeBinary(stream);

                if (data == null)
                {
                    Log.E("Failed Deserialize AssetDataTable:" + path);
                    yield break;
                }

                var sd = data as SerializeData;

                if (sd == null)
                {
                    Log.E("Failed Load AssetDataTable:" + path);
                    yield break;
                }

                Log.I("Load AssetConfig From File:" + path);
                SetSerizlizeData(sd);
            }
        }

        public virtual void Save(string outPath)
        {
            SerializeData sd = new SerializeData
            {
                AssetDataGroup = new AssetDataGroup.SerializeData[mAllAssetDataGroup.Count]
            };

            for (var i = 0; i < mAllAssetDataGroup.Count; ++i)
            {
                sd.AssetDataGroup[i] = mAllAssetDataGroup[i].GetSerializeData();
            }

            if (ResKit.Interface.GetUtility<IBinarySerializer>()
                .SerializeBinary(outPath, sd))
            {
                Log.I("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                Log.E("Failed Save AssetDataTable:" + outPath);
            }
        }

        protected void SetSerizlizeData(SerializeData data)
        {
            if (data == null || data.AssetDataGroup == null)
            {
                return;
            }

            for (int i = data.AssetDataGroup.Length - 1; i >= 0; --i)
            {
                mAllAssetDataGroup.Add(BuildAssetDataGroup(data.AssetDataGroup[i]));
            }

            if (mAssetDataTable == null)
            {
                mAssetDataTable = new AssetDataTable();

                foreach (var serializeData in data.AssetDataGroup)
                {
                    foreach (var assetData in serializeData.assetDataArray)
                    {
                        mAssetDataTable.Add(assetData);
                    }
                }
            }
        }

        private AssetDataGroup BuildAssetDataGroup(AssetDataGroup.SerializeData data)
        {
            return new AssetDataGroup(data);
        }

        private AssetDataGroup GetAssetDataGroup(string key)
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                if (mAllAssetDataGroup[i].key.Equals(key))
                {
                    return mAllAssetDataGroup[i];
                }
            }

            return null;
        }

        private static string GetKeyFromABName(string name)
        {
            int pIndex = name.IndexOf('/');

            if (pIndex < 0)
            {
                return name;
            }

            string key = name.Substring(0, pIndex);

            if (name.Contains("i18res"))
            {
                int i18Start = name.IndexOf("i18res") + 7;
                name = name.Substring(i18Start);
                pIndex = name.IndexOf('/');
                if (pIndex < 0)
                {
                    Log.W("Not Valid AB Path:" + name);
                    return null;
                }

                string language = string.Format("[{0}]", name.Substring(0, pIndex));
                key = string.Format("{0}-i18res-{1}", key, language);
            }

            return key;
        }
    }
}