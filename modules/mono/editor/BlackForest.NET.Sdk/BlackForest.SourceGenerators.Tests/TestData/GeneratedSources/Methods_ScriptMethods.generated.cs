using BlackForest;
using BlackForest.NativeInterop;

partial class Methods
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::BlackForest.GodotObject.MethodName {
        /// <summary>
        /// Cached name for the 'MethodWithOverload' method.
        /// </summary>
        public new static readonly global::BlackForest.StringName @MethodWithOverload = "MethodWithOverload";
    }
    /// <summary>
    /// Get the method information for all the methods declared in this class.
    /// This method is used by BlackForest to register the available methods in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::BlackForest.Bridge.MethodInfo> GetGodotMethodList()
    {
        var methods = new global::System.Collections.Generic.List<global::BlackForest.Bridge.MethodInfo>(3);
        methods.Add(new(name: MethodName.@MethodWithOverload, returnVal: new(type: (global::BlackForest.Variant.Type)0, name: "", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false), flags: (global::BlackForest.MethodFlags)1, arguments: null, defaultArguments: null));
        methods.Add(new(name: MethodName.@MethodWithOverload, returnVal: new(type: (global::BlackForest.Variant.Type)0, name: "", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false), flags: (global::BlackForest.MethodFlags)1, arguments: new() { new(type: (global::BlackForest.Variant.Type)2, name: "a", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        methods.Add(new(name: MethodName.@MethodWithOverload, returnVal: new(type: (global::BlackForest.Variant.Type)0, name: "", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false), flags: (global::BlackForest.MethodFlags)1, arguments: new() { new(type: (global::BlackForest.Variant.Type)2, name: "a", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false), new(type: (global::BlackForest.Variant.Type)2, name: "b", hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        return methods;
    }
#pragma warning restore CS0109
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
    {
        if (method == MethodName.@MethodWithOverload && args.Count == 0) {
            @MethodWithOverload();
            ret = default;
            return true;
        }
        if (method == MethodName.@MethodWithOverload && args.Count == 1) {
            @MethodWithOverload(global::BlackForest.NativeInterop.VariantUtils.ConvertTo<int>(args[0]));
            ret = default;
            return true;
        }
        if (method == MethodName.@MethodWithOverload && args.Count == 2) {
            @MethodWithOverload(global::BlackForest.NativeInterop.VariantUtils.ConvertTo<int>(args[0]), global::BlackForest.NativeInterop.VariantUtils.ConvertTo<int>(args[1]));
            ret = default;
            return true;
        }
        return base.InvokeGodotClassMethod(method, args, out ret);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool HasGodotClassMethod(in godot_string_name method)
    {
        if (method == MethodName.@MethodWithOverload) {
           return true;
        }
        return base.HasGodotClassMethod(method);
    }
}
