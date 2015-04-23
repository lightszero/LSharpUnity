using System;
using System.Collections.Generic;

using System.Reflection.Emit;
using System.Text;
using UnityEngine;



public class MyClass2
{
    public static Vector3 GetInputMousePos()
    {
        return UnityEngine.Input.mousePosition;
    }
}
public class config
{
    public static int Cell(double i)
    {
        return (int)i;
    }
}

