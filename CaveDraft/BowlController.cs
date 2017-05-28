using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the spice contained in the bowl, the amount and the display. I'm not that experienced, so it might be dumb at some places.
/// </summary>
public class BowlController : MonoBehaviour
{
    [Header("Index 0 can stay empty")]
    public Material[] spiceMat = new Material[(int)Spice.Count]; // stange! see in enum below ...
    public GameObject spiceGraphic;
    [Header("Debug")]
    public bool debug = false;
    public Spice setSpice;
    public Spice currentSpice;
    public float addAmount;
    public float currentAmount;
    public bool setRefill = true;

    public Spice spice { get; private set; }
    static float max = 10f;
    public float amount { get { return _amount; } private set { _amount = ( value > max ? max : _amount > 0 ? 0 : _amount ); } }
    float _amount = 0;

    void Start()
    {
        spice = Spice.Empty;
    }

    void Update()
    {
        if (debug)
        {
            FillSpice(setSpice, addAmount, setRefill);
            currentSpice = spice;
            currentAmount = amount;
        }
    }

    public void FillSpice(Spice s, float a, bool refill)
    {
        if (spice == Spice.Count) return;
        if (spice == Spice.Empty)
        {
            SetSpiceGraphic(Spice.Empty);
            amount = 0;
        }
        if (spice == s || refill)
        {
            spice = s;
            amount += a;
        }
        if (amount != 0)
        {
            SetSpiceGraphic(spice);
            return;
        }
        SetSpiceGraphic(Spice.Empty);
    }

    private void SetSpiceGraphic(Spice s)
    {
        if (spiceGraphic == null) return;
        if (s == Spice.Empty)
        {
            spiceGraphic.SetActive(false);
            return;
        }
        spiceGraphic.SetActive(true);
        spiceGraphic.GetComponent<Renderer>().material = spiceMat[(int)s];
    }

    public void FillSpice(Spice s, float a)
    {
        FillSpice(s, a, false);
    }

    public enum Spice
    {
        Empty,
        BlackPepper,
        WasabiPowder, // on pizza?
        ToastedSesameSeed,
        Count // Didnt found a better solution than keeping this the last to count! Please change if you know better :D
    }
}
