using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace INB.Infra.Data.Util
{
    public class CopyObject
    {


        public static IEnumerable<PropertyInfo> GetEFProperties(object instance)
        {
            return GetEFProperties(instance.GetType());
        }

        public static IEnumerable<PropertyInfo> GetEFProperties(Type instanceType)
        {
            return instanceType.GetProperties(BindingFlags.DeclaredOnly |
                                        BindingFlags.Public |
                                        BindingFlags.Instance)
                //.Where(pi => !(pi.PropertyType.IsSubclassOf(typeof(System.Data.Entity.Core.Objects.DataClasses.EntityObject)) || pi.PropertyType.IsSubclassOf(typeof(EntityReference))));
                .Where(p => p.PropertyType.IsClass);
        }

        public static IEnumerable<PropertyInfo> GetEFProperties(string cType)
        {
            return GetEFProperties(Type.GetType(cType + ", " + GetCurrentNamespace()));
        }

        public static IEnumerable<PropertyInfo> GetEFPropertiesCollections(object instance)
        {
            return GetEFPropertiesCollections(instance.GetType());
        }

        public static IEnumerable<PropertyInfo> GetEFPropertiesCollections(string cType)
        {
            return GetEFPropertiesCollections(Type.GetType(cType + ", " + GetCurrentNamespace()));
        }

        public static IEnumerable<PropertyInfo> GetEFPropertiesCollections(Type instanceType)
        {
            return instanceType.GetProperties(BindingFlags.DeclaredOnly |
                                        BindingFlags.Public |
                                        BindingFlags.Instance)
                .Where(p => !p.PropertyType.IsClass);
        }

        public static string GetCurrentNamespace()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

        public static void CopyObjectData(object source, object target)
        {
            CopyObjectData(source, target, string.Empty);
        }

        public static void CopyObjectData(object source, object target, string excludedProperties)
        {


            CopyObjectData(source, target, excludedProperties, BindingFlags.Public |
                                        BindingFlags.Instance | BindingFlags.NonPublic);
        }


        public static void CopyData(MemberInfo[] miT, object source, object target, string excludedProperties, BindingFlags memberAccess)
        {


            string[] excluded = null;
            if (!string.IsNullOrEmpty(excludedProperties))
                excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;

                // Skip over any property exceptions
                if (!string.IsNullOrEmpty(excludedProperties) &&
                    excluded.Contains(name))
                    continue;

                object SourceValue;

                if (Field.MemberType == MemberTypes.Field)
                {
                    FieldInfo SourceField = source.GetType().GetField(name);
                    if (SourceField == null)
                        continue;

                    SourceValue = SourceField.GetValue(source);
                    ((FieldInfo)Field).SetValue(target, SourceValue);
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    PropertyInfo piTarget = Field as PropertyInfo;
                    PropertyInfo SourceField = source.GetType().GetProperty(name, memberAccess);
                    if (SourceField == null)
                        continue;

                    if (piTarget.CanWrite && SourceField.CanRead)
                    {
                        SourceValue = SourceField.GetValue(source, null);
                        piTarget.SetValue(target, SourceValue, null);
                    }
                }
            }
        }


        public static void CopyObjectData(object source, object target, string excludedProperties, BindingFlags memberAccess)
        {
            if (source == null | target == null)
                return;

            MemberInfo[] miT = target.GetType().GetMembers(memberAccess);
            CopyData(miT, source, target, excludedProperties, memberAccess);

            miT = target.GetType().BaseType.GetMembers(memberAccess);
            if (miT.Count() > 0)
            {
                CopyData(miT, source, target, excludedProperties, memberAccess);
            }

        }

        public static void ReflectObject(Object Target, Object Source)
        {
            if (Source == null | Target == null)
            {
                return;
            }
            //BindingFlags Flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly; // BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            PropertyInfo[] arrPropertiesSource = Source.GetType().GetProperties(Flags);

            //Procura em cada propriedade do Source uma propriedade 
            //correspondente em Target
            foreach (PropertyInfo Item in arrPropertiesSource)
            {
                PropertyInfo oProperty = Target.GetType().GetProperty(Item.Name);
                //Verifica se a propriedade existe no destino
                if (oProperty == null)
                {
                    continue;
                }
                //Verifica se é um tipo válido
                if (!(oProperty.PropertyType.IsPrimitive | oProperty.PropertyType.IsArray | oProperty.PropertyType.IsSerializable | object.ReferenceEquals(oProperty.PropertyType, typeof(System.DateTime)) | object.ReferenceEquals(oProperty.PropertyType, typeof(System.DateTime?))))
                {
                    continue;
                }

                //Armazena o valor do destino para uma avaliação: evitar que se tente gravar dados nessa propriedade. O linq gera erro quando tem valor.
                object oValueTarget = oProperty.GetValue(Target, null);

                //Busca a propriedade na fonte
                oProperty = Source.GetType().GetProperty(Item.Name, Flags);
                if (oProperty == null)
                {
                    continue;
                }

                object oValue = oProperty.GetValue(Source, null);
                bool IsPK = false;
                oProperty = Target.GetType().GetProperty(Item.Name, Flags);
                if (object.ReferenceEquals(oProperty.PropertyType, typeof(System.Data.Linq.Mapping.ColumnAttribute)))
                {
                    IsPK = IsPrimaryKey(Source, oProperty.Name);
                }
                if (IsPK & HasValue(oValueTarget))
                {
                    //Não atualiza
                }
                else
                {
                    if (oProperty.CanWrite)
                    {
                        oProperty.SetValue(Target, oValue, null);
                    }
                }
            }
        }

        private static bool HasValue(object Source)
        {
            //Somente para campos chave primaria
            bool Result = false;
            if ((Source) is int | (Source) is long | (Source) is double)
            {
                double val = 0;
                double.TryParse(Source.ToString(), out val);
                if (val > 0)
                {
                    Result = true;
                }
            }
            else if ((Source) is string)
            {
                string val = (string)Source;
                Result = string.IsNullOrEmpty(val);
            }
            return Result;
        }

        public static bool HasProperty(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return null;
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop == null) return null;
            return prop.GetValue(obj);
        }

        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, value, null);
            }

        }

        public static PropertyInfo GetPrimaryKey(object Source)
        {
            PropertyInfo[] infos = Source.GetType().GetProperties();
            PropertyInfo PKProperty = null;
            foreach (PropertyInfo info in infos)
            {
                dynamic column = info.GetCustomAttributes(false).Where(x => x.GetType() == typeof(System.Data.Linq.Mapping.ColumnAttribute)).FirstOrDefault(x => (((System.Data.Linq.Mapping.ColumnAttribute)x).IsPrimaryKey && ((System.Data.Linq.Mapping.ColumnAttribute)x).DbType.Contains("NOT NULL")));
                if (column != null)
                {
                    PKProperty = info;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            return PKProperty;
        }

        public static bool IsPrimaryKey(object Source, string PropertyName)
        {
            PropertyInfo PropAux = GetPrimaryKey(Source);
            if (PropAux != null && PropAux.Name.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}
