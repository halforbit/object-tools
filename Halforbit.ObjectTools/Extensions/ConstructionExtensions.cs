using System;

namespace Halforbit.ObjectTools.Extensions
{
    public static class ConstructionExtensions
    {
        public static Guid OrNewIfEmpty(this Guid guid) => guid == default(Guid) ? Guid.NewGuid() : guid;
    }
}
