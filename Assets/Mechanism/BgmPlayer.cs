using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LanternTrip {
	public class BgmPlayer : MonoBehaviour {
		public AudioClip[] tracks;
		[Min(0)] public float fadeTime;
		public bool loop = true;

		List<AudioSource> currentPlaying = new List<AudioSource>();
		Dictionary<AudioSource, Coroutine> playingCoroutines = new Dictionary<AudioSource, Coroutine>();

		protected AudioSource CreateSource(AudioClip clip) {
			var obj = new GameObject($"BGM Track ({clip.name})");
			obj.transform.parent = GameplayManager.instance.camera.camera.transform;
			obj.transform.localPosition = Vector3.zero;
			var source = obj.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.clip = clip;
			source.loop = loop;
			return source;
		}

		protected IEnumerator PlayCoroutine(AudioSource source) {
			source.volume = 0;
			source.Play();
			while(source.volume < 1) {
				yield return new WaitForFixedUpdate();
				source.volume += Time.fixedDeltaTime / fadeTime;
			}
			playingCoroutines.Remove(source);
		}

		protected IEnumerator StopCoroutine(AudioSource source) {
			if(playingCoroutines.ContainsKey(source)) {
				StopCoroutine(playingCoroutines[source]);
				playingCoroutines.Remove(source);
			}
			currentPlaying.Remove(source);
			while(source.volume > 0) {
				yield return new WaitForFixedUpdate();
				source.volume -= Time.fixedDeltaTime / fadeTime;
			}
			Destroy(source.gameObject);
		}

		public void Play() {
			foreach(var track in tracks) {
				AudioSource source = CreateSource(track);
				currentPlaying.Add(source);
				playingCoroutines[source] = StartCoroutine(PlayCoroutine(source));
			}
		}
		
		public void Stop() {
			foreach(var playing in currentPlaying)
				StartCoroutine(StopCoroutine(playing));
		}
	}
}