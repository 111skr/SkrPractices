using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkrConsole
{
    public abstract class TaskHandler : IDisposable
    {
        public void Dispose()
        {
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Dispose();
        }

        protected internal abstract Task<IDictionary<string, object>> SendAsync(IDictionary<string, object> Data);
    }

    public class DelegatingTaskHandler : TaskHandler
    {
        protected DelegatingTaskHandler() : base()
        { }

        protected DelegatingTaskHandler(TaskHandler innerHandler)
        {
            this.InnerHandler = innerHandler;
        }

        public TaskHandler InnerHandler { get; set; }

        protected override void Dispose(bool disposing)
        {
            this.Dispose();
        }

        protected internal override Task<IDictionary<string, object>> SendAsync(IDictionary<string, object> Data)
        {
            return this.InnerHandler.SendAsync(Data);
        }

        public Task<IDictionary<string, object>> Begin(IDictionary<string, object> Data)
        {
            return this.SendAsync(Data);
        }
    }
}
