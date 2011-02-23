using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoMessageBus.Core
{
    /// <summary>
    /// Indicated the ability to manage the current UOW.
    /// </summary>
    /// <remarks>
    /// Object instances which implement this interface should be used to manage the currently active UOW.
    /// Examples could be ThreadStatic UOW, WebRequest UOW, Manual UOW
    /// </remarks>
    public interface IManageCurrentUnitOfWork: IDisposable
    {
        /// <summary>
        /// Returns a reference to the currently active Unit Of Work.
        /// </summary>
        IHandleUnitOfWork CurrentUnitOfWork { get; }
    }
}
