using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Xml.Linq;
using System.Xml;

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
            if (path != @"C:\" && path != @"c:\" && path != @"C:" && path != @"c:") Console.WriteLine($"0.\tНазад\n");

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
            Console.WriteLine("| · Открыть файл из списка            | <номер файла>                       |");
            Console.WriteLine("| · Открыть путь                      | -open [path]                        |");
            Console.WriteLine("| · Открыть файл с кодировкой         | -open [path / number] [page_code]   |");
            Console.WriteLine("| · Выйти из программы                | -exit                               |");
            Console.WriteLine("| · Перейти к текущему каталогу       | -cur                                |");
            Console.WriteLine("| · Перейти к корневой папке          | -top                                |");
            Console.WriteLine("+-------------------------------------+-------------------------------------+\n");
            Pause();
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
                        string[] tmp = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length > 2) //если задана кодировка
                        {
                            try //если задан файл числом
                            {
                                Open(Convert.ToInt32(tmp[1]) - 1, null, Convert.ToInt32(tmp[2]));
                            }
                            catch //если задан путь
                            {
                                Open(-1, tmp[1], Convert.ToInt32(tmp[2]));
                            }
                            return;
                        }
                        else //если кодировка не задана
                        {
                            Open(-1, tmp[1]);
                            return;
                        }
                    }
                    else if (command.StartsWith("-top"))
                    {
                        Open(-1);
                        return;
                    }
                    else if (command.StartsWith("-cur"))
                    {
                        Open(-1, Directory.GetCurrentDirectory());
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
                            Open(num - 1);
                            return;
                        }
                        else if (num == 0 && PathLinker(pathIndex) != @"C:\" && PathLinker(pathIndex) != @"c:\" && PathLinker(pathIndex) != @"C:" && PathLinker(pathIndex) != @"c:") // "назад"
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

        private static void Open(int num = -1, string p = @"C:", int codePage = 65001) //Открывает файлы и папки
        {
            if (num != -1) //открытие файла по числу
            {
                try
                {
                    path[++pathIndex] = dirList[num];
                    Console.WriteLine($"Открытие {PathLinker(pathIndex)}...");

                    // проверяем на поддерживаемый файл для открытия
                    if (path[pathIndex].ToLower().EndsWith(".txt") ||
                        path[pathIndex].ToLower().EndsWith(".log") ||
                        path[pathIndex].ToLower().EndsWith(".md") ||
                        path[pathIndex].ToLower().EndsWith(".cs") ||
                        path[pathIndex].ToLower().EndsWith(".c") ||
                        path[pathIndex].ToLower().EndsWith(".cpp") ||
                        path[pathIndex].ToLower().EndsWith(".html") ||
                        path[pathIndex].ToLower().EndsWith(".xml") ||
                        path[pathIndex].ToLower().EndsWith(".fb2"))
                    {
                        OpenFile(codePage);
                        Pause();
                        FileList(PathLinker(--pathIndex));
                        return;
                    }
                    else
                    {
                        FileList(PathLinker(pathIndex));
                    }
                }
                catch
                {
                    Console.WriteLine($"Ошибка: невозможно открыть {PathLinker(pathIndex)}");
                    Pause();
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
                    path = new string[1000];
                    int tempIndex = 0;
                    string[] temp = p.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    pathIndex = temp.Length - 1;
                    foreach (string s in temp)
                    {
                        path[tempIndex++] = s;
                    }

                    Console.WriteLine($"Открытие {PathLinker(pathIndex)}...");

                    // проверяем на поддерживаемый файл для открытия
                    if (path[pathIndex].ToLower().EndsWith(".txt") ||
                        path[pathIndex].ToLower().EndsWith(".log") ||
                        path[pathIndex].ToLower().EndsWith(".md") ||
                        path[pathIndex].ToLower().EndsWith(".cs") ||
                        path[pathIndex].ToLower().EndsWith(".c") ||
                        path[pathIndex].ToLower().EndsWith(".cpp") ||
                        path[pathIndex].ToLower().EndsWith(".html") ||
                        path[pathIndex].ToLower().EndsWith(".xml") ||
                        path[pathIndex].ToLower().EndsWith(".fb2"))
                    {
                        OpenFile(codePage);
                        Pause();
                        path = pathBackup;
                        pathIndex = pathIndexBackup;
                        FileList(PathLinker(pathIndex));
                        return;
                    }
                    else
                    {
                        FileList(PathLinker(pathIndex));
                    }
                }
                catch
                {
                    Console.WriteLine($"Ошибка: невозможно открыть {p}");
                    Pause();
                    path = pathBackup;
                    pathIndex = pathIndexBackup;
                    FileList(PathLinker(pathIndex));
                }
            }
        }

        private static void OpenFile(int codePage = 65001) //Выводит текстовые файлы на консоль
        {
            string fName = path[pathIndex];
            string[] fFormat = fName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string filePath = PathLinker(pathIndex).TrimEnd('\\');
            // страница 1251 имеет имя "windows-1251", а страница 866 – "cp866" и, соответственно, 65001 – "UTF-8".
            Console.WriteLine($"\n>>>\tФайл: {fName}\t|\tКодировка: {codePage}\t<<<\n");
            try
            {
                switch (fFormat.Last())
                {
                    case "xml":
                        XDocument xmlBook = XDocument.Load(filePath);
                        Console.WriteLine(xmlBook.Declaration);
                        foreach (XNode element in xmlBook.Nodes()) OpenXML(element);
                        break;

                    case "fb2":
                        Console.WriteLine("Вывести документ полностью [Enter] или по частям [0]?");
                        Console.Write("> ");
                        string req = Console.ReadLine();
                        bool partly = false;
                        try
                        {
                            char symbol = req[0];
                            if (symbol == '0')
                                partly = true;
                        }
                        catch
                        {
                            //Console.WriteLine("Ошибка. Попробуйте снова.");
                            //goto ERROR1;
                            //return;
                        }
                        XElement doc = XElement.Load(filePath);

                        //Вывод "шапки"
                        string lastName = null, firstName = null,
                              title = null, sequence = null, annotation = null,
                              year = null, id = null;
                        string[] genre = new string[10];
                        int genreIndex = 0;
                        
                        var titleinfo = doc.Elements().First(x =>
                          x.Name.LocalName == "description").Elements().First(
                                x => x.Name.LocalName == "title-info").Elements();
                        foreach (var element in titleinfo)
                        {
                            switch (element.Name.LocalName)
                            {

                                case "genre": genre[genreIndex++] = element.Value; break;
                                case "author"://Извлекаем элементы этого узла
                                    foreach (var el in element.Elements())
                                    {
                                        switch (el.Name.LocalName)
                                        {
                                            case "last-name": lastName = el.Value; break;
                                            case "first-name": firstName = el.Value; break;
                                            default: break;
                                        }
                                    }
                                    break;
                                case "book-title": title = element.Value; break;
                                case "date": year = element.Value; break;
                                //Наименование серии – атрибут элемента sequence
                                case "sequence":
                                    sequence = element.Attribute("name").Value;
                                    break;
                                case "annotation": annotation = element.Value; break;
                                default: break;
                            }
                        }

                        var documentinfo = doc.Elements().First(x =>
                            x.Name.LocalName == "description").Elements().First(
                            x => x.Name.LocalName == "document-info").Elements();
                        foreach (var element in documentinfo)
                        {
                            switch (element.Name.LocalName)
                            {
                                case "id": id = element.Value; break;
                            }
                        }

                        Console.Write($"Тематика: ");
                        for (int g=0; g<genreIndex; g++)
                            Console.Write($"{genre[g]} ");
                        Console.WriteLine($"\nАвтор: " +
                            $"{lastName} {firstName}\nНазвание: {title}\n" +
                            $"Серия: {sequence}\nАннотация: {annotation}\nДата: {year}\nid: {id}\n");

                        //Вывод "тушки"
                        int loop = 0; //для вывода документа по частям
                        var body = doc.Elements().First(x =>
                            x.Name.LocalName == "body").Elements();
                        foreach (var item in body)
                        {
                            if (item.Name.LocalName == "section") //section
                                foreach (var el in item.Elements())
                                {
                                    switch (el.Name.LocalName)
                                    {
                                        case "title":
                                            foreach (var el_title in el.Elements())
                                            {
                                                if (el_title.Name.LocalName == "p")
                                                {
                                                    Console.WriteLine($"\t{el_title.Value.ToUpper()}");
                                                    loop += (Convert.ToString(el_title.Value)).Length;
                                                }
                                            }
                                            break;
                                        case "epigraph":
                                            foreach (var el_epigraph in el.Elements())
                                            {
                                                if (el_epigraph.Name.LocalName == "p")
                                                {
                                                    Console.WriteLine(el_epigraph.Value);
                                                    loop += (Convert.ToString(el_epigraph.Value)).Length;
                                                }
                                            }
                                            break;
                                        case "cite":
                                            foreach (var el_cite in el.Elements())
                                            {
                                                if (el_cite.Name.LocalName == "p")
                                                {
                                                    Console.WriteLine(el_cite.Value);
                                                    loop += (Convert.ToString(el_cite.Value)).Length;
                                                }
                                            }
                                            break;
                                        case "p": 
                                            Console.WriteLine($"   {el.Value}");
                                            loop += (Convert.ToString(el.Value)).Length;
                                            break; 
                                        case "empty-line": 
                                            Console.WriteLine("\n");
                                            loop += (Convert.ToString(el.Value)).Length;
                                            break;
                                        case "empty-line/":
                                            Console.WriteLine("\n");
                                            loop += (Convert.ToString(el.Value)).Length;
                                            break;
                                        default: break;
                                    }
                                    if (loop > 1000 && partly)
                                    {
                                        Pause();
                                        loop = 0;
                                        //break;
                                    }
                                }

                            if (item.Name.LocalName == "title")
                            {
                                foreach (var el_title in item.Elements())
                                {
                                    if (el_title.Name.LocalName == "p")
                                    {
                                        Console.WriteLine($"\t{el_title.Value.ToUpper()}");
                                        loop += (Convert.ToString(el_title.Value)).Length;
                                    }
                                }
                            }
                            if (item.Name.LocalName == "epigraph")
                            {
                                foreach (var el_epigraph in item.Elements())
                                {
                                    if (el_epigraph.Name.LocalName == "p")
                                    {
                                        Console.WriteLine(el_epigraph.Value);
                                        loop += (Convert.ToString(el_epigraph.Value)).Length;
                                    }
                                }
                            }
                            if (item.Name.LocalName == "cite")
                            {
                                foreach (var el_cite in item.Elements())
                                {
                                    if (el_cite.Name.LocalName == "p")
                                    {
                                        Console.WriteLine(el_cite.Value);
                                        loop += (Convert.ToString(el_cite.Value)).Length;
                                    }
                                }
                            }

                            if (item.Name.LocalName == "p")
                            {
                                Console.WriteLine(item.Value);
                                loop += (Convert.ToString(item.Value)).Length;
                            }
                            if (item.Name.LocalName == "empty-line")
                            {
                                Console.WriteLine(item.Value);
                                loop += (Convert.ToString(item.Value)).Length;
                            }
                            if (item.Name.LocalName == "empty-line/")
                            {
                                Console.WriteLine(item.Value);
                                loop += (Convert.ToString(item.Value)).Length;
                            }
                            
                            if (loop > 1000 && partly)
                            {
                                Pause();
                                loop = 0;
                                //break;
                            }
                        }
                        break;

                    case "html":
                        XElement dochtml = XElement.Load(filePath);
                        var html = dochtml.Elements();
                        foreach (var item in html)
                        {
                            if (item.Name.LocalName == "head")
                            {
                                Console.Write($"Заголовок: ");
                                foreach (var el_title in item.Elements())
                                {
                                    if (el_title.Name.LocalName == "title")
                                        Console.WriteLine($"{el_title.Value}");
                                }
                            }
                            Console.Write("\n\n");

                            if (item.Name.LocalName == "body")
                            {
                                foreach (var el in item.Elements())
                                {
                                    switch (el.Name.LocalName)
                                    {
                                        case "section":
                                            foreach (var el_section in el.Elements())
                                            {
                                                if (el_section.Name.LocalName == "p")
                                                    Console.WriteLine($"   {el_section.Value}");
                                                if (el_section.Name.LocalName == "h1")
                                                    Console.WriteLine($"\t\t{el_section.Value.ToUpper()}");
                                                if (el_section.Name.LocalName == "h2")
                                                    Console.WriteLine($"\t{el_section.Value.ToUpper()}");
                                                if (el_section.Name.LocalName == "h3")
                                                    Console.WriteLine($"\t{el_section.Value}");
                                            }
                                            break;
                                        case "table":
                                            foreach (var el_table in el.Elements()) // строчки
                                            {
                                                foreach (var el_tr in el_table.Elements()) // столбцы
                                                {
                                                    Console.Write($"{el_tr.Value}\t");
                                                }
                                                Console.Write("\n");
                                            }
                                            break;
                                        case "ul":
                                            foreach (var el_ul in el.Elements())
                                            {
                                                if (el_ul.Name.LocalName == "li")
                                                    Console.WriteLine($"   · {el_ul.Value}");
                                            }
                                            break;
                                        case "p": Console.WriteLine($"   {el.Value}"); break;
                                        case "h1": Console.WriteLine($"\t\t{el.Value.ToUpper()}"); break;
                                        case "h2": Console.WriteLine($"\t{el.Value.ToUpper()}"); break;
                                        case "h3": Console.WriteLine($"\t{el.Value}"); break;
                                        default: break;
                                    }
                                }
                            }

                            if (item.Name.LocalName == "p")
                                Console.WriteLine($"   {item.Value}");
                            if (item.Name.LocalName == "h1")
                                Console.WriteLine($"\t\t{item.Value.ToUpper()}");
                            if (item.Name.LocalName == "h2")
                                Console.WriteLine($"\t{item.Value.ToUpper()}");
                            if (item.Name.LocalName == "h3")
                                Console.WriteLine($"\t{item.Value}");
                        }
                        break;

                    default:
                        StreamReader fStr = new StreamReader(filePath, Encoding.GetEncoding(codePage));
                        string s;
                        while ((s = fStr.ReadLine()) != null) Console.WriteLine(s);
                        fStr.Close();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка открытия файла: {e.Message}");
            }
        }

        private static string PathLinker(int i) //собирает путь воедино
        {
            string pathLinked = "";
            for (int j = 0; j <= i; j++) pathLinked = pathLinked + path[j] + @"\";
            return pathLinked;
        }

        private static void OpenXML(XNode node)
        {
            if (node.NodeType == XmlNodeType.Comment)
            { Console.WriteLine(node); return; }
            XElement e = (XElement)node;
            Console.Write($"{e.Name} : ");
            if (!e.HasElements)//Если элемент не имеет дочерних элементов
            {
                Console.WriteLine(e.Value);   //вывод элемента
                if (e.HasAttributes)          //и его атрибутов
                    foreach (var at in e.Attributes()) Console.WriteLine(at);
            }
            else
            {
                Console.WriteLine();
                foreach (var nd in e.Nodes()) OpenXML(nd);
            }
        }

        private static void Pause() //функция паузы
        {
            Console.WriteLine($"\nНажмите [Enter] для продолжения...");
            Console.ReadLine();
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
