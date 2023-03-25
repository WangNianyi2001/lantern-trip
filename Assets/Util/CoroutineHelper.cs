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

		/// <summary>��ȡ��ǰ������ʵ�������������򴴽��󷵻�</summary>
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
		/// <summary>�ݻٵ�ǰ������ʵ��������У�</summary>
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
		/// <summary>ͨ���� helper ����Э��</summary>
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
		/// <summary>��ֵ���������е�Я��</summary>
		/// <remarks>֧��ԭ��Э�̼����߳�Я�̣���ֵΪ���첽���壬�򷵻�ƽ��Э�̡�</remarks>
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