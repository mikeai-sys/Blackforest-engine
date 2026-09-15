partial class ExportedProperties
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
        var values = new global::System.Collections.Generic.Dictionary<global::BlackForest.StringName, global::BlackForest.Variant>(71);
        string __NotGenerateComplexLamdaProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateComplexLamdaProperty, global::BlackForest.Variant.From<string>(__NotGenerateComplexLamdaProperty_default_value));
        string __NotGenerateLamdaNoFieldProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateLamdaNoFieldProperty, global::BlackForest.Variant.From<string>(__NotGenerateLamdaNoFieldProperty_default_value));
        string __NotGenerateComplexReturnProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateComplexReturnProperty, global::BlackForest.Variant.From<string>(__NotGenerateComplexReturnProperty_default_value));
        string __NotGenerateReturnsProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateReturnsProperty, global::BlackForest.Variant.From<string>(__NotGenerateReturnsProperty_default_value));
        string __FullPropertyString_default_value = "FullPropertyString";
        values.Add(PropertyName.@FullPropertyString, global::BlackForest.Variant.From<string>(__FullPropertyString_default_value));
        string __FullPropertyString_Complex_default_value = new string("FullPropertyString_Complex")   + global::System.Convert.ToInt32("1");
        values.Add(PropertyName.@FullPropertyString_Complex, global::BlackForest.Variant.From<string>(__FullPropertyString_Complex_default_value));
        float __FullPropertyStaticImport_default_value = global::BlackForest.Mathf.Pi;
        values.Add(PropertyName.@FullPropertyStaticImport, global::BlackForest.Variant.From<float>(__FullPropertyStaticImport_default_value));
        string __LamdaPropertyString_default_value = "LamdaPropertyString";
        values.Add(PropertyName.@LamdaPropertyString, global::BlackForest.Variant.From<string>(__LamdaPropertyString_default_value));
        float __LambdaPropertyStaticImport_default_value = global::BlackForest.Mathf.Tau;
        values.Add(PropertyName.@LambdaPropertyStaticImport, global::BlackForest.Variant.From<float>(__LambdaPropertyStaticImport_default_value));
        string __PrimaryCtorParameter_default_value = default;
        values.Add(PropertyName.@PrimaryCtorParameter, global::BlackForest.Variant.From<string>(__PrimaryCtorParameter_default_value));
        float __ConstantMath_default_value = 2  * global::BlackForest.Mathf.Pi;
        values.Add(PropertyName.@ConstantMath, global::BlackForest.Variant.From<float>(__ConstantMath_default_value));
        float __ConstantMathStaticImport_default_value = global::BlackForest.Mathf.RadToDeg(2  * global::BlackForest.Mathf.Pi);
        values.Add(PropertyName.@ConstantMathStaticImport, global::BlackForest.Variant.From<float>(__ConstantMathStaticImport_default_value));
        string __StaticStringAddition_default_value = string.Empty   + string.Empty;
        values.Add(PropertyName.@StaticStringAddition, global::BlackForest.Variant.From<string>(__StaticStringAddition_default_value));
        bool __PropertyBoolean_default_value = true;
        values.Add(PropertyName.@PropertyBoolean, global::BlackForest.Variant.From<bool>(__PropertyBoolean_default_value));
        char __PropertyChar_default_value = 'f';
        values.Add(PropertyName.@PropertyChar, global::BlackForest.Variant.From<char>(__PropertyChar_default_value));
        sbyte __PropertySByte_default_value = 10;
        values.Add(PropertyName.@PropertySByte, global::BlackForest.Variant.From<sbyte>(__PropertySByte_default_value));
        short __PropertyInt16_default_value = 10;
        values.Add(PropertyName.@PropertyInt16, global::BlackForest.Variant.From<short>(__PropertyInt16_default_value));
        int __PropertyInt32_default_value = 10;
        values.Add(PropertyName.@PropertyInt32, global::BlackForest.Variant.From<int>(__PropertyInt32_default_value));
        long __PropertyInt64_default_value = -10_000;
        values.Add(PropertyName.@PropertyInt64, global::BlackForest.Variant.From<long>(__PropertyInt64_default_value));
        byte __PropertyByte_default_value = 10;
        values.Add(PropertyName.@PropertyByte, global::BlackForest.Variant.From<byte>(__PropertyByte_default_value));
        ushort __PropertyUInt16_default_value = 10;
        values.Add(PropertyName.@PropertyUInt16, global::BlackForest.Variant.From<ushort>(__PropertyUInt16_default_value));
        uint __PropertyUInt32_default_value = 10;
        values.Add(PropertyName.@PropertyUInt32, global::BlackForest.Variant.From<uint>(__PropertyUInt32_default_value));
        ulong __PropertyUInt64_default_value = 10;
        values.Add(PropertyName.@PropertyUInt64, global::BlackForest.Variant.From<ulong>(__PropertyUInt64_default_value));
        float __PropertySingle_default_value = 10;
        values.Add(PropertyName.@PropertySingle, global::BlackForest.Variant.From<float>(__PropertySingle_default_value));
        double __PropertyDouble_default_value = 10;
        values.Add(PropertyName.@PropertyDouble, global::BlackForest.Variant.From<double>(__PropertyDouble_default_value));
        string __PropertyString_default_value = "foo";
        values.Add(PropertyName.@PropertyString, global::BlackForest.Variant.From<string>(__PropertyString_default_value));
        global::BlackForest.Vector2 __PropertyVector2_default_value = new(10f, 10f);
        values.Add(PropertyName.@PropertyVector2, global::BlackForest.Variant.From<global::BlackForest.Vector2>(__PropertyVector2_default_value));
        global::BlackForest.Vector2I __PropertyVector2I_default_value = global::BlackForest.Vector2I.Up;
        values.Add(PropertyName.@PropertyVector2I, global::BlackForest.Variant.From<global::BlackForest.Vector2I>(__PropertyVector2I_default_value));
        global::BlackForest.Rect2 __PropertyRect2_default_value = new(new global::BlackForest.Vector2(10f, 10f), new global::BlackForest.Vector2(10f, 10f));
        values.Add(PropertyName.@PropertyRect2, global::BlackForest.Variant.From<global::BlackForest.Rect2>(__PropertyRect2_default_value));
        global::BlackForest.Rect2I __PropertyRect2I_default_value = new(new global::BlackForest.Vector2I(10, 10), new global::BlackForest.Vector2I(10, 10));
        values.Add(PropertyName.@PropertyRect2I, global::BlackForest.Variant.From<global::BlackForest.Rect2I>(__PropertyRect2I_default_value));
        global::BlackForest.Transform2D __PropertyTransform2D_default_value = global::BlackForest.Transform2D.Identity;
        values.Add(PropertyName.@PropertyTransform2D, global::BlackForest.Variant.From<global::BlackForest.Transform2D>(__PropertyTransform2D_default_value));
        global::BlackForest.Vector3 __PropertyVector3_default_value = new(10f, 10f, 10f);
        values.Add(PropertyName.@PropertyVector3, global::BlackForest.Variant.From<global::BlackForest.Vector3>(__PropertyVector3_default_value));
        global::BlackForest.Vector3I __PropertyVector3I_default_value = global::BlackForest.Vector3I.Back;
        values.Add(PropertyName.@PropertyVector3I, global::BlackForest.Variant.From<global::BlackForest.Vector3I>(__PropertyVector3I_default_value));
        global::BlackForest.Basis __PropertyBasis_default_value = new global::BlackForest.Basis(global::BlackForest.Quaternion.Identity);
        values.Add(PropertyName.@PropertyBasis, global::BlackForest.Variant.From<global::BlackForest.Basis>(__PropertyBasis_default_value));
        global::BlackForest.Quaternion __PropertyQuaternion_default_value = new global::BlackForest.Quaternion(global::BlackForest.Basis.Identity);
        values.Add(PropertyName.@PropertyQuaternion, global::BlackForest.Variant.From<global::BlackForest.Quaternion>(__PropertyQuaternion_default_value));
        global::BlackForest.Transform3D __PropertyTransform3D_default_value = global::BlackForest.Transform3D.Identity;
        values.Add(PropertyName.@PropertyTransform3D, global::BlackForest.Variant.From<global::BlackForest.Transform3D>(__PropertyTransform3D_default_value));
        global::BlackForest.Vector4 __PropertyVector4_default_value = new(10f, 10f, 10f, 10f);
        values.Add(PropertyName.@PropertyVector4, global::BlackForest.Variant.From<global::BlackForest.Vector4>(__PropertyVector4_default_value));
        global::BlackForest.Vector4I __PropertyVector4I_default_value = global::BlackForest.Vector4I.One;
        values.Add(PropertyName.@PropertyVector4I, global::BlackForest.Variant.From<global::BlackForest.Vector4I>(__PropertyVector4I_default_value));
        global::BlackForest.Projection __PropertyProjection_default_value = global::BlackForest.Projection.Identity;
        values.Add(PropertyName.@PropertyProjection, global::BlackForest.Variant.From<global::BlackForest.Projection>(__PropertyProjection_default_value));
        global::BlackForest.Aabb __PropertyAabb_default_value = new global::BlackForest.Aabb(10f, 10f, 10f, new global::BlackForest.Vector3(1f, 1f, 1f));
        values.Add(PropertyName.@PropertyAabb, global::BlackForest.Variant.From<global::BlackForest.Aabb>(__PropertyAabb_default_value));
        global::BlackForest.Color __PropertyColor_default_value = global::BlackForest.Colors.Aquamarine;
        values.Add(PropertyName.@PropertyColor, global::BlackForest.Variant.From<global::BlackForest.Color>(__PropertyColor_default_value));
        global::BlackForest.Plane __PropertyPlane_default_value = global::BlackForest.Plane.PlaneXZ;
        values.Add(PropertyName.@PropertyPlane, global::BlackForest.Variant.From<global::BlackForest.Plane>(__PropertyPlane_default_value));
        global::BlackForest.Callable __PropertyCallable_default_value = new global::BlackForest.Callable(global::BlackForest.Engine.GetMainLoop(), "_process");
        values.Add(PropertyName.@PropertyCallable, global::BlackForest.Variant.From<global::BlackForest.Callable>(__PropertyCallable_default_value));
        global::BlackForest.Signal __PropertySignal_default_value = new global::BlackForest.Signal(global::BlackForest.Engine.GetMainLoop(), "Propertylist_changed");
        values.Add(PropertyName.@PropertySignal, global::BlackForest.Variant.From<global::BlackForest.Signal>(__PropertySignal_default_value));
        global::ExportedProperties.MyEnum __PropertyEnum_default_value = global::ExportedProperties.MyEnum.C;
        values.Add(PropertyName.@PropertyEnum, global::BlackForest.Variant.From<global::ExportedProperties.MyEnum>(__PropertyEnum_default_value));
        global::ExportedProperties.MyFlagsEnum __PropertyFlagsEnum_default_value = global::ExportedProperties.MyFlagsEnum.C;
        values.Add(PropertyName.@PropertyFlagsEnum, global::BlackForest.Variant.From<global::ExportedProperties.MyFlagsEnum>(__PropertyFlagsEnum_default_value));
        byte[] __PropertyByteArray_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyByteArray, global::BlackForest.Variant.From<byte[]>(__PropertyByteArray_default_value));
        int[] __PropertyInt32Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyInt32Array, global::BlackForest.Variant.From<int[]>(__PropertyInt32Array_default_value));
        long[] __PropertyInt64Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyInt64Array, global::BlackForest.Variant.From<long[]>(__PropertyInt64Array_default_value));
        float[] __PropertySingleArray_default_value = { 0f, 1f, 2f, 3f, 4f, 5f, 6f  };
        values.Add(PropertyName.@PropertySingleArray, global::BlackForest.Variant.From<float[]>(__PropertySingleArray_default_value));
        double[] __PropertyDoubleArray_default_value = { 0d, 1d, 2d, 3d, 4d, 5d, 6d  };
        values.Add(PropertyName.@PropertyDoubleArray, global::BlackForest.Variant.From<double[]>(__PropertyDoubleArray_default_value));
        string[] __PropertyStringArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@PropertyStringArray, global::BlackForest.Variant.From<string[]>(__PropertyStringArray_default_value));
        string[] __PropertyStringArrayEnum_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@PropertyStringArrayEnum, global::BlackForest.Variant.From<string[]>(__PropertyStringArrayEnum_default_value));
        global::BlackForest.Vector2[] __PropertyVector2Array_default_value = { global::BlackForest.Vector2.Up, global::BlackForest.Vector2.Down, global::BlackForest.Vector2.Left, global::BlackForest.Vector2.Right   };
        values.Add(PropertyName.@PropertyVector2Array, global::BlackForest.Variant.From<global::BlackForest.Vector2[]>(__PropertyVector2Array_default_value));
        global::BlackForest.Vector3[] __PropertyVector3Array_default_value = { global::BlackForest.Vector3.Up, global::BlackForest.Vector3.Down, global::BlackForest.Vector3.Left, global::BlackForest.Vector3.Right   };
        values.Add(PropertyName.@PropertyVector3Array, global::BlackForest.Variant.From<global::BlackForest.Vector3[]>(__PropertyVector3Array_default_value));
        global::BlackForest.Color[] __PropertyColorArray_default_value = { global::BlackForest.Colors.Aqua, global::BlackForest.Colors.Aquamarine, global::BlackForest.Colors.Azure, global::BlackForest.Colors.Beige   };
        values.Add(PropertyName.@PropertyColorArray, global::BlackForest.Variant.From<global::BlackForest.Color[]>(__PropertyColorArray_default_value));
        global::BlackForest.GodotObject[] __PropertyGodotObjectOrDerivedArray_default_value = { null  };
        values.Add(PropertyName.@PropertyGodotObjectOrDerivedArray, global::BlackForest.Variant.CreateFrom(__PropertyGodotObjectOrDerivedArray_default_value));
        global::BlackForest.StringName[] __field_StringNameArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@field_StringNameArray, global::BlackForest.Variant.From<global::BlackForest.StringName[]>(__field_StringNameArray_default_value));
        global::BlackForest.NodePath[] __field_NodePathArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@field_NodePathArray, global::BlackForest.Variant.From<global::BlackForest.NodePath[]>(__field_NodePathArray_default_value));
        global::BlackForest.Rid[] __field_RidArray_default_value = { default, default, default  };
        values.Add(PropertyName.@field_RidArray, global::BlackForest.Variant.From<global::BlackForest.Rid[]>(__field_RidArray_default_value));
        global::BlackForest.Variant __PropertyVariant_default_value = "foo";
        values.Add(PropertyName.@PropertyVariant, global::BlackForest.Variant.From<global::BlackForest.Variant>(__PropertyVariant_default_value));
        global::BlackForest.GodotObject __PropertyGodotObjectOrDerived_default_value = default;
        values.Add(PropertyName.@PropertyGodotObjectOrDerived, global::BlackForest.Variant.From<global::BlackForest.GodotObject>(__PropertyGodotObjectOrDerived_default_value));
        global::BlackForest.Texture __PropertyGodotResourceTexture_default_value = default;
        values.Add(PropertyName.@PropertyGodotResourceTexture, global::BlackForest.Variant.From<global::BlackForest.Texture>(__PropertyGodotResourceTexture_default_value));
        global::BlackForest.Texture __PropertyGodotResourceTextureWithInitializer_default_value = new()  { ResourceName  = ""   };
        values.Add(PropertyName.@PropertyGodotResourceTextureWithInitializer, global::BlackForest.Variant.From<global::BlackForest.Texture>(__PropertyGodotResourceTextureWithInitializer_default_value));
        global::BlackForest.StringName __PropertyStringName_default_value = new global::BlackForest.StringName("foo");
        values.Add(PropertyName.@PropertyStringName, global::BlackForest.Variant.From<global::BlackForest.StringName>(__PropertyStringName_default_value));
        global::BlackForest.NodePath __PropertyNodePath_default_value = new global::BlackForest.NodePath("foo");
        values.Add(PropertyName.@PropertyNodePath, global::BlackForest.Variant.From<global::BlackForest.NodePath>(__PropertyNodePath_default_value));
        global::BlackForest.Rid __PropertyRid_default_value = default;
        values.Add(PropertyName.@PropertyRid, global::BlackForest.Variant.From<global::BlackForest.Rid>(__PropertyRid_default_value));
        global::BlackForest.Collections.Dictionary __PropertyGodotDictionary_default_value = new()  { { "foo", 10  }, { global::BlackForest.Vector2.Up, global::BlackForest.Colors.Chocolate   }  };
        values.Add(PropertyName.@PropertyGodotDictionary, global::BlackForest.Variant.From<global::BlackForest.Collections.Dictionary>(__PropertyGodotDictionary_default_value));
        global::BlackForest.Collections.Array __PropertyGodotArray_default_value = new()  { "foo", 10, global::BlackForest.Vector2.Up, global::BlackForest.Colors.Chocolate   };
        values.Add(PropertyName.@PropertyGodotArray, global::BlackForest.Variant.From<global::BlackForest.Collections.Array>(__PropertyGodotArray_default_value));
        global::BlackForest.Collections.Dictionary<string, bool> __PropertyGodotGenericDictionary_default_value = new()  { { "foo", true  }, { "bar", false  }  };
        values.Add(PropertyName.@PropertyGodotGenericDictionary, global::BlackForest.Variant.CreateFrom(__PropertyGodotGenericDictionary_default_value));
        global::BlackForest.Collections.Array<int> __PropertyGodotGenericArray_default_value = new()  { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyGodotGenericArray, global::BlackForest.Variant.CreateFrom(__PropertyGodotGenericArray_default_value));
        return values;
    }
#endif // TOOLS
#pragma warning restore CS0109
}
