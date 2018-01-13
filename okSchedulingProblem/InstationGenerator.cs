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
        public int size = 50;

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
                isModified = true;
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
                if (element.GetOperationID() == pair)
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
                Entity element = (Entity)firstMachineTasks[0];
                int a;
                if (firstMachine.GetLastEndTime() + element.GetStartTime() + element.GetTime() < getTimeOfNextMaintance(firstMachineTempMaintance) && element.GetReadyTime() > firstMachine.GetLastEndTime())
                {
                    element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                    element.SetEndTime(firstMachine.GetLastEndTime() + element.GetTime() + 1);
                    firstMachine.AddElement(element);
                    firstMachineTasks.Remove(element);
                }
                else if (firstMachineTempMaintance.Count == 0)
                {
                    element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                    element.SetEndTime(firstMachine.GetLastEndTime() + element.GetTime() + 1);
                    firstMachine.AddElement(element);
                    firstMachineTasks.Remove(element);
                }
                else
                {
                    if (getTimeOfNextMaintance(firstMachineTempMaintance) > 0)
                    {
                        Entity tms = new Entity(2, (getTimeOfNextMaintance(firstMachineTempMaintance) - firstMachine.GetLastEndTime() - 2), 0, 0, 0, 0, 0);
                        tms.SetStartTime(firstMachine.GetLastEndTime() + 1);
                        tms.SetEndTime(getTimeOfNextMaintance(firstMachineTempMaintance) - 1);
                        tms.SetTime(tms.GetEndTime() - tms.GetStartTime());
                        firstMachine.AddElement(tms);
                        if (firstMachineTempMaintance.Count > 0)
                        {
                            Entity tpc = (Entity)firstMachineTempMaintance[0];
                            firstMachine.AddElement(tpc);
                            firstMachineTempMaintance.Remove(tpc);
                        }
                    }
                }
                int pos = 0;
                while (firstMachineTasks.Count > 0) {
                    a = firstMachine.GetLastElementID();
                    int take = TakeBest(ACO.arr, a);
                    if (IsPossibleToTakeFromList(firstMachineTasks, take) == true)
                    {
                        pos = GetPositionAtList(take, firstMachineTasks);
                    }
                    else
                    {
                        take = WhichElementIShouldTake(ACO.arr, a);
                        if(IsPossibleToTakeFromList(firstMachineTasks, take) == true)
                        {
                            pos = GetPositionAtList(take, firstMachineTasks);
                        }
                        else
                        {
                            pos = 0;
                        }
                    }
                    Random rnd = new Random();
                    //if (rnd.Next(0, 10) > 6) pos = 0;
                    element = (Entity)firstMachineTasks[pos];
                    if (firstMachine.GetLastEndTime() + element.GetStartTime() + element.GetTime() < getTimeOfNextMaintance(firstMachineTempMaintance) && element.GetReadyTime() > firstMachine.GetLastEndTime())
                    {
                        element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                        element.SetEndTime(firstMachine.GetLastEndTime() + element.GetTime() + 1);
                        firstMachine.AddElement(element);
                        firstMachineTasks.Remove(element);
                    }
                    else if (firstMachineTempMaintance.Count == 0)
                    {
                        element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                        element.SetEndTime(firstMachine.GetLastEndTime() + element.GetTime() + 1);
                        firstMachine.AddElement(element);
                        firstMachineTasks.Remove(element);
                    }
                    else
                    {
                        if (getTimeOfNextMaintance(firstMachineTempMaintance) > 0)
                        {
                            Entity tms = new Entity(2, (getTimeOfNextMaintance(firstMachineTempMaintance) - firstMachine.GetLastEndTime() - 2), 0, 0, 0, 0, 0);
                            tms.SetStartTime(firstMachine.GetLastEndTime() + 1);
                            tms.SetEndTime(getTimeOfNextMaintance(firstMachineTempMaintance) - 1);
                            tms.SetTime(tms.GetEndTime() - tms.GetStartTime());
                            firstMachine.AddElement(tms);
                            if (firstMachineTempMaintance.Count > 0)
                            {
                                Entity tpc = (Entity)firstMachineTempMaintance[0];
                                firstMachine.AddElement(tpc);
                                firstMachineTempMaintance.Remove(tpc);
                            }
                        }
                    }
                }
            }
            else
            {
                while (firstMachineTasks.Count > 0)
                {
                    int i = 0;
                    Entity element = (Entity)firstMachineTasks[0];
                    if (firstMachine.GetLastEndTime() + element.GetStartTime() + element.GetTime() < getTimeOfNextMaintance(firstMachineTempMaintance) && element.GetReadyTime() > firstMachine.GetLastEndTime())
                    {
                        element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                        element.SetEndTime(firstMachine.GetLastEndTime() + element.GetTime() + 1);
                        firstMachine.AddElement(element);
                        firstMachineTasks.Remove(element);
                    }
                    else if (firstMachineTempMaintance.Count == 0)
                    {
                        element.SetStartTime(firstMachine.GetLastEndTime() + 1);
                        element.SetEndTime(firstMachine.GetLastEndTime() + element.GetTime() + 1);
                        firstMachine.AddElement(element);
                        firstMachineTasks.Remove(element);
                    }
                    else
                    {
                        if (getTimeOfNextMaintance(firstMachineTempMaintance) > 0)
                        {
                            Entity tms = new Entity(2, (getTimeOfNextMaintance(firstMachineTempMaintance) - firstMachine.GetLastEndTime() - 2), 0, 0, 0, 0, 0);
                            tms.SetStartTime(firstMachine.GetLastEndTime() + 1);
                            tms.SetEndTime(getTimeOfNextMaintance(firstMachineTempMaintance) - 1);
                            tms.SetTime(tms.GetEndTime() - tms.GetStartTime());
                            firstMachine.AddElement(tms);
                            if (firstMachineTempMaintance.Count > 0)
                            {
                                Entity tpc = (Entity)firstMachineTempMaintance[0];
                                firstMachine.AddElement(tpc);
                                firstMachineTempMaintance.Remove(tpc);
                            }
                        }
                    }
                }
            }
        }

        private void FillSecondMachine(ArrayList secondMachineTasks, ArrayList secondMachineTempMaintance, Machine secondMachine, ArrayList firstMachineTasks, bool usingTable, int tableEl, AcoMaster ACO)
        {
            if (usingTable == true)
            {
                Entity element = (Entity)secondMachineTasks[0];
                int a;
                if (secondMachine.GetLastEndTime() + element.GetStartTime() + element.GetTime() < getTimeOfNextMaintance(secondMachineTempMaintance) && GetEndTimeOfPair(firstMachineTasks, element.GetOperationID()) > secondMachine.GetLastEndTime())
                {
                    element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                    element.SetEndTime(secondMachine.GetLastEndTime() + element.GetTime() + 1);
                    secondMachine.AddElement(element);
                    secondMachineTasks.Remove(element);
                }
                else if (secondMachineTempMaintance.Count == 0)
                {
                    element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                    element.SetEndTime(secondMachine.GetLastEndTime() + element.GetTime() + 1);
                    secondMachine.AddElement(element);
                    secondMachineTasks.Remove(element);
                }
                else
                {
                    if (getTimeOfNextMaintance(secondMachineTempMaintance) > 0)
                    {
                        Entity tms = new Entity(2, (getTimeOfNextMaintance(secondMachineTempMaintance) - secondMachine.GetLastEndTime() - 2), 0, 0, 0, 0, 0);
                        tms.SetStartTime(secondMachine.GetLastEndTime() + 1);
                        tms.SetEndTime(getTimeOfNextMaintance(secondMachineTempMaintance) - 1);
                        tms.SetTime(tms.GetEndTime() - tms.GetStartTime());
                        secondMachine.AddElement(tms);
                        if (secondMachineTempMaintance.Count > 0)
                        {
                            Entity tpc = (Entity)secondMachineTempMaintance[0];
                            secondMachine.AddElement(tpc);
                            secondMachineTempMaintance.Remove(tpc);
                        }
                    }
                }
                int pos = 0;
                while (secondMachineTasks.Count > 0)
                {
                    a = secondMachine.GetLastElementID();
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
                    Random rnd = new Random();
                    //if (rnd.Next(0, 10) > 5) pos = 0;
                    element = (Entity)secondMachineTasks[pos];
                    if (secondMachine.GetLastEndTime() + element.GetStartTime() + element.GetTime() < getTimeOfNextMaintance(secondMachineTempMaintance) && GetEndTimeOfPair(firstMachineTasks, element.GetOperationID()) > secondMachine.GetLastEndTime())
                    {
                        element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                        element.SetEndTime(secondMachine.GetLastEndTime() + element.GetTime() + 1);
                        secondMachine.AddElement(element);
                        secondMachineTasks.Remove(element);
                    }
                    else if (secondMachineTempMaintance.Count == 0)
                    {
                        element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                        element.SetEndTime(secondMachine.GetLastEndTime() + element.GetTime() + 1);
                        secondMachine.AddElement(element);
                        secondMachineTasks.Remove(element);
                    }
                    else
                    {
                        if (getTimeOfNextMaintance(secondMachineTempMaintance) > 0)
                        {
                            Entity tms = new Entity(2, (getTimeOfNextMaintance(secondMachineTempMaintance) - secondMachine.GetLastEndTime() - 2), 0, 0, 0, 0, 0);
                            tms.SetStartTime(secondMachine.GetLastEndTime() + 1);
                            tms.SetEndTime(getTimeOfNextMaintance(secondMachineTempMaintance) - 1);
                            tms.SetTime(tms.GetEndTime() - tms.GetStartTime());
                            secondMachine.AddElement(tms);
                            if (secondMachineTempMaintance.Count > 0)
                            {
                                Entity tpc = (Entity)secondMachineTempMaintance[0];
                                secondMachine.AddElement(tpc);
                                secondMachineTempMaintance.Remove(tpc);
                            }
                        }
                    }

                }
                }
            else
            {
                while(secondMachineTasks.Count > 0)
                {
                    int i = 0;
                    Entity element = (Entity)secondMachineTasks[i];
                    if (secondMachine.GetLastEndTime() + element.GetStartTime() + element.GetTime() < getTimeOfNextMaintance(secondMachineTempMaintance) && GetEndTimeOfPair(firstMachineTasks, element.GetOperationID()) > secondMachine.GetLastEndTime())
                    {
                        element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                        element.SetEndTime(secondMachine.GetLastEndTime() + element.GetTime() + 1);
                        secondMachine.AddElement(element);
                        secondMachineTasks.Remove(element);
                    }
                    else if (secondMachineTempMaintance.Count == 0)
                    {
                        element.SetStartTime(secondMachine.GetLastEndTime() + 1);
                        element.SetEndTime(secondMachine.GetLastEndTime() + element.GetTime() + 1);
                        secondMachine.AddElement(element);
                        secondMachineTasks.Remove(element);
                    }
                    else
                    {
                        if (getTimeOfNextMaintance(secondMachineTempMaintance) > 0)
                        {
                            Entity tms = new Entity(2, (getTimeOfNextMaintance(secondMachineTempMaintance) - secondMachine.GetLastEndTime() - 2), 0, 0, 0, 0, 0);
                            tms.SetStartTime(secondMachine.GetLastEndTime() + 1);
                            tms.SetEndTime(getTimeOfNextMaintance(secondMachineTempMaintance) - 1);
                            tms.SetTime(tms.GetEndTime() - tms.GetStartTime());
                            secondMachine.AddElement(tms);
                            if (secondMachineTempMaintance.Count > 0)
                            {
                                Entity tpc = (Entity)secondMachineTempMaintance[0];
                                secondMachine.AddElement(tpc);
                                secondMachineTempMaintance.Remove(tpc);
                            }
                        }
                    }
                }
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
            return ((firstMachine.GetMachineScore()) + (secondMachine.GetMachineScore()));
        }

    }
}
