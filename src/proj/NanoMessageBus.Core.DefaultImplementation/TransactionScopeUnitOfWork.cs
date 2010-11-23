namespace NanoMessageBus.Core
{
	using System;
	using System.Transactions;

	public class TransactionScopeUnitOfWork : IHandleUnitOfWork
	{
		private readonly TransactionScope transaction;
		private bool disposed;

		public TransactionScopeUnitOfWork()
			: this(IsolationLevel.ReadCommitted)
		{
		}

		public TransactionScopeUnitOfWork(IsolationLevel level)
			: this(new TransactionOptions { IsolationLevel = level })
		{
		}

		public TransactionScopeUnitOfWork(TransactionOptions options)
			: this(new TransactionScope(TransactionScopeOption.Required, options))
		{
		}

		public TransactionScopeUnitOfWork(TransactionScope transaction)
		{
			this.transaction = transaction;
		}

		~TransactionScopeUnitOfWork()
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
			this.transaction.Dispose();
		}

		public virtual void Complete()
		{
			this.transaction.Complete();
		}
	}
}