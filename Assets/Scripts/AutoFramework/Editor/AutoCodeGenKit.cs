
using System.Collections.Generic;

namespace AutoCode
{
    public class AutoCodeGenKit
    {


        public static void Generate(AutoViewController viewController)
        {
            //var task = GetTemplate(bindGroup.TemplateName).CreateTask(bindGroup);
            AutoCodeGenTask task = new AutoCodeGenTask() {
                GameObject = viewController.gameObject,
                From = GameObjectFrom.Scene,
                ClassName = viewController.ScriptName,
                ScriptsFolder = viewController.ScriptsFolder,
                Namespace = viewController.Namespace
            };
            
            Generate(task);
        }

        public static void Generate(AutoCodeGenTask task)
        {
            AutoCodeGenKitPipeline.Default.Generate(task);
        }

        public static AutoCodeGenKitSetting Setting => AutoCodeGenKitSetting.Load();
    }
}

