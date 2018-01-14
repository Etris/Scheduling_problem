using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace okSchedulingProblem
{
    class RandomGenerator
    {
        protected int size;
        protected ArrayList elements = new ArrayList();
        protected ArrayList maintances = new ArrayList();
        private int maxLen;

        public void SetNumberOfElements(int tmp)
        {
            size = tmp;
        }

        public void GenereteMaintances()
        {
            Random rnd = new Random();
            int type = 1;
            int tmpMachine = 0;
            for (int x = 0; x < 2; x++)
            {   
                for (int i = 0; i < (size / 5); i++)
                {
                    int startTime = rnd.Next(i*(maxLen / (size/5))+1 , (i*(maxLen / (size/5)) + (maxLen / (size/5))));
                    int par = 10;
                    if ((i * (maxLen / (size / 5)) + (maxLen / (size / 5))) - startTime < 10){
                        par = ((i * (maxLen / (size / 5)) + (maxLen / (size / 5)))) - startTime;
                    }
                    int tmpTime = rnd.Next(1, 10);
                    Entity tmpMaintance = new Entity(type, tmpTime, i, tmpMachine, 0, 0, 0);
                    tmpMaintance.SetStartTime(startTime);
                    tmpMaintance.SetEndTime(startTime + tmpTime);
                    maintances.Add(tmpMaintance);
                }
                tmpMachine++;
            }
        }

        
        public void GenereteOperations()
        {
            Random rnd = new Random();
            int type = 0;
            int rdTime;
            int summaricTimeFirstMachine = 0;
            for(int x = 0; x < size; x++)
            {
                int tmpTime = rnd.Next(1, 5);
                Entity tmpOperation = new Entity(type, tmpTime, 0, 0, x, 0, 0);
                elements.Add(tmpOperation);
                summaricTimeFirstMachine += tmpTime;
            }
            this.maxLen = summaricTimeFirstMachine;
            for(int x = 0; x < size; x++)
            {
                int tmpTime = rnd.Next(1, 5);
                Entity tmpOperation = new Entity(type, tmpTime, 0, 1, x, 1, 0);
                elements.Add(tmpOperation);
            }
            int counter = 0;
            foreach(Entity element in elements)
            {
                if((counter < (size/2)) && element.GetMachine() == 0)
                {
                    rdTime = rnd.Next(1, (summaricTimeFirstMachine / 2));
                    element.SetReadyTime(rdTime);
                    counter++;
                }
                else
                {
                    break;
                }
            }
        }

        public void PrintMaintances()
        {
            foreach(Entity element in maintances)
            {
                Console.WriteLine("Maintance time: " + element.GetStartTime() + "-" + element.GetEndTime() + " at machine: " + element.GetMachine());
            }
        }

        public void PrintOperations()
        {
            foreach(Entity element in elements)
            {
                Console.WriteLine("Op: " + element.GetOperationID() + "#" + element.GetOperationNumber() + " time: " + element.GetTime()+ " rTime: " + element.GetReadyTime() + " at machine: " + element.GetMachine());
            }
        }

        public void TransferData(InstationGenerator tmp, AcoMaster tmr)
        {
            ArrayList trs = new ArrayList();
            foreach(Entity el in elements)
            {
                Entity a = (Entity)el.Clone();
                trs.Add(a);
            }
            tmp.CreateNewInstantion(trs, maintances, false, tmr);
        }

        public void TransferWithRoulette(InstationGenerator tmp, AcoMaster tmr)
        {
            ArrayList trs = new ArrayList();
            foreach (Entity el in elements)
            {
                Entity a = (Entity)el.Clone();
                trs.Add(a);
            }
            tmp.CreateNewInstantion(trs, maintances, true, tmr);
        }

        public void SaveInstantion(string name)
        { 
            string fileName = string.Format(name + "-instance.txt");
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("***" + name + "****");
                sw.WriteLine(size);
                for (int i = 0; i < size; i++)
                {
                    int timeFirst = 0, timeSecond = 0, ReadyTime = 0;
                    foreach (Entity el in elements)
                    {
                        if(el.GetOperationID() == i && el.GetOperationNumber() == 0)
                        {
                            timeFirst = el.GetTime();
                            ReadyTime = el.GetReadyTime();
                            break;
                        }
                    }
                    foreach (Entity el in elements)
                    {
                        if (el.GetOperationID() == i && el.GetOperationNumber() == 1)
                        {
                            timeSecond = el.GetTime();
                            break;
                        }
                    }
                    sw.WriteLine(timeFirst + "; " + timeSecond + "; 0; 1; " + ReadyTime);
                }
                int tmp = 0;
                foreach (Entity el in maintances)
                {
                    sw.WriteLine(tmp + "; " + el.GetMachine() + "; " + el.GetTime() + "; " + el.GetStartTime() + ";");
                    tmp++;
                }
                sw.WriteLine("*** EOF ***");
            }
            Console.WriteLine("Instantion file created!");
        }

    }
}
