using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator myAnimator;
    private const float END_DELTA = 1.2f;
    private const float _interpTime = .8f;
    public AudioSource yourTurnSFX;
    private float initialVolume;
    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        initialVolume = yourTurnSFX.volume;
    }
    private void OnDisable()
    {
        yourTurnSFX.volume = initialVolume;
    }
    private void Update()
    {
        if (myAnimator != null)
        {
            if(AnimatorAboutToStop())
            {
                Debug.Log("Im gonna stop");
                UpdateAudioVolume(Time.deltaTime);     
            }
            if (!AnimatorIsPlaying())
            {
                Debug.Log("i stopped");
                gameObject.SetActive(false);
            }
        }
    }
    bool AnimatorAboutToStop()
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).length >
               myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    bool AnimatorIsPlaying()
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).length+END_DELTA>
               myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
 
    
      
    
    private void UpdateAudioVolume(float alpha)
    {
        
            yourTurnSFX.volume -= (alpha / 4);
        
        
    }
}
