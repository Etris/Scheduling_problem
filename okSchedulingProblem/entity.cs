using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okSchedulingProblem
{
    class Entity : ICloneable
    {
        //TYPES: 0 - task; 1 - maintance; 2 - null job
        protected int typeOfEntity;
        protected int timeOfEntity;
        protected int positionOfEntity;
        protected int machineOfEntity;
        protected int readyTime;
        protected int operationID; //number of set
        protected int operationNumber; //0-1 

        protected int startTime, endTime;
        
        public Entity(int type, int time, int postition, int machine, int operation, int subOperation, int rTime)
        {
            SetType(type);
            SetTime(time);
            SetPostition(postition);
            SetMachine(machine);
            operationID = operation;
            operationNumber = subOperation;
        }

        public int GetOperationID()
        {
            return operationID;
        }
        //Values 0-1 (machine)
        public int GetOperationNumber()
        {
            return operationNumber;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void SetReadyTime(int value)
        {
            readyTime = value;
        }

        public void SetType(int value)
        {
            typeOfEntity = value;
        }

        public int GetTypeOfEntity()
        {
            return typeOfEntity;
        }

        public void SetTime(int value)
        {
            timeOfEntity = value;
        }

        public int GetTime()
        {
            return timeOfEntity;
        }

        public void SetPostition(int value)
        {
            positionOfEntity = value;
        }

        public int GetReadyTime()
        {
            return readyTime;
        }

        public int GetPosition()
        {
            return positionOfEntity;
        }

        public void SetMachine(int value)
        {
            machineOfEntity = value;
        }

        public int GetMachine()
        {
            return machineOfEntity;
        }

        public void SetStartTime(int value)
        {
            startTime = value;
        }

        public int GetStartTime()
        {
            return startTime;
        }

        public void SetEndTime(int value)
        {
            endTime = value;
        }

        public int GetEndTime()
        {
            return endTime;
        }
    }
}
