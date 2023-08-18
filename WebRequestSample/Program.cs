// See https://aka.ms/new-console-template for more information
using System.Net;

Console.WriteLine("Hello, World!");

WebRequest request = WebRequest.Create("http://you.com");
request.Method = "POST";
Stream reqStream = request.GetRequestStream();

using (StreamWriter sw = new StreamWriter(reqStream))
{
    sw.Write("Our test data query");
}
var responseTask = request.GetResponseAsync();

var webResponse = await responseTask;

using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
{
    var result = await sr.ReadToEndAsync();
}

