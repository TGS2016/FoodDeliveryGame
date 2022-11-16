using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

}
[System.Serializable]
public class TutorialSteps
{
    public Transform ObjectToPoint;
    public string TutorialText;
    public float PopUpDuration;
    public bool HasFollowUpPopUp = false;
    public bool SkipThisStep = false;
    public string Code;
    public AudioClip clip;
}
