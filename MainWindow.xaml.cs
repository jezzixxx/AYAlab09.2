using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace lab09._2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetStrings();
        }
        private void AddText(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = 12;
            this.Content = textBlock;
        }
        public struct City
        {
            public City(string name, string fst, string scnd) { Name = name; FirstNum = fst; SecondNum = scnd; }
            public string Name { get; set; }
            public string FirstNum { get; set; }
            public string SecondNum { get; set; }
            public override string ToString()
            {
                return $"{Name} {FirstNum} {SecondNum}";
            }
        }
        public struct Weather
        {
            private string _country, _name, _description;
            private double _temp;
            public Weather(string country, string name, double temp, string description)
            {
                _country = country;
                _name = name;
                _temp = temp;
                _description = description;
            }
            public string Country { get { return _country; } }
            public double Temp { get { return _temp; } }
            public string Name { get { return _name; } }
            public string Description { get { return _description; } }
            public override string ToString()
            {
                return $"{Country} {Name} {Temp} {Description}";
            }
        }
        async Task<string> GetHtmlContent(string par1, string par2)
        {
            HttpClient client = new HttpClient();
            const string API = "da5caa93fd2b4350a6eeab276cb3e9bd";
            client.BaseAddress = new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={par1}&lon={par2}&appid={API}");
            return await client.GetStringAsync(client.BaseAddress);
        }
        async Task<List<Weather>> GetStrings()
        {
            string city = "C:\\Users\\pkapa\\source\\repos\\lab09.2\\city.txt";
            string[] cities = File.ReadAllLines(city);
            string list = "";
            List<Weather> result = new List<Weather>();
            string name, fst, scnd;
            foreach (string c in cities)
            {
                string[] line = c.Split('\t');
                name = line[0];
                string[] nums = line[1].Split(',');
                City town = new City(name, nums[0].Replace(" ", ""), nums[1].Replace(" ", ""));
                list = town.ToString();
                ListBoxItem item = new ListBoxItem();
                item.Content = list;
                myListBox.Items.Add(item);
            }
            return result;
        }
        private async void GetWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (myListBox.SelectedItem == null) { MessageBox.Show("Выберите город"); return; }
            string selectedCityName = myListBox.SelectedItem.ToString();
            string[] parts = selectedCityName.Split(' ');
            string html = await GetHtmlContent(parts[2], parts[3]);
            JObject jObject = JObject.Parse(html);
            Weather weather = new Weather(jObject["sys"]["country"].ToString(), jObject["name"].ToString(), Convert.ToDouble(jObject["main"]["temp"]), jObject["weather"][0]["description"].ToString());
            MessageBox.Show($"Текущая погода в городе {parts[1]}: Темпуратура = {weather.Temp}; Описание = {weather.Description}");

        }
    }
}

