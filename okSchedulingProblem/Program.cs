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
            AcoMaster aco = new AcoMaster();
            Console.WriteLine(setID);
            for (int i = 0; i < 5; i++)
            {
                aco.setID = (setID + "." + i.ToString());
                aco.SetData(num);
                for (int c = 0; c< 10; c++) {
                    aco.Init(num, pop, 6.5, 0.25);
                    break;
                }
            }
            Console.ReadLine();
        }
    }
}
