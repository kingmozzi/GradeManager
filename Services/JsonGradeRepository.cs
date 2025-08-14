using GradeManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace GradeManager.Services
{
    public class JsonGradeRepository : IGradeRepository
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<GradeFile> LoadAsync(string path)
        {
            if (!File.Exists(path)) return new GradeFile();
            await using var fs = File.OpenRead(path);
            var data = await JsonSerializer.DeserializeAsync<GradeFile>(fs, Options);
            return data ?? new GradeFile();
        }

        public async Task SaveAsync(string path, GradeFile data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, data, Options);
        }
    }
}
