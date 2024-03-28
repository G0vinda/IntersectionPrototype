using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class UBahnRails : MonoBehaviour
{
    [SerializeField] float UBahnYOffset;

    private SplineContainer _railSpline;

    void Awake()
    {
        _railSpline = GetComponent<SplineContainer>();
    }

    public SplineContainer BuildRails(Vector3[] points)
    {
        if(points.Length != 6)
            Debug.LogError("BuildRails needs to be called with 3 Knot positions!");
        
        var knots = _railSpline.Spline.ToArray();
        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = transform.position.y;
            knots[i+1].Position = _railSpline.transform.InverseTransformPoint(points[i]);
            _railSpline.Spline.SetKnot(i+1, knots[i+1]);
        }

        return _railSpline;
    }

}
