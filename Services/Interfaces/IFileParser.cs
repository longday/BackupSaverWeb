using System.Collections.Generic;

namespace WebUI.Services.Interfaces
{
    public interface IFileParser
    {
        string[] GetParsedValues(string path, params string[] ignoredValues);
    }
}
