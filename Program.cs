//Working:
//  - Explorer(moving through directtories)
//  - Command -exit

//Working not correct:
//  - Command -open


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Explorer_and_Reader
{
    internal class Program
    {
        private static string[] path = new string[1000]; // хранит путь поэлементно
        private static string[] pathBackup = new string[1000]; // копия path на случай ошибки, дающая возможность отката
        private static int pathIndex = 0; // хранит кол-во объектов в пути
        private static int pathIndexBackup = 0; // копия pathIndex на случай ошибки, дающая возможность отката
        private static string[] dirList; // хранит список файлов в текущем каталоге
        private static int dirIndex = 0; // кол-во файлов в текущем каталоге
        private static string command; // хранит прописанную с командной строки команду
        private static bool status = true; // пока true, цикл обработки запросов работает

        private static void FileList(string path) //выводит список файлов в директории
        {
            dirIndex = 0;
            Console.WriteLine($"\n>>>\t{path}\t<<<\n");
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectoryInfo[] dir = dInfo.GetDirectories();
            FileInfo[] spisok = dInfo.GetFiles();

            dirList = new string[1000];
            Console.WriteLine($"№\t{"Имя файла",-50}\tРазмер (байт)");
            if (path != @"C:\") Console.WriteLine($"0.\tНазад\n");

            foreach (var d in dir)
            {
                dirList[dirIndex++] = d.Name;
                Console.WriteLine($"{dirIndex}.\t[{d.Name.ToUpper() + "]",-50}");
            }

            foreach (var f in spisok)
            {
                dirList[dirIndex++] = f.Name;
                Console.WriteLine($"{dirIndex}.\t{f.Name,-50}\t{f.Length:#,#}");
            }
            Console.WriteLine("\n<Введите номер или команду>");
        }

        private static void Intro() //вступительное сообщение
        {

            Console.WriteLine("+---------------------------------------------------------------------------+");
            Console.WriteLine("|                                 Проводник                                 |");
            Console.WriteLine("+-------------------------------------+-------------------------------------+");
            Console.WriteLine("|               Команда               |                  Код                |");
            Console.WriteLine("+-------------------------------------+-------------------------------------+");
            Console.WriteLine("| · Открыть                           | -open [path]                        |");
            Console.WriteLine("| · Открыть файл с кодировкой         | -openen [path] [page_code]          |");
            Console.WriteLine("| · Выйти из программы                | -exit                               |");
            Console.WriteLine("| · Перейти к текущему каталогу       | -cur                                |");
            Console.WriteLine("+-------------------------------------+-------------------------------------+\n");

            FileList(PathLinker(pathIndex));
            
        }

        private static void Request(string command) //обрабатывает введенную команду
        {
            try 
            {
                char symbol = command[0];

                if (symbol == '-') //обрабока команд
                {
                    if (command.StartsWith("-exit"))
                    {
                        status = false;
                        return;
                    }
                    else if (command.StartsWith("-open"))
                    {
                        Open(-1, command.Substring(6));
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Такой команды не существует. Попробуйте снова.");
                        return;
                    }
                }
                else //обработка числа (выбор файла)
                {
                    try
                    {
                        int num = Convert.ToInt32(command);
                        if (num <= dirIndex && num > 0)
                        {
                            Open(num-1);
                            return;
                        }
                        else if (num == 0 && PathLinker(pathIndex) != @"C:\") // "назад"
                        {
                            pathIndex--;
                            FileList(PathLinker(pathIndex));
                        }
                        else
                        {
                            Console.WriteLine("Ошибка. Попробуйте снова.");
                            return;
                        }

                    }
                    catch
                    {
                        Console.WriteLine("Ошибка. Попробуйте снова.");
                        return;
                    }
                }
            }
            catch 
            {
                Console.WriteLine("Ошибка. Попробуйте снова.");
                return;
            }
            
        }

        private static void Open(int num = -1, string p = @"C:") //Открывает файлы и папки
        {
            if (num != -1) //открытие файла по числу
            {
                try
                {
                    path[++pathIndex] = dirList[num]; //!!! выходит за границы массива 
                    Console.WriteLine($"Открытие {PathLinker(pathIndex)}...");
                    FileList(PathLinker(pathIndex));
                }
                catch
                {
                    Console.WriteLine($"Ошибка: невозможно открыть {PathLinker(pathIndex)}");
                    Console.WriteLine($"Нажмите [Enter] для продолжения...");
                    Console.ReadLine();
                    --pathIndex;
                    FileList(PathLinker(pathIndex));
                }
            }
            else //открытие файла по пути
            {
                try
                {
                    // сделать резервную копию "path[]" и "pathIndex" на случай ошибки открытия
                    pathBackup = path;
                    pathIndexBackup = pathIndex;
                    // разделить путь "р" на состовляющие и узнать новый "pathIndex"
                    // записать поверх путь из "р" в "path[]"
                    path = p.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    pathIndex = path.Length - 1;

                    Console.WriteLine($"Открытие {PathLinker(pathIndex)}...");
                    FileList(PathLinker(pathIndex));
                }
                catch
                {
                    Console.WriteLine($"Ошибка: невозможно открыть {p}");
                    Console.WriteLine($"Нажмите [Enter] для продолжения...");
                    Console.ReadLine();
                    path = pathBackup;
                    pathIndex = pathIndexBackup;
                    FileList(PathLinker(pathIndex));
                }
            }
        }

        private static string PathLinker(int i) //собирает путь воедино
        {
            string pathLinked = "";
            for (int j = 0; j <= i; j++) pathLinked = pathLinked + path[j]+@"\";
            return pathLinked;
        }

        static void Main(string[] args)
        {
            path[0] = @"C:"; //начальный каталог
            Intro();

            while (status)
            {
                // код основной программы
                // использовать
                // continue;
                // break;

                Console.Write("> ");
                command = Console.ReadLine();
                Request(command);
            }
        }
    }
}
