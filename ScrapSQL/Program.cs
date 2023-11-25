using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScrapSQL
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var url = "http://www.parsianzarinfund.com/Data/Industries";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<Industries>>(jsonString);
            
            SqlConnection con = new SqlConnection(@"Data Source=HANA;Initial Catalog=ParseSQL;Integrated Security=True");
            string query = "INSERT INTO Industries (name, y) VALUES ('@name',' @y')";

            try
            {
                await con.OpenAsync();
                
                foreach(var item in data)
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", item.name);
                    cmd.Parameters.AddWithValue("@y", item.y);
                    await cmd.ExecuteNonQueryAsync();
                }
                    
                
            }
            catch (SqlException e)
            {
                Console.WriteLine("ERROR" + e.ToString());

            }
            finally
            {
                con.Close();
                
            }

            Console.WriteLine("Data inserted successfully!");
            Console.ReadKey();

        }
    }
}