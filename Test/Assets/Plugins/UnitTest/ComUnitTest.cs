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
            foreach (var t in tests)
            {
                GUILayout.BeginHorizontal();
                if (t.bSucc)
                    GUI.contentColor = Color.green;
                else if (t.bError)
                    GUI.contentColor = Color.red;
                else
                    GUI.contentColor = Color.white;
                GUILayout.Label(t.ToString());
                if (GUILayout.Button("Test"))
                {
                    try
                    {
                        TestOne(t.m, true, false);
                    }
                    catch(Exception err)
                    {
                        Debug.LogError(CLRSharp.ThreadContext.activeContext.Dump());
                        Debug.LogError(err.ToString());
                    }
                }
                if (GUILayout.Button("Test No Try"))
                {
                    TestOne(t.m, true, true);
                }
                GUILayout.EndHorizontal();
            }
            GUI.contentColor = Color.white;
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
    class TestItem
    {
        public TestItem(CLRSharp.IMethod _m)
        {
            this.m = _m;
        }
        public CLRSharp.IMethod m;
        public bool bError = false;
        public bool bSucc = false;
        public override string ToString()
        {
            return m.DeclaringType.Name + ":::" + m.Name;
        }
    }
    List<TestItem> tests = new List<TestItem>();
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
                            tests.Add(new TestItem(mm));
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
                                    tests.Add(new TestItem(m));
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
            Log(i.ToString("D03") + " " + tests[i].ToString());
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
                TestOne(m.m, false, false);
                finish++;
                m.bSucc = true;
                m.bError = false;
            }
            catch (Exception err)
            {
                m.bError = true;
                m.bSucc = false;
                Log_Error(m.ToString() + "|||" + err.ToString());
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
