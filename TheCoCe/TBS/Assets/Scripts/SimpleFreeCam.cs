using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFreeCam : MonoBehaviour {
    [SerializeField] private float m_Speed = 1.0f;
    [SerializeField] private float m_SpeedMul = 2.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_DeadZone = 0.0f;
    [SerializeField] private Transform cameraPivot;
    float m_x, m_y, m_Jump, m_Mul;

    // Use this for initialization
    void Start () {
        if (cameraPivot == null)
            Debug.LogWarning("No Pivot Trnsform assigned!");
    }
	
	// Update is called once per frame
	void Update () {
         m_x = Input.GetAxis("Horizontal");
         m_y = Input.GetAxis("Vertical");
         m_Jump = Input.GetAxis("Jump");

        //Speed multiplier
        if (Input.GetButton("Run"))
            m_Mul = m_SpeedMul;
        else
            m_Mul = 1.0f;

        //Horizontal movement
        transform.position += (cameraPivot != null) ? 
            (cameraPivot.forward) * m_Speed * Time.deltaTime * m_y * m_Mul : 
            transform.forward * m_Speed * Time.deltaTime * m_y * m_Mul;

        if (m_x > m_DeadZone || m_x < -m_DeadZone)
            transform.position += transform.right * m_Speed * Time.deltaTime * m_x * m_Mul;

        //Vertical movement
        if (m_Jump > m_DeadZone || m_Jump < -m_DeadZone)
            transform.position += transform.up * m_Speed * Time.deltaTime * m_Jump * m_Mul;
    }
}
