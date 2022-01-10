using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using Microsoft.ML;
using RandomNameGen;

namespace ASP.NET_Humans
{
    public static class Generators
    {
        static object locker = new object();
        static object locker_2 = new object();
        private static (Sex sex, string name) GenerateName()
        {
            var rand = new Random();
            var nameGen = new RandomName(rand);
            Sex sex = (Sex)rand.Next(2);
            var name = nameGen.Generate(sex);
            return (sex, name);
        }
        public static async void SavePhotosAsync(int count)
        {
            await Task.Run(() =>
            {
                SavePhotos(count);
            });
        }
        private static void SavePhotos(int count)
        {
            lock (locker_2)
            {
                using (WebClient client = new WebClient())
                {
                    for (int i = 0; i < count; i++)
                    {
                        bool done = false;
                        client.DownloadFile("https://thispersondoesnotexist.com/image", @$"wwwroot\Images\Unchecked\{Thread.CurrentThread.ManagedThreadId}_{i}.jpg");
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private static void PredictGender()
        {
            lock (locker)
            {
                int seed = new Random().Next(Int32.MinValue, Int32.MaxValue);
                DirectoryInfo dir = new DirectoryInfo(@$"wwwroot\Images\Unchecked");
                var count = dir.GetFiles().Length;
                List<YOModel.ModelInput> inputs = new List<YOModel.ModelInput>();

                if (count <= 0)
                {
                    return;
                }

                for (int i = 0; i < count; i++)
                {
                    YOModel.ModelInput model = new YOModel.ModelInput();
                    model.ImageSource = dir.GetFiles()[i].FullName;
                    inputs.Add(model);
                }

                YOModel.ModelInput sampleData = new YOModel.ModelInput();

                List<YOModel.ModelOutput> predict = new List<YOModel.ModelOutput>();
                predict.AddRange(YOModel.PredictGroup(inputs));

                for (int i = 0; i < count; i++)
                {
                    if (predict[i].Prediction == "male")
                    {
                        File.Move(inputs[i].ImageSource, @$"wwwroot\Images\Checked\male_{seed}_{i}.jpg");
                    }
                    else if (predict[i].Prediction == "female")
                    {
                        File.Move(inputs[i].ImageSource, @$"wwwroot\Images\Checked\female_{seed}_{i}.jpg");
                    }
                    else
                    {
                        File.Delete(inputs[i].ImageSource);
                    }
                }
            }

        }

        private static string GetNameToPhoto(string sex)
        {
            DirectoryInfo di = new DirectoryInfo(@$"wwwroot\Images\Checked");

            string firstFileName = di.GetFiles()
                    .Select(fi => fi.Name)
                    .FirstOrDefault(name => name.StartsWith(sex.ToLower()));

            return firstFileName;
        }

        private static async void PredictToCountAsync(int count)
        {
            await Task.Run(() =>
            {
                var path = @"wwwroot\Images\Checked";
                var dir = new DirectoryInfo(path);
                var cur_count = dir.GetFiles().Length;
                if (cur_count < count)
                {
                    SavePhotosAsync(cur_count - cur_count);
                }
                PredictGender();
            });
        }

        public static Worker GenerateWorker()
        {
            var rand = new Random();
            var profession = (Profession)rand.Next(2);
            Worker worker;

            worker = new Worker() { Name = GenerateName().name, Sex = GenerateName().sex, Salary = rand.Next(100, 1001), Age = rand.Next(23, 50), Profession = profession.ToString() };
            return worker;
        }

        public static List<Worker> GenerateWorkersWithPhoto(int count, string id) //TODO if Prediction old chance > 40 set Age >35
        {
            List<Worker> workers = new List<Worker>();
            var id_folder_path = @$"wwwroot\Images\{id}";

            if (!Directory.Exists(id_folder_path))
            {
                Directory.CreateDirectory(id_folder_path);
            }

            for (int i = 0; i < count; i++)
            {
                Worker worker = GenerateWorker();
                workers.Add(worker);
            }

            foreach (Worker worker in workers)
            {
                bool done = false;
                while (!done)
                {
                    var photoName = GetNameToPhoto(worker.Sex.ToString());    //  Выдаем имя исходя из пола

                    if (photoName != null)
                    {
                        File.Move(@$"wwwroot\Images\Checked\{photoName}", @$"wwwroot\Images\{id}\{worker.Sex.ToString().ToLower()}_{worker.Name}.jpg");
                        done = true;
                    }
                    else
                    {
                        SavePhotosAsync(10);
                        PredictGender();
                    }
                }
            }
            PredictToCountAsync(40);
            return workers;
        }


        private enum Profession
        {
            Cook,
            Courier
        }
    }
}