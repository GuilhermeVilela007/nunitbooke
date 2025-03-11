//blibliotecas
using Newtonsoft.Json; //dependencia para o JsonConvert
using RestSharp;

// namespace
namespace resesvas;

// classe.

public class Reservas
{
 private string BASE_URL = "https://restful-booker.herokuapp.com/";

[Test, Order(1)]
  public void PostBookeTest()
 {
    // configura
   var client = new RestClient(BASE_URL);
   var request = new RestRequest("booking", Method.Post);
   string jsonBody = File.ReadAllText (@"C:\iterasys\nunitbooke\fixtures\booking.json");

   //executa
   request.AddBody(jsonBody);
   var response = client.Execute(request);

   //valida
   var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
   Console.WriteLine(responseBody);
   

   Assert.That((int)response.StatusCode, Is.EqualTo(200));

   String name = responseBody.firstname.ToString();
   Assert.That(name, Is.EqualTo("Guilherme"));
 }



}