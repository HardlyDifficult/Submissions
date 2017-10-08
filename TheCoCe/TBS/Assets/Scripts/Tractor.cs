using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor : MonoBehaviour
{
    public Transform attachment;

    //I had issues getting these in code so I just assigned them in the inspector
    public SplineWalker splineWalker;
    public Animator animator;

    private RearHydraulicsState previousState;
    private Transform referenceUpper;
    private Transform referenceLower;
    private Equipment equipment;

    int previousAnimationStateIndex = -1;

     
    void Start()
    {
        if (referenceUpper == null)
            referenceUpper = this.transform.Find("Tractor/Main/3Point04/3PointUpperRef");
        if (referenceLower == null)
            referenceLower = this.transform.Find("Tractor/Main/3Point01/3Point03/3Point02/3PointLowerRef");

        attachment.parent = referenceLower;
        attachment.localPosition = new Vector3(0f, 0f, 0f);

        equipment = attachment.GetComponent<Equipment>();
        if (equipment != null)
        { // Set initial dirt scale to 0
            Vector3 scale = new Vector3(0f, 0f, 0f);
            for (int i = 0; i < equipment.dirt.Length; i++)
            {
                if (equipment.dirt[i] != null)
                {
                    equipment.dirt[i].localScale = scale;
                }
            }
        }
    }

    void Update()
    {
        // Positioning Plough
        if (attachment != null && referenceLower != null && referenceUpper != null)
        { 
            Vector3 newUp = referenceUpper.position - referenceLower.position;
            attachment.rotation = Quaternion.LookRotation(Vector3.Cross(referenceLower.right, newUp), newUp);
        }

        // Plays animations set in the SplineWalker
        UpdateRearAnimation();
    }

    void UpdateRearAnimation()
    {
        if (splineWalker == null)
        {
            return;
        }

        int nextStateIndex = previousAnimationStateIndex + 1;
        if (nextStateIndex >= splineWalker.rearAnimation.Length)
        { // Animation complete, loop back
            if (splineWalker.spline.Loop == false)
            {
                return;
            }
            nextStateIndex = 0;
        }

        float deltaProgress = splineWalker.rearAnimation[nextStateIndex].progress - splineWalker.Progress;
        if (deltaProgress < 0 && deltaProgress > -.5) // .5 is some threshold to detect if the animation has looped yet
        {
            previousAnimationStateIndex = nextStateIndex;

            RearHydraulicsState targetState = splineWalker.rearAnimation[nextStateIndex].equipmentState;
            if (targetState == previousState)
            { // No visable change
                return;
            }

            switch (targetState)
            {
                case RearHydraulicsState.ToTop:
                    animator.Play("Tractor_MidToHighBaked");
                    break;
                case RearHydraulicsState.ToMiddle:
                    if (previousState == RearHydraulicsState.ToTop)
                    {
                        animator.Play("Tractor_HighToMidBaked");
                    }
                    else if (previousState == RearHydraulicsState.ToBottom)
                    {
                        animator.Play("Tractor_LowToMidBaked");
                        if(equipment != null)
                            StartCoroutine(Dirt(false, equipment.fadeOutTime));
                    }
                    break;
                case RearHydraulicsState.ToBottom:
                    animator.Play("Tractor_MidToLowBaked");
                    if (equipment != null)
                        StartCoroutine(Dirt(true, equipment.fadeInTime));
                    break;
                default:
                    break;
            }

            previousState = targetState;
        }
    }

    // Lerping dirt scale
    private IEnumerator Dirt(bool scaleUpOrDown, float time)
    {
        float currentTime = 0.0f;
        Vector3 fromScale = scaleUpOrDown == true ? new Vector3(0f, 0f, 0f) : new Vector3(1f, 1f, 1f);
        Vector3 toScale = scaleUpOrDown == true ? new Vector3(1f, 1f, 1f) : new Vector3(0f, 0f, 0f);

        do
        {
            for (int i = 0; i < equipment.dirt.Length; i++)
            {
                if (equipment.dirt[i] != null)
                    equipment.dirt[i].localScale = Vector3.Lerp(fromScale, toScale, currentTime/time);
            }
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }
}
