//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Soundbank that dynamically loads its sounds based on the content of a JSON file
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bird {
	public class Soundbank : MonoBehaviour {
		[System.Flags]
		public enum soundEntryTags_e {
			NONE = 0,
			RAND = 1,       // Play randomly
			SHUFFLE = 2     // Don't repeat the same sound twice in a row
		}

		public bool m_bStaticSoundbank = false; // Add this to the global list of soundbanks?
		public string m_StaticSoundbankName;
		static Dictionary<string, Soundbank> s_StaticSoundbanks = new Dictionary<string, Soundbank>();

		static public Soundbank GetStaticSoundbank(string name) {
			if (s_StaticSoundbanks.ContainsKey(name)) {
				return s_StaticSoundbanks[name];
			} else {
				return null;
			}
		}

		static public AudioClip GetAudioclip(string bank, string snd) {
			Soundbank sb = GetStaticSoundbank(bank);
			if (sb) {
				return sb.GetAudioclip(snd);
			} else {
				return null;
			}
		}

		static public void PlayOneShot(string bank, string snd) {
			Soundbank sb = GetStaticSoundbank(bank);
			if (sb) {
				sb.PlaySoundOneshot(snd);
			}
		}


		public class soundEntry_s {
			public string m_entryName;
			public List<AudioClip> m_listAudioClips;
			public int m_nLastRandomSnd = -1;
			public soundEntryTags_e m_eFlags = soundEntryTags_e.NONE;

			public AudioClip GetAudioClip() {
				if ((m_eFlags & soundEntryTags_e.RAND) == soundEntryTags_e.RAND) {
					// Random playback
					int nNextSnd = m_nLastRandomSnd;
					int nEscape = 0;
					while (nNextSnd == m_nLastRandomSnd) {
						nNextSnd = UnityEngine.Random.Range(0, m_listAudioClips.Count);
						if (nEscape++ >= 10 || !((m_eFlags & soundEntryTags_e.SHUFFLE) == soundEntryTags_e.SHUFFLE)) {
							// Just give up on trying to find a new random number
							// You've probably only got one sound anyway!
							break;
						}
					}

					m_nLastRandomSnd = nNextSnd;
					return m_listAudioClips[nNextSnd];
				} else {
					// Sequential play
					int nNextSnd = m_nLastRandomSnd + 1;
					if (nNextSnd >= m_listAudioClips.Count) {
						nNextSnd = 0;
					}

					m_nLastRandomSnd = nNextSnd;

					return m_listAudioClips[nNextSnd];
				}
			}
		}

		void OnEnable() {
			// m_AudioSource = GetComponent<AudioSource>();
			if (m_AudioSource == null) {
				m_AudioSource = gameObject.AddComponent<AudioSource>();
			}

			if (m_bStaticSoundbank) {
				// Don't overwrite preexisting banks
				if (s_StaticSoundbanks.ContainsKey(m_StaticSoundbankName)) {
					DestroyImmediate(gameObject);
					return;
				}

				s_StaticSoundbanks.Add(m_StaticSoundbankName, this);
				ObjectDB.DontDestroyOnLoad_Managed(gameObject);
			}

			if (m_bLoadFromCFG) {
				//string cfg = Settings.GetSetting("BANK_" + m_StaticSoundbankName);
				string cfg = PlayerPrefs.GetString("BANK_" + m_StaticSoundbankName, "DEFAULT");
				if (cfg != null) {
					m_Soundbank = Resources.Load<TextAsset>(cfg);
				}
			}

			if (m_Soundbank != null) {
				// We've got a predefined bank, load it here
				LoadSoundbank(m_Soundbank);
			}
		}

		Dictionary<string, soundEntry_s> m_dictSoundEntries;
		public TextAsset m_Soundbank;
		AudioSource m_AudioSource;
		public bool m_bLoadFromCFG = false;

		soundEntryTags_e CheckTags(string strEntry) {
			if (strEntry[0] == '$') { // Check for tag escape val
				Array arr = Enum.GetValues(typeof(soundEntryTags_e));
				foreach (soundEntryTags_e curFlag in arr) {
					if (strEntry == '$' + curFlag.ToString()) {
						return curFlag;
					}
				}
			}

			return soundEntryTags_e.NONE;
		}

		void PurgeCurrentSoundbank() {
			if (m_dictSoundEntries != null) {
				m_dictSoundEntries.Clear();
			}
		}

		void OnDrawGizmos() {
			Gizmos.DrawIcon(transform.position, "Soundbank.png", true);
		}

		public void PlaySoundOneshot(string sndName) {
			AudioClip newClip = GetAudioclip(sndName);
			if (newClip != null) {
				m_AudioSource.PlayOneShot(newClip);
			}
		}

		public AudioClip GetAudioclip(string sndName) {
			if (!m_dictSoundEntries.ContainsKey(sndName)) {
				Debug.Log("Soundbank::GetAudioclip - Sound with ID \"" + sndName + "\" not found!");
				return null;
			}

			return m_dictSoundEntries[sndName].GetAudioClip();
		}

		public void PlaySound(string sndName) {
			AudioClip newClip = GetAudioclip(sndName);
			if (newClip != null) {
				m_AudioSource.clip = newClip;
				m_AudioSource.Play();
			}
		}

		public void PlaySound(AudioClip snd) {
			if (snd != null) {
				m_AudioSource.clip = snd;
				m_AudioSource.Play();
			}
		}

		public void StopSound() {
			m_AudioSource.Stop();
		}

		public void LoadSoundbank(string szJSONFileName) {
			TextAsset jsonFile = Resources.Load<TextAsset>("soundbanks/" + szJSONFileName);
			if (jsonFile == null) {
				Debug.LogError("Soundbank::LoadSoundbank - Soundbank file \"" + szJSONFileName + "\" not found!");
				return;
			}
			LoadSoundbank(jsonFile);
		}

		public void LoadSoundbank(TextAsset jsonFile) {
			JSONObject jsonObj = new JSONObject(jsonFile.ToString());
			JSONObject jsonRoot = jsonObj["soundbank"];
			if (!jsonRoot) {
				Debug.LogError("Soundbank::LoadSoundbank - Soundbank file \"" + jsonFile.name + "\" is corrupt!");
				return;
			}

			PurgeCurrentSoundbank();
			m_dictSoundEntries = new Dictionary<string, soundEntry_s>();

			int nEntryCount = jsonRoot.Count;
			for (int i = 0; i < nEntryCount; i++) {
				soundEntry_s newEntry = new soundEntry_s();
				JSONObject curObj = jsonRoot[i];
				newEntry.m_listAudioClips = new List<AudioClip>();
				newEntry.m_entryName = jsonRoot.keys[i];

				int nFileCount = curObj.Count;
				for (int j = 0; j < nFileCount; j++) {

					soundEntryTags_e tag = CheckTags(curObj[j].str);
					newEntry.m_eFlags |= tag;

					if (tag == soundEntryTags_e.NONE) { // Don't try to load tags
						AudioClip newClip = Resources.Load<AudioClip>("sounds/" + curObj[j].str);
						if (!newClip) {
							Debug.LogError("Soundbank::LoadSoundbank - Could not find sound file \"" + curObj[j].str + "\"!");
							continue;
						}

						newEntry.m_listAudioClips.Add(newClip);
					}
				}

				m_dictSoundEntries.Add(newEntry.m_entryName, newEntry);
			}

			// Set current soundbank
			m_Soundbank = jsonFile;

			// Cleanup after ourselves
			GC.Collect();
		}

		public AudioSource GetAudioSource() {
			return m_AudioSource;
		}
	}
}