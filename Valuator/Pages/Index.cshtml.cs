using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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
            if(_storage.IsValueExist(text))
            {
                _storage.Store(similarityKey, "1");
            }
            else
            {
                _storage.Store(similarityKey, "0");
            }

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(textKey, text);

            _nats.Send("valuator.processing.rank", id);

            return Redirect($"summary?id={id}");
        }
    }
}
