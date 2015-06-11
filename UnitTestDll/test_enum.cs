using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTest;
using System.Reflection;
using UnityEngine;

namespace UnitTestDll
{
    class test_enum
    {
        public enum EUIPanelID
        {
            NULL=0,
            INT1=1,
            INT2,
        };

        public static void UnitTest_01()
        {
            Dictionary<EUIPanelID, string> dict = new Dictionary<EUIPanelID, string>();

            bool bo = dict.ContainsKey(EUIPanelID.INT1);
        }
        
        public static void UnitTest_02()
        {
            Test_02(EUIPanelID.INT1);
        }
        static void Test_02(EUIPanelID id)
        {
            Logger.Log("test_enum.UnitTest_02");
        }

        static void UnitTest_03()
        {
            //system type  å’?l# type  ä¸èƒ½ä¸€èµ·æž
            //Type type = typeof(EUIPanelID);
        }


        static void UnitTest_07()
        {
            LinkedList<EUIPanelID> ll = new LinkedList<EUIPanelID>();
            bool b = ll.Contains(EUIPanelID.INT1);
        }
        
        static void UnitTest_09()
        {
            bool[] m_IsDataReady = new bool[4] { false, false, false, false };
            int len = m_IsDataReady.Length;
            bool tmp = m_IsDataReady[0];
            //tmp = m_IsDataReady[1];
            //tmp = m_IsDataReady[2];
            //tmp = m_IsDataReady[3];

        }
        static void UnitTest_10()
        {
            int[] m_IsDataReady = new int[4] { 1, 1, 1, 1 };
            int len = m_IsDataReady.Length;
            int tmp = m_IsDataReady[0];
            //tmp = m_IsDataReady[1];
            //tmp = m_IsDataReady[2];
            //tmp = m_IsDataReady[3];

        }
        static void UnitTest_11()
        {
            float[] m_IsDataReady = new float[4] { 1f, 1f, 1f, 1f };
            int len = m_IsDataReady.Length;
            float tmp = m_IsDataReady[0];
            //tmp = m_IsDataReady[1];
            //tmp = m_IsDataReady[2];
            //tmp = m_IsDataReady[3];

        }
        static void UnitTest_12()
        {
            string[] m_IsDataReady = new string[4] { "1", "1", "1", "1" };
            int len = m_IsDataReady.Length;
            string tmp = m_IsDataReady[0];
            //tmp = m_IsDataReady[1];
            //tmp = m_IsDataReady[2];
            //tmp = m_IsDataReady[3];

        }
        
        static void UnitTest_13()
        {
            bool[] data = new bool[4] { true,true,true,true};
            data[0] = false;
        }

        static void UnitTest_17_TestCSharpEnum()
        {
            MyTestClassA a = new MyTestClassA(CSharpEnum.Zero);
            //Debug.LogWarning("v=" + a.data);
        }
        static void UnitTest_18()
		{
			Vector3 pos = new Vector3(2, 2, 2);
            Vector3 post = new Vector3(3,3,3);
            for (int i = 0; i != 2; ++i)
            {
                if (i == 0)
                {
                    Vector3 pos1 = Vector3.zero;
                    pos = pos1;
                }
                else if (i == 1)
                {
                    Vector3 pos1;
                    pos1.x = post.x;
                    pos1.y = post.y;
                    pos1.z = post.z;
                }
                Debug.LogWarning(pos.x);
            }
		}
		
	static void UnitTest_19()
        {
            TestEvent ca = new TestEvent();
            ca.Test();
        }

        class TestEvent
        {
            public event VoidDelegateBool MyTestEvent;

            public void Test()
            {
                MyTestEvent += VoidDelegateBool;
            }
            void VoidDelegateBool(bool data)
            { }
        }

    } 


            
}
