using GoogleFormsClone.Models;

namespace GoogleFormsClone.Services
{
    public interface IFormService
    {
        Task<List<Form>> GetAllFormsAsync();
        Task<Form?> GetFormAsync(string id);
        Task<List<Form>> GetUserFormsAsync(string userId);
        Task<Form> CreateFormAsync(Form form, string userId);
        Task<Form?> UpdateFormAsync(string id, Form form);
        Task<bool> DeleteFormAsync(string id);
    }
}