using System;
using System.ComponentModel;

namespace ModCore.Database
{
    public class DatabaseContextBuilder
    {
        private string ConnectionString { get; }

        public DatabaseContextBuilder(string cstr)
        {
            this.ConnectionString = cstr;
        }

        public DatabaseContext CreateContext()
        {
            try
            {
                return new DatabaseContext(this.ConnectionString);
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
