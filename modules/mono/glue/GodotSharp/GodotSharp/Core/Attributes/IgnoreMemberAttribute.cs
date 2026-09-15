using System;

namespace BlackForest
{
    /// <summary>
    /// An attribute that excludes a member from registering as a method or property in BlackForest.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IgnoreMemberAttribute : Attribute { }
}
