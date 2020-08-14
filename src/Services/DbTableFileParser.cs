using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using WebUI.Services.Interfaces;

namespace WebUI.Services
{
    public sealed class DbTableFileParser : IFileParser
    {
        public string[] GetParsedValues(string path, params string[] ignoredValues)
        {
            if(string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var lines = File.ReadAllLines(path)
                            .Skip(2)
                            .SkipLast(2)
                            .Select(s => s.Trim(' '))
                            .Except(ignoredValues)
                            .ToArray();

            if(lines == null)
                throw new DbNotFoundException("Databases not found");

            return lines;
        }
    }
}