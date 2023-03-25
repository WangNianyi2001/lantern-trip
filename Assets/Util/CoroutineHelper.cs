using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Threading.Tasks;

namespace LanternTrip {
	[ExecuteInEditMode]
	public class CoroutineHelper : MonoBehaviour {
		static List<IEnumerator> coroutines = new List<IEnumerator>();
		static CoroutineHelper instance = null;

		/// <summary>获取当前场景的实例，若不存在则创建后返回</summary>
		static CoroutineHelper GetInstance() {
			if(instance != null) {
				if(instance.isActiveAndEnabled)
					return instance;
				DestroyInstance();
			}
			if(SceneManager.GetActiveScene() == null)
				throw new NullReferenceException("No active scene loaded, cannot start coroutine");
			var gameObject = new GameObject("Coroutine Helper");
			return instance = gameObject.AddComponent<CoroutineHelper>();
		}
		/// <summary>摧毁当前场景的实例（如果有）</summary>
		static void DestroyInstance() {
			if(!instance)
				return;
			if(Application.isPlaying)
				Destroy(instance.gameObject);
			else
				DestroyImmediate(instance.gameObject);
			instance = null;
		}

		IEnumerator RunInternal(IEnumerator coroutine) {
			yield return StartCoroutine(coroutine);
			coroutines.Remove(coroutine);
			if(coroutines.Count == 0)
				DestroyInstance();
		}
		/// <summary>通过本 helper 运行协程</summary>
		public static Coroutine Run(IEnumerator coroutine) {
			if(coroutine == null)
				return null;
			coroutines.Add(coroutine);
			GetInstance();
			return instance.StartCoroutine(instance.RunInternal(coroutine));
		}

		static IEnumerator MakeSingle(object value) {
			yield return value;
		}
		/// <summary>从值创建可运行的携程</summary>
		/// <remarks>支持原生协程及多线程携程；若值为非异步语义，则返回平凡协程。</remarks>
		public static IEnumerator Make(object value) {
			switch(value) {
				default:
					return MakeSingle(value);
				case null:
					return null;
				case IEnumerator enumerator:
					return enumerator;
				case Task task:
					return MakeSingle(new WaitUntil(() => task.IsCompleted));
			}
		}
	}
}