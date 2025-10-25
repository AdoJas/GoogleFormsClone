using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using GoogleFormsClone.DTOs.Response;
using GoogleFormsClone.Models;

namespace GoogleFormsClone.Services
{
    public class ResponseService
    {
        private readonly IMongoCollection<Response> _responses;

        public ResponseService(MongoDbService db)
        {
            _responses = db.GetCollection<Response>("Responses");
        }

        public async Task<List<Response>> GetAllResponsesAsync() =>
            await _responses.Find(r => true).ToListAsync();

        public async Task<Response?> GetResponseByIdAsync(string id) =>
            await _responses.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task<List<Response>> GetByResponseFormIdAsync(string formId) =>
            await _responses.Find(r => r.FormId == formId).ToListAsync();

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

        public async Task<bool> HasUserSubmittedAsync(string formId, string userId)
        {
            return await _responses.Find(r => r.FormId == formId && r.SubmittedBy == userId)
                .Limit(1)
                .AnyAsync();
        }

        // -----------------------------
        // Aggregated Form Statistics
        // -----------------------------
        public async Task<List<FormStatsDto>> GetFormStatsAsync(string formId)
        {
            var matchStage = new BsonDocument("$match", new BsonDocument("formId", new ObjectId(formId)));

            var pipeline = new[]
            {
                matchStage,
                new BsonDocument("$unwind", "$answers"),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$answers.questionId" },
                    { "textAnswers", new BsonDocument("$push", "$answers.answerText") },
                    { "selectedOptions", new BsonDocument("$push", "$answers.selectedOptions") },
                    { "scaleValues", new BsonDocument("$push", "$answers.linearScaleValue") },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var result = await _responses.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var stats = new List<FormStatsDto>();

            foreach (var doc in result)
            {
                var qid = doc["_id"].IsObjectId ? doc["_id"].AsObjectId.ToString() : doc["_id"].ToString();

                var textAnswers = doc.Contains("textAnswers")
                    ? doc["textAnswers"].AsBsonArray
                        .Where(x => x != BsonNull.Value && x.IsString)
                        .Select(x => x.AsString)
                        .ToList()
                    : new List<string>();

                var selectedOpts = doc.Contains("selectedOptions")
                    ? doc["selectedOptions"].AsBsonArray
                        .SelectMany(x => x.IsBsonArray
                            ? x.AsBsonArray.Where(y => y != BsonNull.Value && y.IsString)
                                .Select(y => y.AsString)
                            : Enumerable.Empty<string>())
                        .GroupBy(x => x)
                        .ToDictionary(g => g.Key, g => g.Count())
                    : new Dictionary<string, int>();

                var scaleCounts = doc.Contains("scaleValues")
                    ? doc["scaleValues"].AsBsonArray
                        .Where(x => x != BsonNull.Value && x.IsInt32)
                        .GroupBy(x => x.AsInt32)
                        .ToDictionary(g => g.Key, g => g.Count())
                    : new Dictionary<int, int>();

                var optionPercents = new Dictionary<string, double>();
                if (selectedOpts.Count > 0)
                {
                    var total = selectedOpts.Values.Sum();
                    foreach (var kv in selectedOpts)
                    {
                        optionPercents[kv.Key] = total > 0
                            ? Math.Round((double)kv.Value / total * 100, 2)
                            : 0;
                    }
                }

                stats.Add(new FormStatsDto
                {
                    QuestionId = qid,
                    TotalResponses = doc.GetValue("count", 0).ToInt32(),
                    TextAnswers = textAnswers,
                    OptionCounts = selectedOpts,
                    ScaleCounts = scaleCounts,
                    OptionPercents = optionPercents
                });
            }

            return stats;
        }
    }
}
