using CLRSharp;
using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;


class TypeForGameObject : Type_Common_System
{
    public TypeForGameObject(ICLRSharp_Environment env)
        : base(env, typeof(GameObject), null)
    {
        methodExt.Add( new MethodCreatePrimitive(env, this));
        methodExt.Add(new MethodFind(env, this));
    }
    public override IMethod GetMethod(string funcname, MethodParamList types)
    {
        int typehash = types.GetHashCode();
        foreach(var f in methodExt)
        {
            if (f.Name == funcname && f.ParamList.GetHashCode() == typehash)
                return f;
        }

        return base.GetMethod(funcname, types);
    }
    List<IMethod> methodExt = new List<IMethod>();
    class MethodCreatePrimitive : IMethod
    {
        public ICLRSharp_Environment env;
        public MethodCreatePrimitive(ICLRSharp_Environment env, ICLRType type)
        {
            this.env = env;
            this.DeclaringType = type;
            this.ReturnType = env.GetType(typeof(GameObject));
            ParamList = MethodParamList.Make(env.GetType(typeof(PrimitiveType)));
        }
        public object Invoke(ThreadContext context, object _this, object[] _params)
        {
            return Invoke(context, _this, _params, false, false);
        }

        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual)
        {
            return Invoke(context, _this, _params, bVisual, false);
        }

        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump)
        {
            return GameObject.CreatePrimitive((PrimitiveType)_params[0]);
        }

        public bool isStatic
        {
            get { return true; }
        }

        public string Name
        {
            get { return "CreatePrimitive"; }
        }

        public ICLRType DeclaringType
        {
            get;
            private set;
        }

        public ICLRType ReturnType
        {
            get;
            private set;
        }

        public MethodParamList ParamList
        {
            get;
            private set;
        }
    }
    class MethodFind : IMethod
    {
        public ICLRSharp_Environment env;
        public MethodFind(ICLRSharp_Environment env, ICLRType type)
        {
            this.env = env;
            this.DeclaringType = type;
            this.ReturnType = env.GetType(typeof(GameObject));
            ParamList = MethodParamList.Make(env.GetType(typeof(string)));
        }
        public object Invoke(ThreadContext context, object _this, object[] _params)
        {
            return Invoke(context, _this, _params, false, false);
        }

        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual)
        {
            return Invoke(context, _this, _params, bVisual, false);
        }

        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump)
        {
            return GameObject.Find((string)_params[0]);
        }

        public bool isStatic
        {
            get { return true; }
        }

        public string Name
        {
            get { return "Find"; }
        }

        public ICLRType DeclaringType
        {
            get;
            private set;
        }

        public ICLRType ReturnType
        {
            get;
            private set;
        }

        public MethodParamList ParamList
        {
            get;
            private set;
        }
    }

}

