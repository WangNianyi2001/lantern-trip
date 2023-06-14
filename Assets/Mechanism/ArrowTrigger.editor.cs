#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	public partial class ArrowTrigger : MonoBehaviour {
		private Scene scene;

		protected void EditorUpdate() {
			var color = type?.mainColor ?? Color.grey;

			if(ball) {
				var renderer = ball.GetComponent<Renderer>();
				if(renderer?.sharedMaterial) {
					var guids = AssetDatabase.FindAssets("t:GameSettings");
					if(guids.Length > 0) {
						var path = AssetDatabase.GUIDToAssetPath(guids[0]);
						var settings = AssetDatabase.LoadAssetAtPath<GameSettings>(path);
						var newMat = new Material(settings.arrowTriggerMaterial);
						newMat.color = color;
						renderer.sharedMaterial = newMat;
					}
				}
			}
			if(target) {
				target.shotType = type?.type ?? Tinder.Type.Invalid;
			}
		}

		[ContextMenu("InfectOtherInstancesInScene", true)]
		protected bool CanInfectOtherInstancesInScene() {
			scene = SceneManager.GetActiveScene();
			return scene.IsValid();
		}

		[ContextMenu("InfectOtherInstancesInScene")]
		protected void InfectOtherInstancesInScene() {
			var rootObjs = scene.GetRootGameObjects();
			List<ArrowTrigger> targets = new List<ArrowTrigger>();
			foreach(var obj in rootObjs)
				targets.AddRange(obj.GetComponentsInChildren<ArrowTrigger>(true));
			targets.Remove(this);

			bool confirm = EditorUtility.DisplayDialog("提示", "你确定熬？", "确定", "算了");
			if(!confirm) {
				EditorUtility.DisplayDialog("提示", "想好了再操作，shab", "确定");
				return;
			}

			GameObject[] targetObjects = targets.Select(t => t.gameObject).ToArray();
			Undo.RecordObjects(targetObjects, "Infect");

			foreach(var target in targets)
				target.onMatchShot = onMatchShot;

			Undo.FlushUndoRecordObjects();
			Selection.objects = targetObjects;
		}
	}
}
#endif