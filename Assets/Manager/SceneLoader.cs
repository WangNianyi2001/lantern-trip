using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

namespace LanternTrip {
	public class SceneLoader : MonoBehaviour {
		public static SceneLoader instance;

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

		IEnumerator LoadAsyncCoroutine(string levelName) {
			float phi = (Mathf.Sqrt(5) - 1) * .5f;
			yield return new WaitForSeconds(fadeTime * phi);
			realProgress = 0;
			var task = SceneManager.LoadSceneAsync(levelName);
			while(!task.isDone) {
				realProgress = task.progress;
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

		public void LoadAsync(string levelName) {
			gameObject.SetActive(true);
			realProgress = 0;
			StartCoroutine(BackgroundCoroutine());
			StartCoroutine(LoadAsyncCoroutine(levelName));
		}

		void Awake() {
			instance = this;
			gameObject.SetActive(false);
		}
	}
}