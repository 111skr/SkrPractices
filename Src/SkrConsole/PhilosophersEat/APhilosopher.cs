using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkrConsole.PhilosophersEat
{
    public abstract class APhilosopher<Chopsticks> where Chopsticks : class, new()
    {
        public int HandMs { get; set; }

        public int ThinkMs { get; set; }

        public int EatMs { get; set; }

        public string Name { get; protected set; }

        protected Chopsticks left, right;
        
        public APhilosopher(string name, Chopsticks left, Chopsticks right, int sleepMs = 1000)
        {
            this.Name = name;
            this.left = left;
            this.right = right;
        }

        public abstract void Run();
    }
}
