using System;
using UnityEngine;

public enum RearHydraulicsState
{
  ToTop,
  ToMiddle,
  ToBottom
}

public class SplineWalker : MonoBehaviour
{
  public BezierSpline spline;

  public static int samples = 1000;

  public float duration;
  public bool checkBehind;
  public float vehicleLength;

  [Range(0f, 1f)]
  public float steerAhead;
  public Transform[] wheelsSteer;


  //Defines what and when to play an animation
  [Serializable]
  public struct RearHydraulics
  {
    public RearHydraulicsState equipmentState;
    public float progress;
  }
  public RearHydraulics[] rearAnimation;

  //Defines the wheels and their radius to roll on the ground
  [Serializable]
  public struct Wheel
  {
    public Transform wheel;
    public float radius;
    public bool invertRotation;
  }
  public Wheel[] wheels;


  private float progress;
  public float Progress
  {
    get
    {
      return progress;
    }
  }

  private Vector3 lastPosition;
  private float splineLength;

  private void Start()
  {
    splineLength = 0f;
    Vector3 prev = spline.GetPoint(0);
    for (int i = 0; i < samples; i++)
    {
      Vector3 point = spline.GetPoint((1f / (float)samples) * i);
      splineLength += (prev - point).magnitude;
      prev = point;

    }
  }

  void Update()
  {
    progress += Time.deltaTime / duration;

    //loop or don't loop
    if (progress > 1f)
    {
      progress = spline.Loop ? 0f : 1f;
    }
    Vector3 position = spline.GetPoint(progress);
    transform.localPosition = position;

    //Rotate wheels depending on distance
    float distance = Vector3.Distance(lastPosition, position);
    for (int i = 0; i < wheels.Length; i++)
    {
      if (wheels[i].wheel != null)
      {
        float wheelRot = distance / wheels[i].radius;
        if (wheels[i].invertRotation)
          wheels[i].wheel.Rotate(new Vector3(0f, (wheelRot * 180 / Mathf.PI) * -1, 0f));
        else
          wheels[i].wheel.Rotate(new Vector3(0f, wheelRot * 180 / Mathf.PI, 0f));

      }
    }
    lastPosition = position;

    //Align rear of the vehicle to the spline
    if (checkBehind)
    {
      float percent = (vehicleLength / splineLength);
      float abs = progress - percent;

      if (abs < 0)
        abs = 1 + abs;

      transform.LookAt(spline.GetPoint(abs));
    }

    //Steer wheels
    Vector3 direction = spline.GetDirection((progress + steerAhead) % 1f);
    for (int i = 0; i < wheelsSteer.Length; i++)
    {
      if (wheelsSteer[i] != null)
      {
        Vector3 pos = wheelsSteer[i].position + direction;
        float distanceToPlane = Vector3.Dot(wheelsSteer[i].up, pos - wheelsSteer[i].position);
        Vector3 pointOnPlane = pos - (wheelsSteer[i].up * distanceToPlane);

        wheelsSteer[i].LookAt(pointOnPlane, wheelsSteer[i].up);
      }
    }
  }
}
