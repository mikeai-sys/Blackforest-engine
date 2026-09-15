using System;

namespace BlackForest
{
    /// <summary>
    /// Allows the annotated class to execute in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ToolAttribute : Attribute { }
}
