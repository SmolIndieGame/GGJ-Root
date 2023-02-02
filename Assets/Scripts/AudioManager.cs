using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct AudioData
{
    public string name;
    public AudioClip audioClip;
    public float volume;
    public float pitch;
    public bool loop;

    public double lastTimePlay { get; set; }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    const double playCoolDown = 0.05f;

    [SerializeField] List<AudioData> data;

    Dictionary<string, AudioData> dataDict;
    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        dataDict = new Dictionary<string, AudioData>();
        foreach (var item in data)
        {
            var obj = item;
            obj.lastTimePlay = -999999;
            dataDict.Add(item.name, obj);
        }

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        StopAll();
    }

    public void Play(string name)
    {
        var data = dataDict[name];
        if (!data.loop && data.lastTimePlay + playCoolDown > Time.timeAsDouble)
            return;
        var source = gameObject.AddComponent<AudioSource>();
        source.clip = data.audioClip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = data.loop;
        source.playOnAwake = false;
        data.lastTimePlay = Time.timeAsDouble;
        if (!source.loop)
            Destroy(source, data.audioClip.length);

        source.Play();

        dataDict[name] = data;
    }

    public void StopAll()
    {
        var sources = gameObject.GetComponents<AudioSource>();
        foreach (var source in sources)
            Destroy(source);
    }
}
