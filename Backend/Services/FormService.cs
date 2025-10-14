using MongoDB.Driver;
using GoogleFormsClone.Models;

namespace GoogleFormsClone.Services
{
    public class FormService : IFormService
    {
        private readonly IMongoCollection<Form> _forms;

        public FormService(MongoDbService db)
        {
            _forms = db.GetCollection<Form>("Forms");
        }

        public async Task<List<Form>> GetAllFormsAsync()
        {
            return await _forms.Find(f => true).ToListAsync();
        }

        public async Task<Form?> GetFormAsync(string id)
        {
            return await _forms.Find(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Form>> GetUserFormsAsync(string userId)
        {
            return await _forms.Find(f => f.CreatedBy == userId).ToListAsync();
        }

        public async Task<Form> CreateFormAsync(Form form, string userId)
        {
            form.CreatedBy = userId;
            form.CreatedAt = DateTime.UtcNow;
            form.UpdatedAt = DateTime.UtcNow;
            await _forms.InsertOneAsync(form);
            return form;
        }

        public async Task<Form?> UpdateFormAsync(string id, Form updatedForm)
        {
            updatedForm.UpdatedAt = DateTime.UtcNow;
            var result = await _forms.ReplaceOneAsync(f => f.Id == id, updatedForm);
            return result.ModifiedCount > 0 ? updatedForm : null;
        }

        public async Task<bool> DeleteFormAsync(string id)
        {
            var result = await _forms.DeleteOneAsync(f => f.Id == id);
            return result.DeletedCount > 0;
        }
    }
}