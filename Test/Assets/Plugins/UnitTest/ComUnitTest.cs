using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ComUnitTest : MonoBehaviour, CLRSharp.ICLRSharp_Logger
{

    // Use this for initialization
    void Start()
    {
        env = new CLRSharp.CLRSharp_Environment(this);
        InitTest();
    }
    public static CLRSharp.CLRSharp_Environment env = null;
    // Update is called once per frame
    void Update()
    {

    }

    public void Log(string str)
    {
        Debug.Log(str);
    }

    public void Log_Warning(string str)
    {
        Debug.LogWarning(str);
    }

    public void Log_Error(string str)
    {
        Debug.LogError(str);
    }
    Vector2 pos = Vector2.zero;
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            pos = GUILayout.BeginScrollView(pos);
            foreach(var t in tests)
            {
                GUILayout.Label(t.DeclaringType.Name + ":::" + t.Name);
            }
            GUILayout.EndScrollView();
        }
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("TestAll"))
                {
                    TestAll();
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

    }
    List<CLRSharp.IMethod> tests = new List<CLRSharp.IMethod>();
    void InitTest()
    {
        tests.Clear();

        var bytes = Resources.Load<TextAsset>("unittestdll.dll").bytes;
        var bytespdb = Resources.Load<TextAsset>("unittestdll.pdb").bytes;
        //var bytesmdb = System.IO.File.ReadAllBytes("UnitTestDll.dll.mdb");//现在支持mdb了
        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
        System.IO.MemoryStream mspdb = new System.IO.MemoryStream(bytespdb);
        //System.IO.MemoryStream msmdb = new System.IO.MemoryStream(bytesmdb);

        //Log(" L# Ver:" + env.version);
        try
        {
            env.LoadModule(ms, mspdb, new Mono.Cecil.Pdb.PdbReaderProvider());
            //env.LoadModule(ms, msmdb, new Mono.Cecil.Mdb.MdbReaderProvider());
        }
        catch (Exception err)
        {
            Log_Error(err.ToString());
            Log_Error("模块未加载完成，请检查错误");
        }
        var types = env.GetAllTypes();
        foreach (var t in types)
        {
            var tclr = env.GetType(t) as CLRSharp.Type_Common_CLRSharp;
            if (tclr != null && tclr.type_CLRSharp.HasMethods)
            {

                foreach (var m in tclr.type_CLRSharp.Methods)
                {
                    var mm = tclr.GetMethod(m.Name, CLRSharp.MethodParamList.constEmpty());
                    if (mm != null)
                    {
                        if (mm.Name.Contains("UnitTest"))
                            tests.Add(mm);
                    }
                }
                if (tclr.type_CLRSharp.HasNestedTypes)
                {

                    foreach (var ttt in tclr.type_CLRSharp.NestedTypes)
                    {
                        var tclr2 = env.GetType(ttt.FullName) as CLRSharp.Type_Common_CLRSharp;

                        foreach (var m in tclr2.GetAllMethods())
                        {
                            if (m.ParamList.Count == 0)
                            {
                                if (m.Name.Contains("UnitTest"))
                                    tests.Add(m);
                            }
                        }
                    }
                }

            }

        }
        Log(" Got Test:" + tests.Count);
    }
    void ListAll()
    {
        Log_Warning("ListALL.");

        for (int i = 0; i < tests.Count; i++)
        {
            Log(i.ToString("D03") + " " + tests[i].DeclaringType.Name + "|" + tests[i].Name);
        }
        Log_Warning("ListEnd. Count:" + tests.Count);
    }
    void TestAll()
    {
        Log_Warning("TestAll.");
        //reset
        var types = env.GetAllTypes();
        foreach (var t in types)
        {
            CLRSharp.ICLRType_Sharp type = env.GetType(t) as CLRSharp.ICLRType_Sharp;
            if (type != null)
                type.ResetStaticInstace();
        }
        int finish = 0;
        foreach (var m in tests)
        {
            try
            {
                TestOne(m, false, false);
                finish++;
            }
            catch (Exception err)
            {
                Log_Error(m.DeclaringType.Name + ":::" + m.Name + "|||" + err.ToString());
            }
        }
        Log_Warning("TestAllEnd. Count:" + finish + "/" + tests.Count);
    }


    object TestOne(CLRSharp.IMethod method, bool LogStep = false, bool notry = false)
    {


        int debug = LogStep ? 9 : 0;
        if (CLRSharp.ThreadContext.activeContext == null)
        {
            CLRSharp.ThreadContext context = new CLRSharp.ThreadContext(env, debug);
        }
        CLRSharp.ThreadContext.activeContext.SetNoTry = notry;
        return method.Invoke(CLRSharp.ThreadContext.activeContext, null, null);
    }

}
