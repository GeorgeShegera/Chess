using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class DataBase
    {
        [JsonProperty("User")]
        public List<User> Persons { get; set; }

        [JsonProperty("Matches")]
        public List<Match> Matches { get; set; }

        public DataBase(List<User> persons, List<Match> matches)
        {
            Persons = persons;
            Matches = matches;
        }
        public DataBase()
        {
            Persons = new List<User>();
            Matches = new List<Match>();
        }

        public User GetPerson(string login, string password)
        {
            foreach (User person in Persons)
            {
                if (person.Login == login &&
                    person.Password == password)
                {
                    return person;
                }
            }
            return null;
        }

        public bool ContainsLogin(string login)
        {
            var persons = from p in Persons
                          where p.Login == login
                          select p;
            return persons.Count() != 0;
        }

        public void AddPerson(User person)
        {
            Persons.Add(person);
        }
        public void AddPersons(List<User> persons)
        {
            Persons.AddRange(persons);
        }
        public void AddMatch(Match match)
        {
            Matches.Add(match);
        }
        public void ShowTopPersons(int limit, User user)
        {
            List<User> persons = Persons.Select(x => x).OrderByDescending(x => x.Rating).Take(limit).ToList();
            for (int i = 0; i < limit && i < persons.Count; i++)
            {
                Console.WriteLine($"{i + 1}# {persons[i].Login} - {persons[i].Rating}");
            }
            if (!persons.Contains(user))
            {
                for (int i = 0; i < Persons.Count; i++)
                {
                    if (Persons[i] == user)
                    {
                        Console.WriteLine("...\n" +
                                         $"{i + 1}# {user.Login} - {user.Rating}\n" +
                                         $"...");
                        break;
                    }
                }

            }
        }

        public void ShowMatchHistory(User person)
        {
            List<Match> matches = (from match in Matches
                                  where match.ContainsPerson(person)
                                  select match).ToList();
            if (matches.Count != 0) ShowMatchces(matches);
        }

        public void ShowRecentMatches(int limit)
        {
            List<Match> matches = Matches.Select(x => x).Take(limit).ToList();
            if (matches.Count != 0) ShowMatchces(matches);
        }

        public void ShowMatchces(List<Match> matches)
        {
            foreach (var match in matches)
            {
                Console.WriteLine("-------------------------------");
                match.Show();
                Console.WriteLine();
            }
            Console.WriteLine("-------------------------------");
        }
    }
}
