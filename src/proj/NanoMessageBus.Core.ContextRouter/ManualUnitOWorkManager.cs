using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoMessageBus.Core
{
    public class ManualUnitOWorkManager : IManageCurrentUnitOfWork
    {
        private readonly IHandleUnitOfWork unitOfWork;
        private bool disposed;

        public ManualUnitOWorkManager(IHandleUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IHandleUnitOfWork CurrentUnitOfWork
        {
            get 
            {
                return this.unitOfWork;
            }
        }

        ~ManualUnitOWorkManager()
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
			this.unitOfWork.Dispose();
		}
    }
}
