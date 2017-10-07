using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor : MonoBehaviour
{
  public Transform equipment;

  //I had issues getting these in code so I just assigned them in the inspector
  public SplineWalker splineWalker;
  public Animator animator;

  private RearHydraulicsState previousState;
  private Transform referenceUpper;
  private Transform referenceLower;

  int previousAnimationStateIndex = -1;

  // Sets up the ref points (if not set by hand)
  void Start()
  {
    if (referenceUpper == null)
      referenceUpper = this.transform.Find("Tractor/Main/3Point04/3PointUpperRef");
    if (referenceLower == null)
      referenceLower = this.transform.Find("Tractor/Main/3Point01/3Point03/3Point02/3PointLowerRef");

    equipment.parent = referenceLower;
    equipment.localPosition = new Vector3(0f, 0f, 0f);
  }

  void Update()
  {
    //Positioning Plough
    if (equipment != null && referenceLower != null && referenceUpper != null)
    {
      Vector3 newUp = referenceUpper.position - referenceLower.position;
      equipment.rotation = Quaternion.LookRotation(Vector3.Cross(referenceLower.right, newUp), newUp);
    }

    //Plays animations set in the SplineWalker. This is kinda ugly though
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
          }
          break;
        case RearHydraulicsState.ToBottom:
          animator.Play("Tractor_MidToLowBaked");
          break;
        default:
          Debug.Fail();
          break;
      }

      previousState = targetState;
    }
  }
}
