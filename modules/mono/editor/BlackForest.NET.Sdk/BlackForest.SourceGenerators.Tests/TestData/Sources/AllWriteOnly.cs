using BlackForest;

public partial class AllWriteOnly : GodotObject
{
    private bool _writeOnlyBackingField = false;
    public bool WriteOnlyProperty { set => _writeOnlyBackingField = value; }
}
