using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GitTool
{
    [MenuItem("Tools/GitTool")]
    public static void Open()
    {
        GitTool tool = new GitTool();
        tool.ShowGitLog(true);
        //tool.ShowGitLog(false);

    }

    private void ShowGitLog(bool onlyLogOutput)
    {
        string output;
        string error;

        string testPath = Application.dataPath;
        string commit = string.Empty;
        string lastcommit = string.Empty;

        GetGitLogs(out output, out error, testPath, commit, lastcommit, onlyLogOutput);

        Debug.Log("------------------- Git -------------------");
        if (!string.IsNullOrEmpty(error))
        {
            Debug.Log("Error:" + error);
        }
        Debug.Log("Git:" + output);
    }

    private void GetGitLogs(out string output, out string error, string fullModuleDirectionPath, string commit, string lastCommit, bool onlyLogOutput = true)
    {
        if (!string.IsNullOrEmpty(commit))
        {
            commit += "..";
        }
        // -- 重写自己的命令
        string[] inputs = new string[]
        {
            string.Format("git log {0}{1} --pretty=oneline -- {2}", commit, lastCommit,fullModuleDirectionPath)
        };

        RunCMD(out output, out error, inputs);

        if (onlyLogOutput)
        {
            using (StringReader stringReader = new StringReader(output))
            {
                string tempOutput = string.Empty;
                string line;
                bool analyze = false;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (analyze)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            break;
                        }

                        line = line.Trim();
                        tempOutput += line + "\n";
                    }

                    if (line.Contains("git log"))
                    {
                        analyze = true;
                    }
                }

                output = tempOutput;
            }
        }
    }


    private void RunCMD(out string output, out string error, string[] inputs)
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = @"cmd.exe";  //确定程序名       

        p.StartInfo.UseShellExecute = false;   //是否使用Shell
        p.StartInfo.RedirectStandardInput = true;   //重定向输入
        p.StartInfo.RedirectStandardOutput = true;   //重定向输出
        p.StartInfo.RedirectStandardError = true;    //重定向输出错误
        p.StartInfo.CreateNoWindow = true;        //设置不显示窗口
        p.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(936);

        p.Start();

        StreamWriter streamWriter = p.StandardInput;
        streamWriter.AutoFlush = true;

        streamWriter.WriteLine("rundll32.exe git-cmd.exe");  //启动git
        for (int index = 0; index < inputs.Length; index++)
        {
            streamWriter.WriteLine(inputs[index]);
        }
        streamWriter.WriteLine("exit");  //退出

        streamWriter.Close();

        output = p.StandardOutput.ReadToEnd();  //获得所有标准输出流
        error = p.StandardError.ReadToEnd();

        p.WaitForExit(); //等待命令执行完成后退出
        p.Close(); //关闭窗口
    }

}
