using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace okSchedulingProblem
{
    class InstationGenerator : ICloneable
    {
        public bool isModified = false;
        ArrayList firstMachineTasks = new ArrayList();
        ArrayList secondMachineTasks = new ArrayList();
        Machine firstMachine;
        Machine secondMachine;
        public int size;

        private int getTimeOfNextMaintance(ArrayList tmp)
        {
            int val = 0;
            foreach(Entity element in tmp)
            {
                val = element.GetStartTime();
                break;
            }
            return val;
        }

        private bool IsPossibleToTakeFromList(ArrayList tmp, int id)
        {
            foreach(Entity element in tmp)
            {
                if(element.GetTypeOfEntity() == 0)
                {
                    if(element.GetOperationID() == id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int TakeBest(double[,] arr, int actualID)
        {
            int tmpPos = 0;
            double max = 0;
            int iy;
            for (iy = 0; iy < size; iy++)
            {
                if(arr[actualID, iy] > max)
                {
                    max = arr[actualID, iy];
                    tmpPos = iy;
                }
            }
            return iy;
        }

        private int WhichElementIShouldTake(double[,] arr, int actualID)
        {
            double sum = 0;
            int iy;
            for (iy = 0; iy < size; iy++)
            {
                sum += arr[actualID, iy];
            }
            //Console.WriteLine((int)sum);
            Random rnd = new Random();
            double roulette = rnd.Next(0, (int)sum);
            double tmpValue = 0;
            for (iy = 0; iy < size; iy++){
                if (tmpValue >= roulette)
                {
                    break;
                }
                tmpValue += arr[actualID, iy];
            }
            return iy;  
        }

        private int GetPositionAtList(int actualID, ArrayList tmp)
        {
            bool found = false;
            int tmpPos = 0;
            foreach(Entity element in tmp)
            {
                if(element.GetOperationID() == actualID)
                {
                    found = true;
                    break;
                }
                tmpPos++;
            }
            if (found == false) tmpPos = -1;
            return tmpPos;
        }

        public void CreateNewInstantion(ArrayList fullList, ArrayList maintances, bool roulette, AcoMaster ACO)
        {
            firstMachine = new Machine();
            firstMachine.SetMachineNumber(0);
            firstMachine.SetNumberOfElements(size);
            secondMachine = new Machine();
            ArrayList firstMachineTemp = new ArrayList();
            ArrayList secondMachineTemp = new ArrayList();
            ArrayList firstMachineTempMaintance = new ArrayList();
            ArrayList secondMachineTempMaintance = new ArrayList();
            Random r = new Random();
            int randomIndex = 0;
            //Console.WriteLine("START DIVIDING DATA");
            foreach(Entity element in fullList)
            {
                if(element.GetMachine() == 0) firstMachineTemp.Add(element);
                else secondMachineTemp.Add(element);
            }
            foreach (Entity element in maintances)
            {
                if (element.GetMachine() == 0) firstMachineTempMaintance.Add(element);
                else secondMachineTempMaintance.Add(element);
            }
            while (firstMachineTemp.Count > 0)
            {
                randomIndex = r.Next(0, firstMachineTemp.Count); //Choose a random object in the list
                firstMachineTasks.Add(firstMachineTemp[randomIndex]); //add it to the new, random list
                firstMachineTemp.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            while (secondMachineTemp.Count > 0)
            {
                randomIndex = r.Next(0, secondMachineTemp.Count); //Choose a random object in the list
                secondMachineTasks.Add(secondMachineTemp[randomIndex]); //add it to the new, random list
                secondMachineTemp.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            //Console.WriteLine("SHUFFLE DONE!");
            if (roulette == false)
            {
                
                FillFirstMachine(firstMachineTasks, firstMachineTempMaintance, firstMachine, false, 0, ACO);
                FillSecondMachine(secondMachineTasks, secondMachineTempMaintance, secondMachine, firstMachineTasks, false, 0, ACO);
            }else if(roulette == true)
            {
                FillFirstMachine(firstMachineTasks, firstMachineTempMaintance, firstMachine, true, 0, ACO);
                FillSecondMachine(secondMachineTasks, secondMachineTempMaintance, secondMachine, firstMachineTasks, true, 0, ACO);
            }
            //Console.WriteLine("FILLING DONE!");
            //PrintFirstMachine();

            //PrintSecondMachine();

        }

        public ArrayList getFirstMachine()
        {
            return firstMachine.getMachineList();
        }

        public ArrayList getSecondMachine()
        {
            return secondMachine.getMachineList();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void PrintMachines()
        {
            PrintFirstMachine(firstMachine);
            PrintSecondMachine(secondMachine);
            Console.WriteLine(getInstantionScore());
        }
        private void PrintFirstMachine(Machine firstMachine)
        {
            firstMachine.PrintMachine();
        }
        
        private void PrintSecondMachine(Machine secondMachine)
        {
            secondMachine.PrintMachine();
        }

        private int GetEndTimeOfPair(ArrayList firstMachineTasks, int pair)
        {
            int value = 0;
            foreach (Entity element in firstMachineTasks)
            {
                if (element.GetOperationID() == pair && element.GetTypeOfEntity() == 0)
                {
                    value = element.GetEndTime();
                    break;
                }
            }
            return value;
        }

        private void FillFirstMachine(ArrayList firstMachineTasks, ArrayList firstMachineTempMaintance, Machine firstMachine, bool usingTable, int tableEl, AcoMaster ACO)
        {
            if (usingTable == true)
            {
                Random rnd = new Random();
                while (firstMachineTasks.Count > 0)
                {
                    //firstMachine.PrintMachine();
                    TimeSpan span = new TimeSpan(0, 0, 0);
                    TimeSpan perc = (span = DateTime.Now - MyTimer.Start);
                    double ft = span.Seconds;
                    double sc = ACO.endVar.Seconds + (60 * ACO.endVar.Minutes);
                    int holder = (int)((ft / sc) * 100);
                    int roultette = rnd.Next(0, 100);
                    int pos = 0;
                    if (roultette < holder - 5)
                    {
                        isModified = true;
                        int a = firstMachine.GetLastElementID();
                        int take = WhichElementIShouldTake(ACO.arr, a);
                        if (IsPossibleToTakeFromList(firstMachineTasks, take) == true)
                        {
                            pos = GetPositionAtList(take, firstMachineTasks);
                        }
                        else
                        {
                            take = WhichElementIShouldTake(ACO.arr, a);
                            if (IsPossibleToTakeFromList(firstMachineTasks, take) == true)
                            {
                                pos = GetPositionAtList(take, firstMachineTasks);
                            }
                            else
                            {
                                pos = 0;
                            }
                        }
                    }
                    else
                    {
                        pos = 0;
                    }
                    Entity element = (Entity)firstMachineTasks[pos];
                    bool added = false;
                    while (!added)
                    {
                        if(element.GetReadyTime() < firstMachine.GetLastEndTime())
                        {
                            if(firstMachineTempMaintance.Count != 0 && getTimeOfNextMaintance(firstMachineTempMaintance) <= (firstMachine.GetLastEndTime() + element.GetTime()))
                            {
                                Entity maint = (Entity)firstMachineTempMaintance[0];
                                if(maint.GetStartTime() > firstMachine.GetLastEndTime())
                                {
                                    Entity idle = new Entity(2, 0, 0, 0, 0, 0, 0);
                                    idle.SetStartTime(firstMachine.GetLastEndTime() + 1);
                                    idle.SetTime(maint.GetStartTime() - firstMachine.GetLastEndTime());
                                    idle.SetEndTime(idle.GetStartTime() + idle.GetTime());
                                    firstMachine.AddElement(idle);
                                }
                                firstMachine.AddElement(maint);
                                firstMachineTempMaintance.Remove(maint);
                            }
                            else
                            {
                                element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                                element.SetEndTime(element.GetStartTime() + element.GetTime());
                                firstMachine.AddElement(element);
                                firstMachineTasks.Remove(element);
                                added = true;
                            }
                        }
                        else
                        {
                            if(firstMachineTempMaintance.Count != 0 
                                && getTimeOfNextMaintance(firstMachineTempMaintance) < (element.GetTime() + element.GetReadyTime()))
                            {
                                Entity maint = (Entity)firstMachineTempMaintance[0];
                                if(maint.GetStartTime() > firstMachine.GetLastEndTime())
                                {
                                    Entity idle = new Entity(2, 0, 0, 0, 0, 0, 0);
                                    idle.SetStartTime(firstMachine.GetLastEndTime() + 1);
                                    idle.SetTime(maint.GetStartTime() - firstMachine.GetLastEndTime());
                                    idle.SetEndTime(idle.GetStartTime() + idle.GetTime());
                                    firstMachine.AddElement(idle);
                                }
                                element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                                element.SetEndTime(element.GetStartTime() + element.GetTime());
                                firstMachine.AddElement(maint);
                                firstMachineTempMaintance.Remove(maint);
                            }
                            else
                            {
                                if(element.GetReadyTime() > firstMachine.GetLastEndTime())
                                {
                                    Entity idle = new Entity(2, 0, 0, 0, 0, 0, 0);
                                    idle.SetStartTime(firstMachine.GetLastEndTime() + 1);
                                    idle.SetTime(element.GetReadyTime() - firstMachine.GetLastEndTime());
                                    idle.SetEndTime(idle.GetStartTime() + idle.GetTime());
                                    firstMachine.AddElement(idle);
                                }
                                element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                                element.SetEndTime(element.GetStartTime() + element.GetTime());
                                firstMachine.AddElement(element);
                                firstMachineTempMaintance.Remove(element);
                                added = true;
                            }
                        }
                    }
                }
            }
            else
            {

            }
        }

        private void FillSecondMachine(ArrayList secondMachineTasks, ArrayList secondMachineTempMaintance, Machine secondMachine, ArrayList firstMachineTasks, bool usingTable, int tableEl, AcoMaster ACO)
        {
            if (usingTable == true)
            {
                Random rnd = new Random();
                while (secondMachineTasks.Count > 0)
                {
                    TimeSpan span = new TimeSpan(0, 0, 0);
                    TimeSpan perc = (span = DateTime.Now - MyTimer.Start);
                    double ft = span.Seconds;
                    double sc = ACO.endVar.Seconds + (60 * ACO.endVar.Minutes);
                    int holder = (int)((ft / sc) * 100);
                    int roultette = rnd.Next(0, 100);
                    int pos = 0;
                    if (roultette < holder)
                    {
                        int a = secondMachine.GetLastElementID();
                        int take = WhichElementIShouldTake(ACO.arrx2, a);
                        if (IsPossibleToTakeFromList(secondMachineTasks, take) == true)
                        {
                            pos = GetPositionAtList(take, secondMachineTasks);
                        }
                        else
                        {
                            take = WhichElementIShouldTake(ACO.arrx2, a);
                            if (IsPossibleToTakeFromList(secondMachineTasks, take) == true)
                            {
                                pos = GetPositionAtList(take, secondMachineTasks);
                            }
                            else
                            {
                                pos = 0;
                            }
                        }
                    }
                    else
                    {
                        pos = 0;
                    }
                    Entity element = (Entity)secondMachineTasks[pos];
                    bool added = false;
                    while (!added)
                    {
                        if (GetEndTimeOfPair(firstMachine.getMachineList(), element.GetOperationID()) < secondMachine.GetLastEndTime())
                        {
                            if (secondMachineTempMaintance.Count != 0 && getTimeOfNextMaintance(secondMachineTempMaintance) <= (secondMachine.GetLastEndTime() + element.GetTime()))
                            {
                                Entity maint = (Entity)secondMachineTempMaintance[0];
                                if (maint.GetStartTime() > secondMachine.GetLastEndTime())
                                {
                                    Entity idle = new Entity(2, 0, 0, 1, 0, 0, 0);
                                    idle.SetStartTime(secondMachine.GetLastEndTime() + 1);
                                    idle.SetTime(maint.GetStartTime() - secondMachine.GetLastEndTime());
                                    idle.SetEndTime(idle.GetStartTime() + idle.GetTime());
                                    secondMachine.AddElement(idle);
                                }
                                secondMachine.AddElement(maint);
                                secondMachineTempMaintance.Remove(maint);
                            }
                            else
                            {
                                element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                                element.SetEndTime(element.GetStartTime() + element.GetTime());
                                secondMachine.AddElement(element);
                                secondMachineTasks.Remove(element);
                                added = true;
                            }
                        }
                        else
                        {
                            if (secondMachineTempMaintance.Count != 0
                                && getTimeOfNextMaintance(secondMachineTempMaintance) < (element.GetTime() + GetEndTimeOfPair(firstMachine.getMachineList(), element.GetOperationID())))
                            {
                                Entity maint = (Entity)secondMachineTempMaintance[0];
                                if (maint.GetStartTime() > secondMachine.GetLastEndTime())
                                {
                                    Entity idle = new Entity(2, 0, 0, 1, 0, 0, 0);
                                    idle.SetStartTime(secondMachine.GetLastEndTime() + 1);
                                    idle.SetTime(maint.GetStartTime() - secondMachine.GetLastEndTime());
                                    idle.SetEndTime(idle.GetStartTime() + idle.GetTime());
                                    secondMachine.AddElement(idle);
                                }
                                element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                                element.SetEndTime(element.GetStartTime() + element.GetTime());
                                secondMachine.AddElement(maint);
                                secondMachineTempMaintance.Remove(maint);
                            }
                            else
                            {
                                if (GetEndTimeOfPair(firstMachine.getMachineList(), element.GetOperationID()) > secondMachine.GetLastEndTime())
                                {
                                    Entity idle = new Entity(2, 0, 0, 1, 0, 0, 0);
                                    idle.SetStartTime(secondMachine.GetLastEndTime() + 1);
                                    idle.SetTime(GetEndTimeOfPair(firstMachine.getMachineList(), element.GetOperationID()) - secondMachine.GetLastEndTime());
                                    idle.SetEndTime(idle.GetStartTime() + idle.GetTime());
                                    secondMachine.AddElement(idle);
                                }
                                element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                                element.SetEndTime(element.GetStartTime() + element.GetTime());
                                secondMachine.AddElement(element);
                                secondMachineTempMaintance.Remove(element);
                                added = true;
                            }
                        }
                    }
                }
                }
            else
            {

            }
        }

        private ArrayList Shuffle(ref ArrayList list)
        {
            ArrayList randomList = new ArrayList();
            Random r = new Random();
            int randomIndex = 0;
            while (list.Count > 0)
            {
                randomIndex = r.Next(0, list.Count); //Choose a random object in the list
                randomList.Add(list[randomIndex]); //add it to the new, random list
                list.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList;
        }

        public int getInstantionScore()
        {
            return (firstMachine.GetLastEndTime() + secondMachine.GetLastEndTime());
        }

    }
}
