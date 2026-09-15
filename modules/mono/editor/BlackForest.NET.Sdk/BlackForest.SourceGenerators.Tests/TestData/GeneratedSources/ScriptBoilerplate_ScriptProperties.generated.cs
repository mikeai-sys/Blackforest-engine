using BlackForest;
using BlackForest.NativeInterop;

partial class ScriptBoilerplate
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::BlackForest.Node.PropertyName {
        /// <summary>
        /// Cached name for the '_nodePath' field.
        /// </summary>
        public new static readonly global::BlackForest.StringName @_nodePath = "_nodePath";
        /// <summary>
        /// Cached name for the '_velocity' field.
        /// </summary>
        public new static readonly global::BlackForest.StringName @_velocity = "_velocity";
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
    {
        if (name == PropertyName.@_nodePath) {
            this.@_nodePath = global::BlackForest.NativeInterop.VariantUtils.ConvertTo<global::BlackForest.NodePath>(value);
            return true;
        }
        if (name == PropertyName.@_velocity) {
            this.@_velocity = global::BlackForest.NativeInterop.VariantUtils.ConvertTo<int>(value);
            return true;
        }
        return base.SetGodotClassPropertyValue(name, value);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
    {
        if (name == PropertyName.@_nodePath) {
            value = global::BlackForest.NativeInterop.VariantUtils.CreateFrom<global::BlackForest.NodePath>(this.@_nodePath);
            return true;
        }
        if (name == PropertyName.@_velocity) {
            value = global::BlackForest.NativeInterop.VariantUtils.CreateFrom<int>(this.@_velocity);
            return true;
        }
        return base.GetGodotClassPropertyValue(name, out value);
    }
    /// <summary>
    /// Get the property information for all the properties declared in this class.
    /// This method is used by BlackForest to register the available properties in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::BlackForest.Bridge.PropertyInfo> GetGodotPropertyList()
    {
        var properties = new global::System.Collections.Generic.List<global::BlackForest.Bridge.PropertyInfo>();
        properties.Add(new(type: (global::BlackForest.Variant.Type)22, name: PropertyName.@_nodePath, hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::BlackForest.Variant.Type)2, name: PropertyName.@_velocity, hint: (global::BlackForest.PropertyHint)0, hintString: "", usage: (global::BlackForest.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}
