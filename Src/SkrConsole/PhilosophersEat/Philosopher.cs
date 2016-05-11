using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkrConsole.PhilosophersEat
{
    public class Philosopher : APhilosopher<Chopsticks>
    {
        public Philosopher(string name, Chopsticks left, Chopsticks right)
            : base(name, left, right)
        {

        }

        public override void Run()
        {
            while (true)
            {
                Console.WriteLine(string.Format("[{0}] thinking begin", this.Name));
                Thread.Sleep(this.ThinkMs);
                Console.WriteLine(string.Format("[{0}] thinking end", this.Name));

                Console.WriteLine(string.Format("[{0}] [wait] left chopsticks, cp id:[{1}]", this.Name, left.ID));
                lock (left)
                {
                    Console.WriteLine(string.Format("[{0}] [hand] left chopsticks, cp id:[{1}]", this.Name, left.ID));
                    Thread.Sleep(this.HandMs);
                    Console.WriteLine(string.Format("[{0}] [wait] right chopsticks, cp id:[{1}]", this.Name, right.ID));
                    lock (right)
                    {
                        Console.WriteLine(string.Format("[{0}] hand right chopsticks, cp id:[{1}]", this.Name, right.ID));
                        Console.WriteLine(string.Format("[{0}] eating", this.Name));
                        Thread.Sleep(this.EatMs);

                        Console.WriteLine(string.Format("[{0}] eat end", this.Name));
                    }
                }
            }
        }
    }
}
