using MinhaEntrada;

var organizer = "PoaComedyClub";
var minhaEntradaCrawler = new MinhaEntradaCrawler(organizer);

DateTime startDate = DateTime.Today;
DateTime endDate = DateTime.Today.AddDays(7);

var events = await minhaEntradaCrawler.CrawlEventsAsync(startDate, endDate);

foreach (var eventData in events)
{
	Console.WriteLine($"Title: {eventData.Title}");
	Console.WriteLine($"Date/Time: {eventData.DateTime}");
	Console.WriteLine($"Location: {eventData.Location}");
	Console.WriteLine($"Image URL: {eventData.ImageUrl}");
	Console.WriteLine($"Event Link: {eventData.EventLink}");
	Console.WriteLine();
}