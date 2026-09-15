using BlackForest;
using BlackForest.NativeInterop;

partial struct OuterClass
{
partial class NestedClass
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::BlackForest.RefCounted.MethodName {
        /// <summary>
        /// Cached name for the '_Get' method.
        /// </summary>
        public new static readonly global::BlackForest.StringName @_Get = "_Get";
    }
    /// <summary>
    /// Get the method information for all the methods declared in this class.
    /// This method is used by BlackForest to register the available methods in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::BlackForest.Bridge.MethodInfo> GetGodotMethodList()
    {
        var methods = new global::System.Collections.Generic.List<global::BlackForest.Bridge.MethodInfo>(1);
        methods.Add(new(name: MethodName.@_Get, returnVal: new(type: (global::BlackForest.Variant.Type)0, name: "", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)131078, exported: false), flags: (global::BlackForest.MethodFlags)1, arguments: new() { new(type: (global::BlackForest.Variant.Type)21, name: "property", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        return methods;
    }
#pragma warning restore CS0109
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
    {
        if (method == MethodName.@_Get && args.Count == 1) {
            var callRet = @_Get(global::BlackForest.NativeInterop.VariantUtils.ConvertTo<global::BlackForest.StringName>(args[0]));
            ret = global::BlackForest.NativeInterop.VariantUtils.CreateFrom<global::BlackForest.Variant>(callRet);
            return true;
        }
        return base.InvokeGodotClassMethod(method, args, out ret);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool HasGodotClassMethod(in godot_string_name method)
    {
        if (method == MethodName.@_Get) {
           return true;
        }
        return base.HasGodotClassMethod(method);
    }
}
}
