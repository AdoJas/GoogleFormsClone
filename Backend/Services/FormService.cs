using MongoDB.Driver;
using GoogleFormsClone.Models;
using MongoDB.Bson;
using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.DTOs.File;
using GoogleFormsClone.Mappers;

namespace GoogleFormsClone.Services;

public class FormService : IFormService
{
    private readonly IMongoCollection<Form> _forms;
    private readonly IMongoCollection<Response> _responses;
    private readonly FileService _fileService;

    public FormService(MongoDbService db, FileService fileService)
    {
        _forms = db.GetCollection<Form>("Forms");
        _fileService = fileService;
        _responses = db.GetCollection<Response>("Responses");
    }

    public async Task<List<Form>> GetAllFormsAsync() =>
        await _forms.Find(_ => true).ToListAsync();

    public async Task<Form?> GetFormAsync(string id) =>
        await _forms.Find(f => f.Id == id).FirstOrDefaultAsync();

    public async Task<List<Form>> GetUserFormsAsync(string userId) =>
        await _forms.Find(f => f.CreatedBy == userId).ToListAsync();

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
        var existing = await _forms.Find(f => f.Id == id).FirstOrDefaultAsync();
        if (existing == null) return null;

        updatedForm.CreatedBy = existing.CreatedBy;
        updatedForm.CreatedAt = existing.CreatedAt;
        updatedForm.UpdatedAt = DateTime.UtcNow;

        EnsureQuestionIds(updatedForm);

        var result = await _forms.ReplaceOneAsync(f => f.Id == id, updatedForm);
        return result.ModifiedCount > 0 ? updatedForm : null;
    }

    public async Task<bool> DeleteFormCascadeAsync(string id)
    {
        var form = await _forms.Find(f => f.Id == id).FirstOrDefaultAsync();
        if (form == null)
            return false;

        await _responses.DeleteManyAsync(r => r.FormId == id);

        await _fileService.DeleteFilesByFormAsync(form);

        var result = await _forms.DeleteOneAsync(f => f.Id == id);

        return result.DeletedCount > 0;
    }
    

    private void EnsureQuestionIds(Form form)
    {
        if (form.Questions == null || form.Questions.Count == 0)
            return;

        var idMap = new Dictionary<string, string>();

        foreach (var question in form.Questions)
        {
            if (string.IsNullOrEmpty(question.Id))
            {
                var newId = ObjectId.GenerateNewId().ToString();
                question.Id = newId;
            }

        }

        foreach (var q in form.Questions)
        {
            if (q.Options == null) continue;

            foreach (var opt in q.Options)
            {
                if (string.IsNullOrEmpty(opt.Id))
                    opt.Id = ObjectId.GenerateNewId().ToString();
            }
        }

        foreach (var q in form.Questions)
        {
            if (q.Logic == null || string.IsNullOrEmpty(q.Logic.DependsOn))
                continue;

            if (idMap.TryGetValue(q.Logic.DependsOn, out var mappedId))
                q.Logic.DependsOn = mappedId;
        }
    }

    public async Task<FormWithFilesDto?> GetFormWithFilesAsync(string id)
    {
        var form = await _forms.Find(f => f.Id == id).FirstOrDefaultAsync();
        if (form == null)
            return null;

        var allFiles = await _fileService.GetFilesByFormAndQuestionsAsync(form);
        return FormMapper.ToWithFilesDto(form, allFiles);
    }

    public async Task<List<DashboardFormDto>> GetDashboardFormsAsync(string userId)
    {
        var results = await _forms.Aggregate()
            .Match(f => f.CreatedBy == userId)
            .Lookup<Form, Response, FormWithResponses>(
                _responses,
                localField: f => f.Id,
                foreignField: r => r.FormId,
                @as: x => x.Responses)
            .Project(f => new DashboardFormDto
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                UpdatedAt = f.UpdatedAt,
                IsPublic = f.AccessControl.IsPublic,
                ResponseCount = f.Responses.Count
            })
            .ToListAsync();

        return results;
    }
}

public class FormWithResponses : Form
{
    public List<Response> Responses { get; set; } = new();
}
