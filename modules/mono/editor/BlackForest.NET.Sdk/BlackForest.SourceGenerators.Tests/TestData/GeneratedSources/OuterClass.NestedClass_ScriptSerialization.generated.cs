using BlackForest;
using BlackForest.NativeInterop;

partial struct OuterClass
{
partial class NestedClass
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::BlackForest.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::BlackForest.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
    }
}
}
