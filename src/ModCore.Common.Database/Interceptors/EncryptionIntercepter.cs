using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ModCore.Common.Cryptography;
using ModCore.Common.Database.Attributes;
using System.Reflection;

namespace ModCore.Common.Database.Interceptors
{
    public class EncryptionInterceptor : SaveChangesInterceptor
    {
        private readonly string _base64Key;

        private readonly List<RestoreValue> _restoreValues = new();

        public EncryptionInterceptor(string base64Key)
        {
            _base64Key = base64Key;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            EncryptChangedValues(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            EncryptChangedValues(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(
            SaveChangesCompletedEventData eventData,
            int result)
        {
            RestorePlaintextValues();
            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            RestorePlaintextValues();
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            RestorePlaintextValues();
            base.SaveChangesFailed(eventData);
        }

        public override Task SaveChangesFailedAsync(
            DbContextErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            RestorePlaintextValues();
            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }

        private void EncryptChangedValues(DbContext? context)
        {
            if (context == null) return;

            _restoreValues.Clear();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
                {
                    continue;
                }

                var entityType = entry.Entity.GetType();

                foreach (var property in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var encryptAttr = property.GetCustomAttribute<EncryptedColumnAttribute>();
                    if (encryptAttr == null || property.PropertyType != typeof(string))
                    {
                        continue;
                    }

                    var plainText = (string?)property.GetValue(entry.Entity);
                    if (string.IsNullOrEmpty(plainText))
                    {
                        continue;
                    }

                    if (plainText.StartsWith("{\"t\":") || plainText.StartsWith("{\"d\":"))
                    {
                        continue;
                    }

                    var combinedContext = BuildCombinedContext(
                        entry.Entity,
                        entityType,
                        encryptAttr.ContextPropertyNames
                    );

                    if (string.IsNullOrEmpty(combinedContext))
                    {
                        continue;
                    }

                    var encrypted = CryptographyHelper.Encrypt(plainText, combinedContext, _base64Key);

                    property.SetValue(entry.Entity, encrypted);

                    _restoreValues.Add(new RestoreValue(
                        entry,
                        property.Name,
                        plainText
                    ));
                }
            }
        }

        private void RestorePlaintextValues()
        {
            foreach (var restore in _restoreValues)
            {
                var propertyEntry = restore.Entry.Property(restore.PropertyName);

                propertyEntry.CurrentValue = restore.PlainText;
                propertyEntry.OriginalValue = restore.PlainText;
                propertyEntry.IsModified = false;
            }

            _restoreValues.Clear();
        }

        private static string BuildCombinedContext(object entity, Type entityType, string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                return string.Empty;
            }

            var segments = new string[propertyNames.Length];

            for (int i = 0; i < propertyNames.Length; i++)
            {
                var propInfo = entityType.GetProperty(
                    propertyNames[i],
                    BindingFlags.Public | BindingFlags.Instance
                );

                if (propInfo == null)
                {
                    return string.Empty;
                }

                var value = propInfo.GetValue(entity)?.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                segments[i] = value;
            }

            return string.Join("||", segments);
        }

        private sealed record RestoreValue(
            EntityEntry Entry,
            string PropertyName,
            string PlainText
        );
    }
}