using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    internal class Program
    {
        public static void SaveDataBase(DataBase dataBase)
        {
            string json = JsonConvert.SerializeObject(dataBase);
            using (Stream stream = new FileStream("chess.json", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(json);
                }
            }
        }

        public static string LoadDataBase()
        {
            string json = "";
            using (Stream stream = new FileStream("chess.json", FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        json = reader.ReadLine();
                    }
                }
            }
            return json;
        }

        private static Player Authorization(DataBase dataBase)
        {
            string login;
            string password;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1 - Sign in\n" +
                                  "2 - Sign on");
                int.TryParse(Console.ReadLine(), out int action);
                switch (action)
                {
                    case 1:
                        {
                            Console.Write("Input your login: ");
                            login = Console.ReadLine();
                            Console.Write("Input your password: ");
                            password = Console.ReadLine();
                            Player result = dataBase.FindPlayer(login, password);
                            if (result is null)
                            {
                                 Console.WriteLine("Invalid login or password");
                            }
                            else
                            {
                                return result;
                            }
                        }
                        break;
                    case 2:
                        {
                            Console.Write("Input your login: ");
                            login = Console.ReadLine();
                            if(dataBase.ContainsLogin(login))
                            {
                                Console.WriteLine("This login is taken");
                                break;
                            }
                            Console.Write("Input your password: ");
                            password = Console.ReadLine();
                            dataBase.AddPlayer(new Player(login, password));
                            SaveDataBase(dataBase);
                            Console.WriteLine("New account has been added");
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Invalid mode");
                        }
                        break;                        
                }
                Console.ReadKey();
            }
        }

        static void Main(string[] args)
        {            
            DataBase dataBase = new DataBase();
            string data = LoadDataBase();
            if (data.Length != 0)
            {
                dataBase = JsonConvert.DeserializeObject<DataBase>(data);
            }
            while (true)
            {
                bool endOfSession = false;
                Player curPlayer = Authorization(dataBase);
                while(!endOfSession)
                {
                    
                }
            }
        }
    }
}
