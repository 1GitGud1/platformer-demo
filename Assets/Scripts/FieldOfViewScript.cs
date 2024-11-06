using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewScript : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private Mesh mesh;
    Vector3 origin;
    int rayCount;
    float fov;
    float angleIncrease;
    public float edgeDstThreshold;
    public float viewDistance;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
        rayCount = 90;
        fov = 360f;
        angleIncrease = fov / rayCount;
        viewDistance = 2f;
        edgeDstThreshold = 0.20f;
    }

    void LateUpdate()
    {
        float angle = 0f;
        RayCastInfo oldRayCast = new RayCastInfo();

        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i <= rayCount; i++) {
            //Casting vertices
            RayCastInfo newRayCast = RayCast(angle);

            //Finding vertex closest to an edge
            if (i > 0){
                bool DstThresholdExceeded = Mathf.Abs(oldRayCast.dst - newRayCast.dst) > edgeDstThreshold;
                if (oldRayCast.hit != newRayCast.hit || (DstThresholdExceeded)){
                    float edgeAngleIncrease = angleIncrease/2;
                    float edgeAngle = angle + edgeAngleIncrease;
                    Vector3 minPoint = Vector3.zero;
                    Vector3 maxPoint = Vector3.zero;
                    
                    for (int e = 0; e < 5; e++) {
                        //Casting an edge ray
                        RayCastInfo edgeRayCast = RayCast(edgeAngle);

                        edgeAngleIncrease = edgeAngleIncrease / 2;

                        bool edgeDstThresholdExceeded = Mathf.Abs(oldRayCast.dst - edgeRayCast.dst) > edgeDstThreshold;
                        if (edgeRayCast.hit == oldRayCast.hit && !edgeDstThresholdExceeded){
                            minPoint = edgeRayCast.point;
                            edgeAngle -= edgeAngleIncrease;
                        } else {
                            maxPoint = edgeRayCast.point;
                            edgeAngle += edgeAngleIncrease;
                        }
                    }

                    if (minPoint != Vector3.zero){
                        vertices.Add(minPoint);
                    }
                    if (maxPoint != Vector3.zero){
                        vertices.Add(maxPoint);
                    }
                }

            }

            //adding vertex of fov to the list
            vertices.Add(newRayCast.point);
            oldRayCast = newRayCast;
            angle -= angleIncrease;
        }

        int vertexCount = vertices.Count + 1;
        Vector3[] verticesArray = new Vector3[vertexCount];
        Vector2[] uvArray = new Vector2[vertexCount];
        int[] trianglesArray = new int[(vertexCount - 2) * 3];

        verticesArray[0] = origin;
        uvArray[0] = new Vector2(0, 0);
        for (int i = 0; i < vertexCount - 1; i++)
        {
            verticesArray[i+1] = transform.InverseTransformPoint(vertices[i]);
            uvArray[i+1] = new Vector2(0, 1);

            if (i < vertexCount - 2) {
                trianglesArray[i*3] = 0;
                trianglesArray[i*3+1] = i + 1;
                trianglesArray[i*3+2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = verticesArray;
        mesh.uv = uvArray;
        mesh.triangles = trianglesArray;
        mesh.RecalculateNormals();
    }

    private static Vector3 GetVectorFromAngle(float angle) {
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public void SetOrigin(Vector3 origin){
        this.origin = origin;
    }

    RayCastInfo RayCast(float angle){
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
        if (raycastHit2D.collider == null) {
            Vector3 point = origin + GetVectorFromAngle(angle) * (viewDistance+0.25f);
            return new RayCastInfo(false, point, viewDistance, angle);
        } else {
            Vector3 point = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, origin.z) + GetVectorFromAngle(angle) * 0.25f;
            return new RayCastInfo(true, point, raycastHit2D.distance, angle);
        }
    }

    public struct RayCastInfo{
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public RayCastInfo(bool _hit, Vector3 _point, float _dst, float _angle){
            //whether the raycast hit a wall or not
            hit = _hit;
            //coordinates of the raycast destination
            point = _point;
            //distance of the raycast
            dst = _dst;
            //angle of the raycast (subtract to go clockwise(for unknown reason))
            angle = _angle;
        }
    }
}
