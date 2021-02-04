using System.Threading.Tasks;

namespace SearchStory.App.UseCases
{
    public interface IUseCase<Command, Response>
    {
        Response Exectute(Command input);
        Task<Response> ExectuteAsync(Command input) => Task.FromResult(Exectute(input));
    }
}