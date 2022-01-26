using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASP_NET_Humans;
using Microsoft.Extensions.Configuration;
using PersonGeneratorApi.Models;

namespace PersonGeneratorApi
{
    public static class PersonGenerator
    {
        private static IConfiguration configuration;

        private const string IdentPath = "Images\\Identified\\";
        private const string UnidentPath = "Images\\Unidentified\\";
        private static readonly object SaveIdentLocker = new object();

        private static string GenerateName(Gender gender)
        {
            var nameGen = new RandomNameGen(new Random());
            var name = nameGen.Generate(gender);
            return name;
        }

        public static void SaveIdentifyImages(int count)
        {
            lock (SaveIdentLocker)
            {
                SavePhotos(count);
                IdentifyAllImages();
            }
        }

        private static void SavePhotos(int count)
        {
            using (WebClient client = new WebClient())
            {
                for (int i = 0; i < count; i++)
                {
                    bool done = false;
                    client.DownloadFile("https://thispersondoesnotexist.com/image",
                        @$"Images\Unidentified\{DateTime.Now.Millisecond}_{i}.jpg");
                    Thread.Sleep(1000);
                }
            }
        }

        private static void IdentifyAllImages()
        {
            DirectoryInfo dir = new DirectoryInfo(UnidentPath);
            var count = dir.GetFiles().Length;

            List<GenderModel.ModelInput> inputs = new List<GenderModel.ModelInput>();
            List<GenderModel.ModelOutput> outputs = new List<GenderModel.ModelOutput>();
            List<string> paths = new List<string>();

            for (int i = 0; i < count; i++)
            {
                GenderModel.ModelInput model = new GenderModel.ModelInput();
                model.ImageSource = File.ReadAllBytes(dir.GetFiles()[i].FullName);
                paths.Add(dir.GetFiles()[i].FullName);
                inputs.Add(model);
            }

            //My method to predict all group
            outputs.AddRange(GenderModel.PredictGroup(inputs));

            for (int i = 0; i < count; i++)
            {
                var path = paths[i];

                if (outputs[i].PredictedLabel == "male")
                {
                    File.Move(path, $"{IdentPath}male_{DateTime.Now:yyyy_MM_dd_T_HH_mm_ss}_{i}.jpg");
                }
                else if (outputs[i].PredictedLabel == "female")
                {
                    File.Move(path, $"{IdentPath}female_{DateTime.Now:yyyy_MM_dd_T_HH_mm_ss}_{i}.jpg");
                }
                else
                {
                    File.Delete(path);
                }
            }
        }

        public static Worker GenerateWorker(Rarity rarity)
        {
            var rand = new Random();
            int salary;

            switch (rarity)
            {
                case Rarity.Legendary:
                    salary = rand.Next(1000, 2000);
                    break;
                case Rarity.Epic:
                    salary = rand.Next(750, 1150);
                    break;
                case Rarity.Rare:
                    salary = rand.Next(450, 800);
                    break;
                case Rarity.Normal:
                    salary = rand.Next(100, 500);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
            }

            Gender gender = (Gender)rand.Next(2);
            var worker = new Worker() { Name = GenerateName(gender), Gender = gender, Salary = salary, Rarity = rarity, Id = Guid.NewGuid() };
            FoundImageToWorker(worker);
            return worker;
        }
        private static void FoundImageToWorker(Worker worker)
        {
            DirectoryInfo di = new DirectoryInfo(IdentPath);

            string firstFileName;
            do
            {
                firstFileName = di.GetFiles() //TODO null ref.
                   .Select(fi => fi.Name)
                   .FirstOrDefault(name => name.StartsWith(worker.Gender.ToString()));

                Task.Run(() => SaveIdentifyImages(4));
            } while (firstFileName == null);

            var bytes = File.ReadAllBytes(IdentPath + firstFileName);

            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                using (Image img = Image.FromStream(ms))
                {
                    int h = 200;
                    int w = 200;

                    using (Bitmap b = new Bitmap(img, new Size(w, h)))
                    {
                        using (MemoryStream ms2 = new MemoryStream())
                        {
                            b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                            bytes = ms2.ToArray();
                        }
                    }
                }
            }

            worker.ImageBytes = bytes;

            File.Delete(IdentPath+firstFileName);
            //var newPath = @"Images\" + worker.Id + ".jpg";
            //File.Move(@"Images\Identified\" + firstFileName, newPath);

            //Download more if there's not enough photos left
            if (di.GetFiles().Length < 12)
            {
                Task.Run(() => SaveIdentifyImages(24));
            }
        }

    }
}