using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Xunit.Abstractions;

namespace WebApp.Tests.Concurrent
{
    /// <summary>
    /// 哲学家就餐问题(可视化没实现成功，先不能用)
    /// </summary>
    public class PhilosopherEat
    {
        private readonly ITestOutputHelper output;

        public PhilosopherEat(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(DisplayName = "哲学家吃饭，没有锁策略，会产生死锁")]
        public void Philosophers_eat_wiht_no_strategy_should_create_deadlock()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task t = Task.Factory.StartNew(() =>
            {
                int thinkMs = 1000,
                    handMs = 10,
                    eatMs = 1000;
                Queue<int> lockChopsticksQ = new Queue<int>();

                Action<bool> onRightStateChagne = (isWait) =>
                {
                    if (isWait)
                    {
                        lockChopsticksQ.Enqueue(0);
                    }
                    else
                    {
                        lockChopsticksQ.Dequeue();
                    }
                    if (lockChopsticksQ.Count == 5)
                    {
                        output.WriteLine("[Dead Lock Appeared]");
                        cts.Cancel();
                    }
                };

                Chopsticks chopsticks1 = new Chopsticks() { ID = 1 },
                    chopsticks2 = new Chopsticks() { ID = 2 },
                    chopsticks3 = new Chopsticks() { ID = 3 },
                    chopsticks4 = new Chopsticks() { ID = 4 },
                    chopsticks5 = new Chopsticks() { ID = 5 };

                APhilosopher<Chopsticks>
                    p1 = new Philosopher("P1", chopsticks1, chopsticks2, output) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p2 = new Philosopher("P2", chopsticks2, chopsticks3, output) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p3 = new Philosopher("P3", chopsticks3, chopsticks4, output) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p4 = new Philosopher("P4", chopsticks4, chopsticks5, output) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs },
                    p5 = new Philosopher("P5", chopsticks5, chopsticks1, output) { ThinkMs = thinkMs, HandMs = handMs, EatMs = eatMs };

                p1.onWriteChopsticksStateChange += onRightStateChagne;
                p2.onWriteChopsticksStateChange += onRightStateChagne;
                p3.onWriteChopsticksStateChange += onRightStateChagne;
                p4.onWriteChopsticksStateChange += onRightStateChagne;
                p5.onWriteChopsticksStateChange += onRightStateChagne;

                TaskFactory tf = new TaskFactory(cts.Token);

                Task
                    t1 = tf.StartNew(p1.Run),
                    t2 = tf.StartNew(p2.Run),
                    t3 = tf.StartNew(p3.Run),
                    t4 = tf.StartNew(p4.Run),
                    t5 = tf.StartNew(p5.Run);

                Task.WaitAll(t1, t2, t3, t4);
            });

            t.Wait();
        }
    }


    internal abstract class APhilosopher<Chopsticks> where Chopsticks : class, new()
    {
        protected readonly ITestOutputHelper output;

        public event Action<bool> onWriteChopsticksStateChange;

        protected void Trigger(bool state)
        {
            this.onWriteChopsticksStateChange(state);
        }

        public int HandMs { get; set; }

        public int ThinkMs { get; set; }

        public int EatMs { get; set; }

        public string Name { get; protected set; }

        protected Chopsticks left, right;

        public APhilosopher(string name, Chopsticks left, Chopsticks right, ITestOutputHelper output, int sleepMs = 1000)
        {
            this.output = output;
            this.Name = name;
            this.left = left;
            this.right = right;
        }

        public abstract void Run();
    }

    internal class Chopsticks
    {
        public int ID { get; set; }

        public bool IsUsed { get; private set; }

        event Action<int, bool> OnStateChanged;

        public void Use()
        {
            this.IsUsed = true;
            OnStateChanged.Invoke(this.ID, true);
        }

        public void UnUse()
        {
            this.IsUsed = false;
            OnStateChanged.Invoke(this.ID, false);
        }
    }

    #region 无死锁处理的哲学家

    internal class Philosopher : APhilosopher<Chopsticks>
    {
        public Philosopher(string name, Chopsticks left, Chopsticks right, ITestOutputHelper output)
            : base(name, left, right, output)
        {

        }

        public override void Run()
        {
            while (true)
            {
                output.WriteLine(string.Format("[{0}] thinking begin", this.Name));
                Thread.Sleep(this.ThinkMs);
                output.WriteLine(string.Format("[{0}] thinking end", this.Name));

                output.WriteLine(string.Format("[{0}] [wait] left chopsticks, cp id:[{1}]", this.Name, left.ID));
                lock (left)
                {
                    output.WriteLine(string.Format("[{0}] [hand] left chopsticks, cp id:[{1}]", this.Name, left.ID));

                    Trigger(true);
                    Thread.Sleep(this.HandMs);
                    output.WriteLine(string.Format("[{0}] [wait] right chopsticks, cp id:[{1}]", this.Name, right.ID));
                    lock (right)
                    {
                        Trigger(false);
                        output.WriteLine(string.Format("[{0}] hand right chopsticks, cp id:[{1}]", this.Name, right.ID));
                        output.WriteLine(string.Format("[{0}] eating", this.Name));
                        Thread.Sleep(this.EatMs);

                        output.WriteLine(string.Format("[{0}] eat end", this.Name));
                    }
                }
            }
        }
    }

    #endregion
}
