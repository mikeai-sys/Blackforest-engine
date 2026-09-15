partial class ExportedProperties2
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
#if TOOLS
    /// <summary>
    /// Get the default values for all properties declared in this class.
    /// This method is used by BlackForest to determine the value that will be
    /// used by the inspector when resetting properties.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.Dictionary<global::BlackForest.StringName, global::BlackForest.Variant> GetGodotPropertyDefaultValues()
    {
        var values = new global::System.Collections.Generic.Dictionary<global::BlackForest.StringName, global::BlackForest.Variant>(3);
        int __Health_default_value = default;
        values.Add(PropertyName.@Health, global::BlackForest.Variant.From<int>(__Health_default_value));
        global::BlackForest.Resource __SubResource_default_value = default;
        values.Add(PropertyName.@SubResource, global::BlackForest.Variant.From<global::BlackForest.Resource>(__SubResource_default_value));
        string[] __Strings_default_value = default;
        values.Add(PropertyName.@Strings, global::BlackForest.Variant.From<string[]>(__Strings_default_value));
        return values;
    }
#endif // TOOLS
#pragma warning restore CS0109
}
