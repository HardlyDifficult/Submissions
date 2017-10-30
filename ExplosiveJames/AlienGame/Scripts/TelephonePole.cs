using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TelephonePole : MonoBehaviour
{
    #region Data
    [SerializeField] [HideInInspector]
    private List<TelephonePole> PoleList;

    [SerializeField]
    private TelephonePole _nextPole;
    [SerializeField]
    private TelephonePole _prevNode;
    #endregion

    #region Properties
    // Pole Chain
    public TelephonePole StartPole
    {
        get
        {
            if (PreviousPole == null)
                return this;
            else
                return PreviousPole.StartPole;
        }
    }
    public TelephonePole NextPole
    {
        get
        {
            return _nextPole;
        }
        set
        {
            _nextPole = value;
            StartPole.RecalculateNodes();
        }
    }
    public TelephonePole PreviousPole
    {
        get
        {
            return _prevNode;
        }
        set
        {
            _prevNode = value;
        }
    }

    // Wire Connectors
    public Vector3 LeftConnector
    {
        get
        {
            return transform.position + transform.rotation * new Vector3(-.4f, 0, 5.5f);
        }
    }
    public Vector3 RightConnector
    {
        get
        {
            return transform.position + transform.rotation * new Vector3(.4f, 0, 5.5f);
        }
    }

    // Left Side Vectors
    public Vector3 ForwardPointLeft
    {
        get
        {
            if (NextPole == null)
                return LeftConnector + transform.rotation * new Vector3(0, -1, 0);
            return LeftConnector + Quaternion.LookRotation(NextPole.LeftConnector - LeftConnector) * new Vector3(0, -.4f, NextPoleDistance * .25f);
        }
    }
    public Vector3 BackwardPointLeft
    {
        get
        {
            if(PreviousPole == null)
                return LeftConnector + transform.rotation * new Vector3(0, 1, 0);
            return LeftConnector + Quaternion.LookRotation(PreviousPole.LeftConnector - LeftConnector) * new Vector3(0, -.4f, PreviousPoleDistance * .25f);
        }
    }

    // Right Side Vectors
    public Vector3 ForwardPointRight
    {
        get
        {
            if (NextPole == null)
                return RightConnector + transform.rotation * new Vector3(0, -1, 0);
            return RightConnector + Quaternion.LookRotation(NextPole.RightConnector - RightConnector) * new Vector3(0, -.4f, NextPoleDistance * .25f);
        }
    }
    public Vector3 BackwardPointRight
    {
        get
        {
            if (PreviousPole == null)
                return RightConnector + transform.rotation * new Vector3(0, 1, 0);
            return RightConnector + Quaternion.LookRotation(PreviousPole.RightConnector - RightConnector) * new Vector3(0, -.4f, PreviousPoleDistance * .25f);
        }
    }

    // Distances
    public float NextPoleDistance
    {
        get
        {
            if (NextPole == null)
                return 0;
            return Vector3.Distance(transform.position, NextPole.transform.position);
        }
    }
    public float PreviousPoleDistance
    {
        get
        {
            if (PreviousPole == null)
                return 0;
            return Vector3.Distance(transform.position, PreviousPole.transform.position);
        }
    }

    public int PoleCount
    {
        get
        {
            if (NextPole != null && PoleList != null)
                return PoleList.Count - 1;
            else
                return 0;
        }
    }
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (PreviousPole == null)
            RecalculateNodes();

        // Preventing this from happening in edit mode
        if (!Application.isPlaying || PreviousPole != null)
            return;

        int resolution = 3;
        float radius = 0.03f;

        Vector3[] ptsL = GetPathOnLeftLine(PoleCount * 6);
        Vector3[] ptsR = GetPathOnRightLine(PoleCount * 6);

        Mesh meshL = new Mesh();
        Mesh meshR = new Mesh();

        // Initialising variables
        Quaternion[] rotsL = new Quaternion[ptsL.Length];
        Vector3[] verticesL = new Vector3[resolution * ptsL.Length];
        int[] trianglesL = new int[(resolution * ptsL.Length) * 6];

        Quaternion[] rotsR = new Quaternion[ptsR.Length];
        Vector3[] verticesR = new Vector3[resolution * ptsR.Length];
        int[] trianglesR = new int[(resolution * ptsR.Length) * 6];

        // Calculating offset rotations
        for (int i = 0; i < rotsL.Length; i++)
        {
            if (i == 0 || i == rotsL.Length - 1)
                rotsL[i] = i == 0 ? Quaternion.LookRotation(ptsL[i + 1] - ptsL[i]) : Quaternion.LookRotation(ptsL[i] - ptsL[i - 1]);
            else
                rotsL[i] = Quaternion.LookRotation(ptsL[i + 1] - ptsL[i - 1]);
        }
        for (int i = 0; i < rotsR.Length; i++)
        {
            if (i == 0 || i == rotsR.Length - 1)
                rotsR[i] = i == 0 ? Quaternion.LookRotation(ptsR[i + 1] - ptsR[i]) : Quaternion.LookRotation(ptsR[i] - ptsR[i - 1]);
            else
                rotsR[i] = Quaternion.LookRotation(ptsR[i + 1] - ptsR[i - 1]);
        }

        // Vert Calculation
        for (int seg = 0; seg < ptsL.Length; seg++)
        {
            for (int i = 0; i < resolution; i++)
            {
                // Angle around the point
                float newAngle = 360f / resolution;
                Quaternion angle = rotsL[seg] * Quaternion.Euler(new Vector3(i * newAngle, -90, 0));

                // Calculating vert position
                verticesL[i + (seg * resolution)] = ptsL[seg] + angle * new Vector3(0, 0, radius);
            }
        }
        for (int seg = 0; seg < ptsR.Length; seg++)
        {
            for (int i = 0; i < resolution; i++)
            {
                // Angle around the point
                float newAngle = 360f / resolution;
                Quaternion angle = rotsR[seg] * Quaternion.Euler(new Vector3(i * newAngle, -90, 0));

                // Calculating vert position
                verticesR[i + (seg * resolution)] = ptsR[seg] + angle * new Vector3(0, 0, radius);
            }
        }

        // Tri Calculation
        for (int seg = 0; seg < ptsL.Length - 1; seg++)
        {
            for (int i = 0; i < resolution; i++)
            {
                // Pre-Calculation
                // Segmentation offset
                int segTri = seg * resolution * 6;
                int segIndex = seg * resolution;

                int i2 = i * 2 + i;         // i2 allows array index to count sequentially
                int r2 = resolution * 2;
                int r3 = resolution * 3;

                // Forwards along the path
                trianglesL[segTri + i2] = i + segIndex;
                trianglesL[segTri + i2 + 1] = (i + 1) % resolution + segIndex;
                trianglesL[segTri + i2 + 2] = (i % resolution) + resolution + segIndex;

                // Backwards along the path
                trianglesL[segTri + r3 + i2] = r2 - (i + 1) + segIndex;
                trianglesL[segTri + r3 + i2 + 1] = (resolution - ((i + 2) % resolution)) % resolution + resolution + segIndex;
                trianglesL[segTri + r3 + i2 + 2] = (resolution - ((i + 1) % resolution)) % resolution + segIndex;
            }
        }
        for (int seg = 0; seg < ptsR.Length - 1; seg++)
        {
            for (int i = 0; i < resolution; i++)
            {
                // Pre-Calculation
                // Segmentation offset
                int segTri = seg * resolution * 6;
                int segIndex = seg * resolution;

                int i2 = i * 2 + i;         // i2 allows array index to count sequentially
                int r2 = resolution * 2;
                int r3 = resolution * 3;

                // Forwards along the path
                trianglesR[segTri + i2] = i + segIndex;
                trianglesR[segTri + i2 + 1] = (i + 1) % resolution + segIndex;
                trianglesR[segTri + i2 + 2] = (i % resolution) + resolution + segIndex;

                // Backwards along the path
                trianglesR[segTri + r3 + i2] = r2 - (i + 1) + segIndex;
                trianglesR[segTri + r3 + i2 + 1] = (resolution - ((i + 2) % resolution)) % resolution + resolution + segIndex;
                trianglesR[segTri + r3 + i2 + 2] = (resolution - ((i + 1) % resolution)) % resolution + segIndex;
            }
        }

        // Applying meshes
        meshL.vertices = verticesL;
        meshL.triangles = trianglesL;
        meshL.RecalculateNormals();

        meshR.vertices = verticesR;
        meshR.triangles = trianglesR;
        meshR.RecalculateNormals();

        // Combining meshes
        CombineInstance[] combine = new CombineInstance[3];
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh original = mf.mesh;

        combine[0].mesh = original;
        combine[1].mesh = meshL;
        combine[2].mesh = meshR;

        for (int i = 1; i < combine.Length; i++)
            combine[i].transform = transform.worldToLocalMatrix;
        combine[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transform.localScale);

        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);
    }
    private void Start()
    {
        // Preventing this from happening in play mode
        if (!Application.isEditor)
            return;

        // Detecting Duplicate Pole
        if (PreviousPole != null && PreviousPole.NextPole != this)
        {
            PreviousPole = PreviousPole.NextPole;
            PreviousPole.NextPole = this;
        }

        if (NextPole != null && NextPole.PreviousPole != this)
        {
            // Checking if it's a start Pole
            if (PreviousPole == null)
                NextPole = null;
            else
                NextPole.PreviousPole = this;
        }

        // Looking to initialise with another pole
        if (PreviousPole== null && NextPole == null)
        {
            TelephonePole[] list = FindObjectsOfType<TelephonePole>();

            // If we find nothing
            if (list.Length == 0)
                return;

            // Looking through list
            for (int i = 0; i < list.Length; i++)
                if (list[i].PreviousPole == null && list[i].NextPole == null && list[i] != this)
                {
                    list[i].NextPole = this;
                    PreviousPole = list[i];
                }
        }
    }
    private void Update()
    {
        // Fixing weird bug
        if(PreviousPole == null)
        RecalculateNodes();
    }
    private void OnDrawGizmos()
    {
        // Preventing this from happening in play mode
        if (Application.isPlaying)
            return;

        // Identifying Starting Pole
        if (PreviousPole == null)
        {
            Gizmos.color = new Color(1, 0.4f, 0.4f, 0.4f);
            Gizmos.DrawCube(transform.position + new Vector3(0, 3, 0), new Vector3(0.2f, 6, 0.2f));
            Gizmos.DrawWireCube(transform.position + new Vector3(0, 3, 0), new Vector3(0.2f, 6, 0.2f));
        }

        // Drawing Curves
        if (PreviousPole == null && NextPole != null)
        {
            Vector3[] leftPath = GetPathOnLeftLine((PoleList.Count - 1) * 4);
            Vector3[] rightPath = GetPathOnRightLine((PoleList.Count - 1) * 4);

            Gizmos.color = new Color(.2f, .2f, .2f);
            for (int i = 0; i < leftPath.Length - 1; i++)
                Gizmos.DrawLine(leftPath[i], leftPath[i + 1]);

            Gizmos.color = new Color(.25f, .25f, .25f);
            for (int i = 0; i < rightPath.Length - 1; i++)
                Gizmos.DrawLine(rightPath[i], rightPath[i + 1]);
        }
    }
    private void OnDestroy()
    {
        // Handling when a pole is deleted
        if (PreviousPole != null && NextPole != null)
        {
            PreviousPole.NextPole = NextPole;
            NextPole.PreviousPole = PreviousPole;
        }
        else
        {
            if (NextPole != null)
                NextPole.PreviousPole = null;
            if (PreviousPole != null)
                PreviousPole.NextPole = null;
        }
    }
    #endregion

    #region Functions
    public void RecalculateNodes()
    {
        if (PreviousPole != null)
            Debug.LogError("Function called on none start node! " + gameObject.name);
        else
        {
            PoleList = new List<TelephonePole> { this };

            // If only one pole is in the system
            if (NextPole == null)
                return;

            // Finding all connected poles
            TelephonePole node = NextPole;
            while (true)
            {
                PoleList.Add(node);

                if (node.NextPole == null)
                    break;

                node = node.NextPole;
            }
        }
    }
    public Vector3 GetPointOnLeftLine(float time)
    {
        // Making start node calculate
        if (PreviousPole != null)
            return StartPole.GetPointOnLeftLine(time);

        // Segment Maths
        time *= (PoleList.Count - 1);
        int segment = (int)Mathf.Clamp(time, 0, PoleList.Count - 2);
        float t = time - segment;

        // Curve Maths
        float it = 1f - t;
        float it2 = it * it;
        float t2 = t * t;

        return PoleList[segment].LeftConnector * (it2 * it) +
               PoleList[segment].ForwardPointLeft * (3f * it2 * t) +
               PoleList[segment + 1].BackwardPointLeft * (3f * it * t2) +
               PoleList[segment + 1].LeftConnector * (t2 * t);

    }
    public Vector3 GetPointOnRightLine(float time)
    {
        // Making start node calculate
        if (PreviousPole != null)
            return StartPole.GetPointOnRightLine(time);

        // Segment Maths
        time *= (PoleList.Count - 1);
        int segment = (int)Mathf.Clamp(time, 0, PoleList.Count - 2);
        float t = time - segment;

        // Curve Maths
        float it = 1f - t;
        float it2 = it * it;
        float t2 = t * t;

        return PoleList[segment].RightConnector * (it2 * it) +
               PoleList[segment].ForwardPointRight * (3f * it2 * t) +
               PoleList[segment + 1].BackwardPointRight * (3f * it * t2) +
               PoleList[segment + 1].RightConnector * (t2 * t);

    }
    public Vector3[] GetPathOnLeftLine(int resolution)
    {
        // Making start node calculate
        if (PreviousPole != null)
            return StartPole.GetPathOnLeftLine(resolution);

        Vector3[] path = new Vector3[resolution + 1];
        for (int i = 0; i < path.Length; i++)
            path[i] = GetPointOnLeftLine((1f / resolution) * i);

        return path;
    }
    public Vector3[] GetPathOnRightLine(int resolution)
    {
        // Making start node calculate
        if (PreviousPole != null)
            return StartPole.GetPathOnRightLine(resolution);

        Vector3[] path = new Vector3[resolution + 1];
        for (int i = 0; i < path.Length; i++)
            path[i] = GetPointOnRightLine((1f / resolution) * i);

        return path;
    }
    #endregion
}