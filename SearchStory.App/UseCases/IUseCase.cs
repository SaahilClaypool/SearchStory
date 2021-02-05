using System.Threading.Tasks;

namespace SearchStory.App.UseCases
{
    public interface IUseCase<Command, Response>
    {
        Task<Response> Exectute(Command input);
    }
}