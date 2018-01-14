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
        ArrayList populationHandler;
        InstationGenerator instanceBest, instanceFirst;
        RandomGenerator gen;
        public TimeSpan endVar = new TimeSpan(0, 0, 120);
        public string setID;
        ArrayList nonGMO, GMO;

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

        public void SetData(int size)
        {
            topValue = new ArrayList();
            topValuesTimes = new ArrayList();
            populationHandler = new ArrayList();
            gen = new RandomGenerator();
            instanceFirst = new InstationGenerator();
            instanceBest = new InstationGenerator();
            gen.SetNumberOfElements(size);
            gen.GenereteOperations();
            gen.GenereteMaintances();
            gen.SaveInstantion(setID);
            arr = new double[size, size];
            arrx2 = new double[size, size];
            for(int ix = 0; ix < size; ix++)
            {
                for(int iy = 0; iy < size; iy++)
                {
                    arr[ix, iy] = 0;
                    arrx2[ix, iy] = 0;
                }
            }
        }

        public void Init(int size, int population, double smoothParam, double evarParam)
        {
            topValue.Clear();
            topValuesTimes.Clear();
            nonGMO = new ArrayList();
            GMO = new ArrayList();
            //int iter = 0;
            int bestScore = Int32.MaxValue;
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
                            GMO.Clear();
                            mods = "GMO!";
                            GMO.Add(bestScore);
                        }
                        else
                        {
                            nonGMO.Clear();
                            mods = "NGMO!";
                            nonGMO.Add(bestScore);
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
                for (int s = 0; s < 5; s++)
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
                            if (arr[ix, iy] > 0) arr[ix, iy] = Evaporate(arr[ix, iy], evarParam);
                            if (arrx2[ix, iy] > 0) arrx2[ix, iy] = Evaporate(arrx2[ix, iy], evarParam);
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
                        if (arrx2[ix, iy] > 0)
                        {
                            arrx2[ix, iy] = Smooth(arrx2[ix, iy], smoothParam);
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
            SaveOutput(smoothParam, evarParam);
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

        private void SaveOutput(double par, double secPar)
        {
            string fileName = string.Format(setID + "-result-"+par+"-"+secPar+".txt");
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("*** " + setID + " ****");
                sw.WriteLine(GMO[0] + "; " + nonGMO[0]);
                sw.Write("M1: ");
                int maint = 0, idle = 0;
                //1st
                foreach (Entity el in instanceBest.getFirstMachine())
                {
                    if (el.GetTypeOfEntity() == 0) {
                        sw.Write("o" + el.GetOperationNumber() + "_" + el.GetOperationID() + ", " + el.GetStartTime() + ", " + el.GetTime() +"; ");
                    }else if(el.GetTypeOfEntity() == 1)
                    {
                        sw.Write("maint" + maint++ + "_M1" + ", " + el.GetStartTime() + ", " + el.GetTime() + "; ");
                    }
                    else if(el.GetTypeOfEntity() == 2)
                    {
                        sw.Write("idle" + idle++ + "_M1" + ", " + el.GetStartTime() + ", " + el.GetTime() + "; ");
                    }
                }
                sw.WriteLine();
                //2nd
                sw.Write("M2: ");
                maint = 0; 
                idle = 0;
                foreach (Entity el in instanceBest.getSecondMachine())
                {
                    if (el.GetTypeOfEntity() == 0)
                    {
                        sw.Write("o" + el.GetOperationNumber() + "_" + el.GetOperationID() + ", " + el.GetStartTime() + ", " + el.GetTime() + "; ");
                    }
                    else if (el.GetTypeOfEntity() == 1)
                    {
                        sw.Write("maint" + maint++ + "_M2" + ", " + el.GetStartTime() + ", " + el.GetTime() + "; ");
                    }
                    else if (el.GetTypeOfEntity() == 2)
                    {
                        sw.Write("idle" + idle++ + "_M2" + ", " + el.GetStartTime() + ", " + el.GetTime() + "; ");
                    }
                }
                sw.WriteLine();
                //M1 amount of maintance; summaric time
                int summmaricTime = 0, amount = 0;
                foreach(Entity el in instanceBest.getFirstMachine())
                {
                    if(el.GetTypeOfEntity() == 1)
                    {
                        amount++;
                        summmaricTime += el.GetTime();
                    }
                }
                sw.WriteLine(amount + ", " + summmaricTime);
                //M2 amount of maintance; summaric time
                summmaricTime = 0;
                amount = 0;
                foreach (Entity el in instanceBest.getSecondMachine())
                {
                    if (el.GetTypeOfEntity() == 1)
                    {
                        amount++;
                        summmaricTime += el.GetTime();
                    }
                }
                sw.WriteLine(amount + ", " + summmaricTime);
                //M1 amount of idle; summaric time
                summmaricTime = 0;
                amount = 0;
                foreach (Entity el in instanceBest.getFirstMachine())
                {
                    if (el.GetTypeOfEntity() == 2)
                    {
                        amount++;
                        summmaricTime += el.GetTime();
                    }
                }
                sw.WriteLine(amount + ", " + summmaricTime);
                //M2 amount of idle; summaric time
                summmaricTime = 0;
                amount = 0;
                foreach (Entity el in instanceBest.getSecondMachine())
                {
                    if (el.GetTypeOfEntity() == 2)
                    {
                        amount++;
                        summmaricTime += el.GetTime();
                    }
                }
                sw.WriteLine(amount + ", " + summmaricTime);
                sw.WriteLine("*** EOF ***");
            }
        }

    }
}
