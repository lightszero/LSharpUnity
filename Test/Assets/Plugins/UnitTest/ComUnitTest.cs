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
    List<string> log2 = new List<string>();
    int logmode = 1;
    public void Log(string str)
    {
        if (logmode == 0)
            return;
        if (logmode == 2)
            log2.Add(str);
        Debug.Log(str);
    }

    public void Log_Warning(string str)
    {
        if (logmode == 0)
            return;
        if (logmode == 2)
            log2.Add("<W>" + str);
        Debug.LogWarning(str);
    }

    public void Log_Error(string str)
    {
        if (logmode == 0)
            return;
        if (logmode == 2)
            log2.Add("<E>" + str);
        Debug.LogError(str);
    }

    bool bHideRight = false;
    Vector2 pos = Vector2.zero;
    Vector2 pos2 = Vector2.zero;
    Vector2 pos3 = Vector2.zero;
    string dump = "";
    TestItem setitem = null;
    bool showdump = false;
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            pos = GUILayout.BeginScrollView(pos, false, true, GUILayout.Width(Screen.width * 0.4f));
            foreach (var t in tests)
            {

                if (t.bSucc && bHideRight)
                    continue;
                GUILayout.BeginHorizontal();
                if (t.bSucc)
                    GUI.contentColor = Color.green;
                else if (t.bError)
                    GUI.contentColor = Color.red;
                else
                    GUI.contentColor = Color.white;
                GUILayout.Label(t.ToString());
                if (GUILayout.Button("Test", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    dump = "";
                    logmode = 2;
                    log2.Clear();
                    try
                    {
                        setitem = t;
                        TestOne(t.m, true, false);
                    }
                    catch (Exception err)
                    {
                        dump = CLRSharp.ThreadContext.activeContext.Dump();
                        Debug.LogError(dump);
                        Debug.LogError(err.ToString());
                        log2.Add(err.ToString());
                    }
                    logmode = 1;
                }
                if (GUILayout.Button("Test No Try", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    dump = "";
                    logmode = 2;
                    log2.Clear();
                    try
                    {
                        setitem = t;
                        TestOne(t.m, true, true);
                    }
                    catch (Exception err)
                    {
                        dump = CLRSharp.ThreadContext.activeContext.Dump();
                        Debug.LogError(dump);
                        Debug.LogError(err.ToString());
                        log2.Add(err.ToString());
                    }
                    logmode = 1;
                }
                GUILayout.EndHorizontal();
            }
            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();

        }
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("TestAll", GUILayout.Width(200), GUILayout.Height(80)))
                {
                    TestAll();
                }
                if (GUILayout.Button("Hide Right", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    bHideRight = !bHideRight;
                }

                pos2 = GUILayout.BeginScrollView(pos2, false, true, GUILayout.Height(Screen.height * 0.4f));
                {
                    if (setitem != null && setitem.m != null)
                    {
                        var m = setitem.m as CLRSharp.Method_Common_CLRSharp;
                        if (m != null)
                            foreach (var c in m.method_CLRSharp.body.instructions)
                            {
                                var l = c.ToString();
                                if (c.SequencePoint != null)
                                {
                                    l += "|" + c.SequencePoint.Document.Url + "(" + c.SequencePoint.StartLine + ")";
                                }
                                GUILayout.Label(l);
                            }
                    }
                }
                GUILayout.EndScrollView();
                if (GUILayout.Button("showDump/Stack", GUILayout.Width(100), GUILayout.Height(40)))
                {
                    showdump = !showdump;
                }
                pos3 = GUILayout.BeginScrollView(pos3, false, true);
                {
                    if (showdump)
                    {
                        string[] pp = dump.Split('\n');
                        foreach (var p in pp)
                        {
                            GUILayout.Label(p);
                        }

                    }
                    else
                    {
                        for (int i = log2.Count - 1; i >= 0; i--)
                        {
                            string l = log2[i];
                            if (l.Contains("<W>"))
                            {
                                GUI.contentColor = Color.yellow;
                            }
                            else if (l.Contains("<E>"))
                            {
                                GUI.contentColor = Color.red;
                            }
                            else
                            {
                                GUI.contentColor = Color.white;
                            }
                            GUILayout.Label(l.ToString());
                        }

                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

    }
    class TestItem
    {
        public TestItem(CLRSharp.ICLRType type, string method)
        {
            this.type = type;
            this.method = method;
        }
        public CLRSharp.IMethod m
        {
            get
            {
                return type.GetMethod(method, CLRSharp.MethodParamList.constEmpty());
            }
        }
        CLRSharp.ICLRType type;
        public string method;
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

        //var bytes = Resources.Load<TextAsset>("unittestdll.dll").bytes;
        //var bytespdb = Resources.Load<TextAsset>("unittestdll.pdb").bytes;
        var bytes = Resources.Load<TextAsset>("unittestdll2.dll").bytes;
        var bytesmdb = Resources.Load<TextAsset>("unittestdll2.dll.mdb").bytes;//现在支持mdb了
        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
        //System.IO.MemoryStream mspdb = new System.IO.MemoryStream(bytespdb);
        System.IO.MemoryStream msmdb = new System.IO.MemoryStream(bytesmdb);

        //Log(" L# Ver:" + env.version);
        try
        {
            //env.LoadModule(ms, mspdb, new Mono.Cecil.Pdb.PdbReaderProvider());
            env.LoadModule(ms, msmdb, new Mono.Cecil.Mdb.MdbReaderProvider());
        }
        catch (Exception err)
        {
            Log_Error(err.ToString());
            Log_Error("模块未加载完成，请检查错误");
        }

        ResetTest();
        Log(" Got Test:" + tests.Count);

        //for aot
        env.GetType(typeof(Dictionary<int, string>));
        env.GetType(typeof(Dictionary<int, object>));
        env.GetType(typeof(Dictionary<int, CLRSharp.CLRSharp_Instance>));
        env.GetType(typeof(Dictionary<int, Action>));
        env.GetType(typeof(Dictionary<int, Action<MyClass2>>));
        env.GetType(typeof(Dictionary<Int16, Action>));
        env.GetType(typeof(LinkedList<int>));
        env.GetType(typeof(int[,]));
        env.GetType(typeof(List<CLScriptExt.Student>));
        env.GetType(typeof(List<Vector3>));
        env.GetType(typeof(List<int>[]));
        env.GetType(typeof(List<List<int>>));
        env.GetType(typeof(List<List<List<int>>>));
        env.GetType(typeof(Vector3[]));
        env.GetType(typeof(System.Collections.Generic.IEnumerable<int>));

        TestDele.instance.AddDeleT3<int, string>(null);
        CLScriptExt.Student s = new CLScriptExt.Student();
        s.ToString2<int>(2);//call once will help.
        CLScriptExt.P2.TestS2<int, int>();
        //for aot dele
        CLRSharp.Delegate_Binder.RegBind(typeof(Action<int>), new CLRSharp.Delegate_BindTool<int>());
        CLRSharp.Delegate_Binder.RegBind(typeof(Action<MyClass2>), new CLRSharp.Delegate_BindTool<MyClass2>());
        CLRSharp.Delegate_Binder.RegBind(typeof(Action<int, int>), new CLRSharp.Delegate_BindTool<int, int>());
        CLRSharp.Delegate_Binder.RegBind(typeof(Action<int, int, int>), new CLRSharp.Delegate_BindTool<int, int, int>());
        CLRSharp.Delegate_Binder.RegBind(typeof(Func<int, int, int>), new CLRSharp.Delegate_BindTool_Ret<int, int, int>());
        CLRSharp.Delegate_Binder.RegBind(typeof(Action<int, string>), new CLRSharp.Delegate_BindTool<int, string>());
        CLRSharp.Delegate_Binder.RegBind(typeof(Action<string>), new CLRSharp.Delegate_BindTool<string>());
    }

    private void ResetTest()
    {
        tests.Clear();
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
                        if (mm.Name.IndexOf("UnitTest") == 0)
                            tests.Add(new TestItem(tclr, m.Name));
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
                                    tests.Add(new TestItem(tclr, m.Name));
                            }
                        }
                    }
                }

            }

        }
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
        logmode = 0;
        Log_Warning("TestAll.");
        //reset
        var types = env.GetAllTypes();
        foreach (var t in types)
        {
            CLRSharp.ICLRType_Sharp type = env.GetType(t) as CLRSharp.ICLRType_Sharp;
            if (type != null)
            {
                type.ResetStaticInstace();
                Debug.Log("Reset:" + t);
            }
        }
        int finish = 0;
        //string text = "";
        foreach (var m in tests)
        {
            if (m.bSucc && bHideRight) continue;
            try
            {
                if (m.method.Contains("UnitTest_TestThread")) continue;
                //text += "Begin==" + m.method + "==\n";
                //System.IO.File.WriteAllText("e:\\log.txt", text);
                TestOne(m.m, false, false);
                finish++;
                m.bSucc = true;
                //text += "<succ>==" + m.method + "==\n";
                //System.IO.File.WriteAllText("e:\\log.txt", text);
                m.bError = false;
            }
            catch (Exception err)
            {
                //text += "<fail>==" + m.method + "==\n";
                //System.IO.File.WriteAllText("e:\\log.txt", text);

                m.bError = true;
                m.bSucc = false;
                Log_Error(m.ToString() + "|||" + err.ToString());
            }
        }
        Log_Warning("TestAllEnd. Count:" + finish + "/" + tests.Count);
        logmode = 1;
    }


    object TestOne(CLRSharp.IMethod method, bool LogStep = false, bool notry = false)
    {


        int debug = LogStep ? 9 : 0;
        //if (CLRSharp.ThreadContext.activeContext == null)
        {
            CLRSharp.ThreadContext context = new CLRSharp.ThreadContext(env, debug);
        }
        CLRSharp.ThreadContext.activeContext.SetNoTry = notry;
        return method.Invoke(CLRSharp.ThreadContext.activeContext, null, null);
    }

}
