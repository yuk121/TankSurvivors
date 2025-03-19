using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

public class SoundManager : MonoBehaviour
{
    #region Simple SingleTon
    public static SoundManager Instance;
    #endregion

    [System.Serializable]
    private class SoundData
    {
        [Header("Audio Type")]
        public Define.eSoundType type;
        
        [Header("Audio Mixer")]
        public AudioMixerGroup mixerGroup;
       
        [Header("Audio Clip")]
        public List<AudioClip> clipList = new List<AudioClip>();

        [Header("Audio Source\n ---- Count 입력시 Source 자동 생성----")]
        public int sourceCount;
        public List<AudioSource> sourceList = new List<AudioSource>();
    }

    [SerializeField]
    private List<SoundData> _soundData = new List<SoundData>();

    // Start is called before the first frame update
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // 사운드 매니저 세팅
        for (int i = 0; i < _soundData.Count; i++)
        {
            int index = 0;
            for (int j = 0; j < _soundData[i].sourceCount; j++)
            {
                GameObject obj = new GameObject();
                AudioSource source = obj.AddComponent<AudioSource>();
               
                // 각 타입에 맞는 이름으로 변경
                switch (_soundData[i].type)
                {
                    case Define.eSoundType.BGM:
                        obj.name = $"{Define.eSoundType.BGM} {++index}";
                        source.loop = true;
                        break;

                    case Define.eSoundType.SUB_BGM:
                        obj.name = $"{Define.eSoundType.SUB_BGM} {++index}";
                        break;

                    case Define.eSoundType.SFX:
                        obj.name = $"{Define.eSoundType.SFX} {++index}";
                        source.playOnAwake = false;
                        break;
                }
                obj.transform.parent = transform;

                if (_soundData[i].mixerGroup != null)
                    source.outputAudioMixerGroup = _soundData[i].mixerGroup;

                _soundData[i].sourceList.Add(source);
            }
        }
    }

    public void Play(string key, Define.eSoundType type, float volume_db = 0)
    {
        AudioClip clip = LoadClips(key, type);

        if (clip == null)
            return;

        List<AudioSource> sourceList = new List<AudioSource>();

        volume_db = Mathf.Clamp(volume_db, -80f, 20f);

        for(int i =0; i < _soundData.Count; i++)
        {
            if(_soundData[i].type == type)
            {
                if (_soundData[i].mixerGroup != null)
                {
                    _soundData[i].mixerGroup.audioMixer.SetFloat($"{type}{i}", volume_db);
                }

                sourceList = _soundData[i].sourceList;
                break;
            }
        }

        if(sourceList.Count < 1)
        {
            Debug.LogError("Audio Source is missing !!!");
            return;
        }

        // 사용 가능한 오디오 소스를 찾아 재생
        foreach (var source in sourceList)
        {
            if (source.isPlaying == false)
            {
                if (type == Define.eSoundType.SFX)
                {
                    source.PlayOneShot(clip);
                }
                else
                {
                    source.clip = clip;
                    source.Play();
                }
                return;
            }
        }

        // 모든 소스가 재생 중이라면 첫 번째 소스를 강제로 사용
        if (type == Define.eSoundType.SFX)
        {
            sourceList[0].PlayOneShot(clip);
        }
        else
        {
            sourceList[0].clip = clip;
            sourceList[0].Play();
        }
    }

    public void StopAllSound()
    {
        for(int i =0; i <_soundData.Count; i++)
        {
            for(int j =0; j <_soundData[i].sourceList.Count; j++)
            {
                _soundData[i].sourceList[j].Stop();
            }
        }
    }

    public AudioClip LoadClips(string key, Define.eSoundType type)
    {
        AudioClip clip = null;

        for (int i = 0; i < _soundData.Count; i++)
        {
            // 사운드 타입 확인
            if (_soundData[i].type == type)
            {
                // 불러오고자 하는 clip이 포함되어있는지 확인
                clip = _soundData[i].clipList.Find(clip => clip.name == key);
                
                // clip이 없다면 Sound Data에 클립 추가
                if (clip == null)
                {                
                    clip = Managers.Instance.ResourceManager.Load<AudioClip>(key);

                    if (clip == null)
                        break;

                    _soundData[i].clipList.Add(clip);
                }

                break;
            }
        }

        return clip;
    }
}
