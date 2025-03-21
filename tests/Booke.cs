using Newtonsoft.Json;
using RestSharp;

namespace booker;

public class BookerList
{

    public const string URL = "https://restful-booker.herokuapp.com";
    private RestClient client;
    private RestRequest request;
    private static String token;
    private static int bookingid;

    public static IEnumerable<TestCaseData> getTestData()
    {
        String caminhoMassa = @"C:\iterasys\nunitbooke\fixtures\booking.csv";

        using var reader = new StreamReader(caminhoMassa);

        // Pula a primeira linha com os cabeçahos
        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(",");

            yield return new TestCaseData(values[0], values[1], int.Parse(values[2]), bool.Parse(values[3]), values[4], values[5], values[6]);
        }

    }


    [SetUp]
    public void Setup()
    {
        client = new RestClient(URL);
        
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
    }

    [Test, Order(1)]
    public void CreateToken()
    {
        request = new RestRequest("auth", Method.Post);
        string jsonBody = File.ReadAllText(@"C:\iterasys\nunitbooke\fixtures\token.json");
        request.AddBody(jsonBody);

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        token = responseBody.token;
    }

    [Test, Order(2)]
    public void CreateBooking()
    {
        request = new RestRequest("booking", Method.Post);
        string jsonBody = File.ReadAllText(@"C:\iterasys\nunitbooke\fixtures\booking.json");
        request.AddBody(jsonBody);
        request.AddHeader("Accept", "application/json");

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        bookingid = responseBody.bookingid;
        Assert.That((string) responseBody.booking.firstname, Is.EqualTo("Guilherme"));
        Assert.That((string) responseBody.booking.lastname, Is.EqualTo("Vilela"));
        Assert.That((int) responseBody.booking.totalprice, Is.EqualTo(251));
        Assert.That((bool) responseBody.booking.depositpaid, Is.EqualTo(true));
        Assert.That((string) responseBody.booking.bookingdates.checkin, Is.EqualTo("2025-03-20"));

    }

    [Test, Order(3)]
    public void GetBookingIds()
    {
        request = new RestRequest("booking", Method.Get);
        request.AddParameter("firstname", "Guilherme");

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        Assert.That((int) responseBody[0].bookingid, Is.EqualTo(bookingid));
    }

    [Test, Order(4)]
    public void GetBookingId()
    {
        request = new RestRequest($"booking/{bookingid}", Method.Get);
        request.AddHeader("Accept", "application/json");

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        Assert.That((string) responseBody.firstname, Is.EqualTo("Guilherme"));
        Assert.That((string) responseBody.lastname, Is.EqualTo("Vilela"));
        Assert.That((int) responseBody.totalprice, Is.EqualTo(251));
        Assert.That((bool) responseBody.depositpaid, Is.EqualTo(true));
        Assert.That((string) responseBody.bookingdates.checkin, Is.EqualTo("2025-03-20"));

    }

    [Test, Order(5)]
    public void UpdateBooking()
    {
        request = new RestRequest($"booking/{bookingid}", Method.Put);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        string jsonBody = File.ReadAllText(@"C:\iterasys\nunitbooke\fixtures\bookingUpDate.json");
        request.AddBody(jsonBody);

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        Assert.That((string) responseBody.firstname, Is.EqualTo("Guilherme"));
        Assert.That((string) responseBody.lastname, Is.EqualTo("Stein"));
        Assert.That((int) responseBody.totalprice, Is.EqualTo(250));
        Assert.That((bool) responseBody.depositpaid, Is.EqualTo(true));
        Assert.That((string) responseBody.additionalneeds, Is.EqualTo("Almoço"));

    }

    [Test, Order(6)]
    public void PartialUpdateBooking()
    {
        request = new RestRequest($"booking/{bookingid}", Method.Patch);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        string jsonBody = File.ReadAllText(@"C:\iterasys\nunitbooke\fixtures\bookinPatch.json");
        request.AddBody(jsonBody);

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        Assert.That((string) responseBody.firstname, Is.EqualTo("Guilherme"));
        Assert.That((string) responseBody.lastname, Is.EqualTo("Stein"));
        Assert.That((int) responseBody.totalprice, Is.EqualTo(302));
        Assert.That((bool) responseBody.depositpaid, Is.EqualTo(true));
        Assert.That((string) responseBody.bookingdates.checkin, Is.EqualTo("2025-03-20"));
        Assert.That((string) responseBody.bookingdates.checkout, Is.EqualTo("2025-03-25"));
        Assert.That((string) responseBody.additionalneeds, Is.EqualTo("Almoço"));

    }

    [Test, Order(7)]
    public void DeleteBooking()
    {
        request = new RestRequest($"booking/{bookingid}", Method.Delete);
        
        request.AddHeader("Cookie", $"token={token}");
        Console.WriteLine(token);

        var response = client.Execute(request);

        Assert.That((int) response.StatusCode, Is.EqualTo(201));
    }

    [TestCaseSource("getTestData", new object[] { }), Order(8)]
    public void PostBookingTestDDT(string firstname, string lastname, int totalprice, bool depositpaid, string checkin, string checkout, string additionalneeds)
    {
        
        var request = new RestRequest("booking", Method.Post);

        BookingModel bookingModel = new BookingModel();
        bookingModel.firstname = firstname;
        bookingModel.lastname = lastname;
        bookingModel.totalprice = totalprice;
        bookingModel.depositpaid = depositpaid;
        bookingModel.bookingdates = new BookingDates(checkin, checkout);
        bookingModel.additionalneeds = additionalneeds;

        string jsonString = JsonConvert.SerializeObject(bookingModel, Formatting.Indented);
        Console.WriteLine(jsonString);
        request.AddBody(jsonString);
        request.AddHeader("Accept", "application/json");

        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Console.WriteLine(responseBody);

        Assert.That((int) response.StatusCode, Is.EqualTo(200));
        Assert.That((string) responseBody.booking.firstname, Is.EqualTo(firstname));
        Assert.That((string) responseBody.booking.lastname, Is.EqualTo(lastname));
        Assert.That((int) responseBody.booking.totalprice, Is.EqualTo(totalprice));
        Assert.That((bool) responseBody.booking.depositpaid, Is.EqualTo(depositpaid));
        Assert.That((string) responseBody.booking.bookingdates.checkin, Is.EqualTo(checkin));
        Assert.That((string) responseBody.booking.bookingdates.checkout, Is.EqualTo(checkout));
        Assert.That((string) responseBody.booking.additionalneeds, Is.EqualTo(additionalneeds));
    }


}