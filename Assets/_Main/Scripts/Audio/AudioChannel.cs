using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;

public class AudioChannel
{
    private const string TRACK_CONTAINER_NAME_FORMAT = "Channel - [{0}]";
    public int channelIndex {get; private set;}
    public Transform trackContainer {get; private set;} = null;

    private List<AudioTrack> tracks = new List<AudioTrack>();
    Coroutine volumeLevelingCoroutine = null;
    bool isLeveling => volumeLevelingCoroutine != null;

    public AudioTrack activeTrack = null;


    public AudioChannel(int channel){
        channelIndex = channel;

        trackContainer = new GameObject(string.Format(TRACK_CONTAINER_NAME_FORMAT, channel)).transform;
        trackContainer.SetParent(AudioManager.instance.transform);
    }

    public AudioTrack PlayTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, float pitch, string filePath){
        if(TryGetTrack(clip.name, out AudioTrack existingtrack)){
            if(!existingtrack.isPlaying)
                existingtrack.Play();

            SetAsActiveTrack(existingtrack);

            return existingtrack;
        }

        AudioTrack track = new AudioTrack(clip, loop, startingVolume, volumeCap, pitch, this, AudioManager.instance.musicMixer, filePath);
        track.Play();

        SetAsActiveTrack(track);

        return track;
    }

    public bool TryGetTrack(string trackName, out AudioTrack value){
        trackName = trackName.ToLower();

        foreach(AudioTrack track in tracks){
            if(track.name.ToLower() == trackName){
                value = track;
                return true;
            }
        }

        value = null;
        return false;
    }

    private void SetAsActiveTrack(AudioTrack track){
        if(!tracks.Contains(track)){
            tracks.Add(track);
        }

        activeTrack = track;

        TryStartVolumeLevel();
    }

    private void TryStartVolumeLevel(){
        if(!isLeveling){
            volumeLevelingCoroutine = AudioManager.instance.StartCoroutine(VolumeLeveling());
        }
    }

    private IEnumerator VolumeLeveling(){
        while((activeTrack != null && (tracks.Count > 1 || activeTrack.volume != activeTrack.volumeCap)) || (activeTrack == null && tracks.Count > 0)){
            for(int i = tracks.Count-1; i >= 0; i--){
                AudioTrack track = tracks[i];
                float targetVolume = (activeTrack == track)? track.volumeCap : 0;

                if(track == activeTrack && track.volume == targetVolume)
                    continue;

                track.volume = Mathf.MoveTowards(track.volume, targetVolume, AudioManager.TRACK_TRANSITION_SPEED * Time.deltaTime);

                if(track != activeTrack && track.volume == 0){
                    DestroyTrack(track);
                }
            }

            yield return null;
        }

        volumeLevelingCoroutine = null;
    }

    private void DestroyTrack(AudioTrack track){
        if(tracks.Contains(track))
            tracks.Remove(track);

        Object.Destroy(track.root);
    }

    public void Stoptrack()
    {
        if(activeTrack == null) return;

        activeTrack = null;
        TryStartVolumeLevel();
    }
}

