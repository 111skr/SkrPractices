using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Xunit.Abstractions;
using System.Threading;

namespace WebApp.Tests.Singleton
{
    public class SingletonTest
    {
        #region 初始化
        private readonly ITestOutputHelper output;
        public SingletonTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        #endregion

        void DoConcurrentCreateTest<T>(int concurrentCount, Func<T> creator)
        {
            List<Task<T>> taskList = new List<Task<T>>();
            List<T> instances = new List<T>();

            Parallel.For(0, concurrentCount, i =>
            {
                var obj = creator();
                instances.Add(obj);
                output.WriteLine("iterator index :[{0}], thread :[{1}], obj hash code: {2}.", i, Thread.CurrentThread.ManagedThreadId, obj.GetHashCode());
            });

            //for (int i = 0; i < concurrentCount; i++)
            //{
            //    taskList.Add(Task.Factory.StartNew<T>(creator));
            //}

            //taskList.ForEach(t =>
            //{
            //    if (!t.IsFaulted)
            //    {
            //        instances.Add(t.Result);
            //    }
            //});

            instances.ForEach(item =>
            {
                item.ShouldBe<T>(creator());
            });
        }

        [Fact(DisplayName = "v1-没有保护，多线程并发创建会产生多示例，不适应用在创建消耗高单例对象")]
        public void Singleton01_should_not_get_only_one_instance_with_concurrent_create()
        {
            DoConcurrentCreateTest(30, () => Singleton01.Instance);
        }

        [Fact(DisplayName = "v2-并发创建无问题")]
        public void Singleton02_should_get_only_one_instance_with_concurrent_create()
        {
            DoConcurrentCreateTest(30, () => Singleton02.Instance);
        }

        [Fact(DisplayName = "v3-并发创建无问题")]
        public void Singleton03_should_get_only_one_instance_with_concurrent_create()
        {
            DoConcurrentCreateTest(30, () => Singleton03.Instance);
        }

        [Fact(DisplayName = "v4-并发创建无问题")]
        public void Singleton04_should_get_only_one_instance_with_concurrent_create()
        {
            DoConcurrentCreateTest(30, () => Singleton04.Instance);
        }

        [Fact(DisplayName = "v5-并发创建无问题")]
        public void Singleton05_should_get_only_one_instance_with_concurrent_create()
        {
            DoConcurrentCreateTest(30, () => Singleton05.Instance);
        }

        [Fact(DisplayName = "v6-并发创建无问题")]
        public void Singleton06_should_get_only_one_instance_with_concurrent_create()
        {
            DoConcurrentCreateTest(30, () => Singleton06.Instance);
        }
    }
}
