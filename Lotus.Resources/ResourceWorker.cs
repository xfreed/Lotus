using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace Lotus.Resources
{
    public static class ResourceWorker
    {
        private const string ResxFile = @"Settings.resx";

        public static void ChangeResourceText(string key, string value)
        {
            using var reader = new ResourceReader(ResxFile);
            var resx = reader.Cast<DictionaryEntry>().ToList();
            var existingResource = resx.FirstOrDefault(r => r.Key.ToString() == key);
            {
                var modifiedResx = new DictionaryEntry()
                    { Key = existingResource.Key, Value = value };
                resx.Remove(existingResource);  // Remove resource
                resx.Add(modifiedResx);  // and then add new one
            }
        }
    }
}
