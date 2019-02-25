using UnityEngine;


public class ClassTest : MonoBehaviour
{
    public int _a = 0;
    public int _a2 = 0;
    protected int _b = 0;
    private int _c = 0;

    public int a
    {
        get
        {
            return _a;
        }
    }

    public int a2
    {
        get
        {
            return _a2;
        }

        set
        {
            _a2 = value;
        }
    }

    protected int b
    {
        get
        {
            return _b;
        }
    }

    private int c
    {
        get
        {
            return _c;
        }
    }
}

