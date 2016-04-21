using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Tests.Singleton
{
    internal sealed class Singleton01
    {
        private static Singleton01 instance;

        private Singleton01() { }

        public static Singleton01 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton01();
                }
                return instance;
            }
        }
    }

    internal sealed class Singleton02
    {
        // 在静态私有字段上声明单例
        private static readonly Singleton02 instance = new Singleton02();

        // 私有构造函数，确保用户在外部不能实例化新的实例
        private Singleton02() { }

        // 只读属性返回静态字段
        public static Singleton02 Instance
        {
            get
            {
                return instance;
            }
        }

        //标记类为sealed是好的，可以防止被集成，然后在子类实例化，
        //使用在静态私有字段上通过new的形式，来保证在该类第一次被调用的时候创建实例，是不错的方式，
        //但有一点需要注意的是，C#其实并不保证实例创建的时机，因为C#规范只是在IL里标记该静态字段是BeforeFieldInit，
        //也就是说静态字段可能在第一次被使用的时候创建，也可能你没使用了，它也帮你创建了，也就是周期更早，我们不能确定到底是什么创建的实例。
    }

    public sealed class Singleton03
    {
        // 依然是静态自动hold实例
        private static volatile Singleton03 instance = null;
        // Lock对象，线程安全所用
        private static object syncRoot = new Object();

        private Singleton03() { }

        public static Singleton03 Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Singleton03();
                    }
                }

                return instance;
            }
        }

        //使用volatile来修饰，是个不错的注意，确保instance在被访问之前被赋值实例，一般情况都是用这种方式来实现单例。
        //volatile修饰符的作用需要研究一下
    }

    public class Singleton04
    {
        // 因为下面声明了静态构造函数，所以在第一次访问该类之前，new Singleton()语句不会执行
        private static readonly Singleton04 _instance = new Singleton04();

        public static Singleton04 Instance
        {
            get { return _instance; }
        }

        private Singleton04()
        {
        }

        // 声明静态构造函数就是为了删除IL里的BeforeFieldInit标记
        // 以去北欧静态自动在使用之前被初始化
        static Singleton04()
        {
        }
    }

    public sealed class Singleton05
    {
        private Singleton05()
        {
        }

        public static Singleton05 Instance { get { return Nested._instance; } }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly Singleton05 _instance = new Singleton05();
        }
    }

    public class Singleton06
    {
        // 因为构造函数是私有的，所以需要使用lambda
        private static readonly Lazy<Singleton06> _instance = new Lazy<Singleton06>(() => new Singleton06());

        private Singleton06()
        {
        }

        public static Singleton06 Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}
