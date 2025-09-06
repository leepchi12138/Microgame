using UnityEngine;
using System.Collections;

public class MeshCombineUtility
{
    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
    }

    public static Mesh Combine(MeshInstance[] combines, bool generateStrips = false)
    {
        int vertexCount = 0;
        int triangleCount = 0;

        // 统计顶点和三角形数量
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                vertexCount += combine.mesh.vertexCount;
                triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
            }
        }

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector4[] tangents = new Vector4[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        Vector2[] uv1 = new Vector2[vertexCount];
        int[] triangles = new int[triangleCount];

        int offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 invTranspose = combine.transform.inverse.transpose;
                CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
            }
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 invTranspose = combine.transform.inverse.transpose;
                CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
            }
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.uv2, uv1, ref offset);
        }

        int triangleOffset = 0;
        int vertexOffset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                int[] inputTriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
                for (int i = 0; i < inputTriangles.Length; i++)
                {
                    triangles[i + triangleOffset] = inputTriangles[i] + vertexOffset;
                }
                triangleOffset += inputTriangles.Length;
                vertexOffset += combine.mesh.vertexCount;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Combined Mesh";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.uv2 = uv1;
        mesh.triangles = triangles;

        return mesh;
    }

    static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyPoint(src[i]);
        offset += vertexcount;
    }

    static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
        offset += vertexcount;
    }

    static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = src[i];
        offset += vertexcount;
    }

    static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
        {
            Vector4 p4 = src[i];
            Vector3 p = new Vector3(p4.x, p4.y, p4.z);
            p = transform.MultiplyVector(p);
            dst[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
        }

        offset += vertexcount;
    }
}
