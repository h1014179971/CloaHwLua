﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CommonUtilWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("CommonUtil");
		L.RegFunction("Initialize", Initialize);
		L.RegFunction("Dispose", Dispose);
		L.RegFunction("AddBtnMsg", AddBtnMsg);
		L.RegFunction("RemoveBtnMsg", RemoveBtnMsg);
		L.RegFunction("AddSingleComponent", AddSingleComponent);
		L.RegFunction("AddCustomComponent", AddCustomComponent);
		L.RegFunction("GetGameObjectByName", GetGameObjectByName);
		L.RegFunction("FindChildByPath", FindChildByPath);
		L.RegFunction("GetComponentByPath", GetComponentByPath);
		L.RegFunction("GetDeviceInfo", GetDeviceInfo);
		L.RegFunction("GetUIRootObj", GetUIRootObj);
		L.RegFunction("GetUIRootTransform", GetUIRootTransform);
		L.RegFunction("GetUICameraObj", GetUICameraObj);
		L.RegFunction("GetUIRoot", GetUIRoot);
		L.RegFunction("GetUICamera", GetUICamera);
		L.RegFunction("GetMainCamera", GetMainCamera);
		L.RegFunction("GetMainGameObj", GetMainGameObj);
		L.RegFunction("GetEffectCameraObj", GetEffectCameraObj);
		L.RegFunction("GetEffectCamera", GetEffectCamera);
		L.RegFunction("GetBatteryLevel", GetBatteryLevel);
		L.RegFunction("GetBatteryStatus", GetBatteryStatus);
		L.RegFunction("GetNetworkStatus", GetNetworkStatus);
		L.RegFunction("WorldToUIPoint", WorldToUIPoint);
		L.RegFunction("ActivedChildCount", ActivedChildCount);
		L.RegFunction("GetAssetPath", GetAssetPath);
		L.RegFunction("GetHostIp", GetHostIp);
		L.RegFunction("ChildCount", ChildCount);
		L.RegFunction("GetChild", GetChild);
		L.RegFunction("CheckLocalFileExist", CheckLocalFileExist);
		L.RegFunction("GetComponentsInChildren", GetComponentsInChildren);
		L.RegFunction("GetTerrainHeight", GetTerrainHeight);
		L.RegFunction("ShowUIBlur", ShowUIBlur);
		L.RegFunction("ShowUIMask", ShowUIMask);
		L.RegFunction("DestroyUIBlur", DestroyUIBlur);
		L.RegFunction("GetSceneMgr", GetSceneMgr);
		L.RegFunction("DelayInvokeNextFrame", DelayInvokeNextFrame);
		L.RegFunction("IsNull", IsNull);
		L.RegFunction("InstantiatePrefab", InstantiatePrefab);
		L.RegFunction("SetCapcitySize", SetCapcitySize);
		L.RegFunction("SetDisposeInterval", SetDisposeInterval);
		L.RegFunction("GetGameObject", GetGameObject);
		L.RegFunction("ReleaseGameObject", ReleaseGameObject);
		L.RegFunction("DiscardGameObject", DiscardGameObject);
		L.RegFunction("GetAsset", GetAsset);
		L.RegFunction("ReleaseAsset", ReleaseAsset);
		L.RegFunction("LoadTextWithBytes", LoadTextWithBytes);
		L.RegFunction("HandleMainCameraEvent", HandleMainCameraEvent);
		L.RegVar("NetAvailable", get_NetAvailable, null);
		L.RegVar("IsWifi", get_IsWifi, null);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Initialize(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			CommonUtil.Initialize();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Dispose(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			CommonUtil.Dispose();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddBtnMsg(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			System.Action<UnityEngine.GameObject> arg1 = (System.Action<UnityEngine.GameObject>)ToLua.CheckDelegate<System.Action<UnityEngine.GameObject>>(L, 2);
			CommonUtil.AddBtnMsg(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveBtnMsg(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			System.Action<UnityEngine.GameObject> arg1 = (System.Action<UnityEngine.GameObject>)ToLua.CheckDelegate<System.Action<UnityEngine.GameObject>>(L, 2);
			CommonUtil.RemoveBtnMsg(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddSingleComponent(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<System.Type>(L, 2))
			{
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				System.Type arg1 = (System.Type)ToLua.ToObject(L, 2);
				UnityEngine.Component o = CommonUtil.AddSingleComponent(arg0, arg1);
				ToLua.Push(L, o);
				return 1;
			}
			else if (count == 2 && TypeChecker.CheckTypes<string>(L, 2))
			{
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg1 = ToLua.ToString(L, 2);
				UnityEngine.Component o = CommonUtil.AddSingleComponent(arg0, arg1);
				ToLua.Push(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: CommonUtil.AddSingleComponent");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddCustomComponent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			string arg1 = ToLua.CheckString(L, 2);
			UnityEngine.Component o = CommonUtil.AddCustomComponent(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetGameObjectByName(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			string arg1 = ToLua.CheckString(L, 2);
			UnityEngine.GameObject o = CommonUtil.GetGameObjectByName(arg0, arg1);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindChildByPath(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			string arg1 = ToLua.CheckString(L, 2);
			UnityEngine.GameObject o = CommonUtil.FindChildByPath(arg0, arg1);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetComponentByPath(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			string arg1 = ToLua.CheckString(L, 2);
			string arg2 = ToLua.CheckString(L, 3);
			UnityEngine.Component o = CommonUtil.GetComponentByPath(arg0, arg1, arg2);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDeviceInfo(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = CommonUtil.GetDeviceInfo();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUIRootObj(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.GameObject o = CommonUtil.GetUIRootObj();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUIRootTransform(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Transform o = CommonUtil.GetUIRootTransform();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUICameraObj(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.GameObject o = CommonUtil.GetUICameraObj();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUIRoot(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Canvas o = CommonUtil.GetUIRoot();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUICamera(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Camera o = CommonUtil.GetUICamera();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMainCamera(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Camera o = CommonUtil.GetMainCamera();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMainGameObj(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.GameObject o = CommonUtil.GetMainGameObj();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetEffectCameraObj(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.GameObject o = CommonUtil.GetEffectCameraObj();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetEffectCamera(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Camera o = CommonUtil.GetEffectCamera();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBatteryLevel(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			float o = CommonUtil.GetBatteryLevel();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBatteryStatus(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			int o = CommonUtil.GetBatteryStatus();
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNetworkStatus(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			int o = CommonUtil.GetNetworkStatus();
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WorldToUIPoint(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			UnityEngine.Camera arg0 = (UnityEngine.Camera)ToLua.CheckObject(L, 1, typeof(UnityEngine.Camera));
			UnityEngine.Canvas arg1 = (UnityEngine.Canvas)ToLua.CheckObject(L, 2, typeof(UnityEngine.Canvas));
			UnityEngine.Vector3 arg2 = ToLua.ToVector3(L, 3);
			UnityEngine.Vector2 o = CommonUtil.WorldToUIPoint(arg0, arg1, arg2);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ActivedChildCount(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.Transform arg0 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 1);
			int o = CommonUtil.ActivedChildCount(arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAssetPath(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = CommonUtil.GetAssetPath();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetHostIp(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = CommonUtil.GetHostIp();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChildCount(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			int o = CommonUtil.ChildCount(arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetChild(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.GameObject o = CommonUtil.GetChild(arg0, arg1);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CheckLocalFileExist(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			bool o = CommonUtil.CheckLocalFileExist(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetComponentsInChildren(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg1 = ToLua.CheckString(L, 2);
				UnityEngine.Component[] o = CommonUtil.GetComponentsInChildren(arg0, arg1);
				ToLua.Push(L, o);
				return 1;
			}
			else if (count == 3)
			{
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg1 = ToLua.CheckString(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				UnityEngine.Component[] o = CommonUtil.GetComponentsInChildren(arg0, arg1, arg2);
				ToLua.Push(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: CommonUtil.GetComponentsInChildren");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTerrainHeight(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				float arg0 = (float)LuaDLL.luaL_checknumber(L, 1);
				float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
				float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
				float o = CommonUtil.GetTerrainHeight(arg0, arg1, arg2);
				LuaDLL.lua_pushnumber(L, o);
				return 1;
			}
			else if (count == 4)
			{
				float arg0 = (float)LuaDLL.luaL_checknumber(L, 1);
				float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
				float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
				float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
				float o = CommonUtil.GetTerrainHeight(arg0, arg1, arg2, arg3);
				LuaDLL.lua_pushnumber(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: CommonUtil.GetTerrainHeight");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowUIBlur(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			string arg1 = ToLua.CheckString(L, 2);
			CommonUtil.ShowUIBlur(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowUIMask(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			string arg1 = ToLua.CheckString(L, 2);
			CommonUtil.ShowUIMask(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyUIBlur(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			CommonUtil.DestroyUIBlur();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSceneMgr(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			SceneMgr o = CommonUtil.GetSceneMgr();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DelayInvokeNextFrame(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 1);
			CommonUtil.DelayInvokeNextFrame(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsNull(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			object arg0 = ToLua.ToVarObject(L, 1);
			bool o = CommonUtil.IsNull(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InstantiatePrefab(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			UnityEngine.GameObject o = CommonUtil.InstantiatePrefab(arg0, arg1);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCapcitySize(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
			CommonUtil.SetCapcitySize(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDisposeInterval(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
			CommonUtil.SetDisposeInterval(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetGameObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			UnityEngine.GameObject o = CommonUtil.GetGameObject(arg0, arg1);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReleaseGameObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			UnityEngine.GameObject arg1 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			CommonUtil.ReleaseGameObject(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DiscardGameObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			UnityEngine.GameObject arg1 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			CommonUtil.DiscardGameObject(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAsset(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.Type arg1 = ToLua.CheckMonoType(L, 2);
			UnityEngine.Object o = CommonUtil.GetAsset(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReleaseAsset(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 2);
			CommonUtil.ReleaseAsset(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadTextWithBytes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			byte[] o = CommonUtil.LoadTextWithBytes(arg0);
			LuaDLL.tolua_pushlstring(L, o, o.Length);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HandleMainCameraEvent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			CommonUtil.HandleMainCameraEvent(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NetAvailable(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, CommonUtil.NetAvailable);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsWifi(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, CommonUtil.IsWifi);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

