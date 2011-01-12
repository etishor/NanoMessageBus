namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Transactions;

	public class TransactionScopeUnitOfWork : IHandleUnitOfWork
	{
		private readonly ICollection<Action> callbacks = new LinkedList<Action>();
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
			: this(new TransactionScope(TransactionScopeOption.RequiresNew, options))
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

		public void Register(Action callback)
		{
			if (callback != null)
				this.callbacks.Add(callback);
		}
		public virtual void Complete()
		{
			foreach (var callback in this.callbacks)
				callback();

			this.Clear();
			this.transaction.Complete();
		}
		public virtual void Clear()
		{
			this.callbacks.Clear();
		}
	}
}