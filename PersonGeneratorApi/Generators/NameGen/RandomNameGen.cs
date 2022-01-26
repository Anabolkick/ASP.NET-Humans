﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace PersonGeneratorApi
{
    /// <summary>
    /// RandomNameGen class, used to generate a random name.
    /// </summary>
    public class RandomNameGen
    {  
        /// <summary>
        /// Class for holding the lists of names from names.json
        /// </summary>
        class NameList
        {
            public string[] boys { get; set; }
            public string[] girls { get; set; }
            public string[] last { get; set; }

            public NameList()
            {
                boys = new string[] { };
                girls = new string[] { };
                last = new string[] { };
            }
        }

        Random rand;
        List<string> Male;
        List<string> Female;
        List<string> Last;

        /// <summary>
        /// Initialises a new instance of the RandomNameGen class.
        /// </summary>
        /// <param name="rand">A Random that is used to pick names</param>
        public RandomNameGen(Random rand)
        {
            this.rand = rand;
            NameList l = new NameList();

            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader reader = new StreamReader("Generators/NameGen/names.json"))
            using (JsonReader jreader = new JsonTextReader(reader))
            {
                l = serializer.Deserialize<NameList>(jreader);
            }

            Male = new List<string>(l.boys);
            Female = new List<string>(l.girls);
            Last = new List<string>(l.last);
        }

        /// <summary>
        /// Returns a new random name
        /// </summary>
        /// <param name="gender">The gender of the person to be named. true for male, false for female</param>
        /// <param name="middle">How many middle names do generate</param>
        /// <param name="isInital">Should the middle names be initials or not?</param>
        /// <returns>The random name as a string</returns>
        public string Generate(Gender gender, int middle = 0, bool isInital = false)
        {
            string first = gender == Gender.male ? Male[rand.Next(Male.Count)] : Female[rand.Next(Female.Count)]; // determines if we should select a name from male or female, and randomly picks
            string last = Last[rand.Next(Last.Count)]; // gets the last name

            List<string> middles = new List<string>();

            for (int i = 0; i < middle; i++)
            {
                if (isInital)
                {
                    middles.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ"[rand.Next(0, 25)].ToString() + "."); // randomly selects an uppercase letter to use as the inital and appends a dot
                }
                else
                {
                    middles.Add(gender == Gender.male ? Male[rand.Next(Male.Count)] : Female[rand.Next(Female.Count)]); // randomly selects a name that fits with the gender of the person
                }
            }

            StringBuilder b = new StringBuilder();
            b.Append(first + " "); // put a space after our names;
            foreach (string m in middles)
            {
                b.Append(m + " ");
            }
            b.Append(last);

            return b.ToString();
        }

        /// <summary>
        /// Generates a list of random names
        /// </summary>
        /// <param name="number">The number of names to be generated</param>
        /// <param name="maxMiddleNames">The maximum number of middle names</param>
        /// <param name="sex">The gender of the names, if null gender is randomised</param>
        /// <param name="initials">Should the middle names have initials, if null this will be randomised</param>
        /// <returns>List of strings of names</returns>
        public List<string> RandomNames(int number, int maxMiddleNames, Gender? sex = null, bool? initials = null)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < number; i++)
            {
                if (sex != null && initials != null)
                {
                    names.Add(Generate((Gender)sex, rand.Next(0, maxMiddleNames + 1), (bool)initials));
                }
                else if (sex != null)
                {
                    bool init = rand.Next(0, 2) != 0;
                    names.Add(Generate((Gender)sex, rand.Next(0, maxMiddleNames + 1), init));
                }
                else if (initials != null)
                {
                    Gender s = (Gender)rand.Next(0, 2);
                    names.Add(Generate(s, rand.Next(0, maxMiddleNames + 1), (bool)initials));
                }
                else
                {
                    Gender s = (Gender)rand.Next(0, 2);
                    bool init = rand.Next(0, 2) != 0;
                    names.Add(Generate(s, rand.Next(0, maxMiddleNames + 1), init));
                }
            }

            return names;
        }
    }

    public enum Gender
    {
        male,
        female
    }
}
