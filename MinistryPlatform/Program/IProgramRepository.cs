using System.Threading.Tasks;
using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IProgramRepository
    {
        Task<MpProgram> GetProgramByName(string programName);
    }
}
