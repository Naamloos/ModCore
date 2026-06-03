using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ModCore.Common.Database.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ModCore.Common.Cryptography;

namespace ModCore.Common.Database.Interceptors
{
    public class EncryptionInterceptor : SaveChangesInterceptor
    {
        private readonly string _base64Key;

        public EncryptionInterceptor(string base64Key)
        {
            _base64Key = base64Key;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            EncryptEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            EncryptEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void EncryptEntities(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in properties)
                {
                    var encryptAttr = property.GetCustomAttribute<EncryptedColumnAttribute>();
                    if (encryptAttr == null || property.PropertyType != typeof(string)) continue;

                    var plainText = (string?)property.GetValue(entry.Entity);
                    if (string.IsNullOrEmpty(plainText)) continue;

                    // Build the compound context string from the array of names
                    string combinedContext = BuildCombinedContext(entry.Entity, entityType, encryptAttr.ContextPropertyNames);
                    if (string.IsNullOrEmpty(combinedContext)) continue;

                    var encryptedEnvelope = CryptographyHelper.Encrypt(plainText, combinedContext, _base64Key);
                    property.SetValue(entry.Entity, encryptedEnvelope);
                }
            }
        }

        private static string BuildCombinedContext(object entity, Type entityType, string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                throw new InvalidOperationException($"[EncryptedColumn] on '{entityType.Name}' must specify at least one context property.");
            }

            var segments = new string[propertyNames.Length];

            for (int i = 0; i < propertyNames.Length; i++)
            {
                var propName = propertyNames[i];
                var propInfo = entityType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo == null)
                {
                    throw new InvalidOperationException($"Context tracker property '{propName}' was not found on entity type '{entityType.Name}'.");
                }

                var val = propInfo.GetValue(entity)?.ToString();
                if (string.IsNullOrEmpty(val))
                {
                    throw new InvalidOperationException($"Context tracker property '{propName}' on '{entityType.Name}' was null or empty during encryption execution.");
                }

                segments[i] = val;
            }

            // Join values cleanly (e.g., "123456||789012")
            return string.Join("||", segments);
        }
    }
}
