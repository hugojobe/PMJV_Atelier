using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTrack
{
    public const string TRACK_NAME_FORMAT = "Track - [{0}]";
    public string name {get; private set;}
    public string path {get; private set;}

    public GameObject root => source.gameObject;
    private AudioChannel channel;
    private AudioSource source;
    public bool loop => source.loop;
    public float volumeCap {get; private set;}
    public float pitch {get{ return source.pitch;} set{source.pitch = value;} }

    public bool isPlaying => source.isPlaying;
    public float volume {get {return source.volume;} set {source.volume = value;}}

    public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, float pitch, AudioChannel channel, AudioMixerGroup mixer, string filePath){
        name = clip.name;
        path = filePath;
        this.channel = channel;
        this.volumeCap = volumeCap;
        
        source = CreateSource();
        source.clip = clip;
        source.loop = loop;
        source.volume = startingVolume;
        source.pitch = pitch;
        source.outputAudioMixerGroup = mixer;
    }

    private AudioSource CreateSource(){
        GameObject obj = new GameObject(string.Format(TRACK_NAME_FORMAT, name));
        obj.transform.SetParent(channel.trackContainer);
        AudioSource source = obj.AddComponent<AudioSource>();

        return source;
    }

    public void Play(){
        source.Play();
    }

    public void Stop(){
        source.Stop();
    }
}
