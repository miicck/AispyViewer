using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Level8 : MonoBehaviour
{
    [SerializeField] private Mesh _instanceMesh;
    [SerializeField] private Material _instanceMaterial;
    [SerializeField] private ComputeShader _compute;
    [SerializeField] private int _count = 1000;
    [SerializeField] private Transform _pusher;

    private readonly uint[] _args = { 0, 0, 0, 0, 0 };
    private ComputeBuffer _argsBuffer;
    private int _kernel;
    private List<AtomData> atoms;

    private ComputeBuffer _meshPropertiesBuffer;
    private struct MeshData
    {
        public float3 BasePos;
        public Matrix4x4 Mat;
        public float Amount;
    }

    public void SetAtoms(List<AtomData> atoms)
    {
        this.atoms = atoms;
        _kernel = _compute.FindKernel("cs_main");
        _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        var data = new MeshData[atoms.Count];
        for (var i = 0; i < atoms.Count; i++)
        {
            var pos = atoms[i].position;
            var rot = Quaternion.Euler(Random.insideUnitSphere.normalized);
            var sca = Vector3.one;

            data[i] = new MeshData
            {
                BasePos = pos,
                Mat = Matrix4x4.TRS(pos, rot, sca),
                Amount = 0
            };
        }

        _meshPropertiesBuffer = new ComputeBuffer(atoms.Count, 80);
        _meshPropertiesBuffer.SetData(data);

        _compute.SetBuffer(_kernel, "data", _meshPropertiesBuffer);
        _instanceMaterial.SetBuffer("data", _meshPropertiesBuffer);

        // Verts
        _args[0] = _instanceMesh.GetIndexCount(0);
        _args[1] = (uint)atoms.Count;
        _args[2] = _instanceMesh.GetIndexStart(0);
        _args[3] = _instanceMesh.GetBaseVertex(0);

        _argsBuffer.SetData(_args);
    }

    private void Start()
    {
        var atoms = new List<AtomData>();

        int[] size = new int[] { 100, 10, 100 };

        for (int x = 0; x < size[0]; ++x)
            for (int y = 0; y < size[1]; ++y)
                for (int z = 0; z < size[2]; ++z)
                {
                    var a = new AtomData();
                    a.position = new Vector3(x, y, z);
                    atoms.Add(a);
                }

        SetAtoms(atoms);
    }

    private void Update()
    {
        _compute.SetVector("pusher_position", _pusher.position);
        _compute.Dispatch(_kernel, Mathf.CeilToInt(this.atoms.Count / 64f), 1, 1);
        Graphics.DrawMeshInstancedIndirect(_instanceMesh, 0, _instanceMaterial, new Bounds(Vector3.zero, Vector3.one * 1000), _argsBuffer);
    }

    private void StartOld()
    {
        _kernel = _compute.FindKernel("cs_main");
        _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    private void UpdateOld()
    {
        _compute.SetVector("pusher_position", _pusher.position);
        _compute.Dispatch(_kernel, Mathf.CeilToInt(_count / 64f), 1, 1);
        Graphics.DrawMeshInstancedIndirect(_instanceMesh, 0, _instanceMaterial, new Bounds(Vector3.zero, Vector3.one * 1000), _argsBuffer);
    }

    private void OnDisable()
    {
        _argsBuffer?.Release();
        _argsBuffer = null;

        _meshPropertiesBuffer?.Release();
        _meshPropertiesBuffer = null;
    }

    private void UpdateBuffers()
    {
        var offset = Vector3.zero;
        var data = new MeshData[_count];
        for (var i = 0; i < _count; i++)
        {
            var pos = Random.insideUnitSphere.normalized * Random.Range(10, 50) + offset;
            var rot = Quaternion.Euler(Random.insideUnitSphere.normalized);
            var sca = Vector3.one;

            data[i] = new MeshData
            {
                BasePos = pos,
                Mat = Matrix4x4.TRS(pos, rot, sca),
                Amount = 0
            };
        }

        _meshPropertiesBuffer = new ComputeBuffer(_count, 80);
        _meshPropertiesBuffer.SetData(data);

        _compute.SetBuffer(_kernel, "data", _meshPropertiesBuffer);
        _instanceMaterial.SetBuffer("data", _meshPropertiesBuffer);

        // Verts
        _args[0] = _instanceMesh.GetIndexCount(0);
        _args[1] = (uint)_count;
        _args[2] = _instanceMesh.GetIndexStart(0);
        _args[3] = _instanceMesh.GetBaseVertex(0);

        _argsBuffer.SetData(_args);
    }


}