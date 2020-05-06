using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSync : NetworkBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> Clips = new List<AudioClip>();

    [Command]
    public void CmdSyncAudioClip(string name)
    {
        RpcSyncAudioClip(name);
    }

    [ClientRpc]
    public void RpcSyncAudioClip(string name)
    {
        audioSource.PlayOneShot(clip(name));
    }

    public AudioClip clip(string name)
    {
        return Clips.Find(x => x.name == name);
    }
}