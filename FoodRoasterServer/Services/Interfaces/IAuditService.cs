
namespace FoodRoasterServer.Services
{
    public interface IAuditService
    {
        void Track(string message);
    }
}
