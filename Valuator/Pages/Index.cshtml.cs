using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        NatsMessageBroker _nats;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
            _nats = new NatsMessageBroker();
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string rankKey = "RANK-" + id;

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            int similarity = GetSimilarity(text);
            _storage.Store(similarityKey, similarity.ToString());
            PublishSimilarityCalculatedEvent(similarityKey, similarity);

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(textKey, text);

            _nats.Send("valuator.processing.rank", id);

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
            _nats.Send("valuator.similarity_calculated", similarityJson);
        }
    }
}
