using System;
using System.Collections;
using Extension;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "New MeshHideAsset", menuName = "MeshHideAsset", order = 52)]
public class MeshHideAsset : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    [SerializeField] private bool[] _serializedFlags;
    private BitArray _triangleFlags;

    [SerializeField] private int _neededHexLen = -1;
    [SerializeField] private string _optimisationSerializeFlags;

    public SkinnedMeshRenderer MeshRenderer
    {
        get => _meshRenderer;
        set => _meshRenderer = value;
    }

    public BitArray TriangleFlags => _triangleFlags;

    public int TriangleCount
    {
        get
        {
            if (_meshRenderer == null || _meshRenderer.sharedMesh == null)
                return 0;
            return _meshRenderer.sharedMesh.triangles.Length / 3;
        }
    }

    public int HiddenCount
    {
        get
        {
            if (_triangleFlags == null)
                return 0;
            return _triangleFlags.GetCardinality();
        }
    }

    public string OptimizedFlags => _optimisationSerializeFlags;
    public int NeededHexLen => _neededHexLen;

    public void Initialize()
    {
        if (_meshRenderer == null)
        {
            _triangleFlags = null;
            return;
        }

        if (_meshRenderer.sharedMesh == null)
            return;

        _triangleFlags = new BitArray(TriangleCount);
    }

    public void SaveSelection(BitArray selection)
    {
        _triangleFlags = new BitArray(selection);

        _serializedFlags = new bool[TriangleCount];
        _triangleFlags.CopyTo(_serializedFlags, 0);

        _neededHexLen = _triangleFlags.Length;
        _optimisationSerializeFlags = GetOptimizeFlags(_triangleFlags);
    }

    public Mesh GetNotHideMesh()
    {
        var tris = _meshRenderer.sharedMesh.triangles;
        var selectedCount = _triangleFlags.GetCardinality();
        var newSelectedTriangles = new int[tris.Length - selectedCount * 3];

        var selectedIndex = 0;
        for (int i = 0; i < _triangleFlags.Length; i++)
        {
            if (!_triangleFlags[i])
            {
                newSelectedTriangles[selectedIndex + 0] = tris[(i * 3) + 0];
                newSelectedTriangles[selectedIndex + 1] = tris[(i * 3) + 1];
                newSelectedTriangles[selectedIndex + 2] = tris[(i * 3) + 2];

                selectedIndex += 3;
            }
        }

        var result = _meshRenderer.sharedMesh.GetCopy();
        result.SetTriangles(newSelectedTriangles, 0);
        return result;
    }

    public Mesh GetInitialMesh() => _meshRenderer.sharedMesh.GetCopy();


    public void OnBeforeSerialize()
    {
        // if (_triangleFlags == null) return;
        // if (TriangleCount > 0)
        //     _serializedFlags = new bool[TriangleCount];
        //
        // _triangleFlags.CopyTo(_serializedFlags, 0);
        //
        // if (_serializedFlags == null)
        //     Debug.LogError("Ошибка сериализации!");
    }

    public void OnAfterDeserialize()
    {
        if (_serializedFlags == null) return;
        _triangleFlags = new BitArray(_serializedFlags);

        var test = GetBitArrayFromOptimizeFlags(_optimisationSerializeFlags, _neededHexLen);
    }

    // neededLen всегда <= bitArray.Length
    private BitArray GetBitArrayFromOptimizeFlags(string hex, int neededLen)
    {
        if (string.IsNullOrEmpty(hex) || neededLen < 0)
            return null;

        var bitArray = hex.ToBitArray();

        if (neededLen > bitArray.Length)
            return null;

        if (bitArray.Length == neededLen)
            return bitArray;
        else
        {
            var removedLen = bitArray.Length - neededLen;
            return bitArray.RemoveBitsInRange(0, removedLen);
        }
    }

    private string GetOptimizeFlags(BitArray bitArray)
    {
        if (bitArray is null)
            return null;

        var r = bitArray.Length % 4;
        if (r != 0)
            bitArray = bitArray.AddInsignificantZeros(r);
        return bitArray.ToHex();
    }
}