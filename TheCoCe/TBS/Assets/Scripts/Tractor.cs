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

        equipment.parent = referenceLower;
        equipment.localPosition = new Vector3(0f, 0f, 0f);
	}
	
	void Update () {
        //Positioning Plough
        if (equipment != null && referenceLower != null && referenceUpper != null)
        {
            Vector3 newUp = referenceUpper.position - referenceLower.position;
            equipment.rotation = Quaternion.LookRotation(Vector3.Cross(referenceLower.right, newUp), newUp);
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
                                animator.Play("Tractor_MidToHighBaked");
                                state = RearHydraulicsState.ToTop;
                            }
                            break;
                        case RearHydraulicsState.ToMiddle:
                            if (state != RearHydraulicsState.ToMiddle)
                            {
                                if (state == RearHydraulicsState.ToTop)
                                    animator.Play("Tractor_HighToMidBaked");
                                else if (state == RearHydraulicsState.ToBottom)
                                    animator.Play("Tractor_LowToMidBaked");
                                state = RearHydraulicsState.ToMiddle;
                            }
                            break;
                        case RearHydraulicsState.ToBottom:
                            if (state != RearHydraulicsState.ToBottom)
                            {
                                animator.Play("Tractor_MidToLowBaked");
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
