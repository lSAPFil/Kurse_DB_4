using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using Project1_01._08._2022.Models;
using Microsoft.Graph;
using System.Net;
using System.Text.Encodings.Web;
using System.Reflection.Metadata;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System.Reflection;
using System.Diagnostics;
using System.Net.Sockets;

namespace Project1_01._08._2022.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        public static Random rnd = new Random();
        [HttpGet]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IList<ValueDate>> GetAsync(string AdmArea, string District, int? ID, string OrgName)
        {
            //создаем объект
            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();

            // возвращаем информацию по файлу
            var textDataTask = TextDataAsync();

            // Получаем данные IP пользователя
            string host = Dns.GetHostName();
            Console.WriteLine($"Имя компьютера: {host}");
            string clientip = Dns.GetHostAddresses(host).First<IPAddress>(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();

            // Формируем данные для отправки в базу
            string Filters = $"AdmArea : {(string.IsNullOrEmpty(AdmArea) ? "Null" : AdmArea)};" +
                $" District : {(string.IsNullOrEmpty(District) ? "Null" : District)};" +
                $" ID : {((ID == 0) ? "Null".ToString() : ID)};" +
                $" OrgName : {(string.IsNullOrEmpty(OrgName) ? "Null" : OrgName)}";

            // десериализуем полученный файл в лист
            var deserialized = JsonConvert.DeserializeObject<IList<ValueDate>>(await textDataTask);

            // Вывод отфильтрованных значений
            var listDataSwagger = deserialized.Where(i => ((!string.IsNullOrEmpty(AdmArea) ? i.AdmArea == AdmArea : true)
             && (!string.IsNullOrEmpty(District) ? i.District == District : true)
             && (ID != null ? i.ID == ID : true)
             && (!string.IsNullOrEmpty(OrgName) ? i.DeveloperInfo[0].OrgName == OrgName : true))).ToList();

            // Передаем данные в базу
            await WriteDataBaseAsync(clientip, DateTime.Now.ToString(), Filters);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(listDataSwagger, options);

            deserialized = JsonConvert.DeserializeObject<IList<ValueDate>>(JsonDataChenge(json));

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            return deserialized;
        }

        private string JsonDataChenge(string json)
        {
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            for (int i = 0; i < Convert.ToInt32(jsonObj.Count); i++)
            {
                jsonObj[i]["global_id"] = rnd.Next(100000000, 999999999);
            }

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

            return output;
        }

        private static string PathToLogFile = string.Empty;
        private async Task<String> TextDataAsync()
        {
            PathToLogFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // чтение файла
            var data = await System.IO.File.ReadAllTextAsync(PathToLogFile + "\\" + "Moscow_Portal_Data.json");

            // Если нет информации по чтению, выводим 404
            if (data == null)
            {
                NotFound();
            }

             return data;
        }

        private async Task WriteDataBaseAsync(string IP, string Data, string Filters)
        {
            //Парсим фильтры для записи в БД
            var filters = Filters
                .Replace("AdmArea : ", "")
                .Replace(" District :", "")
                .Replace(" ID : ", "")
                .Replace(" OrgName : ", "")
                .Split(";");

            //Подключаемся к базе данных
            SqlConnection sqlConnection =
                new SqlConnection(@"data source=LAPTOP-B1HPKED9;initial"+
                " catalog=Dudo;integrated security=True;MultipleActiveResultSets=True;TrustServerCertificate=True");

            // Открываем доступ к базе
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand($"INSERT INTO Log values (N'{IP}', '{Data}','{Filters}')", sqlConnection);

            // Выполяем введенную команду из sqlCommand
            sqlCommand.ExecuteNonQuery();


            sqlCommand = new SqlCommand($"INSERT INTO Filters values (N'{filters[0]}', '{filters[3]}','{filters[1]}', '{filters[2]}', '{Data}')", sqlConnection);

            // Выполяем введенную команду из sqlCommand
            sqlCommand.ExecuteNonQuery();

            // Закрываем доступ к базе
            sqlConnection.Close();
        }

        // Получение адреса компьютера
        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
    }
}
