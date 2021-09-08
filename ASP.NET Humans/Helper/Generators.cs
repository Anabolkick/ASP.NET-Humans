using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using RandomNameGen;

namespace ASP.NET_Humans
{
    public static class Generators
    {
        private static (Sex sex, string name) GenerateName()
        {
            var rand = new Random();
            var nameGen = new RandomName(rand);
            Sex sex = (Sex)rand.Next(2);
            var name = nameGen.Generate(sex);
            return (sex, name);
        }

        //public static Worker CreateWorker()
        //{
        //    Console.WriteLine("Pick role:\n 1)Cook\t 2)Courier");
        //    var profession = (Profession) Convert.ToInt32(Console.ReadLine());
        //    Console.WriteLine("Enter name of the worker:");
        //    var name = Console.ReadLine();
        //    Console.WriteLine("Enter age of the worker:");
        //    var age = Convert.ToInt32(Console.ReadLine());
        //    Console.WriteLine("Enter salary of the worker:");
        //    var salary = Convert.ToInt32(Console.ReadLine());

        //    Worker worker;

        //    switch (profession)
        //    {
        //        case Profession.Cook:
        //            worker = new Cook(name, salary, age);
        //            break;
        //        case Profession.Courier:
        //            worker = new Сourier(name, salary, age);
        //            break;
        //        default:
        //            worker = new Cook(name, salary, age); //TODO Exeption
        //            Console.WriteLine("ERROR!");
        //            break;
        //    }

        //    return worker;
        //}

        public static Worker GenerateWorker()
        {
            var rand = new Random();
            var profession = (Profession)rand.Next(2);
            Worker worker;

            switch (profession)
            {
                case Profession.Cook:
                    worker = new Cook(GenerateName().name, GenerateName().sex, rand.Next(100, 1001), rand.Next(23, 50));
                    break;
                case Profession.Courier:
                    worker = new Сourier(GenerateName().name, GenerateName().sex, rand.Next(100, 1001), rand.Next(23, 50));
                    break;
                default:
                    worker = new Cook(GenerateName().name, GenerateName().sex, rand.Next(100, 1001), rand.Next(23, 50)); //TODO Exeption
                    Console.WriteLine("ERROR in Name Generator!");
                    break;
            }

            return worker;
        }

        public static List<Worker> GenerateWorkersWithPhoto(int count)     //TODO if Prediction old chance > 40 set Age >35
        {
            List<Worker> workers = new();

            for (int i = 0; i < count; i++)
            {
                Worker worker = GenerateWorker();
                workers.Add(worker);
            }


            foreach (Worker worker in workers)
            {
                var tmp = GetPhotoName(worker.Sex.ToString().ToLower());
                if (tmp != null)
                {
                    File.Move(@$"wwwroot\Images\{tmp}", @$"wwwroot\Images\Used\{worker.Name}.jpg");
                }
             

            }

            return workers;
        }

        public static async Task SavePhotosAsync(int count)
        {
            await Task.Run(() => SavePhotos(count));                // выполняется асинхронно
        }
        public static void SavePhotos(int count)
        {
            YOModel.ModelInput sampleData = new YOModel.ModelInput();

            string path = @$"wwwroot\Images\{Task.CurrentId}.jpg";

            using (WebClient client = new WebClient())
            {
                for (int i = 0; i < count; i++)
                {
                    again: 
                    client.DownloadFile("https://thispersondoesnotexist.com/image", path); 
                    sampleData.ImageSource = path;
                    var predict = YOModel.Predict(sampleData);


                    if (predict.Prediction == "male")
                    {
                        File.Move(path, @$"wwwroot\Images\male{i+ Task.CurrentId}.jpg");
                    }
                    else if (predict.Prediction == "female")
                    {
                        File.Move(path, @$"wwwroot\Images\female{i + Task.CurrentId}.jpg");
                    }

                    else
                    {
                        goto again;
                    }

                    // File.Delete(@$"wwwroot\Images\{worker.Name}.jpg");         //TODO add to delete list and then delete to improve working speed
                }

            }
        }

        public static string GetPhotoName(string sex)
        {
            DirectoryInfo di = new DirectoryInfo(@$"wwwroot\Images");

            string firstFileName = di.GetFiles()
                    .Select(fi => fi.Name)
                    .FirstOrDefault(name => name.StartsWith(sex));

            return firstFileName;

        }


        private enum Profession
        {
            Cook,
            Courier
        }
    }
}