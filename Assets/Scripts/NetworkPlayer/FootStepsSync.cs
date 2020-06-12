using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsSync : NetworkBehaviour
{
    public AudioSource FootStepAudioSource;
    public AudioClip[] FootSteps; 

    CharacterController characterController;
    NetworkSync ns;

    float velocity;
    bool isSneaking;

    // Start is called before the first frame update
    void Start()
    {
		if (isServer)
		{
            characterController = GetComponent<CharacterController>();
            ns = GetComponent<NetworkSync>();
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (isServer)
		{
            RpcSyncFootStep((byte)ns.Velocity, ns.isSneaking);
        }

		if (isClient)
		{
            if (!FootStepAudioSource.isPlaying && velocity > 1 && !isSneaking)
			{
                int index = Random.Range(0, FootSteps.Length);
                FootStepAudioSource.PlayOneShot(FootSteps[index]);
            }
		}
    }

    [ClientRpc]
    public void RpcSyncFootStep(byte velocity, bool isSneaking)
    {
        this.velocity = velocity;
        this.isSneaking = isSneaking;
    }
}