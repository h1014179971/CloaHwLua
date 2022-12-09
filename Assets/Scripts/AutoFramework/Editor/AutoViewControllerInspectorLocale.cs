using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace AutoCode
{
    public class AutoViewControllerInspectorLocale
    {
        public string CodegenPart =>" 代码生成设置" ;
        public string Namespace => "命名空间:" ;
        public string ScriptName =>"生成脚本名:";
        public string ScriptsFolder => "脚本生成目录:" ;
        public string GeneratePrefab =>"生成 Prefab" ;
        public string PrefabGenerateFolder => "Prefab 生成目录:" ;
        public string OpenScript =>" 打开脚本" ;
        public string SelectScript =>" 选择脚本";
        public string Generate =>" 生成代码";

        public string DragDescription =>
             "请将要生成脚本的文件夹拖到下边区域 或 自行填写目录到上一栏中" ;

        public string PrefabDragDescription =>
            "请将要生成 Prefab 的文件夹拖到下边区域 或 自行填写目录到上一栏中" ;
    }
}
#endif

