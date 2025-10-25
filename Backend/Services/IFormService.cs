using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.Models;

namespace GoogleFormsClone.Services
{
    public interface IFormService
    {
        Task<List<Form>> GetAllFormsAsync();
        Task<Form?> GetFormAsync(string id);
        Task<Form> CreateFormAsync(Form form, string userId);
        Task<Form?> UpdateFormAsync(string id, Form updatedForm);
        Task<bool> DeleteFormCascadeAsync(string id); 
        Task<FormWithFilesDto?> GetFormWithFilesAsync(string id);
        Task<List<Form>> GetUserFormsAsync(string userId);

        Task<List<DashboardFormDto>> GetDashboardFormsAsync(string userId);
    }
}