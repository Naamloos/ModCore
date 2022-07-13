using System;

namespace ModCore.Logic.EntityFramework
{
    public sealed class EfAttributeException : Exception
    {
        public EfAttributeException(string message) : base(message)
        {
        }
    }
}