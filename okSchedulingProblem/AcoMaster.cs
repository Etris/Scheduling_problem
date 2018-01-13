using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Timers;
using System.Diagnostics;
using System.IO;

namespace okSchedulingProblem
{
    class MyTimer
    {
        public static DateTime Start { get; set; }
    }

    class AcoMaster
    {
        public double[,] arr;
        public double[,] arrx2;
        ArrayList topValue = new ArrayList();
        ArrayList topValuesTimes = new ArrayList();
        public TimeSpan endVar = new TimeSpan(0, 0, 30);

        private double Evaporate(double tmp, double param)
        {
            double output = (1 - param) * tmp;
            return Math.Round(output, 3);
        }

        private double Smooth(double tmp, double param)
        {
            double output = (0.1 * (1 + (Math.Log(param, (tmp / 0.1)))));
            return Math.Round(output, 3);
        }

        public void Init(int size, int population, double smoothParam, double evarParam)
        {
            topValue = new ArrayList();
            topValuesTimes = new ArrayList();
            ArrayList populationHandler = new ArrayList();
            RandomGenerator gen = new RandomGenerator();
            InstationGenerator instanceFirst = new InstationGenerator();
            InstationGenerator instanceBest = new InstationGenerator();
            gen.SetNumberOfElements(size);
            gen.GenereteMaintances();
            //gen.PrintMaintances();
            gen.GenereteOperations();
            //gen.PrintOperations();
            arr = new double[size, size];
            arrx2 = new double[size, size];
            int iter = 0;
            int bestScore = 1500;
            MyTimer.Start = DateTime.Now;
            TimeSpan span = new TimeSpan(0, 0, 0);
            //TimeSpan endVar = new TimeSpan(0, 0, 45);
            while (1 == 1)
            {
                if(( span = DateTime.Now - MyTimer.Start) > endVar ) break;
                populationHandler.Clear();
                for (int i = 0; i < population; i++)
                {
                    instanceFirst = new InstationGenerator
                    {
                        size = size
                    };
                    gen.TransferWithRoulette(instanceFirst, this);
                    if (instanceFirst.getInstantionScore() < bestScore)
                    {
                        bestScore = instanceFirst.getInstantionScore();
                        instanceBest = (InstationGenerator)instanceFirst.Clone();
                        string mods = "";
                        if (instanceBest.isModified == true)
                        {
                            mods = "MO!";
                        }
                        else
                        {
                            mods = "NMO!";
                        }
                        Console.WriteLine(bestScore + " "+ mods);
                        //instanceFirst.PrintMachines();
                        topValue.Add(bestScore);
                        topValuesTimes.Add(string.Format("{0:ss}", (MyTimer.Start - DateTime.Now)));
                    }

                    //Console.WriteLine(instanceFirst.getInstantionScore());
                    //instanceFirst.PrintMachines();
                    populationHandler.Add(instanceFirst);
                }
                //Console.WriteLine("*BEEP* : " + iter);
                //iter = 0;
                ArrayList topElements = new ArrayList();
                for (int s = 0; s < size/10; s++)
                {
                    Random rnd = new Random();
                    int rulette = rnd.Next(0, 10);
                    int tmpPos = 0;
                    int maxValue = 0;
                    int positionAtList = 0;
                    if (rulette < 10)
                    {
                        foreach (InstationGenerator instance in populationHandler)
                        {
                            if (instance.getInstantionScore() > maxValue)
                            {
                                maxValue = instance.getInstantionScore();
                                positionAtList = tmpPos;
                            }
                            tmpPos++;
                        }
                    }
                    else
                    {
                        positionAtList = rnd.Next(0, (populationHandler.Count - 1));
                    }
                    topElements.Add((InstationGenerator)populationHandler[positionAtList]);
                    populationHandler.RemoveAt(positionAtList);
                }
                //smooth table/evaporate pheromone
                for (int ix = 0; ix < size; ix++)
                {
                    for (int iy = 0; iy < size; iy++)
                    {
                        if (arr[ix, iy] > 0)
                        {
                            if (arr[ix, iy] > evarParam) arr[ix, iy] = Evaporate(arr[ix, iy], evarParam);
                        }
                    }
                }
                //grants new values
                //1st machine
                double grants = 1;
                foreach (InstationGenerator instance in topElements)
                {
                    ArrayList tmp = instance.getFirstMachine();
                    int previous = 0, actual = 0;
                    foreach (Entity element in tmp)
                    {
                        if (element.GetTypeOfEntity() == 0)
                        {
                            previous = actual;
                            actual = element.GetOperationID();
                            if (previous == actual) continue;
                            arr[previous, actual] += grants;
                        }
                    }
                    grants = (grants - 0.1);
                }
                //2nd machine
                grants = 1;
                foreach (InstationGenerator instance in topElements)
                {
                    ArrayList tmp = instance.getSecondMachine();
                    int previous = 0, actual = 0;
                    foreach (Entity element in tmp)
                    {
                        if (element.GetTypeOfEntity() == 0)
                        {
                            previous = actual;
                            actual = element.GetOperationID();
                            if (previous == actual) continue;
                            arrx2[previous, actual] += grants;
                        }
                    }
                    grants = (grants - 0.1);
                }
                // smooth
                for (int ix = 0; ix < size; ix++)
                {
                    for (int iy = 0; iy < size; iy++)
                    {
                        if (arr[ix, iy] > 0)
                        {
                            arr[ix, iy] = Smooth(arr[ix, iy], smoothParam);
                        }
                    }
                }
            }
            //best one
            string mod = "";
            if(instanceBest.isModified == true)
            {
                mod = "GMO!";
            }
            else
            {
                mod = "GMO Free!";
            }
            Console.WriteLine(instanceBest.getInstantionScore() + " " + mod);
            SaveScores();
            //instanceBest.PrintMachines();
        }

        private void SaveScores()
        {
            Random rnd = new Random();
            string fileName = string.Format("{0:HH-mm-ss}", DateTime.Now) +  ".txt";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                for (int i = 0; i < topValue.Count; i++)
                {
                    sw.WriteLine((int)topValue[i] + "," + topValuesTimes[i] + ";");
                }
            }
        }

    }
}
