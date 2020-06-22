using Microsoft.AspNetCore.Http;

namespace PortfolioWeb.Services
{
    public interface IPhotoService
    {
        string AddPhoto(IFormFile file);
        void DeletePhoto(string fileName);
    }
}