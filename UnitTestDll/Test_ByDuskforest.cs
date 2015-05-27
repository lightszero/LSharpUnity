using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTest;
using System.Collections;

namespace UnitTestDll
{
    class Test_ByDuskforest
    {
        public static void UnitTest_String()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>()
            {
                {0, "今天天气不错风和日丽鸟语花香"}
            };

            int a = 5;
            float b = 5.0f;
            byte c = 5;
            string d = "5";

            string test1 = dic[0] + "-" + a + "(" + dic[0];
            string test2 = dic[0] + "-" + b + "(" + dic[0];
            string test3 = dic[0] + "-" + c + "(" + dic[0];
            string test4 = dic[0] + "-" + d + "(" + dic[0];

            UnityEngine.Debug.LogError(test1);
            UnityEngine.Debug.LogError(test2);
            UnityEngine.Debug.LogError(test3);
            UnityEngine.Debug.LogError(test4);
        }
        
        public static void UnitTest_ByteCast()
        {
            Hashtable hash = new Hashtable();
            byte a = 2;
            hash.Add("state", a);
            byte index = (byte)hash["state"];
            
            switch(index)
            {
                case 1:
                    break;
                case 2:
                    UnityEngine.Debug.Log("a=" + a);
                    break;
                default:
                    break;
            }
        }
        
        //2015.4.22 by ldw
        public static void UnitTest_mousePosition()
        {
            float a = MyClass2.GetInputMousePos().x;
            UnityEngine.Debug.Log("a=" + a);
        }

        public static void UnitTest_Bool2Object()
        {
            TestClass.TestObjectArg(true);
        }

        public static void UnitTest_Int2Byte()
        {
            TestClass.TestByteArg(32);
        }
        public static void UnitTest_CtorWithNoArg()
        {
            System.Activator.CreateInstance(Type.GetType("TestClass,Assembly-CSharp-firstpass"));
        }

        public static void UnitTest_CtorWithByteArg()
        {
            System.Activator.CreateInstance(Type.GetType("TestClass,Assembly-CSharp-firstpass"), new Object[] { (byte)32 });
        }

        public static void UnitTest_2DimArray()
        {
            int[,] a = new int[1, 1];
            a[0, 0] = 4;
            Logger.Log("abc=" + a[0, 0]);
        }

        private static Object ReturnObject()
        {
            return 100;
        }

        public static IEnumerator UnitTest_Yield()
        {
            yield return null;
        }

        public static void UnitTest_ConvertReturnObject2Float()
        {
            //在.net下不会报错，但是在unity3d4下会报错，unity3d5又不报错了！
            if ((float)ReturnObject() != 0)
            {
                Logger.Log("100 != 0");
            }
        }
        public static void UnitTest_ConvertReturnObject2Double()
        {
            //在.net下不会报错，但是在unity3d4下会报错，unity3d5又不报错了！
            if ((double)ReturnObject() != 0)
            {
                Logger.Log("100 != 0");
            }
        }
    }
}
