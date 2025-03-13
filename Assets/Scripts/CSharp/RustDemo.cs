using UnityEngine;
using CsBindgen;
using System.Runtime.InteropServices;

public class RustDemo : MonoBehaviour
{
    float[] floats;
    float time;

    GCHandle hdl;

    Mesh mesh;

    Vector3[] vertices;
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new Vector3[mesh.vertices.Length];
        floats = new float[mesh.vertices.Length * 3];
        hdl = GCHandle.Alloc(floats, GCHandleType.Pinned);

        foreach (var v in mesh.vertices)
        {
            floats[0] = v.x;
            floats[1] = v.y;
            floats[2] = v.z;
        }
    }

    void Start()
    {
        var res = NativeMethods.my_add(1, 2);
        Debug.Log("call rust code: " + res);
    }

    void Update()
    {
        time += Time.deltaTime;
        unsafe
        {
            // pass array to rust code
            var ptr = (float*)hdl.AddrOfPinnedObject();
            NativeMethods.process_float_arr(ptr, floats.Length, time*0.01f);
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(floats[i * 3], floats[i * 3 + 1], floats[i * 3 + 2]);
        }

        mesh.vertices = vertices;

        if (time > 2.0f)
        {
            time = 0.0f;
            Debug.Log($"array rust:{floats[0]},{floats[1]},{floats[2]} ");
        }

        mesh.RecalculateNormals();
    }

    void OnDestroy()
    {
        if (hdl.IsAllocated)
        {
            hdl.Free();
        }
    }
}
