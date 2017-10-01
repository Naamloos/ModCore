namespace ModCore.Database
{
    public class DatabaseContextBuilder
    {
        private string ConnectionString { get; }

        public DatabaseContextBuilder(string cstr)
        {
            this.ConnectionString = cstr;
        }

        public DatabaseContext CreateContext() =>
            new DatabaseContext(this.ConnectionString);
    }
}
