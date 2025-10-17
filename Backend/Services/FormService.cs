using MongoDB.Driver;
using GoogleFormsClone.Models;
using MongoDB.Bson;

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

            EnsureQuestionIds(form);

            await _forms.InsertOneAsync(form);
            return form;
        }

        public async Task<Form?> UpdateFormAsync(string id, Form updatedForm)
        {
            updatedForm.UpdatedAt = DateTime.UtcNow;

            EnsureQuestionIds(updatedForm);

            var result = await _forms.ReplaceOneAsync(f => f.Id == id, updatedForm);
            return result.ModifiedCount > 0 ? updatedForm : null;
        }

        public async Task<bool> DeleteFormAsync(string id)
        {
            var result = await _forms.DeleteOneAsync(f => f.Id == id);
            return result.DeletedCount > 0;
        }
        private void EnsureQuestionIds(Form form)
        {
            if (form.Questions == null || !form.Questions.Any())
                return;

            // 1️⃣ Build a mapping of temporary/local IDs → new ObjectIds
            var idMap = new Dictionary<string, string>();

            foreach (var question in form.Questions)
            {
                if (string.IsNullOrEmpty(question.Id) || !ObjectId.TryParse(question.Id, out _))
                {
                    var newId = ObjectId.GenerateNewId().ToString();
                    if (!string.IsNullOrEmpty(question.Id))
                        idMap[question.Id] = newId; // remember temp ID
                    question.Id = newId;
                }
            }

            // 2️⃣ Assign ObjectIds to options
            foreach (var question in form.Questions)
            {
                if (question.Options != null)
                {
                    foreach (var option in question.Options)
                    {
                        if (string.IsNullOrEmpty(option.Id) || !ObjectId.TryParse(option.Id, out _))
                            option.Id = ObjectId.GenerateNewId().ToString();
                    }
                }
            }

            // 3️⃣ Resolve logic dependencies using the map
            foreach (var question in form.Questions)
            {
                if (question.Logic != null && !string.IsNullOrEmpty(question.Logic.DependsOn))
                {
                    var dependsOn = question.Logic.DependsOn;
                    if (idMap.ContainsKey(dependsOn))
                    {
                        // Replace temp ID with the new ObjectId
                        question.Logic.DependsOn = idMap[dependsOn];
                    }
                    else if (!ObjectId.TryParse(dependsOn, out _))
                    {
                        // Fallback: assign a new ObjectId only if no mapping found
                        question.Logic.DependsOn = ObjectId.GenerateNewId().ToString();
                    }
                }
            }
        }
    }
}