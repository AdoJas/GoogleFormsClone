using MongoDB.Driver;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;

namespace GoogleFormsClone.Services;

public class ResponseService
{
    private readonly IMongoCollection<Response> _responses;

    public ResponseService(MongoDbService db)
    {
        _responses = db.GetCollection<Response>("Responses");
    }

    public async Task<List<Response>> GetAllResponsesAsync()
    {
        return await _responses.Find(r => true).ToListAsync();
    }

    public async Task<Response?> GetResponseByIdAsync(string id)
    {
        return await _responses.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Response>> GetByResponseFormIdAsync(string formId)
    {
        return await _responses.Find(r => r.FormId == formId).ToListAsync();
    }

    public async Task<Response> CreateResponseAsync(Response response)
    {
        response.CreatedAt = DateTime.UtcNow;
        response.UpdatedAt = DateTime.UtcNow;
        response.SubmittedAt = DateTime.UtcNow;
        await _responses.InsertOneAsync(response);
        return response;
    }

    public async Task<bool> UpdateResponseAsync(string id, Response updatedResponse)
    {
        updatedResponse.UpdatedAt = DateTime.UtcNow;
        var result = await _responses.ReplaceOneAsync(r => r.Id == id, updatedResponse);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteResponseAsync(string id)
    {
        var result = await _responses.DeleteOneAsync(r => r.Id == id);
        return result.DeletedCount > 0;
    }
}