using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public class ExampleWindow : ScrollableDebuggerWindowBase
    {
        private long userId = 0;
        private long roleId = 0;
        private string roleName = "";
        private int camp = 0;
        private string mClientID = "";
        private string mRoleID = "";

        protected override void OnDrawScrollableWindow()
        {
            GUILayout.Label("<b>UserData</b>");
            GUILayout.BeginVertical("box");
            {
                DrawItem("UserId:", userId.ToString());
                DrawItem("RoleId:", roleId.ToString());
                DrawItem("RoleName:", roleName);
                DrawItem("Camp:", camp.ToString());
                DrawItem("PlayerId:", mClientID);
                DrawItem("RoleId:", mRoleID);
            }
            GUILayout.EndVertical();
        }

        //public void InitUserData()
        //{
        //    SLua.LuaTable roleDataMg = (SLua.LuaTable)BSFramework.InitLua.getInstance().callLuaTableFunc("SHGameRoleDataMg", "GetInstance");
        //    roleId = System.Convert.ToInt64(BSFramework.InitLua.getInstance().callLuaTableFunc(roleDataMg, "GetRoleid", roleDataMg));
        //    userId = System.Convert.ToInt32(BSFramework.InitLua.getInstance().callLuaTableFunc(roleDataMg, "GetUserId", roleDataMg));
        //    roleName = System.Convert.ToString(BSFramework.InitLua.getInstance().callLuaTableFunc(roleDataMg, "GetRolename", roleDataMg));
        //    camp = NFGameRoomManager.getInstance().GetRoom().direction;
        //    mClientID = NFSDK.NFCNetLogic.Instance().mClientID.ToString();
        //    mRoleID = NFSDK.NFCNetLogic.Instance().mRoleID.ToString();
        //}
    }
}
