using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

namespace LanternTrip {
	public class SceneLoader : MonoBehaviour {
		public static SceneLoader instance;

		public GameSettings settings;
		public Graphic background;
		[Range(0, 2)] public float fadeTime;
		public RectTransform progressBar;

		float realProgress;

		float Alpha {
			get => background.color.a;
			set {
				Color c = background.color;
				c.a = Mathf.Clamp01(value);
				background.color = c;
			}
		}

		public static List<Scene> LoadedScenes {
			get {
				var loadedScenes = new List<Scene>();
				for(int i = 0; i < SceneManager.sceneCount; ++i) {
					Scene scene = SceneManager.GetSceneAt(i);
					if(scene.isLoaded)
						loadedScenes.Add(scene);
				}
				return loadedScenes;
			}
		}

		IEnumerator LoadAsyncCoroutine(params string[] levelNames) {
			if(levelNames.Length <= 0) {
				realProgress = 1;
				yield break;
			}
			float phi = (Mathf.Sqrt(5) - 1) * .5f;
			yield return new WaitForSeconds(fadeTime * phi);
			realProgress = 0;

			float progress = 0;
			var tasks = levelNames.Select((name, i) => {
				var mode = i > 0 ? LoadSceneMode.Additive : LoadSceneMode.Single;
				return SceneManager.LoadSceneAsync(name, mode);
			});
			while(!tasks.All(task => !task.isDone)) {
				progress = 0;
				foreach(var task in tasks)
					progress += task.progress;
				realProgress = progress / levelNames.Length;
				yield return new WaitForEndOfFrame();
			}

			realProgress = 1;
		}

		IEnumerator BackgroundCoroutine() {
			Alpha = 0;
			for(float t = 0; ; t += Time.deltaTime) {
				Alpha = t / fadeTime;

				float fakeProgress = Mathf.SmoothStep(0, 1, Mathf.Clamp01(Alpha));
				float threshold = Mathf.Pow(realProgress, .5f) * .5f + 1;
				float visualProgress = fakeProgress * threshold;

				var fullWidth = (progressBar.parent as RectTransform).rect.width;
				var width = fullWidth * visualProgress;
				progressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

				yield return new WaitForEndOfFrame();
				if(t >= fadeTime)
					break;
			}
		}

		public void LoadAsync(params string[] levelNames) {
			gameObject.SetActive(true);
			realProgress = 0;
			StartCoroutine(BackgroundCoroutine());
			StartCoroutine(LoadAsyncCoroutine(levelNames));
		}

		public void LoadMain() => LoadAsync(settings.mainScene);
		public void LoadGameOver() => LoadAsync(settings.gameOverScene);
		
		public void ReloadCurrent() => LoadAsync(LoadedScenes.Select(s => s.name).ToArray());

		void Awake() {
			instance = this;
			gameObject.SetActive(false);
		}
	}
}