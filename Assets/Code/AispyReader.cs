using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.PlayerLoop;

public class AispyReader : MonoBehaviour
{
    public string path = "\\\\wsl.localhost\\Ubuntu\\home\\mick\\calculations\\aispy\\defects\\defect_mc";
    public AtomRenderer atomRenderer;
    List<List<AtomData>> frames;
    int currentFrame = 0;
    bool paused = true;

    void Start()
    {
        frames = new List<List<AtomData>>();

        Debug.Log("Loading data in " + path);
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist!");
            return;
        }

        Actions.actions.Player.NextFrame.performed += ctx => SwitchFrame(true);
        Actions.actions.Player.PreviousFrame.performed += ctx => SwitchFrame(false);
        Actions.actions.Player.PlayPause.performed += ctx => paused = !paused;

        // Load first frame
        SwitchFrame(true);
    }

    bool LoadNextFrame()
    {
        int i = frames.Count;
        string fname = path + "\\frame_" + i + ".sites";
        if (!File.Exists(fname))
            return false;

        Debug.Log("Parsing frame " + i);

        List<AtomData> frame = new List<AtomData>();
        string[] lines = File.ReadAllLines(fname);
        foreach (var l in lines)
        {
            var s = l.Split();
            if (s.Length != 4)
                continue;

            Vector3 p = Vector3.zero;
            if (!float.TryParse(s[1], out p.x))
                continue;
            if (!float.TryParse(s[2], out p.y))
                continue;
            if (!float.TryParse(s[3], out p.z))
                continue;

            p += new Vector3(
                Random.Range(0, 1f),
                Random.Range(0, 1f),
                Random.Range(0, 1f)
            ) * 0.1f;

            frame.Add(new AtomData
            {
                position = p,
                species = s[0]
            });
        }

        Debug.Log("Frame " + i + " has " + frame.Count + " sites");

        frames.Add(frame);
        return true;
    }

    void SwitchFrame(bool forward)
    {
        LoadNextFrame();

        currentFrame += forward ? 1 : -1;
        if (currentFrame < 0)
            currentFrame = frames.Count - 1;
        if (currentFrame > frames.Count - 1)
            currentFrame = 0;

        atomRenderer.SetAtoms(frames[currentFrame]);
    }

    private void FixedUpdate()
    {
        if (!paused)
            if (Time.frameCount % 10 == 0)
                SwitchFrame(true);
    }
}
