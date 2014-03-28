using UnityEngine;
using System.Collections;
using System.Reflection;

public static class ActionBarUtils
{
    public class Field<T>
    {
        object obj;
        FieldInfo field;

        public T Value
        {
            get { return (T)field.GetValue(obj); }
            set { field.SetValue(obj, value); }
        }

        public Field(object o, string name)
        {
            obj = o;
            field = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }

    public static Field<T> GetField<T>(object obj, string name)
    {
        return new Field<T>(obj, name);
    }

    public static System.Enum EditFlagsEnum(this System.Enum value)
    {
        int intValue = (int)(System.ValueType)value;

        Debug.Log(intValue);

        return value;
    }
}