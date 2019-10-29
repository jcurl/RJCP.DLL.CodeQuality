namespace NUnit.Framework
{
    using System;
    using System.Collections.Generic;

    internal class DelegateTargets
    {
        private Dictionary<string, Dictionary<Delegate, DelegateTarget>> m_Events =
            new Dictionary<string, Dictionary<Delegate, DelegateTarget>>();

        /// <summary>
        /// Adds the target to a look up table for later.
        /// </summary>
        /// <param name="eventName">Name of the event used as part of the look up.</param>
        /// <param name="source">The source delegate which the user provided.</param>
        /// <param name="target">The target delegate that is attached.</param>
        public void AddTarget(string eventName, Delegate source, Delegate target)
        {
            if (!m_Events.TryGetValue(eventName, out Dictionary<Delegate, DelegateTarget> delegateMap)) {
                delegateMap = new Dictionary<Delegate, DelegateTarget>();
                m_Events.Add(eventName, delegateMap);
            }

            if (!delegateMap.TryGetValue(source, out DelegateTarget delegateTarget)) {
                delegateTarget = new DelegateTarget(target);
                delegateMap.Add(source, delegateTarget);
            }

            delegateTarget.AddRef();
        }

        /// <summary>
        /// Removes the target from the look up table.
        /// </summary>
        /// <param name="eventName">Name of the event for the look up.</param>
        /// <param name="source">The source delegate which the user provided.</param>
        /// <returns>The delegate that was registered in the look up.</returns>
        /// <exception cref="ArgumentException">
        /// <para>Event <paramref name="eventName"/> not found</para>
        /// - or -
        /// <para>Delegate <paramref name="source"/> not found</para>
        /// </exception>
        public Delegate RemoveTarget(string eventName, Delegate source)
        {
            if (!m_Events.TryGetValue(eventName, out Dictionary<Delegate, DelegateTarget> delegateMap)) {
                throw new ArgumentException("Event not found", nameof(eventName));
            }

            if (!delegateMap.TryGetValue(source, out DelegateTarget delegateTarget)) {
                throw new ArgumentException("Delegate not found", nameof(source));
            }

            bool last = delegateTarget.RemoveRef();
            Delegate target = delegateTarget.Target;
            if (last) delegateMap.Remove(source);
            if (delegateMap.Count == 0) m_Events.Remove(eventName);
            return target;
        }
    }
}
