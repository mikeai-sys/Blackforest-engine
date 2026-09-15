using BlackForest;
using BlackForest.NativeInterop;

partial class ScriptBoilerplate
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::BlackForest.Node.MethodName {
        /// <summary>
        /// Cached name for the '_Process' method.
        /// </summary>
        public new static readonly global::BlackForest.StringName @_Process = "_Process";
        /// <summary>
        /// Cached name for the 'Bazz' method.
        /// </summary>
        public new static readonly global::BlackForest.StringName @Bazz = "Bazz";
    }
    /// <summary>
    /// Get the method information for all the methods declared in this class.
    /// This method is used by BlackForest to register the available methods in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::BlackForest.Bridge.MethodInfo> GetGodotMethodList()
    {
        var methods = new global::System.Collections.Generic.List<global::BlackForest.Bridge.MethodInfo>(2);
        methods.Add(new(name: MethodName.@_Process, returnVal: new(type: (global::BlackForest.Variant.Type)0, name: "", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false), flags: (global::BlackForest.MethodFlags)1, arguments: new() { new(type: (global::BlackForest.Variant.Type)3, name: "delta", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        methods.Add(new(name: MethodName.@Bazz, returnVal: new(type: (global::BlackForest.Variant.Type)2, name: "", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false), flags: (global::BlackForest.MethodFlags)1, arguments: new() { new(type: (global::BlackForest.Variant.Type)21, name: "name", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        return methods;
    }
#pragma warning restore CS0109
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
    {
        if (method == MethodName.@_Process && args.Count == 1) {
            @_Process(global::BlackForest.NativeInterop.VariantUtils.ConvertTo<double>(args[0]));
            ret = default;
            return true;
        }
        if (method == MethodName.@Bazz && args.Count == 1) {
            var callRet = @Bazz(global::BlackForest.NativeInterop.VariantUtils.ConvertTo<global::BlackForest.StringName>(args[0]));
            ret = global::BlackForest.NativeInterop.VariantUtils.CreateFrom<int>(callRet);
            return true;
        }
        return base.InvokeGodotClassMethod(method, args, out ret);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool HasGodotClassMethod(in godot_string_name method)
    {
        if (method == MethodName.@_Process) {
           return true;
        }
        if (method == MethodName.@Bazz) {
           return true;
        }
        return base.HasGodotClassMethod(method);
    }
}
