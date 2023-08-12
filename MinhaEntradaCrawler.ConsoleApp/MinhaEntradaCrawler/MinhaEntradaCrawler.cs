using AngleSharp.Dom;
using AngleSharp;
using Microsoft.VisualBasic;
using System.Globalization;

namespace MinhaEntrada
{
	public class MinhaEntradaCrawler
	{
		const string MINHAENTRADA_DOMAIN = "https://minhaentrada.com.br";
        public string OrganizerSchedule { get; private set; }

		/// <summary>
		/// A crawler class for extracting event information from a 'https://minhaentrada.com.br'
		/// </summary>
		/// <param name="eventOrganizer">https://minhaentrada.com.br/{eventOrganizer}</param>
		public MinhaEntradaCrawler(string eventOrganizer)
        {
			OrganizerSchedule = $"{MINHAENTRADA_DOMAIN}/{eventOrganizer}/agenda-geral";
		}
        public async Task<List<Event>> CrawlEventsAsync(DateTime? startDate = null, DateTime? endDate = null)
		{
			string scheduleUrl = OrganizerSchedule;
			if (startDate >= DateTime.Today)
			{
				endDate ??= startDate;
				scheduleUrl = $"{OrganizerSchedule}?data-inicio={startDate:yyyy-MM-dd}&data-fim={endDate:yyyy-MM-dd}";
			}
			var document = await GetHtmlDocumentAsync(scheduleUrl);
			return ExtractEvents(document);
		}

		private static async Task<IDocument> GetHtmlDocumentAsync(string scheduleUrl)
		{
			var config = Configuration.Default.WithDefaultLoader();
			var context = BrowsingContext.New(config);

			using (var httpClient = new HttpClient())
			{
				var html = await httpClient.GetStringAsync(scheduleUrl);
				return await context.OpenAsync(req => req.Content(html));
			}
		}

		private static List<Event> ExtractEvents(IDocument document)
		{
			var events = document.QuerySelectorAll(".card-agenda-geral");

			List<Event> eventList = new();

			foreach (var eventNode in events)
			{
				var title = eventNode.QuerySelector("h4").TextContent.Trim();
				var dateTimeString = eventNode.QuerySelector(".zmdi-calendar-check").Parent.TextContent.Trim().Substring(6).Trim();
				var location = eventNode.QuerySelector(".zmdi-pin").Parent.TextContent.Trim();
				var imageUrl = eventNode.QuerySelector("img")?.GetAttribute("src");
				var eventLink = eventNode.QuerySelector("a.color-font-black")?.GetAttribute("href");
				DateTime.TryParseExact(dateTimeString, "dd/MM/yyyy - HH:mm'h'", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);

				var eventData = new Event
				{
					Title = title,
					DateTime = dateTime,
					Location = location,
					ImageUrl = imageUrl,
					EventLink = eventLink
				};

				eventList.Add(eventData);
			}

			return eventList;
		}		
	}
}
