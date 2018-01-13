using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace okSchedulingProblem
{
   
    class Machine
    {
        protected ArrayList elementsList = new ArrayList();
        protected int machineNumber;
        protected int size = 50;

        public void SetNumberOfElements(int tmp)
        {
            size = tmp;
        }

        public void SetMachineNumber(int tmp)
        {
            machineNumber = tmp;
        }

        public int GetMachineNumber()
        {
            return machineNumber;
        }

        public int GetLastEndTime()
        {
            int value = 0;
            foreach (Entity tmpElement in elementsList)
            {
                value = tmpElement.GetEndTime();
            }
            return value;
        }

        public int GetLastElementID()
        {
            int value = 0;
            foreach (Entity tmpElement in elementsList)
            {
                if(tmpElement.GetTypeOfEntity() == 0) value = tmpElement.GetOperationID();
            }
            return value;
        }

        public void PrintMachine()
        {
            foreach(Entity element in elementsList)
            {
                Console.Write(element.GetTypeOfEntity() + ": " + element.GetStartTime() + "-" + element.GetEndTime() + "  ");
            }
            Console.WriteLine();
        }

        public void AddElement(Entity Element)
        {
            elementsList.Add(Element);
        }

        public int GetFirstAvailablePosition()
        {
            int tmp = 0;
            foreach(Entity element in elementsList)
            {
                if (element.GetEndTime() == GetLastEndTime()) break;
                tmp++;
            }
            return tmp;
        }

        public ArrayList getMachineList()
        {
            return elementsList;
        }

        public int GetMachineScore()
        {
            int score = 0;
            foreach(Entity el in elementsList)
            {
                score += el.GetTime();
            }
            return score;
        }
    }
}
