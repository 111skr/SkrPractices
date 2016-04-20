using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System.Threading;
using System.Diagnostics;

namespace WebApp.Tests.Concurrent
{
    #region data Class
    internal abstract class AIncreaseObject
    {
        //用不用static都能产生期望的竞态条件效果，但是经过测试发现加static更容易发生
        protected static int count = 0;

        public abstract void Increase();

        public virtual int Number { get { return count; } }
    }

    /// <summary>
    /// 并发情况下被操作的数据类(查看一下IL代码分析Increase()方法是怎么实现的)
    /// </summary>
    internal class IncreaseObject : AIncreaseObject
    {
        public override void Increase()
        {
            count++;
        }
    }

    /// <summary>
    /// 加锁的数据对象
    /// </summary>
    internal class IncreaseObjectWithInterlocked : AIncreaseObject
    {
        public override void Increase()
        {
            Interlocked.Increment(ref count);
        }
    } 
    #endregion

    public class ThreadAndLock
    {
        private AIncreaseObject DealOperationWithTask<D>(int threadCount = 10, int increaseTimes = 100)
            where D : AIncreaseObject, new()
        {
            AIncreaseObject incObj = new D();

            int totalResult = threadCount * increaseTimes;

            List<Task> taskList = new List<Task>();

            Action taskDetail = () =>
            {
                for (int i = 0; i < increaseTimes; i++)
                {
                    incObj.Increase();
                }
                //Trace.WriteLine(string.Format("Task:[{2}] Thread:[{0}], result:{1};", Thread.CurrentThread.ManagedThreadId, incOjb.Number, Task.CurrentId));
            };

            for (int tIndex = 0; tIndex < threadCount; tIndex++)
            {
                Task t = new Task(taskDetail);

                t.Start();

                taskList.Add(t);
            }

            Task.WaitAll(taskList.ToArray());

            return incObj;
        }

        #region no lock opertaions

        [Fact(DisplayName = "Task实现：不使用同步锁保护被操作数据进行多线程操作同一个对象数据时不一定会得到期望的结果")]
        public void Tasks_do_increase_without_lock_should_get_wrong_result()
        {
            int threadCount = 10,
                increaseTimes = 100,
                totalResult = threadCount * increaseTimes;

            AIncreaseObject incObj = DealOperationWithTask<IncreaseObject>(threadCount, increaseTimes);

            incObj.Number.ShouldBe<Int32>(totalResult);
        }

        [Fact(DisplayName = "Thread实现：不使用同步锁保护被操作数据进行多线程操作同一个对象数据时不一定会得到期望的结果")]
        public void Threads_do_increase_without_lock()
        {
            IncreaseObject incOjb = new IncreaseObject();

            int threadCount = 20,
                increaseTimes = 1000,
                totalResult = threadCount * increaseTimes;

            List<Thread> taskList = new List<Thread>();

            Action taskDetail = () =>
            {
                for (int i = 0; i < increaseTimes; i++)
                {
                    incOjb.Increase();
                }

                //Trace.WriteLine(string.Format("Task:[{2}] Thread:[{0}], result:{1};", Thread.CurrentThread.ManagedThreadId, incOjb.Number, Task.CurrentId));
            };

            for (int tIndex = 0; tIndex < threadCount; tIndex++)
            {
                Thread t = new Thread(new ThreadStart(taskDetail));

                t.Start();

                taskList.Add(t);
            }

            taskList.ForEach(t => t.Join());

            incOjb.Number.ShouldBe<Int32>(totalResult);
        }

        #endregion

        #region interlocked operations

        [Fact(DisplayName = "Task实现：多线程操作拥有Interlocked锁的数据")]
        public void Tasks_do_increase_with_Interlocked_should_get_right_result()
        {
            int threadCount = 10,
                increaseTimes = 100,
                totalResult = threadCount * increaseTimes;

            AIncreaseObject incObj = DealOperationWithTask<IncreaseObjectWithInterlocked>(threadCount, increaseTimes);

            incObj.Number.ShouldBe<Int32>(totalResult);
        }

        #endregion
    }
}
