using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;

public class AudioManager : NetworkedBehaviour
{
    public Sound[] sounds;


    void Awake() {
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name) {
        // InvokeServerRpc(Play1,name);
        InvokeClientRpcOnEveryone(Play1, name);
    }

    public void Stop(string name) {
        // InvokeServerRpc(Stop1, name);
        InvokeClientRpcOnEveryone(Stop1, name);
    }


    [ClientRPC]
    public void Play1(string name) {
        
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s==null) {
            Debug.Log("no such sound as " + name);
            return;
        }
        s.source.Play();
        
    }

    [ClientRPC]
    public void Stop1(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s==null || !s.source.isPlaying) {
            return;
        }
        s.source.Stop();
    }
}
