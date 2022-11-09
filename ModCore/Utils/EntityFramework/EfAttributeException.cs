using System;

namespace ModCore.Utils.EntityFramework
{
    public sealed class EfAttributeException : Exception
    {
        public EfAttributeException(string message) : base(message)
        {
        }
    }
}