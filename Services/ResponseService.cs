using GoogleFormsClone.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GoogleFormsClone.Services;

public class ResponseService
{
    private readonly IMongoCollection<Response> _responses;
    private readonly IMongoCollection<Form> _forms;

    public ResponseService(MongoDbService mongoDbService)
    {
        _responses = mongoDbService.GetCollection<Response>("responses");
        _forms = mongoDbService.GetCollection<Form>("forms");
    }

    public async Task<Response> CreateResponseAsync(string formId, string? submittedBy, List<Answer> answers, ResponseMetadata metadata)
    {
        var form = await _forms.Find(f => f.Id == formId).FirstOrDefaultAsync();
        
        if (form == null)
            throw new Exception("Form not found");

        foreach (var answer in answers)
        {
            var question = form.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question != null)
            {
                answer.QuestionSnapshot = new QuestionSnapshot
                {
                    QuestionText = question.QuestionText,
                    QuestionType = question.Type,
                    Options = question.Options
                };
            }
        }

        var response = new Response
        {
            Id = ObjectId.GenerateNewId().ToString(),
            FormId = formId,
            SubmittedBy = submittedBy,
            Answers = answers,
            Metadata = metadata,
            SubmittedAt = DateTime.UtcNow
        };

        await _responses.InsertOneAsync(response);
        return response;
    }

    public async Task<IEnumerable<Response>> GetResponsesByFormIdAsync(string formId)
    {
        return await _responses.Find(r => r.FormId == formId)
            .SortByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Response?> GetResponseByIdAsync(string id)
    {
        return await _responses.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task DeleteResponseAsync(string id)
    {
        await _responses.DeleteOneAsync(r => r.Id == id);
    }

    public async Task<long> GetResponseCountForFormAsync(string formId)
    {
        return await _responses.CountDocumentsAsync(r => r.FormId == formId);
    }
}
