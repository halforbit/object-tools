using Halforbit.ObjectTools.InvariantExtraction.Implementation;
using System;
using System.Linq.Expressions;

namespace Halforbit.ObjectTools.InvariantExtraction.Extensions
{
    public static class ObjectBuildExtensions
    {
        public static TObject Mutate<TObject>(
            this TObject obj,
            Expression<Func<TObject, bool>> expression)
        {
            Expression<Func<TObject, bool>> invariant;

            return new InvariantExtractor().ExtractInvariants(
                expression,
                out invariant,
                obj);
        }
    }
}
