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
        private static readonly object _saveLocker = new object();
        private static readonly object _identLocker = new object();
        private static string _identPath;
        private static string _unidentPath;

        public static void PersonGeneratorConfigure(IConfiguration configuration)
        {
            _identPath = configuration["IdentPath"];
            _unidentPath = configuration["UnidentPath"];
        }

        private static string GenerateName(Gender gender)
        {
            var nameGen = new RandomNameGen(new Random());
            var name = nameGen.Generate(gender);
            return name;
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
            await ImageToWorkerAsync(worker);

            return worker;
        }

        private static async Task ImageToWorkerAsync(Worker worker)
        {
            DirectoryInfo dir = new DirectoryInfo(_identPath);
            int i = 0; //i -  iteration
            string firstFileName;
            do
            {
                firstFileName = dir.GetFiles()
                   .Select(fi => fi.Name)
                   .FirstOrDefault(name => name.StartsWith(worker.Gender.ToString()));

                if (firstFileName == null)
                {
                    if (i == 0)
                    {
                        IdentifyImagesAsync();
                    }
                    else
                    {
                        await SaveIdentifyAsync(4);
                    }
                }

                i++;
            } while (firstFileName == null);

            var path = _identPath + firstFileName;

            #region Resize image if exist
            if (!File.Exists(path))
            {
                await ImageToWorkerAsync(worker);
                return;
            }

            var bytes = ResizeImageAsync(path).Result;
            if (bytes == null)
            {
                await ImageToWorkerAsync(worker);
                return;
            }
            #endregion

            worker.ImageBytes = bytes;
            File.Delete(path);


            //Download more if there's not enough photos left
            //if (dir.GetFiles().Length < 8)
            //{
            //    await SaveIdentifyAsync(12);
            //}
        }

        private static async Task<byte[]> ResizeImageAsync(string path)
        {
            Byte[] bytes = Array.Empty<byte>();

            //Because File.Move can still use file
            try
            {
                bytes = File.ReadAllBytesAsync(path).Result;
            }
            catch (Exception ex)
            {
                //Message = "The process cannot access the file 'D:\\ASP.NET Humans\\PersonGeneratorApi\\Images\\Identified\\female_2022_02_02_T_19_04_54_1.jpg' because it is being used by another process."
                if (ex.HResult == -2147024864)
                {
                    if (File.Exists(path))
                    {
                        Thread.Sleep(1500);
                        bytes = File.ReadAllBytesAsync(path).Result;
                    }
                }

                //File deleted
                else
                {
                    return null;
                }
            }

            if (bytes.Length <= 1)
            {
                return null;
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

            return bytes;
        }

        private static async Task SaveIdentifyAsync(int count)
        {
            await Task.Run(() => SaveImages(count));
            IdentifyImagesAsync();
        }

        private static void SaveImages(int count)
        {
            lock (_saveLocker)
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

        private static async void IdentifyImagesAsync()
        {
            DirectoryInfo dir = new DirectoryInfo(_unidentPath);
            var seed = new Random().Next(Int32.MaxValue);
            var tmp_dir = Directory.CreateDirectory(dir.FullName + $"\\{seed}");

            List<GenderModel.ModelInput> inputs = new List<GenderModel.ModelInput>();
            List<GenderModel.ModelOutput> outputs = new List<GenderModel.ModelOutput>();
            List<string> paths = new List<string>();

            lock (_identLocker)
            {
                FileInfo[] images = dir.GetFiles("*.jpg");
                for (int i = 0; i < images.Count(); i++)
                {
                    try
                    {
                        if (images[i].Length >= 10)
                        {
                            images[i].MoveTo($"{tmp_dir.FullName}\\{i}.jpg");
                        }
                      
                    }
                    //Image can be already identified and replaced/deleted
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }
            }

            var tmp_length = tmp_dir.GetFiles().Length;

            await Task.Run(() =>
            {
                for (int i = 0; i < tmp_length; i++)
                {
                    GenderModel.ModelInput model = new GenderModel.ModelInput();
                    model.ImageSource = File.ReadAllBytesAsync(tmp_dir.GetFiles()[i].FullName).Result;
                    paths.Add(tmp_dir.GetFiles()[i].FullName);
                    inputs.Add(model);
                }

                //My own method to predict groups
                outputs.AddRange(GenderModel.PredictGroup(inputs));

                for (int i = 0; i < tmp_length; i++)
                {
                    var path = paths[i];

                    if (outputs[i].Score[0] >= 0.65f) //using numbers instead PredictionLabel to adjust the accuracy
                    {
                        File.Move(path, $"{_identPath}male_{DateTime.Now:yyyy_MM_dd_HH_mm_ss.fff}_{i}.jpg");
                    }
                    else if (outputs[i].Score[1] >= 0.65f)
                    {
                        File.Move(path, $"{_identPath}female_{DateTime.Now:yyyy_MM_dd_T_HH_mm_ss.fff}_{i}.jpg");
                    }
                    else
                    {
                        File.Delete(path);
                    }
                }
            });
            
            tmp_dir.Delete();
        }
    }
}