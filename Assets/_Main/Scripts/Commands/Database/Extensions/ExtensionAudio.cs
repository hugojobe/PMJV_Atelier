using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public class ExtensionAudio : CommandDatabaseExtension
{
    private static string[] PARAM_VOLUME => new string[]{"-v", "-volume"};
    private static string[] PARAM_VOLUMECAP => new string[]{"-vc", "-volumecap"};
    private static string[] PARAM_PITCH => new string[]{"-p", "-pitch"};
    private static string[] PARAM_LOOP => new string[]{"-l", "-loop"};
    private static string[] PARAM_CHANNEL => new string[]{"-ch", "-channel"};


    new public static void Extend(CommandDatabase database){
        database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
        database.AddCommand("stopsfx", new Action<string>(StopSFX));
        
        database.AddCommand("playmusic", new Action<string[]>(PlayMusic));
        database.AddCommand("stopmusic", new Action<string>(StopMusic));
    }

    private static void PlaySFX(string[] data){
        string filePath;
        float volume;
        float pitch;
        bool loop;

        CommandParameters parameters = ConvertDataToParameters(data);

        filePath = FilePaths.resourcesSFX + data[0];
        parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);
        parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
        parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null) return;

        AudioManager.instance.PlaySoundEffect(clip, volume: volume, pitch: pitch, loop: loop);
    }

    private static void StopSFX(string data){
        AudioManager.instance.StopSoundEffect(data);
    }

    private static void PlayMusic(string[] data){
        string filePath;
        float volume;
        float volumeCap;
        float pitch;
        bool loop;
        int channel;

        CommandParameters parameters = ConvertDataToParameters(data);

        filePath = FilePaths.ResourcesMusic + data[0];
        parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 0f);
        parameters.TryGetValue(PARAM_VOLUMECAP, out volumeCap, defaultValue: 1);
        parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
        parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);
        parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null) return;

        AudioManager.instance.PlayTrack(clip, channel: channel, loop: loop, startingVolume: volume, volumeCap: volumeCap, pitch, filePath); 
    }

    private static void StopMusic(string data){
        if(int.TryParse(data, out int channel)){
            AudioManager.instance.StopTrack(channel);
        } else {
            AudioManager.instance.StopTrack(data);
        }
    }
}
