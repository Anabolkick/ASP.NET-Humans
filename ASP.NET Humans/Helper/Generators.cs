using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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

        public static Worker GenerateWorker()
        {
            var rand = new Random();
            var profession = (Profession)rand.Next(2);
            Worker worker;

            //switch (profession)
            //{
            //    case Profession.Cook:
            //        worker = new Cook(GenerateName().name, GenerateName().sex, rand.Next(100, 1001), rand.Next(23, 50));
            //        break;
            //    case Profession.Courier:
            //        worker = new Сourier(GenerateName().name, GenerateName().sex, rand.Next(100, 1001), rand.Next(23, 50));
            //        break;
            //    default:
            //        worker = new Cook(GenerateName().name, GenerateName().sex, rand.Next(100, 1001), rand.Next(23, 50)); //TODO Exeption
            //        Console.WriteLine("ERROR in Name Generator!");
            //        break;
            //}

            worker = new Worker() { Name = GenerateName().name, Sex = GenerateName().sex, Salary = rand.Next(100, 1001), Age = rand.Next(23, 50), Profession = profession.ToString() };
            return worker;
        }

        public static List<Worker> GenerateWorkersWithPhoto(int count) //TODO if Prediction old chance > 40 set Age >35
        {
            List<Worker> workers = new List<Worker>();

            for (int i = 0; i < count; i++)
            {
                Worker worker = GenerateWorker();
                workers.Add(worker);
            }

            foreach (Worker worker in workers)
            {
                var photoName = GetNameToPhoto(worker.Sex.ToString());    //  Выдаем имя исходя из пола
                bool done = false;
                while (!done)
                {
                    if (photoName != null)
                    {
                        File.Move(@$"wwwroot\Images\{photoName}", @$"wwwroot\Images\Used\{worker.Name}.jpg");
                        done = true;
                    }
                    else
                    {
                        SavePhotos(1);
                    }
                }
            }
            return workers;
        }

        //public static async Task SavePhotosAsync(int count)
        //{
        //    await Task.Run(() =>
        //    {
        //        YOModel.ModelInput sampleData = new YOModel.ModelInput();
        //        string path = @$"wwwroot\Images\{Task.CurrentId}.jpg";

        //        using (WebClient client = new WebClient())
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                again:
        //                client.DownloadFile("https://thispersondoesnotexist.com/image", path);
        //                sampleData.ImageSource = path;
        //                var predict = YOModel.Predict(sampleData);


        //                if (predict.Prediction == "male")
        //                {
        //                    File.Move(path, @$"wwwroot\Images\male{i + Task.CurrentId}.jpg");
        //                }
        //                else if (predict.Prediction == "female")
        //                {
        //                    File.Move(path, @$"wwwroot\Images\female{i + Task.CurrentId}.jpg");
        //                }

        //                else
        //                {
        //                    goto again;
        //                }

        //                // File.Delete(@$"wwwroot\Images\{worker.Name}.jpg");         //TODO add to delete list and then delete to improve working speed
        //            }

        //        }
        //    }); // выполняется асинхронно
        //}


        //public static async void SavePhotosAsync(int count)
        //{
        //    await Task.Run(() => SavePhotos(count));
        //}

        public static void SavePhotos(int count)
        {
            Task.Run(() =>
            {
                YOModel.ModelInput sampleData = new YOModel.ModelInput();

                int seed = new Random().Next(Int32.MinValue, Int32.MaxValue);
                var path = @$"wwwroot\Images\{seed}.jpg";

                using (WebClient client = new WebClient())
                {
                    for (int i = 0; i < count; i++)
                    {
                        bool done = false;
                        while (!done)
                        {
                            client.DownloadFile("https://thispersondoesnotexist.com/image", path);
                            sampleData.ImageSource = path;
                            var predict = YOModel.Predict(sampleData);
                            if (predict.Prediction == "male")
                            {
                                File.Move(path, @$"wwwroot\Images\male{i}_{seed}.jpg");
                                done = true;
                            }
                            else if (predict.Prediction == "female")
                            {
                                File.Move(path, @$"wwwroot\Images\female{i}_{seed}.jpg");
                                done = true;
                            }
                            else
                            {
                                File.Delete(path);
                            }
                        }
                    }
                }
            });
        }

        public static string GetNameToPhoto(string sex)
        {
            DirectoryInfo di = new DirectoryInfo(@$"wwwroot\Images");

            string firstFileName = di.GetFiles()
                    .Select(fi => fi.Name)
                    .FirstOrDefault(name => name.StartsWith(sex.ToLower()));

            return firstFileName;
        }


        private enum Profession
        {
            Cook,
            Courier
        }
    }
}