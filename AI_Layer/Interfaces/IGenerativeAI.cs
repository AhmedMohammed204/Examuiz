using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Layer.Interfaces
{
    public interface IGenerativeAI
    {
        Task<string?> TextGenerate(string PrompotText);
        Task<string?> TextGenerate(string PromptText, IList<string>? images = null);
    }
}
