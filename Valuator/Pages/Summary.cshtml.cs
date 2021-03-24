using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading;
using Library;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            Similarity = Convert.ToDouble(_storage.Load(Constants.SIMILARITY + id));
            var rankText = _storage.Load(Constants.RANK + id);
            for (int retryCount = 0; retryCount < 20; retryCount++)
            {
                if (rankText != null)
                {
                    Rank = Convert.ToDouble(rankText);
                    return;
                }
                Thread.Sleep(100);
                rankText = _storage.Load(Constants.RANK + id);
            }

            //TODO: проинициализировать свойства Rank и Similarity сохранёнными в БД значениями
            Rank = Convert.ToDouble(_storage.Load(Constants.RANK + id));
            Similarity = Convert.ToDouble(_storage.Load(Constants.SIMILARITY + id));
        }
    }
}
