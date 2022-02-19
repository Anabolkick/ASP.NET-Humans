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
using PersonGeneratorApi.Models;

namespace PersonGeneratorApi
{
    public static class PersonGenerator
    {

        private const string IdentPath = "Images\\Identified\\";
        private const string UnidentPath = "Images\\Unidentified\\";
        private static readonly object SaveLocker = new object();

        private static string GenerateName(Gender gender)
        {
            var nameGen = new RandomNameGen(new Random());
            var name = nameGen.Generate(gender);
            return name;
        }

        private static async void SaveIdentifyAsync(int count)
        {
            await Task.Run(() => SaveImages(count));
            IdentifyImages();
        }

        private static void SaveImages(int count)
        {
            lock (SaveLocker)
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
        }

        private static void IdentifyImages()
        {
            DirectoryInfo dir = new DirectoryInfo(UnidentPath);
            var length = dir.GetFiles().Length;

            List<GenderModel.ModelInput> inputs = new List<GenderModel.ModelInput>();
            List<GenderModel.ModelOutput> outputs = new List<GenderModel.ModelOutput>();
            List<string> paths = new List<string>();

            var seed = new Random().Next(Int32.MaxValue);
            var tmp_dir = Directory.CreateDirectory(dir.FullName + $"\\{seed}");

            for (int i = 0; i < length; i++)
            {
                try
                {
                    dir.GetFiles()[i].MoveTo($"{tmp_dir.FullName}\\{i}.jpg");
                }
                catch
                {
                    // ignored
                }
            }

            var tmp_length = tmp_dir.GetFiles().Length;
            for (int i = 0; i < tmp_length; i++)
            {
                GenderModel.ModelInput model = new GenderModel.ModelInput();
                model.ImageSource = File.ReadAllBytes(tmp_dir.GetFiles()[i].FullName);
                paths.Add(tmp_dir.GetFiles()[i].FullName);
                inputs.Add(model);
            }

            //My method to predict all group
            outputs.AddRange(GenderModel.PredictGroup(inputs));

            for (int i = 0; i < tmp_length; i++)
            {
                var path = paths[i];

                if (outputs[i].PredictedLabel == "male")
                {
                    File.Move(path, $"{IdentPath}male_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}_{i}.jpg");
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

            tmp_dir.Delete();
        }

        public static async Task<Worker> GenerateWorkerAsync(Rarity rarity)
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
            await FoundImageToWorkerAsync(worker);

            return worker;
        }

        //private static async Task FoundImageToWorkerAsync(Worker worker)
        //{
        //    await Task.Run(() => FoundImageToWorker(worker));
        //}
        private static async Task FoundImageToWorkerAsync(Worker worker)
        {
            DirectoryInfo di = new DirectoryInfo(IdentPath);

            string firstFileName;
            newFile:
            do
            {
                firstFileName = di.GetFiles() //TODO null ref.
                   .Select(fi => fi.Name)
                   .FirstOrDefault(name => name.StartsWith(worker.Gender.ToString()));

                if (firstFileName == null)
                {
                    await Task.Run(() => SaveIdentifyAsync(4));
                }

            } while (firstFileName == null);


            //resize image
            if (File.Exists(IdentPath + firstFileName))
            {
                //Because File.Move still use file
                Byte[] bytes = new byte[0];
                try
                {
                    bytes = File.ReadAllBytes(IdentPath + firstFileName);
                }
                catch (Exception ex)
                {
                    //Message = "The process cannot access the file 'D:\\ASP.NET Humans\\PersonGeneratorApi\\Images\\Identified\\female_2022_02_02_T_19_04_54_1.jpg' because it is being used by another process."
                    if (ex.HResult == -2147024864)
                    {
                        if (File.Exists(IdentPath + firstFileName))
                        {
                            Thread.Sleep(1500);
                            bytes = File.ReadAllBytes(IdentPath + firstFileName);
                        }
                    }
                    //File deleted
                    else
                    {
                        goto newFile;
                    }
                }

                if (bytes.Length <= 1)
                {
                    goto newFile;
                }

                await using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
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

                File.Delete(IdentPath + firstFileName);
                worker.ImageBytes = bytes;
            }

            //Download more if there's not enough photos left
            if (di.GetFiles().Length < 16)
            {
                 SaveIdentifyAsync(24);
            }
        }

    }
}