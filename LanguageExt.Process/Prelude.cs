﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Process' to your code.
    /// </summary>
    public static class Process
    {
        /// <summary>
        /// Registry of named processes for discovery
        /// </summary>
        public static Map<string, ProcessId> Registered =>
            ActorContext.GetProcess(ActorContext.Registered).Children;

        /// <summary>
        /// Current process ID
        /// </summary>
        public static ProcessId Self =>
            ActorContext.Self;

        /// <summary>
        /// Parent process ID
        /// </summary>
        public static ProcessId Parent =>
            ActorContext.GetProcess(ActorContext.Self).Parent;

        /// <summary>
        /// User process ID
        /// The User process is the default entry process
        /// </summary>
        public static ProcessId User =>
            ActorContext.User;

        /// <summary>
        /// Sender process ID
        /// Always valid even if there's not a sender (the 'NoSender' process ID will
        /// be provided).
        /// </summary>
        public static ProcessId Sender =>
            ActorContext.Sender;

        /// <summary>
        /// Get the child processes of the running process
        /// </summary>
        public static Map<string, ProcessId> Children =>
            children(ActorContext.Self);

        /// <summary>
        /// Create a new child-process by name
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that can be passed around</returns>
        public static ProcessId spawn<T>(ProcessName name, Action<T> messageHandler) =>
            spawn<Unit, T>(name, () => unit, (state, msg) => { messageHandler(msg); return state; });

        /// <summary>
        /// Create a new child-process by name
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that can be passed around</returns>
        public static ProcessId spawn<S, T>(ProcessName name, Func<S> setup, Func<S, T, S> messageHandler) =>
            ActorContext.ActorCreate<S, T>(ActorContext.Self, name, messageHandler, setup);

        /// <summary>
        /// Find a registered process by name
        /// </summary>
        /// <param name="name">Process name</param>
        /// <returns>ProcessId or ProcessId.None</returns>
        public static ProcessId find(string name) =>
            Registered.Find(
                name,
                Some: x  => x,
                None: () => ProcessId.None);

        /// <summary>
        /// Register self as a named process
        /// </summary>
        /// <param name="name">Name to register under</param>
        public static ProcessId reg(ProcessName name) =>
            ActorContext.Register(name, Self);

        /// <summary>
        /// Register the name with the process
        /// </summary>
        /// <param name="name">Name to register under</param>
        /// <param name="process">Process to be registered</param>
        public static ProcessId reg(ProcessName name, ProcessId process) =>
            ActorContext.Register(name, process);

        /// <summary>
        /// Un-register the process associated with the name
        /// </summary>
        /// <param name="name">Name of the process to un-register</param>
        public static Unit unreg(ProcessName name) =>
            ActorContext.UnRegister(name);

        /// <summary>
        /// Forces the current running process to shutdown.  The kill message 
        /// jumps ahead of any messages already in the queue.
        /// </summary>
        public static Unit kill() =>
            raise<Unit>(new SystemKillActorException());

        /// <summary>
        /// Forces the specified process to shutdown.  The kill message jumps 
        /// ahead of any messages already in the queue.
        /// </summary>
        public static Unit kill(ProcessId pid) =>
            tell(pid, SystemMessage.Shutdown);

        /// <summary>
        /// Shutdown the currently running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messages allowing any backlog
        /// to be processed first.
        /// </summary>
        public static Unit shutdown() =>
            shutdown(Self);

        /// <summary>
        /// Shutdown a specified running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messagesallowing any backlog
        /// to be processed first.
        /// </summary>
        public static Unit shutdown(ProcessId pid) =>
            tell(pid, UserControlMessage.Shutdown);

        /// <summary>
        /// Send a message to a process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="message">Message to send</param>
        public static Unit tell<T>(ProcessId pid, T message, ProcessId sender = default(ProcessId)) =>
            message is SystemMessage
                ? ActorContext.TellSystem(pid, message as SystemMessage)
                : message is UserControlMessage
                    ? ActorContext.TellUserControl(pid, message as UserControlMessage)
                    : ActorContext.Tell(pid, message, sender);

        /// <summary>
        /// Send a message at a specified time in the futre
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="pid">Process ID</param>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        public static Unit tell<T>(ProcessId pid, T message, DateTime delayUntil, ProcessId sender = default(ProcessId)) =>
            tell(pid, message, delayUntil - DateTime.Now, sender);

        /// <summary>
        /// Send a message at a specified time in the futre
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pid">Process ID</param>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        public static Unit tell<T>(ProcessId pid, T message, TimeSpan delayFor, ProcessId sender = default(ProcessId))
        {
            if (delayFor.TotalMilliseconds < 1)
            {
                return tell(pid, message, sender);
            }
            else
            {
                Timer t = null;
                t = new Timer(_ => { tell(pid, message, sender); t.Dispose(); }, null, delayFor, Timeout.InfiniteTimeSpan);
                return unit;
            }
        }

        /// <summary>
        /// Publish a message for any listening subscribers
        /// </summary>
        /// <param name="message">Message to publish</param>
        public static Unit pub<T>(T message) =>
            ObservableRouter.Publish(ActorContext.Self, message);

        /// <summary>
        /// Get the child processes of the process provided
        /// </summary>
        public static Map<string, ProcessId> children(ProcessId pid) =>
            ActorContext.GetProcess(pid).Children;

        /// <summary>
        /// Shutdown all processes and restart
        /// </summary>
        public static Unit shutdownAll() =>
            tell(ActorContext.Root, RootMessage.ShutdownAll);

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, IObserver<T> observer) =>
            ObservableRouter.Subscribe(pid, observer);

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError, Action onComplete) =>
            ObservableRouter.Subscribe(pid, onNext, onError, onComplete);

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError) =>
            ObservableRouter.Subscribe(pid, onNext, onError, () => { });

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext) =>
            ObservableRouter.Subscribe(pid, onNext, ex => { }, () => { });

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext, Action onComplete) =>
            ObservableRouter.Subscribe(pid, onNext, ex => { }, onComplete);

        /// <summary>
        /// Get an IObservable for a process.  
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IObservable<T> observe<T>(ProcessId pid) =>
            ObservableRouter.Observe<T>(pid);
    }
}
