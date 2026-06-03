using Microsoft.EntityFrameworkCore.Diagnostics;
using ModCore.Common.Cryptography;
using ModCore.Common.Database.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Interceptors
{
    public class DecryptionInterceptor : IMaterializationInterceptor
    {
        private readonly string _base64Key;

        public DecryptionInterceptor(string base64Key)
        {
            _base64Key = base64Key;
        }

        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            var entityType = instance.GetType();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var encryptAttr = property.GetCustomAttribute<EncryptedColumnAttribute>();
                if (encryptAttr == null || property.PropertyType != typeof(string)) continue;

                var dbValue = (string?)property.GetValue(instance);
                if (string.IsNullOrEmpty(dbValue)) continue;

                // Rebuild the exact same compound context layout
                string combinedContext = BuildCombinedContext(instance, entityType, encryptAttr.ContextPropertyNames);
                if (string.IsNullOrEmpty(combinedContext)) continue;

                var plainText = CryptographyHelper.Decrypt(dbValue, combinedContext, _base64Key);
                property.SetValue(instance, plainText);
            }

            return instance;
        }

        private static string BuildCombinedContext(object entity, Type entityType, string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0) return string.Empty;

            var segments = new string[propertyNames.Length];

            for (int i = 0; i < propertyNames.Length; i++)
            {
                var propName = propertyNames[i];
                var propInfo = entityType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null) return string.Empty;

                var val = propInfo.GetValue(entity)?.ToString();
                if (string.IsNullOrEmpty(val)) return string.Empty;

                segments[i] = val;
            }

            return string.Join("||", segments);
        }
    }
}
