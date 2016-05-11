using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkrConsole.PhilosophersEat
{
    public class Chopsticks
    {
        public int ID { get; set; }

        public bool IsUsed { get; private set; }                

        public void Use()
        {
            this.IsUsed = true;            
        }

        public void UnUse()
        {
            this.IsUsed = false;
        }
    }
}
