using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class XYZReader : Reader
{
    public string path = "C:\\Users\\micha\\Documents\\1en1.xyz";

    private List<AtomData> _atoms;
    public override List<AtomData> atoms => _atoms;

    void Start()
    {
        _atoms = Load();
        atomRenderer.dirty = true;
    }

    List<AtomData> Load()
    {
        if (!File.Exists(path))
            return new List<AtomData>();

        var atoms = new List<AtomData>();

        string[] lines = File.ReadAllLines(path);
        foreach (var l in lines)
        {
            var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (s.Length != 4)
                continue;

            Vector3 p = Vector3.zero;
            if (!float.TryParse(s[1], out p.x))
                continue;
            if (!float.TryParse(s[2], out p.y))
                continue;
            if (!float.TryParse(s[3], out p.z))
                continue;

            atoms.Add(new AtomData
            {
                position = p + transform.position,
                species = s[0]
            });
        }

        return atoms;
    }
}
