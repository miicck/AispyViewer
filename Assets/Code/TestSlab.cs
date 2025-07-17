using UnityEngine;

public class TestSlab : MonoBehaviour
{
    void Start()
    {
        Atom atom = Resources.Load<Atom>("Atom");

        int[] size = new int[] { 100, 10, 100 };

        for (int x = 0; x < size[0]; ++x)
            for (int y = 0; y < size[1]; ++y)
                for (int z = 0; z < size[2]; ++z)
                {
                    var a = Instantiate(atom);
                    a.transform.position = new Vector3(x, y, z);
                    a.transform.SetParent(transform);
                }
    }
}
