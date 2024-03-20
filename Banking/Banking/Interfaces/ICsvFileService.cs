using System.Collections.ObjectModel;

namespace Banking.Interfaces
{
    public interface ICsvFileService
    {
        ReadOnlyCollection<T> Read<T>(string path) where T : class;
    }
}
