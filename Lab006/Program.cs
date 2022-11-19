using System.Diagnostics.Metrics;
using System.Globalization;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;

namespace HttpClientStatus;

struct Weather
{
    string country;
    string name;
    double temp;
    string description;
    public Weather()
    {
        country = "";
        name = "";
        temp = 0;
        description = "";
    }
    public Weather(string country, string name, double temp, string description)
    {
        this.country = country;
        this.name = name;
        this.temp = temp;
        this.description = description;
    }
    public string Country
    {
        get { return country; }
        set { country = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public double Temp
    {
        get { return temp; }
        set { temp = value; }
    }
    public string Description
    {
        get { return description; }
        set { description = value; }
    }
}

class Program
{
    static async Task Main()
    {
        List<Weather> weatherList = new();
        while (weatherList.Count < 50)
        {
            var rand = new Random();
            double lat = 90 * (rand.NextDouble() * 2 - 1);
            double lon = 180 * (rand.NextDouble() * 2 - 1);
            using var client = new HttpClient();
            string s1 = "https://api.openweathermap.org/data/2.5/weather?lat=";
            string s2 = "&lon=";
            string s3 = "&appid=a764d78375cef095ddfb384cfd507ddb";
            string url = s1 + lat + s2 + lon + s3;
            var content = await client.GetStringAsync(url);

            string country0;
            string country;
            country0 = content[(content.IndexOf("country") + 10)..];
            country = country0[..country0.IndexOf("\"")];
            if (country == "{")
                continue;

            string name0;
            string name;
            name0 = content[(content.IndexOf("name") + 7)..];
            name = name0[..name0.IndexOf("\"")];
            if ((name == "Globe") || (name == ""))
                continue;

            string temp0;
            string temp1;
            temp0 = content[(content.IndexOf("temp") + 6)..];
            temp1 = temp0[..temp0.IndexOf(",")];
            temp1 = temp1.Replace(".", ",");
            double temp = Convert.ToDouble(temp1);

            string description0;
            string description;
            description0 = content[(content.IndexOf("description") + 14)..];
            description = description0[..description0.IndexOf("\"")];

            weatherList.Add(new Weather(country, name, temp, description));
        }

        foreach (Weather weather in weatherList)
            Console.WriteLine
                ("Country: " + weather.Country + "; Name: " + weather.Name 
                + "; Temp : " + weather.Temp + "; Discription: " + weather.Description);
        //1)

        Console.WriteLine("\n1)");
        var max = weatherList.Max(m => m.Temp);
        var min = weatherList.Min(m => m.Temp);

        var res1max = from weather in weatherList
                      where weather.Temp == max
                      select weather;
        foreach (Weather weather in res1max)
            Console.WriteLine("with max temp: " + weather.Country);

        var res1min = from weather in weatherList
                      where weather.Temp == min
                      select weather;
        foreach (Weather weather in res1min)
            Console.WriteLine("with min temp: " + weather.Country);

        //2)

        Console.WriteLine("\n2)");
        var avg = weatherList.Average(a => a.Temp);
        Console.WriteLine("average temp: " + avg.ToString());

        //3)

        Console.WriteLine("\n3)");
        var res = weatherList.GroupBy(g => g.Country).Select(g=>g.First()).ToList();
        Console.WriteLine(" number of countries: " + res.Count);

        //4)

        Console.WriteLine("\n4)");
        Console.WriteLine("Clear sky: " + (weatherList.FirstOrDefault(f => f.Description == "clear sky")).Country + " " +
            (weatherList.FirstOrDefault(f => f.Description == "clear sky")).Name);
        Console.WriteLine("Rain: " + (weatherList.FirstOrDefault(f => f.Description == "rain")).Country + " " +
            (weatherList.FirstOrDefault(f => f.Description == "rain")).Name);
        Console.WriteLine("Few clouds: " + (weatherList.FirstOrDefault(f => f.Description == "few clouds")).Country + " " +
            (weatherList.FirstOrDefault(f => f.Description == "few clouds")).Name);
    }
}