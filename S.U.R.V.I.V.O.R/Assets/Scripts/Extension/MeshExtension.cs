using UnityEngine;
using UnityEngine.Rendering;

public static class MeshExtension
{
    public static Mesh GetCopy(this Mesh mesh)
    {
        var newMesh = new Mesh
        {
            name = $"Copy {mesh.name}",
            bindposes = mesh.bindposes,
            subMeshCount = 1,
            vertices = mesh.vertices,
            normals = mesh.normals,
            tangents = mesh.tangents,
            uv = mesh.uv,
            uv2 = mesh.uv2,
            uv3 = mesh.uv3,
            uv4 = mesh.uv4,
            colors32 = mesh.colors32,
            boneWeights = mesh.boneWeights
        };
        
        newMesh.SetTriangles(mesh.triangles, 0);
        newMesh.RecalculateBounds();
        
        return newMesh;
    }
}