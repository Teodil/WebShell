using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebShell.ShellScripts
{
    public class Shell
    {

        static IWebHostEnvironment _appEnvironment;

        public Shell(IWebHostEnvironment webHostEnvironment)
        {
            _appEnvironment = webHostEnvironment;
        }

        private static string database;
        private static string serverName;
        private static string userName;
        private static string password;
        private Dictionary<string, Func<string,string>> CommandDictionary = new Dictionary<string, Func<string,string>>
        {
            {"webhelp", webHelp},
            {"dbconf", dbConf },
            {"sql",sql }
        };

        private static string webHelp(string command)
        {
            string path = _appEnvironment.WebRootPath;
            path += "/files/help.txt";
            FileStream fileStream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);
            string output = reader.ReadToEnd();
            reader.Close();
            fileStream.Close();
            return output;
        }

        private static string dbConf(string command)
        {
            string output = "";
            //command.Remove(0, "dbconf".Length);
            string[] parameters = command.Split(' ');
            for(int i = 0; i<parameters.Length; i++)
            {
                if (parameters[i].Contains("-s"))
                {
                    serverName = parameters[i + 1];
                    output += $"Установлено имя сервера {serverName}\n";
                }
                if (parameters[i].Contains("-d"))
                {
                    database = parameters[i + 1];
                    output += $"Установлено имя базы данных {database}\n";
                }
                if (parameters[i].Contains("-u"))
                {
                    userName = parameters[i + 1];
                    output += $"Установлено имя пользователя {userName}\n";
                }
                if (parameters[i].Contains("-p"))
                {
                    password = parameters[i + 1];
                    output += $"Установлен пароль пользователя {password}\n";
                }
                if (parameters[i].Contains("-e"))
                {
                    password = null;
                    userName = null;
                    output += $"Установлено доверительное соединения\n";
                }
            }
            if (output.Length == 0)
            {
                output = "Ошибка!\nПараметры не были найдены\nВведите webhelp чтобы посмотреть справку";
            }
            return output;
        }

        private static string sql(string command)
        {
            if (command.Split(' ').Length < 2)
                return "Ошибка!\nКоманда введина не правильно\nВведите webhelp чтобы посмотреть справку";

            if(string.IsNullOrEmpty(command.Split(' ')[1]))
                return "Ошибка!\nКоманда введина не правильно\nВведите webhelp чтобы посмотреть справку";

            if (serverName == null)
                return "Ошибка!\nНе установлено имя сервера";
            if(database ==null)
                return "Ошибка!\nНе установлено имя базы данных";

            string request = $"/c sqlcmd -S {serverName} -d {database}";
            if (userName != null)
            {
                if (password == null)
                    return "Введите пароль пользователя";
                request += $" -U {userName} -P {password}";
            }

            request += $" -Q \"{command.Remove(0, "sql".Length)}\"";

            string output = "";
            string error = "";
            Process si = new Process();
            si.StartInfo.UseShellExecute = false;
            si.StartInfo.FileName = "cmd";
            si.StartInfo.Arguments = request;
            si.StartInfo.CreateNoWindow = true;
            si.StartInfo.RedirectStandardInput = true;
            si.StartInfo.RedirectStandardOutput = true;
            si.StartInfo.RedirectStandardError = true;
            si.Start();
            //error = si.StandardError.ReadToEnd();
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

        public bool IsWebShellCommand(string command, out string result)
        {
            string commandKey = command.Split(' ')[0];
            if (CommandDictionary.ContainsKey(commandKey))
            {
                result = CommandDictionary[commandKey].Invoke(command);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
                
        }

    }
}
