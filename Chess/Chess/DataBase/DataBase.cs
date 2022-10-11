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
        [JsonProperty("Persons")]
        public List<Person> Persons { get; set; }

        [JsonProperty("Matches")]
        public List<Match> Matches { get; set; }

        public DataBase(List<Person> persons, List<Match> matches)
        {
            Persons = persons;
            Matches = matches;
        }
        public DataBase()
        {
            Persons = new List<Person>();
            Matches = new List<Match>();
        }

        public Person GetPerson(string login, string password)
        {
            foreach (Person person in Persons)
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

        public void AddPerson(Person person)
        {
            Persons.Add(person);
        }
        public void AddPersons(List<Person> persons)
        {
            Persons.AddRange(persons);
        }
        public void AddMatch(Match match)
        {
            Matches.Add(match);
        }
        public void ShowTopPersons(int limit, Person user)
        {
            List<Person> persons = Persons.Select(x => x).OrderByDescending(x => x.Rating).Take(limit).ToList();
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

        public void ShowMatchHistory(Person person)
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
