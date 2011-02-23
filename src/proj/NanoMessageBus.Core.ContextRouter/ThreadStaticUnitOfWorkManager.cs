using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoMessageBus.Core
{
    public class ThreadStaticUnitOfWorkManager : IManageCurrentUnitOfWork
    {
        private readonly Func<IHandleUnitOfWork> unitOfWorkFactory;
        private bool disposed;

        [ThreadStatic]
        private static IHandleUnitOfWork unitOfWork;

        public ThreadStaticUnitOfWorkManager(Func<IHandleUnitOfWork> unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IHandleUnitOfWork CurrentUnitOfWork
        {
            get
            {
                if (unitOfWork == null)
                {
                    unitOfWork = unitOfWorkFactory();
                }
                return unitOfWork;
            }
        }


        ~ThreadStaticUnitOfWorkManager()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed || !disposing)
				return;

			this.disposed = true;
            if (unitOfWork != null)
            {
                unitOfWork.Dispose();
                unitOfWork = null;
            }
		}
    }
}
