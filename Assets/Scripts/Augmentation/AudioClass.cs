using UnityEngine;

public class AudioClass : MonoBehaviour
{

    public AudioSource AudioSourceCorrect;
    public AudioSource AudioSourceWrong; 
    public AudioSource AudioSourceSuccess;
    public AudioSource AudioSourceHalfWay;
    public AudioSource AudioSourcePhaseChange;
    public AudioSource AudioSourceClick;
    public AudioSource AudioSourceStartTrainingPhase;

    public void PlayCorrect()
    {        
        AudioSourceCorrect.Play();
    }

    public void PlayWrong()
    {        
        AudioSourceWrong.Play();
    }

    public void PlaySuccess()
    {
        AudioSourceSuccess.Play();
    }
    public void PlayHalfWay()
    {
        AudioSourceHalfWay.Play();
    }
    public void PlayPhaseChange()
    {
        AudioSourcePhaseChange.Play();
    }

    public void PlayClick()
    {
        AudioSourceClick.Play();
    }

    public void PlayStartTrainingPhase()
    {
        AudioSourceStartTrainingPhase.Play();
    }
}
