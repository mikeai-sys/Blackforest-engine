partial class ExportedFields
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
        var values = new global::System.Collections.Generic.Dictionary<global::BlackForest.StringName, global::BlackForest.Variant>(62);
        bool ___fieldBoolean_default_value = true;
        values.Add(PropertyName.@_fieldBoolean, global::BlackForest.Variant.From<bool>(___fieldBoolean_default_value));
        char ___fieldChar_default_value = 'f';
        values.Add(PropertyName.@_fieldChar, global::BlackForest.Variant.From<char>(___fieldChar_default_value));
        sbyte ___fieldSByte_default_value = 10;
        values.Add(PropertyName.@_fieldSByte, global::BlackForest.Variant.From<sbyte>(___fieldSByte_default_value));
        short ___fieldInt16_default_value = 10;
        values.Add(PropertyName.@_fieldInt16, global::BlackForest.Variant.From<short>(___fieldInt16_default_value));
        int ___fieldInt32_default_value = 10;
        values.Add(PropertyName.@_fieldInt32, global::BlackForest.Variant.From<int>(___fieldInt32_default_value));
        long ___fieldInt64_default_value = -10_000;
        values.Add(PropertyName.@_fieldInt64, global::BlackForest.Variant.From<long>(___fieldInt64_default_value));
        byte ___fieldByte_default_value = 10;
        values.Add(PropertyName.@_fieldByte, global::BlackForest.Variant.From<byte>(___fieldByte_default_value));
        ushort ___fieldUInt16_default_value = 10;
        values.Add(PropertyName.@_fieldUInt16, global::BlackForest.Variant.From<ushort>(___fieldUInt16_default_value));
        uint ___fieldUInt32_default_value = 10;
        values.Add(PropertyName.@_fieldUInt32, global::BlackForest.Variant.From<uint>(___fieldUInt32_default_value));
        ulong ___fieldUInt64_default_value = 10;
        values.Add(PropertyName.@_fieldUInt64, global::BlackForest.Variant.From<ulong>(___fieldUInt64_default_value));
        float ___fieldSingle_default_value = 10;
        values.Add(PropertyName.@_fieldSingle, global::BlackForest.Variant.From<float>(___fieldSingle_default_value));
        double ___fieldDouble_default_value = 10;
        values.Add(PropertyName.@_fieldDouble, global::BlackForest.Variant.From<double>(___fieldDouble_default_value));
        string ___fieldString_default_value = "foo";
        values.Add(PropertyName.@_fieldString, global::BlackForest.Variant.From<string>(___fieldString_default_value));
        float ___fieldStaticImport_default_value = global::BlackForest.Mathf.RadToDeg(2  * global::BlackForest.Mathf.Pi);
        values.Add(PropertyName.@_fieldStaticImport, global::BlackForest.Variant.From<float>(___fieldStaticImport_default_value));
        global::BlackForest.Vector2 ___fieldVector2_default_value = new(10f, 10f);
        values.Add(PropertyName.@_fieldVector2, global::BlackForest.Variant.From<global::BlackForest.Vector2>(___fieldVector2_default_value));
        global::BlackForest.Vector2I ___fieldVector2I_default_value = global::BlackForest.Vector2I.Up;
        values.Add(PropertyName.@_fieldVector2I, global::BlackForest.Variant.From<global::BlackForest.Vector2I>(___fieldVector2I_default_value));
        global::BlackForest.Rect2 ___fieldRect2_default_value = new(new global::BlackForest.Vector2(10f, 10f), new global::BlackForest.Vector2(10f, 10f));
        values.Add(PropertyName.@_fieldRect2, global::BlackForest.Variant.From<global::BlackForest.Rect2>(___fieldRect2_default_value));
        global::BlackForest.Rect2I ___fieldRect2I_default_value = new(new global::BlackForest.Vector2I(10, 10), new global::BlackForest.Vector2I(10, 10));
        values.Add(PropertyName.@_fieldRect2I, global::BlackForest.Variant.From<global::BlackForest.Rect2I>(___fieldRect2I_default_value));
        global::BlackForest.Transform2D ___fieldTransform2D_default_value = global::BlackForest.Transform2D.Identity;
        values.Add(PropertyName.@_fieldTransform2D, global::BlackForest.Variant.From<global::BlackForest.Transform2D>(___fieldTransform2D_default_value));
        global::BlackForest.Vector3 ___fieldVector3_default_value = new(10f, 10f, 10f);
        values.Add(PropertyName.@_fieldVector3, global::BlackForest.Variant.From<global::BlackForest.Vector3>(___fieldVector3_default_value));
        global::BlackForest.Vector3I ___fieldVector3I_default_value = global::BlackForest.Vector3I.Back;
        values.Add(PropertyName.@_fieldVector3I, global::BlackForest.Variant.From<global::BlackForest.Vector3I>(___fieldVector3I_default_value));
        global::BlackForest.Basis ___fieldBasis_default_value = new global::BlackForest.Basis(global::BlackForest.Quaternion.Identity);
        values.Add(PropertyName.@_fieldBasis, global::BlackForest.Variant.From<global::BlackForest.Basis>(___fieldBasis_default_value));
        global::BlackForest.Quaternion ___fieldQuaternion_default_value = new global::BlackForest.Quaternion(global::BlackForest.Basis.Identity);
        values.Add(PropertyName.@_fieldQuaternion, global::BlackForest.Variant.From<global::BlackForest.Quaternion>(___fieldQuaternion_default_value));
        global::BlackForest.Transform3D ___fieldTransform3D_default_value = global::BlackForest.Transform3D.Identity;
        values.Add(PropertyName.@_fieldTransform3D, global::BlackForest.Variant.From<global::BlackForest.Transform3D>(___fieldTransform3D_default_value));
        global::BlackForest.Vector4 ___fieldVector4_default_value = new(10f, 10f, 10f, 10f);
        values.Add(PropertyName.@_fieldVector4, global::BlackForest.Variant.From<global::BlackForest.Vector4>(___fieldVector4_default_value));
        global::BlackForest.Vector4I ___fieldVector4I_default_value = global::BlackForest.Vector4I.One;
        values.Add(PropertyName.@_fieldVector4I, global::BlackForest.Variant.From<global::BlackForest.Vector4I>(___fieldVector4I_default_value));
        global::BlackForest.Projection ___fieldProjection_default_value = global::BlackForest.Projection.Identity;
        values.Add(PropertyName.@_fieldProjection, global::BlackForest.Variant.From<global::BlackForest.Projection>(___fieldProjection_default_value));
        global::BlackForest.Aabb ___fieldAabb_default_value = new global::BlackForest.Aabb(10f, 10f, 10f, new global::BlackForest.Vector3(1f, 1f, 1f));
        values.Add(PropertyName.@_fieldAabb, global::BlackForest.Variant.From<global::BlackForest.Aabb>(___fieldAabb_default_value));
        global::BlackForest.Color ___fieldColor_default_value = global::BlackForest.Colors.Aquamarine;
        values.Add(PropertyName.@_fieldColor, global::BlackForest.Variant.From<global::BlackForest.Color>(___fieldColor_default_value));
        global::BlackForest.Plane ___fieldPlane_default_value = global::BlackForest.Plane.PlaneXZ;
        values.Add(PropertyName.@_fieldPlane, global::BlackForest.Variant.From<global::BlackForest.Plane>(___fieldPlane_default_value));
        global::BlackForest.Callable ___fieldCallable_default_value = new global::BlackForest.Callable(global::BlackForest.Engine.GetMainLoop(), "_process");
        values.Add(PropertyName.@_fieldCallable, global::BlackForest.Variant.From<global::BlackForest.Callable>(___fieldCallable_default_value));
        global::BlackForest.Signal ___fieldSignal_default_value = new global::BlackForest.Signal(global::BlackForest.Engine.GetMainLoop(), "property_list_changed");
        values.Add(PropertyName.@_fieldSignal, global::BlackForest.Variant.From<global::BlackForest.Signal>(___fieldSignal_default_value));
        global::ExportedFields.MyEnum ___fieldEnum_default_value = global::ExportedFields.MyEnum.C;
        values.Add(PropertyName.@_fieldEnum, global::BlackForest.Variant.From<global::ExportedFields.MyEnum>(___fieldEnum_default_value));
        global::ExportedFields.MyFlagsEnum ___fieldFlagsEnum_default_value = global::ExportedFields.MyFlagsEnum.C;
        values.Add(PropertyName.@_fieldFlagsEnum, global::BlackForest.Variant.From<global::ExportedFields.MyFlagsEnum>(___fieldFlagsEnum_default_value));
        byte[] ___fieldByteArray_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldByteArray, global::BlackForest.Variant.From<byte[]>(___fieldByteArray_default_value));
        int[] ___fieldInt32Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldInt32Array, global::BlackForest.Variant.From<int[]>(___fieldInt32Array_default_value));
        long[] ___fieldInt64Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldInt64Array, global::BlackForest.Variant.From<long[]>(___fieldInt64Array_default_value));
        float[] ___fieldSingleArray_default_value = { 0f, 1f, 2f, 3f, 4f, 5f, 6f  };
        values.Add(PropertyName.@_fieldSingleArray, global::BlackForest.Variant.From<float[]>(___fieldSingleArray_default_value));
        double[] ___fieldDoubleArray_default_value = { 0d, 1d, 2d, 3d, 4d, 5d, 6d  };
        values.Add(PropertyName.@_fieldDoubleArray, global::BlackForest.Variant.From<double[]>(___fieldDoubleArray_default_value));
        string[] ___fieldStringArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldStringArray, global::BlackForest.Variant.From<string[]>(___fieldStringArray_default_value));
        string[] ___fieldStringArrayEnum_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldStringArrayEnum, global::BlackForest.Variant.From<string[]>(___fieldStringArrayEnum_default_value));
        global::BlackForest.Vector2[] ___fieldVector2Array_default_value = { global::BlackForest.Vector2.Up, global::BlackForest.Vector2.Down, global::BlackForest.Vector2.Left, global::BlackForest.Vector2.Right   };
        values.Add(PropertyName.@_fieldVector2Array, global::BlackForest.Variant.From<global::BlackForest.Vector2[]>(___fieldVector2Array_default_value));
        global::BlackForest.Vector3[] ___fieldVector3Array_default_value = { global::BlackForest.Vector3.Up, global::BlackForest.Vector3.Down, global::BlackForest.Vector3.Left, global::BlackForest.Vector3.Right   };
        values.Add(PropertyName.@_fieldVector3Array, global::BlackForest.Variant.From<global::BlackForest.Vector3[]>(___fieldVector3Array_default_value));
        global::BlackForest.Color[] ___fieldColorArray_default_value = { global::BlackForest.Colors.Aqua, global::BlackForest.Colors.Aquamarine, global::BlackForest.Colors.Azure, global::BlackForest.Colors.Beige   };
        values.Add(PropertyName.@_fieldColorArray, global::BlackForest.Variant.From<global::BlackForest.Color[]>(___fieldColorArray_default_value));
        global::BlackForest.GodotObject[] ___fieldGodotObjectOrDerivedArray_default_value = { null  };
        values.Add(PropertyName.@_fieldGodotObjectOrDerivedArray, global::BlackForest.Variant.CreateFrom(___fieldGodotObjectOrDerivedArray_default_value));
        global::BlackForest.StringName[] ___fieldStringNameArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldStringNameArray, global::BlackForest.Variant.From<global::BlackForest.StringName[]>(___fieldStringNameArray_default_value));
        global::BlackForest.NodePath[] ___fieldNodePathArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldNodePathArray, global::BlackForest.Variant.From<global::BlackForest.NodePath[]>(___fieldNodePathArray_default_value));
        global::BlackForest.Rid[] ___fieldRidArray_default_value = { default, default, default  };
        values.Add(PropertyName.@_fieldRidArray, global::BlackForest.Variant.From<global::BlackForest.Rid[]>(___fieldRidArray_default_value));
        int[] ___fieldEmptyInt32Array_default_value = global::System.Array.Empty<int>();
        values.Add(PropertyName.@_fieldEmptyInt32Array, global::BlackForest.Variant.From<int[]>(___fieldEmptyInt32Array_default_value));
        int[] ___fieldArrayFromList_default_value = new global::System.Collections.Generic.List<int>(global::System.Array.Empty<int>()).ToArray();
        values.Add(PropertyName.@_fieldArrayFromList, global::BlackForest.Variant.From<int[]>(___fieldArrayFromList_default_value));
        global::BlackForest.Variant ___fieldVariant_default_value = "foo";
        values.Add(PropertyName.@_fieldVariant, global::BlackForest.Variant.From<global::BlackForest.Variant>(___fieldVariant_default_value));
        global::BlackForest.GodotObject ___fieldGodotObjectOrDerived_default_value = default;
        values.Add(PropertyName.@_fieldGodotObjectOrDerived, global::BlackForest.Variant.From<global::BlackForest.GodotObject>(___fieldGodotObjectOrDerived_default_value));
        global::BlackForest.Texture ___fieldGodotResourceTexture_default_value = default;
        values.Add(PropertyName.@_fieldGodotResourceTexture, global::BlackForest.Variant.From<global::BlackForest.Texture>(___fieldGodotResourceTexture_default_value));
        global::BlackForest.Texture ___fieldGodotResourceTextureWithInitializer_default_value = new()  { ResourceName  = ""   };
        values.Add(PropertyName.@_fieldGodotResourceTextureWithInitializer, global::BlackForest.Variant.From<global::BlackForest.Texture>(___fieldGodotResourceTextureWithInitializer_default_value));
        global::BlackForest.StringName ___fieldStringName_default_value = new global::BlackForest.StringName("foo");
        values.Add(PropertyName.@_fieldStringName, global::BlackForest.Variant.From<global::BlackForest.StringName>(___fieldStringName_default_value));
        global::BlackForest.NodePath ___fieldNodePath_default_value = new global::BlackForest.NodePath("foo");
        values.Add(PropertyName.@_fieldNodePath, global::BlackForest.Variant.From<global::BlackForest.NodePath>(___fieldNodePath_default_value));
        global::BlackForest.Rid ___fieldRid_default_value = default;
        values.Add(PropertyName.@_fieldRid, global::BlackForest.Variant.From<global::BlackForest.Rid>(___fieldRid_default_value));
        global::BlackForest.Collections.Dictionary ___fieldGodotDictionary_default_value = new()  { { "foo", 10  }, { global::BlackForest.Vector2.Up, global::BlackForest.Colors.Chocolate   }  };
        values.Add(PropertyName.@_fieldGodotDictionary, global::BlackForest.Variant.From<global::BlackForest.Collections.Dictionary>(___fieldGodotDictionary_default_value));
        global::BlackForest.Collections.Array ___fieldGodotArray_default_value = new()  { "foo", 10, global::BlackForest.Vector2.Up, global::BlackForest.Colors.Chocolate   };
        values.Add(PropertyName.@_fieldGodotArray, global::BlackForest.Variant.From<global::BlackForest.Collections.Array>(___fieldGodotArray_default_value));
        global::BlackForest.Collections.Dictionary<string, bool> ___fieldGodotGenericDictionary_default_value = new()  { { "foo", true  }, { "bar", false  }  };
        values.Add(PropertyName.@_fieldGodotGenericDictionary, global::BlackForest.Variant.CreateFrom(___fieldGodotGenericDictionary_default_value));
        global::BlackForest.Collections.Array<int> ___fieldGodotGenericArray_default_value = new()  { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldGodotGenericArray, global::BlackForest.Variant.CreateFrom(___fieldGodotGenericArray_default_value));
        long[] ___fieldEmptyInt64Array_default_value = global::System.Array.Empty<long>();
        values.Add(PropertyName.@_fieldEmptyInt64Array, global::BlackForest.Variant.From<long[]>(___fieldEmptyInt64Array_default_value));
        return values;
    }
#endif // TOOLS
#pragma warning restore CS0109
}
