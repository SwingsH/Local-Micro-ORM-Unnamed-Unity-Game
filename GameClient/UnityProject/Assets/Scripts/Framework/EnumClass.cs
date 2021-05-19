using System;
using System.Reflection;

public class EnumClassValue : System.Attribute
{
    protected string strValue; 
    protected Type classType;
    public EnumClassValue(Type classType, string fileName)
    {
        strValue = fileName.ToLower();
        this.classType = classType;
    }
    public EnumClassValue(Type classType)
    {
        strValue = "";
        this.classType = classType;
    }
    public EnumClassValue() { }

    public string Value => strValue;
    public Type ClassType => classType;

    public static string GetStringValue(Enum value)
    {
        string output = null;
        EnumClassValue enumType = null;
        if (Retrieve(value, out enumType))
            output = enumType.Value;
        return output;
    }

    public static Type GetClassType(Enum value)
    {
        Type output = null;
        EnumClassValue enumType = null;
        if (Retrieve(value, out enumType))
            output = enumType.ClassType;
        return output;
    }

    private static bool Retrieve(Enum value, out EnumClassValue output)
    {
        output = null;
        Type type = value.GetType();
        FieldInfo fi = type.GetField(value.ToString());
        EnumClassValue[] attrs = fi.GetCustomAttributes(typeof(EnumClassValue), false) as EnumClassValue[]; // Retrieve to self-def object
        if (attrs.Length > 0)
        {
            output = attrs[0];
            return true;
        }
        return false;
    }
}

public class EnumValueSet : System.Attribute
{
    protected Attribute enumAttribute;  
    protected string stringValue;   
    private Enum custumEnum;
    public EnumValueSet(Enum e, string strVal)
    {
        custumEnum = e;
        stringValue = strVal;
    }

    public string Value => stringValue;
    public Attribute Attribute => enumAttribute;

    public static Enum GetKey(Enum value)
    {
        object output = null;
        EnumValueSet enumType;
        if (Retrieve(value, out enumType))
            output = enumType.enumAttribute;
        return output as Enum;
    }

    private static bool Retrieve(Enum enumSet, out EnumValueSet output)
    {
        output = null;
        Type type = enumSet.GetType();
        FieldInfo fi = type.GetField(enumSet.ToString());
        EnumValueSet[] attrs = fi.GetCustomAttributes(typeof(EnumValueSet), false) as EnumValueSet[]; // Retrieve to self-def object
        if (attrs.Length > 0)
        {
            output = attrs[0];
            return true;
        }
        return false;
    }
}
