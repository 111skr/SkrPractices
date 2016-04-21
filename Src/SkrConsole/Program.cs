using SkrConsole.PhilosophersEat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkrConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = PhilosopherEat();
            t.Start();
            t.Wait();
        }

        /// <summary>
        /// 产生死锁的哲学家吃饭
        /// </summary>
        /// <returns></returns>
        static Task PhilosopherEat()
        {
            Task t = new Task(() =>
            {
                int thinkMs = 1000,
                    handMs = 1,
                    eatMs = 100;

                Chopsticks chopsticks1 = new Chopsticks() { ID = 1 },
                    chopsticks2 = new Chopsticks() { ID = 2 },
                    chopsticks3 = new Chopsticks() { ID = 3 },
                    chopsticks4 = new Chopsticks() { ID = 4 },
                    chopsticks5 = new Chopsticks() { ID = 5 };

                APhilosopher<Chopsticks>
                    p1 = new Philosopher("P1", chopsticks1, chopsticks2) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p2 = new Philosopher("P2", chopsticks2, chopsticks3) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p3 = new Philosopher("P3", chopsticks3, chopsticks4) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p4 = new Philosopher("P4", chopsticks4, chopsticks5) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p5 = new Philosopher("P5", chopsticks5, chopsticks1) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs };

                TaskFactory tf = new TaskFactory();

                Task
                    t1 = tf.StartNew(p1.Run),
                    t2 = tf.StartNew(p2.Run),
                    t3 = tf.StartNew(p3.Run),
                    t4 = tf.StartNew(p4.Run),
                    t5 = tf.StartNew(p5.Run);

                Task.WaitAll(t1, t2, t3, t4);
            });

            return t;
        }
    }
}
