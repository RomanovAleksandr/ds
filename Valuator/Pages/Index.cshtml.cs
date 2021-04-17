using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Library;
using System.Collections.Generic;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly NatsMessageBroker _natsMessageBroker;
        public readonly Dictionary<string, string> countries;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, NatsMessageBroker natsMessageBroker)
        {
            _logger = logger;
            _storage = storage;
            _natsMessageBroker = natsMessageBroker;
            countries = Countries.countriesDict;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text, string shardId)
        {
            _logger.LogDebug(text);
            if(text == null)
            {
                return Redirect($"index");
            }

            string id = Guid.NewGuid().ToString();
            _logger.LogDebug("LOOKUP: {0}, {1}", id, shardId);
            _storage.StoreShardKey(id, shardId);

            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            int similarity = GetSimilarity(text);
            _storage.Store(id, Constants.SIMILARITY + id, similarity.ToString());
            
            PublishSimilarityCalculatedEvent(Constants.SIMILARITY + id, similarity);

            //TODO: сохранить в БД text по ключу textKey
            _storage.StoreText(id, Constants.TEXT + id, text);

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
