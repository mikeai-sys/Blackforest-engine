using System;

#nullable enable

namespace BlackForest
{
    /// <summary>
    /// Exposes the target class as a global script class to BlackForest Engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GlobalClassAttribute : Attribute { }
}
