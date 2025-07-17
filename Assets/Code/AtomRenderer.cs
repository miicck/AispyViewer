using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AtomData
{
    public Vector3 position;
    public string species;

    public Vector4 color
    {
        get
        {
            UnityEngine.Random.InitState(species.GetHashCode());
            return new Vector4(
                UnityEngine.Random.Range(0, 1f),
                UnityEngine.Random.Range(0, 1f),
                UnityEngine.Random.Range(0, 1f),
                1f
            );
        }
    }
}

public class AtomRenderer : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private List<AtomData> atoms;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer dataBuffer;

    private struct MeshData
    {
        public float4 color;
        public float4 position;
    }

    const int MeshDataSize = sizeof(float) * (4 + 4);

    public void SetAtoms(List<AtomData> atoms)
    {
        FreeBuffers();
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        this.atoms = atoms;
        uint[] args = { 0, 0, 0, 0, 0, 0 };

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        var data = new MeshData[atoms.Count];
        for (var i = 0; i < atoms.Count; i++)
        {
            //var obj = new GameObject("atom " + i);
            //obj.transform.position = atoms[i].position;
            //obj.transform.SetParent(transform);

            var pos = new Vector4(
                atoms[i].position.x,
                atoms[i].position.y,
                atoms[i].position.z,
                0f
            );

            data[i] = new MeshData
            {
                color = atoms[i].color,
                position = pos,
            };
        }

        dataBuffer = new ComputeBuffer(atoms.Count, MeshDataSize);
        dataBuffer.SetData(data);
        material.SetBuffer("data", dataBuffer);

        // Verts
        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)atoms.Count;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);

        argsBuffer.SetData(args);
    }

    private void Start()
    {

    }

    void RandomColorSlab()
    {
        var atoms = new List<AtomData>();

        int[] size = new int[] { 100, 10, 100 };

        for (int x = 0; x < size[0]; ++x)
            for (int y = 0; y < size[1]; ++y)
                for (int z = 0; z < size[2]; ++z)
                {
                    var a = new AtomData();
                    a.position = new Vector3(x, y, z);
                    a.species = "abcdefghijklmnopqrstuvwxyz"[UnityEngine.Random.Range(0, 26)].ToString();
                    atoms.Add(a);
                }

        SetAtoms(atoms);
    }

    private void Update()
    {
        if (atoms == null)
            return;
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 1000), argsBuffer);
    }

    void FreeBuffers()
    {
        dataBuffer?.Release();
        argsBuffer?.Release();
        dataBuffer = null;
        argsBuffer = null;
        atoms = null;
    }

    private void OnDestroy()
    {
        FreeBuffers();
    }
}
