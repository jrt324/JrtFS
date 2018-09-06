using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JrtFS.Common
{
    public static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<string, dynamic> fDefaultValueCache
            = new ConcurrentDictionary<string, dynamic>();

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <param name="objValue">要转化的源对象</param>
        /// <param name="destType">要返回的对象类型</param>
        /// <param name="defaultValue">转换失败后的默认值</param>
        /// <returns>转化后的指定类型对象</returns>
        public static object ConvertType(this object objValue, Type destType, dynamic defaultValue)
        {
            if (objValue == null)
                throw new ArgumentNullException("destType", "参数不能为空");
            dynamic newDefaultValue = GetDefaultValue(destType, defaultValue);
            if (objValue == null || objValue == DBNull.Value)
                return newDefaultValue;
            try
            {
                var sourceType = objValue.GetType();
                if (sourceType == typeof(void))
                    return newDefaultValue;
                return ConvertType(objValue, destType);
            }
            catch
            {
                return newDefaultValue;
            }
        }


        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <param name="value">要转化的源对象</param>
        /// <param name="destType">要返回的对象类型</param>
        /// <returns>转化后的指定类型对象</returns>
        public static object ConvertType(this object value, Type destType)
        {
            if (value == null)
            {
                return null;
            }
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (destType == null)
            {
                throw new ArgumentNullException("destType");
            } // end if

            if (value.GetType() == destType)
            {
                return value;
            }

            var sourceType = value.GetType();

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType
            if (destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                NullableConverter nullableConverter = new NullableConverter(destType);
                destType = nullableConverter.UnderlyingType;
            } // end if

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(value);
                if (converter != null)
                {
                    if (converter.CanConvertTo(destType))
                    {
                        return converter.ConvertTo(value, destType);
                    }
                }

                converter = TypeDescriptor.GetConverter(destType);
                if (converter != null)
                {
                    if (converter.CanConvertFrom(sourceType))
                    {
                        return converter.ConvertFrom(value);
                    }
                }

                if (value == DBNull.Value)
                {
                    return null;
                }
            }
            catch (Exception)
            {
            }

            if (destType.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(destType, value as string);
                else
                    return Enum.ToObject(destType, value);
            }

            if (value is string)
            {
                if (destType == typeof(Guid))
                {
                    return new Guid(value.ToString());
                }
                else if (destType == typeof(Version))
                {
                    return new Version(value as string);
                }
                else if (destType == typeof(bool))
                {
                    if (value.Equals("1") && string.Equals(value.ToString(), "true", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                    else if (value.Equals("0") && string.Equals(value.ToString(), "false", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(destType);
                        object propValue = typeConverter.ConvertFromString(value.ToString());
                        return propValue;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            try
            {
                // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
                // nullable type), pass the call on to Convert.ChangeType
                return Convert.ChangeType(value, destType);
            }
            catch (Exception ex)
            {
                if (destType.IsAssignableFrom(sourceType))
                    return value;
                throw ex;
            }
        }

        /// <summary>
        /// 把对象类型转化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="value">原始对象</param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value)
        {
            if (value != null)
            {
                try
                {
                    var result = ConvertType(value, typeof(T));
                    return (T)result;
                }
                catch
                {
                    if (value == DBNull.Value)
                    {
                        return (T)(object)null;
                    }
                }
            }
            return (T)value;
        }

        public static bool IsNull(this object o)
        {
            if (o == null) return true;
            if (o.Equals(DBNull.Value)) return true;

            if (o is string)
            {
                return string.IsNullOrEmpty((string)o);
            }
            else if (o is Guid || o is Guid?)
            {
                if (o.ToString() == Guid.Empty.ToString())
                {
                    return true;
                }
            }
            else if (o is DateTime || o is DateTime?)
            {
                if (DateTime.MinValue.Equals(o))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDefaultValue(this Type type, dynamic value)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (value == null)
                return true;
            return value.Equals(GetOrAddDefaultValue(type));
        }

        public static dynamic GetDefaultValue(this Type destType, dynamic defaultValue = null)
        {
            if (destType == null)
            {
                throw new ArgumentNullException("destType");
            }

            if (!IsDefaultValue(destType, defaultValue))
            {
                return ConvertType(defaultValue, destType);
            }

            return GetOrAddDefaultValue(destType);
        }

        private static dynamic GetOrAddDefaultValue(Type type)
        {
            var key = type.AssemblyQualifiedName;
            return fDefaultValueCache.GetOrAdd(key, (newKey) =>
            {
                if (type.IsEnum)
                    return type.GetFirstEnumValue();
                if (type.IsValueType)
                    return Activator.CreateInstance(type);
                return null;
            });
        }

        public static object GetFirstEnumValue(this Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("destType");
            }

            Array values = Enum.GetValues(enumType);
            if (values.Length == 0)
            {
                throw new Exception(string.Format("枚举类型{0}中没有枚举值", enumType));
            }
            return (values as IList)[0];
        }

        
    }
}