using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    Console.WriteLine("Svod Installer v.1");

                    


                    {
                        string Source = GetParamIn(args, "S");
                        string mask = GetParamIn(args, "M");
                        string Dist = GetParamIn(args, "D");
                        Dist= Dist.Replace("#","%");; ;
                        string cmd = GetParamIn(args, "C");
                        string run = GetParamIn(args, "R");
                        string Dist_out = Environment.ExpandEnvironmentVariables(Dist);
                        Console.WriteLine(Dist + "=>" + Dist_out);

                        if (Source=="")
                            Help();
                        if (cmd == "D")
                        {
                            Console.WriteLine("Make delete files:");
                            foreach (FileInfo f in new DirectoryInfo(Source).GetFiles(mask))
                            {
                                try
                                {
                                    f.Delete();
                                }
                                catch (Exception err)
                                {
                                    Console.WriteLine(err.Message.ToString());
                                }
                            }
                        }
                        if (cmd=="C")
                        {
                            DirectoryInfo distfolder = new DirectoryInfo(Dist_out);
                            if (distfolder.Exists == false)
                            {
                                distfolder.Create();
                                Console.WriteLine("Создана папка назначения.");
                            }

                                Console.WriteLine("Make copy:");
                            foreach (FileInfo f in new DirectoryInfo(Source).GetFiles(mask))
                            {
                                try
                                {
                                    f.CopyTo(Dist_out + f.Name, true);
                                    Console.WriteLine("+\t"+f.FullName+"=>"+ Dist_out + f.Name );
                                }
                                catch (Exception err)
                                {
                                    Console.WriteLine("-"+f.FullName + "\terror:\t"+err.Message.ToString());
                                }
                            }
                        }
                        if (run!="")
                        {
                            try
                            {
                                Process.Start(Dist_out + run);
                            }catch (Exception err)
                            {
                                Console.WriteLine(Dist_out + run+"RunError\t" +err.Message.ToString());
                                Help();
                            }
                            
                        }


                     //   Console.ReadLine();

                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("Ошибка в параметрах запуска");
                    Console.WriteLine(err.Message.ToString());
                    Help();
                }

            }
            finally
            { 
            }
        }

        private static void Help()
        {
            
            Console.WriteLine("command line:");
            Console.WriteLine("-S:[FolderName]\t Исходная папка с файлами");
            Console.WriteLine("-M:[FileMask]\t Маска для действия");
            Console.WriteLine("-D:[FolderName]\t Конечная папка с файлами");
            Console.WriteLine("-С:[C,D]\t Действие с папкой и файлами. С-Copy from Source D-Delete from Destination");
            Console.WriteLine("-R:[exe name]\t файл для запуска (в папке Destination");
            //                    Console.WriteLine("-A\t Действие рекурсивное. Не реализовано..");
            Console.WriteLine(@"Example:");
            Console.WriteLine(@"XInst.exe -C:[C] -S:[\\soft\svoddocs\svodinfo\] -M:[*.*]  -D:[%ProgramFiles(x86)%\Svod\JI\] -R:[JIcon.exe]");

        }

        private static string GetParamIn(string[] args, string par)
        {
            try
            {
                foreach (string s in args)
                {

                    
                    if (s.StartsWith("-" + par + ":"))
                    {
                        Console.WriteLine(par + "!!" + s);
                        string [] ss = s.Split('[');
                        if (ss.Length == 2)
                        {
                            if (!ss[1].Trim().EndsWith("]")) throw new Exception("Ошибка в параметере " + s+ " ] not found");
                            string ret = ss[1].Substring(0, ss[1].Length - 1);
                            Console.WriteLine(par + "=" + ret);
                            return ret;
                          
                        }
                        else throw new Exception("Ошибка в параметере " + s+" входное значение ");
                    }

                }

            }catch (Exception err)
            { }
            return "";
        }
    }
}
