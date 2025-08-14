using GradeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeManager.Services
{
    public interface IGradeRepository
    {
        Task SaveAsync(string path, GradeFile data);
        Task<GradeFile> LoadAsync(string path);
    }
}
