﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NanoMessageBus.Endpoints {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Diagnostics {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Diagnostics() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NanoMessageBus.Endpoints.Diagnostics", typeof(Diagnostics).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The account under which the current process is executing does not have permission to access the queue specified, &apos;{0}&apos;..
        /// </summary>
        internal static string AccessDenied {
            get {
                return ResourceManager.GetString("AccessDenied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempting to receive a message from the endpoint &apos;{0}&apos;..
        /// </summary>
        internal static string AttemptingToReceiveMessage {
            get {
                return ResourceManager.GetString("AttemptingToReceiveMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disposing the endpoint &apos;{0}.
        /// </summary>
        internal static string DisposingQueue {
            get {
                return ResourceManager.GetString("DisposingQueue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This endpoint has already been disposed..
        /// </summary>
        internal static string EndpointAlreadyDisposed {
            get {
                return ResourceManager.GetString("EndpointAlreadyDisposed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The message queue address provided &apos;{0}&apos; was not in the format &apos;msmq://MachineName/QueueName/&apos;..
        /// </summary>
        internal static string InvalidAddress {
            get {
                return ResourceManager.GetString("InvalidAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A message of length {0} bytes was received from endpoint &apos;{1}&apos;..
        /// </summary>
        internal static string MessageReceived {
            get {
                return ResourceManager.GetString("MessageReceived", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A message queue address was not given and is required..
        /// </summary>
        internal static string MissingQueueAddress {
            get {
                return ResourceManager.GetString("MissingQueueAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No message was available to be received from endpoint &apos;{0}&apos;..
        /// </summary>
        internal static string NoMessageAvailable {
            get {
                return ResourceManager.GetString("NoMessageAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The message queue specified, &apos;{0}&apos;, is not a transactional queue and could not be opened in transactional mode..
        /// </summary>
        internal static string NonTransactionalQueue {
            get {
                return ResourceManager.GetString("NonTransactionalQueue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Opening endpoint &apos;{0}&apos; for receive with enlistment &apos;{1}&apos;.
        /// </summary>
        internal static string OpeningQueueForReceive {
            get {
                return ResourceManager.GetString("OpeningQueueForReceive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Opening endpoint &apos;{0}&apos; for transmission with enlistment &apos;{1}&apos;.
        /// </summary>
        internal static string OpeningQueueForSend {
            get {
                return ResourceManager.GetString("OpeningQueueForSend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Physical message &apos;{0}&apos; contains a logical message of type &apos;{1}&apos;..
        /// </summary>
        internal static string PhysicalMessageContains {
            get {
                return ResourceManager.GetString("PhysicalMessageContains", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preparing to send physical message &apos;{0}&apos; which contains {1} logical messages..
        /// </summary>
        internal static string PreparingMessageToSend {
            get {
                return ResourceManager.GetString("PreparingMessageToSend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The queue specified, &apos;{0}&apos;, was not found..
        /// </summary>
        internal static string QueueNotFound {
            get {
                return ResourceManager.GetString("QueueNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sending a message to the endpoint &apos;{0}&apos;..
        /// </summary>
        internal static string SendingMessage {
            get {
                return ResourceManager.GetString("SendingMessage", resourceCulture);
            }
        }
    }
}
