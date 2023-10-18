using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class AudioManager : MonoBehaviour
{
    private const string SFX_PARENT_NAME = "SFX";
    private const string SFX_NAME_FORMAT = "SFX - [{0}]";

    public static AudioManager instance {get; private set;}

    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();
    public const float TRACK_TRANSITION_SPEED = 1f;

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    private Transform sfxRoot;

    private void Awake() {
        if(instance == null){
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    public AudioSource PlaySoundEffect(string filePath, float volume = 1, float pitch = 1, bool loop = false){
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null){
            Debug.LogError("Can't find sfx at path " + filePath);
            return null;
        }

        return PlaySoundEffect(clip, volume, pitch, loop);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false){
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;
        
        effectSource.clip = clip;
        
        effectSource.outputAudioMixerGroup = sfxMixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0;
        effectSource.pitch = pitch;
        effectSource.loop = loop;

        effectSource.Play();

        if(!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }

    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

    public void StopSoundEffect(string soundName){
        soundName = soundName.ToLower();
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();

        foreach(AudioSource source in sources){
            if(source.clip.name.ToLower() == soundName){
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f){
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null){
            Debug.LogError("Can't find track at path " + filePath);
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filePath);
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = ""){
        AudioChannel audioChannel = TryGetChannel(channel, true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
        return track;
    }

    public void StopTrack(int channel){
        AudioChannel c = TryGetChannel(channel, false);

        if(c == null) return;

        c.Stoptrack();
    }

    public void StopTrack(string trackName){
        trackName = trackName.ToLower();

        foreach(AudioChannel channel in channels.Values){
            if(channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName){
                channel.Stoptrack();
                return;
            }
        }
    }

    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false){
        AudioChannel channel = null;

        if(channels.TryGetValue(channelNumber, out channel)){
            return channel;
        } else if(createIfDoesNotExist){
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }
}
