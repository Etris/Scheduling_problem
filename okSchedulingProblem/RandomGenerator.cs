using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace okSchedulingProblem
{
    class RandomGenerator
    {
        protected int size;
        protected ArrayList elements = new ArrayList();
        protected ArrayList maintances = new ArrayList();

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
                    int tmpTime = rnd.Next(1, 10);
                    int startTime = rnd.Next(i*50+1, (i*50 + 50));
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
                int tmpTime = rnd.Next(1, 12);
                Entity tmpOperation = new Entity(type, tmpTime, 0, 0, x, 0, 0);
                elements.Add(tmpOperation);
                summaricTimeFirstMachine += tmpTime;
            }
            for(int x = 0; x < size; x++)
            {
                int tmpTime = rnd.Next(1, 12);
                Entity tmpOperation = new Entity(type, tmpTime, 0, 1, x, 1, 0);
                elements.Add(tmpOperation);
            }
            int counter = 0;
            foreach(Entity element in elements)
            {
                if(counter < size/2 && element.GetMachine() == 0)
                {
                    rdTime = rnd.Next(1, (summaricTimeFirstMachine / 2));
                    element.SetReadyTime(rdTime);
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

    }
}
