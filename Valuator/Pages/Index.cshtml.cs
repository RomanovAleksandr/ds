using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Library;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        NatsMessageBroker _natsMessageBroker;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, NatsMessageBroker natsMessageBroker)
        {
            _logger = logger;
            _storage = storage;
            _natsMessageBroker = natsMessageBroker;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);
            if(text == null)
            {
                return Redirect($"index");
            }

            string id = Guid.NewGuid().ToString();

            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            int similarity = GetSimilarity(text);
            _storage.Store(Constants.SIMILARITY + id, similarity.ToString());
            
            PublishSimilarityCalculatedEvent(Constants.SIMILARITY + id, similarity);

            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(Constants.TEXT + id, text);

            _natsMessageBroker.Send("valuator.processing.rank", id);

            return Redirect($"summary?id={id}");
        }

        private int GetSimilarity(string text)
        {
            if(_storage.IsValueExist(text))
            {
                return 1;
            }
            return 0;
        }
        private void PublishSimilarityCalculatedEvent(string id, int similarity)
        {
            Similarity textSmilarity = new Similarity(id, similarity);
            string similarityJson = JsonSerializer.Serialize(textSmilarity);
            _natsMessageBroker.Send("valuator.similarity_calculated", similarityJson);
        }
    }
}
