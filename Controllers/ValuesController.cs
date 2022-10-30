﻿using Microsoft.AspNetCore.Http;
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

namespace Project1_01._08._2022.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        public Random rnd = new Random();
        [HttpGet]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IList<ValueDate>> Get(string AdmArea, string District, int ID, string OrgName)
        {
            // возвращаем информацию по файлу
            var textData = await TextData();

            // десериализуем полученный файл в лист
            var deserialized = JsonConvert.DeserializeObject<IList<ValueDate>>(textData);

            // Формируем данные для отправки в базу
            string Filters = $"AdmArea : {(String.IsNullOrEmpty(AdmArea) ? "Null" : AdmArea)};" +
                $" District : {(String.IsNullOrEmpty(District) ? "Null" : District)};" +
                $" ID : {((ID == 0) ? "Null".ToString() : ID)};" +
                $" OrgName : {(String.IsNullOrEmpty(OrgName) ? "Null" : OrgName)}";

            // Получаем данные IP пользователя
            string Host = Dns.GetHostName();

            // Передаем данные в базу
            WriteDataBase(Dns.GetHostByName(Host).AddressList[0].ToString(), DateTime.Now.ToString(), Filters);

            // Вывод отфильтрованных значений
            var listDataSwagger = await Task.FromResult(deserialized.Where(i => ((!String.IsNullOrEmpty(AdmArea) ? i.AdmArea == AdmArea : true)
             && (!String.IsNullOrEmpty(District) ? i.District == District : true)
             && (ID != 0 ? i.ID == ID : true)
             && (!String.IsNullOrEmpty(OrgName) ? i.DeveloperInfo[0].OrgName == OrgName : true))).ToList());

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(listDataSwagger, options);

            deserialized = JsonConvert.DeserializeObject<IList<ValueDate>>(await JsonDataChenge(json));

            return deserialized;
        }

        private async Task<String> JsonDataChenge(string json)
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
        private async Task<String> TextData()
        {
            PathToLogFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // чтение файла
            var data = await System.IO.File.ReadAllTextAsync(PathToLogFile + "\\" + "Moscow_Portal_Data.json");

            // Если нет информации по чтению, выводим 404
            var a = Task.FromResult(data);
            if (a == null)
            {
                NotFound();
            }

             return await Task.FromResult(data);
        }

        private async void WriteDataBase(string IP, string Data, string Filters)
        {
            //Подключаемся к базе данных
            SqlConnection sqlConnection =
                new SqlConnection(@"data source=LAPTOP-B1HPKED9;initial"+
                " catalog=Dudo;integrated security=True;MultipleActiveResultSets=True;TrustServerCertificate=True");
            // Integrated Security=true - Проверка подлинности Windows
            // data source=LAPTOP-B1HPKED9 - Сервер откуда нужно брать данные
            // catalog=Dudo - база в сервере 
            // MultipleActiveResultSets=True - для выполнения запросов параллельно
            //TrustServerCertificate=True - сертификат подлинности

            // Открываем доступ к базе
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand($"INSERT INTO Log values (N'{IP}', '{Data}','{Filters}')", sqlConnection);

            // Выполяем введенную команду из sqlCommand
            sqlCommand.ExecuteNonQuery();
            // Закрываем доступ к базе
            sqlConnection.Close();

        }
        
    }
    
}
