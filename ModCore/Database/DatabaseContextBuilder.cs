using System;
using ModCore.Entities;

namespace ModCore.Database
{
    public class DatabaseContextBuilder
    {
        private string ConnectionString { get; }
        private DatabaseProvider Provider { get; }

        public DatabaseContextBuilder(DatabaseProvider provider, string cstr)
        {
            this.Provider = provider;
            this.ConnectionString = cstr;
        }

        public DatabaseContext CreateContext()
        {
            try
            {
                return new DatabaseContext(this.Provider, this.ConnectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during database initialization:");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
