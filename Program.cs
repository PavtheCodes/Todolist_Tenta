using System.Collections.Generic;
using static dtp15_todolist.Todo;

namespace dtp15_todolist
{
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "Aktiv";
                case Waiting: return "Väntande";
                case Ready: return "Klar";
                default: return "(felaktig)";
            }
        }
        public class TodoItem
        {
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem(int priority, string task, string taskDescription)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = taskDescription;
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(string command, bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void ReadListFromFile()
        {
            string todoFileName = "todo.lis.txt";
            Console.Write($"Läser från fil {todoFileName} ... ");
            StreamReader sr = new StreamReader(todoFileName);
            int numRead = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");

        }
        public static void SaveListToFile()
        {
            string todoFileName = "todo.lis.txt";
            Console.WriteLine($"Sparar i fil {todoFileName}...");
            int numSaved = 0;
            using (TextWriter sr = new StreamWriter(todoFileName))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string line = $"{list[i].status}|{list[i].priority}|{list[i].task}|{list[i].taskDescription}";
                    sr.WriteLine(line);
                    numSaved++;
                }
            }
            Console.WriteLine($"Sparade {numSaved} rader.");
        }

        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }

        private static void PrintHead(string command, bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(string command, bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintList(string command, bool verbose = false)
        {
            PrintHead(command, verbose);
            if (command == "Lista")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (Todo.list[i].status == Active)
                    {
                        Todo.list[i].Print(command);
                    }
                }
            }
            else if (command == "Beskriv")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (Todo.list[i].status == Active)
                    {
                        Todo.list[i].Print(command, verbose);
                    }
                }
            }
            else if (MyIO.HasArgument(command, "Allt"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Todo.list[i].Print(command, verbose);
                }
            }
            PrintFoot(command, verbose);
        }
        public static void ChangeStatus(string command, bool exist = false)
        {
            string status = command.Trim();
            string[] cwords = status.Split(' ');
            if (cwords.Length == 3)
            {
                string uppgift = $"{cwords[1]} {cwords[2]}";
                for (int i = 0; i < list.Count; i++)
                {
                    if (uppgift == list[i].task)
                    {
                        if (cwords[0] == "Aktivera")
                        {
                            exist = true;
                            list[i].status = Todo.Active;
                            Console.WriteLine($"Status på: '{Todo.list[i].task}' ändrat till 'Aktiv'");

                        }
                        else if (cwords[0] == "Vänta")
                        {
                            exist = true;
                            list[i].status = Todo.Waiting;
                            Console.WriteLine($"Status på: '{Todo.list[i].task}' ändrat till 'Väntande'");
                        }
                        else if (cwords[0] == "Klar")
                        {
                            exist = true;
                            list[i].status = Todo.Ready;
                            Console.WriteLine($"Status på: '{Todo.list[i].task}' ändrat till 'Klar'");
                        }
                    }
                }
            }
            else if (!exist && cwords.Length > 1)
                Console.WriteLine("Uppgiften finns inte!");
        }
        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:");
            Console.WriteLine("(-----------------------------------------------------)");
            Console.WriteLine("| Ny              Skapa en ny uppgift                 |");
            Thread.Sleep(100);
            Console.WriteLine("| Hjälp           Lista denna hjälp                   |");
            Thread.Sleep(100);
            Console.WriteLine("| Beskriv         Lista uppgifter med beskrivning     |");
            Thread.Sleep(100);
            Console.WriteLine("| Lista           Lista aktiva uppgifter              |");
            Thread.Sleep(100);
            Console.WriteLine("| Lista allt      Lista alla uppgifter oavsett status |");
            Thread.Sleep(100);
            Console.WriteLine("| Aktivera        Aktivera en uppgift                 |");
            Thread.Sleep(100);
            Console.WriteLine("| Vänta           Sätt uppgift på 'Väntande'          |");
            Thread.Sleep(100);
            Console.WriteLine("| Klar            Färdigställ en uppgift              |");
            Thread.Sleep(100);
            Console.WriteLine("| Save            Spara filen                         |");
            Thread.Sleep(100);
            Console.WriteLine("| Load            Ladda in filen                      |");
            Thread.Sleep(100);
            Console.WriteLine("| Sluta           Sparar filen och avslutar           |");
            Console.WriteLine("(-----------------------------------------------------)");
        }     
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan, börja med att ladda fil!");
            Todo.PrintHelp();
            string command;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "Hjälp"))
                {
                    Todo.PrintHelp();
                }
                else if (MyIO.Equals(command, "Ny"))
                {
                    Console.WriteLine("Priority, 1 - 3: ");
                    int Priority = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("Task: ");
                    string Task = Console.ReadLine();
                    Console.WriteLine("Description: ");
                    string taskDescription = Console.ReadLine();
                    TodoItem item = new TodoItem(Priority, Task, taskDescription);
                    list.Add(item);
                    Console.WriteLine($" Uppgift har lagts till!");
                }    
                else if (MyIO.Equals(command, "Sluta"))
                {
                    Console.WriteLine("Hej då!");
                    Todo.SaveListToFile();
                    break;
                }
                else if (MyIO.Equals(command, "Beskriv"))
                {
                        Todo.PrintList(command, verbose: true); 
                }

                else if (MyIO.Equals(command, "Lista"))
                {
                        Todo.PrintList(command, verbose: false);
                }
                else if (MyIO.Equals(command, "Load"))
                {
                    ReadListFromFile();
                }
                else if (MyIO.Equals(command, "Save"))
                {
                    SaveListToFile();
                }
                else if (MyIO.Equals(command, "Aktivera"))
                {
                    ChangeStatus(command);
                }
                else if (MyIO.Equals(command, "Vänta"))
                {
                    ChangeStatus(command);
                }
                else if (MyIO.Equals(command, "Klar"))
                {
                    ChangeStatus(command);
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: {command}");
                }
                
            }
            while (true);
        }
    }
    class MyIO
    {
        static public string ReadCommand(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        static public bool Equals(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords[0] == expected) return true;
            }
            return false;
        }
        static public bool HasArgument(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords.Length < 2) return false;
                if (cwords[1] == expected) return true;
            }
            return false;
        }
    }
}
