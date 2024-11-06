using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVSecondaryScript : MonoBehaviour
{
    private Mesh mesh;
    public GameObject FOVMain;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetMesh", 0.01f);
    }

    // Update is called once per frame
    void SetMesh()
    {
        mesh = FOVMain.GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
