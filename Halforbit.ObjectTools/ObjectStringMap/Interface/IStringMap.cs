using System.Collections.Generic;

namespace Halforbit.ObjectTools.ObjectStringMap.Interface
{
    public interface IStringMap<TObject>
    {
        string Source { get; }

        TObject Map(string str);

        string Map(
            TObject obj,
            bool allowPartialMap = false);

        string Map(
            IReadOnlyDictionary<string, object> memberValues,
            bool allowPartialMap = false);

        bool IsMatch(string str);
    }
}
