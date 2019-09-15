using DataFilter.Models;

namespace DataFilter.Services.Interfaces
{
    public interface ISchemeReader
    {
        Field ReadSchema(string path);
    }
}
