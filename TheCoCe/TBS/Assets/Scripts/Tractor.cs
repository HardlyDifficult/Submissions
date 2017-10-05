using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor : MonoBehaviour {

    public Transform equipment;
   
    //I had issues getting these in code so I just assigned them in the inspector
    public SplineWalker splineWalker;
    public Animator animator;

    private RearHydraulicsState state;
    private Transform referenceUpper; 
    private Transform referenceLower;

	// Sets up the ref points (if not set by hand)
	void Start () { 
        if (referenceUpper == null)
            referenceUpper = this.transform.Find("Tractor/Main/3Point04/3PointUpperRef");
        if (referenceLower == null)
            referenceLower = this.transform.Find("Tractor/Main/3Point01/3Point03/3Point02/3PointLowerRef");
	}
	
	void Update () {
        //Positioning Plough
        if (equipment != null && referenceLower != null && referenceUpper != null)
        {
            equipment.transform.position = referenceLower.position;

            //This is causing issues with jittering (flipping from y = 0 to y = 180) 
            //You can see this when moving referenceUpper back and forth in play mode
            equipment.LookAt(referenceUpper);
            equipment.transform.Rotate(new Vector3(1.0f, 0, 0), 90);
        }

        //Plays animations set in the SplineWalker. This is kinda ugly though
        if (splineWalker != null)
        {
            for (int i = 0; i < splineWalker.rearAnimation.Length; i++)
            {
                float dist = splineWalker.rearAnimation[i].progress - splineWalker.Progress;
                if (dist < 0.01 && dist > -0.01)
                {
                    switch (splineWalker.rearAnimation[i].equipmentState)
                    {
                        case RearHydraulicsState.ToTop:
                            if (state != RearHydraulicsState.ToTop)
                            {
                                animator.Play("Tractor|MidToHigh");
                                state = RearHydraulicsState.ToTop;
                            }
                            break;
                        case RearHydraulicsState.ToMiddle:
                            if (state != RearHydraulicsState.ToMiddle)
                            {
                                if (state == RearHydraulicsState.ToTop)
                                    animator.Play("Tractor|HighToMid");
                                else if (state == RearHydraulicsState.ToBottom)
                                    animator.Play("Tractor|LowToMid");
                                state = RearHydraulicsState.ToMiddle;
                            }
                            break;
                        case RearHydraulicsState.ToBottom:
                            if (state != RearHydraulicsState.ToBottom)
                            {
                                animator.Play("Tractor|MidToLow");
                                state = RearHydraulicsState.ToBottom;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
	}

}
