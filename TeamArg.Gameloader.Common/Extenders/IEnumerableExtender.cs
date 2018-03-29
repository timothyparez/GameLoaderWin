using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TeamArg.GameLoader.Common.Extenders
{
    public static class IEnumerableExtender
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source) => new ObservableCollection<T>(source);
    }
}
