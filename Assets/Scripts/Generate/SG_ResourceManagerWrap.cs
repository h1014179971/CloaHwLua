﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class SG_ResourceManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(SG.ResourceManager), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("InitPool", InitPool);
		L.RegFunction("GetObjectFromPool", GetObjectFromPool);
		L.RegFunction("ReturnObjectToPool", ReturnObjectToPool);
		L.RegFunction("ReturnTransformToPool", ReturnTransformToPool);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitPool(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				obj.InitPool(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				SG.PoolInflationType arg2 = (SG.PoolInflationType)ToLua.CheckObject(L, 4, typeof(SG.PoolInflationType));
				obj.InitPool(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: SG.ResourceManager.InitPool");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetObjectFromPool(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				UnityEngine.GameObject o = obj.GetObjectFromPool(arg0);
				ToLua.PushSealed(L, o);
				return 1;
			}
			else if (count == 3)
			{
				SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
				UnityEngine.GameObject o = obj.GetObjectFromPool(arg0, arg1);
				ToLua.PushSealed(L, o);
				return 1;
			}
			else if (count == 4)
			{
				SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
				int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);
				UnityEngine.GameObject o = obj.GetObjectFromPool(arg0, arg1, arg2);
				ToLua.PushSealed(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: SG.ResourceManager.GetObjectFromPool");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReturnObjectToPool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			obj.ReturnObjectToPool(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReturnTransformToPool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			SG.ResourceManager obj = (SG.ResourceManager)ToLua.CheckObject<SG.ResourceManager>(L, 1);
			UnityEngine.Transform arg0 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			obj.ReturnTransformToPool(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.Push(L, SG.ResourceManager.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

