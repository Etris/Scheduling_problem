using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Flowshop, liczba maszyn m=2, liczba zadań n,
operacje niewznawialne ,
dla pierwszej maszyny k okresów przestoju o losowym czasie rozpoczęcia i trwania (określonym przez generator instancji problemu), k >= n/5,
czas gotowości dla operacji nr 1 każdego zadania, nieprzekraczający połowy sumy czasów wszystkich operacji dla maszyny I, dla połowy operacji nr 1: czas gotowości = 0;
minimalizacja całkowitego czasu wykonania wszystkich operacji
*/
namespace okSchedulingProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of elements: ");
            int num = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter size of population: ");
            int pop = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter set id: ");
            string setID = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Enter end time in seconds: ");
            int timmy = Convert.ToInt32(Console.ReadLine());
            //Console.WriteLine("Enter smooth parameter: ");
            //double smooth = Convert.ToDouble(Console.ReadLine());
            //Console.WriteLine("Enter evaporate parameter: ");
            //double eva = Convert.ToDouble(Console.ReadLine());
            AcoMaster aco = new AcoMaster();
            aco.endVar = new TimeSpan(0, 0, timmy);
            Console.WriteLine(setID);
            aco.setID = (setID);
            aco.SetData(num);
            aco.Init(num, pop, 6.5, 0.05);
            Console.WriteLine("Click any key to continue..");
            Console.ReadLine();
        }
    }
}
