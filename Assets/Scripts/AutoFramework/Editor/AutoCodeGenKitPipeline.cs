using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoCode
{
    public class AutoCodeGenKitPipeline : ScriptableObject
    {
        private static AutoCodeGenKitPipeline mInstance;

        public static AutoCodeGenKitPipeline Default
        {
            get
            {
                if (mInstance) return mInstance;

                var filePath = Dir.Value + FileName;

                if (File.Exists(filePath))
                {
                    return mInstance = AssetDatabase.LoadAssetAtPath<AutoCodeGenKitPipeline>(filePath);
                }

                return mInstance = CreateInstance<AutoCodeGenKitPipeline>();
            }
        }

        public void Save()
        {
            var filePath = Dir.Value + FileName;

            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateAsset(this, Dir.Value + FileName);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static readonly Lazy<string> Dir =
            new Lazy<string>(() => "Assets/GameAssets/CodeGenKit/".CreateDirIfNotExists());

        private const string FileName = "Pipeline.asset";

        [SerializeField] public AutoCodeGenTask CurrentTask;

        public void Generate(AutoCodeGenTask task)
        {
            CurrentTask = task;

            CurrentTask.Status = CodeGenTaskStatus.Search;
            AutoBindSearchHelper.Search(task);
            CurrentTask.Status = CodeGenTaskStatus.Gen;


            // var writer = File.CreateText(scriptFile);

            var writer = new StringBuilder();
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine("using UIFramework;");
            writer.AppendLine();

            if (AutoCodeGenKit.Setting.IsDefaultNamespace)
            {
                writer.AppendLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.AppendLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.AppendLine(
                $"namespace {((string.IsNullOrWhiteSpace(task.Namespace)) ? AutoCodeGenKit.Setting.Namespace : task.Namespace)}");
            writer.AppendLine("{");
            writer.AppendLine($"\tpublic partial class {task.ClassName} : AutoCode.AutoViewController");
            writer.AppendLine("\t{");
            writer.AppendLine("\t\tvoid Start()");
            writer.AppendLine("\t\t{");
            writer.AppendLine("\t\t\t// Code Here");
            writer.AppendLine("\t\t}");
            writer.AppendLine("\t}");
            writer.AppendLine("}");

            task.MainCode = writer.ToString();
            writer.Clear();

            writer.AppendLine($"// Generate Id:{Guid.NewGuid().ToString()}");
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine();

            if (AutoCodeGenKit.Setting.IsDefaultNamespace)
            {
                writer.AppendLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.AppendLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.AppendLine(
                $"namespace {(string.IsNullOrWhiteSpace(task.Namespace) ? AutoCodeGenKit.Setting.Namespace : task.Namespace)}");
            writer.AppendLine("{");
            writer.AppendLine($"\tpublic partial class {task.ClassName}");
            writer.AppendLine("\t{");

            foreach (var bindData in task.ComponentTypes)
            {
                writer.AppendLine();
                writer.AppendLine($"\t\tpublic {bindData.TypeName} {bindData.MemberName};");
            }

            writer.AppendLine();
            writer.AppendLine("\t}");
            writer.AppendLine("}");
            task.DesignerCode = writer.ToString();
            writer.Clear();


            var scriptFile = string.Format(task.ScriptsFolder + "/{0}.cs", (task.ClassName));

            if (!File.Exists(scriptFile))
            {
                scriptFile.GetFolderPath().CreateDirIfNotExists();
                File.WriteAllText(scriptFile, CurrentTask.MainCode);
            }


            scriptFile = string.Format(task.ScriptsFolder + "/{0}.Designer.cs", task.ClassName);
            File.WriteAllText(scriptFile, CurrentTask.DesignerCode);

            
            Save();

            CurrentTask.Status = CodeGenTaskStatus.Compile;
        }

        public void OnCompile()
        {
            if (CurrentTask == null) return;
            if (CurrentTask.Status == CodeGenTaskStatus.Compile)
            {
                var generateClassName = CurrentTask.ClassName;
                var generateNamespace = CurrentTask.Namespace;

                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly =>
                    !assembly.FullName.StartsWith("Unity"));

                var typeName = generateNamespace + "." + generateClassName;

                var type = assemblies.Where(a => a.GetType(typeName) != null)
                    .Select(a => a.GetType(typeName)).FirstOrDefault();

                if (type == null)
                {
                    Debug.Log("编译失败");
                    return;
                }

                Debug.Log(type);

                var gameObject = CurrentTask.GameObject;


                var scriptComponent = gameObject.GetComponent(type);

                if (!scriptComponent)
                {
                    scriptComponent = gameObject.AddComponent(type);
                }

                var serializedObject = new SerializedObject(scriptComponent);


                foreach (var bindInfo in CurrentTask.ComponentTypes)
                {

                    var componentName = bindInfo.TypeName.Split('.').Last();
                    var serializedProperty = serializedObject.FindProperty(bindInfo.MemberName);
                    //var component = gameObject.transform.Find(bindInfo.PathToRoot).GetComponent(componentName);
                    var component = bindInfo.obj.GetComponent(componentName);

                    if (!component)
                    {
                        //component = gameObject.transform.Find(bindInfo.PathToRoot).GetComponent(bindInfo.TypeName);
                        component = bindInfo.obj.GetComponent(bindInfo.TypeName);
                    }

                    serializedProperty.objectReferenceValue = component;

                    // Debug.Log(componentName + "@@@@" + serializedProperty + "@@@@" + component);
                }


                var codeGenerateInfo = gameObject.GetComponent<AutoViewController>();

                if (codeGenerateInfo)
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = codeGenerateInfo.ScriptsFolder;
                    serializedObject.FindProperty("PrefabFolder").stringValue = codeGenerateInfo.PrefabFolder;
                    serializedObject.FindProperty("GeneratePrefab").boolValue = codeGenerateInfo.GeneratePrefab;
                    serializedObject.FindProperty("ScriptName").stringValue = codeGenerateInfo.ScriptName;
                    serializedObject.FindProperty("Namespace").stringValue = codeGenerateInfo.Namespace;

                    var generatePrefab = codeGenerateInfo.GeneratePrefab;
                    var prefabFolder = codeGenerateInfo.PrefabFolder;


                    if (codeGenerateInfo.GetType() != type)
                    {
                        DestroyImmediate(codeGenerateInfo, false);
                    }

                    serializedObject.ApplyModifiedPropertiesWithoutUndo();

                    if (generatePrefab)
                    {
                        prefabFolder.CreateDirIfNotExists();

                        var generatePrefabPath = prefabFolder + "/" + gameObject.name + ".prefab";

                        if (File.Exists(generatePrefabPath))
                        {
                            // PrefabUtility.SavePrefabAsset(gameObject);
                        }
                        else
                        {
                            AutoPrefabUtils.SaveAndConnect(generatePrefabPath, gameObject);
                        }
                    }
                }
                else
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = "Assets/GameAssets/Scripts";
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());


                CurrentTask.Status = CodeGenTaskStatus.Complete;
                CurrentTask = null;
            }
        }
        /// <summary>
        /// 代码修改后回调属性
        /// </summary>
        [DidReloadScripts]
        static void Compile()
        {
            Default.OnCompile();
        }
    }
}

