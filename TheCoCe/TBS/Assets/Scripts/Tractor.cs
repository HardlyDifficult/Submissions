using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor : MonoBehaviour {

    public Transform equipment;

    //Can be set by hand as well just in case
    private Transform referenceUpper; 
    private Transform referenceLower;

	// Sets up the ref points (if not set by hand)
	void Start () { 
        if (referenceUpper == null)
            referenceUpper = this.transform.Find("Tractor/Main/3Point04/3PointUpperRef");
        if (referenceLower == null)
            referenceLower = this.transform.Find("Tractor/Main/3Point01/3Point03/3Point02/3PointLowerRef");
	}
	
	// Sets Position and rotation of the equipment
	void Update () {
        if (equipment != null && referenceLower != null && referenceUpper != null)
        {
            equipment.transform.position = referenceLower.position;
            equipment.LookAt(referenceUpper);
            equipment.transform.Rotate(new Vector3(1.0f, 0, 0), 90);
        }

	}

}
