using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkrConsole.PhilosophersEat
{
    public class PhilosopherWithOrderChopstics : Philosopher
    {
        public PhilosopherWithOrderChopstics(string name, Chopsticks left, Chopsticks right)
            : base(name, left, right)
        {
            this.Name = name;
            if (left.ID > right.ID)
            {
                this.left = right;
                this.right = left;
            }
            else
            {
                this.left = left;
                this.right = right;
            }
        }
    }
}
