using MinistryPlatform.Models;

namespace MinistryPlatform.Interfaces
{
    public interface IProgramRepository
    {
        MpProgram GetProgramByName(string programName);
        MpProgram GetProgramById(int programId);
    }
}
