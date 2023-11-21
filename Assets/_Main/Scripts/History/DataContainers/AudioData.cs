using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public int channel = 0;
    public string trackName;
    public string trackPath;
    public float trackVolume;
    public float trackPitch;
    public bool loop;

    public AudioData(AudioChannel channel) {
        this.channel = channel.channelIndex;

        if(channel.activeTrack == null)
            return;

        var track = channel.activeTrack;
        trackName = track.name;
        trackPath = track.path;
        trackVolume = track.volumeCap;
        trackPitch = track.pitch;
        loop = track.loop;
    }

    public static List<AudioData> Capture() {
        List<AudioData> audioChannels = new List<AudioData>();

        foreach(var channel in AudioManager.instance.channels) {
            if(channel.Value.activeTrack == null)
                continue;
            
            AudioData data = new AudioData(channel.Value);
            audioChannels.Add(data);
        }

        return audioChannels;
    }
}
