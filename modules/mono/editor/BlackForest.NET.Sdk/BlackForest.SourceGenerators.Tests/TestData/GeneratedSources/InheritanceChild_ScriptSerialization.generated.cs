using BlackForest;
using BlackForest.NativeInterop;

partial class InheritanceChild
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::BlackForest.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@MyString, global::BlackForest.Variant.From<string>(this.@MyString));
        info.AddProperty(PropertyName.@MyInteger, global::BlackForest.Variant.From<int>(this.@MyInteger));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::BlackForest.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@MyString, out var _value_MyString))
            this.@MyString = _value_MyString.As<string>();
        if (info.TryGetProperty(PropertyName.@MyInteger, out var _value_MyInteger))
            this.@MyInteger = _value_MyInteger.As<int>();
    }
}
