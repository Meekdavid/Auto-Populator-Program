
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Text;
using DSTV_Autopopulate.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace DSTV_Autopopulate
{
    public class Populator
    {
        private readonly ILogger<Worker> _logger;
        public IConfiguration Configuration;
        public Populator(ILogger<Worker> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }
        public DSTVResponse HTTPClient(DSTVRequest dstvRequeryRequest)
        {

            try
            {
                RestClient Client = new RestClient(Configuration["ValidData:RequeryUrl"]);

                RestRequest Request = new RestRequest("request", Method.Post)
                {
                    RequestFormat = DataFormat.Json
                };
                Request.AddHeader("x-api-key", "Key");
                Request.AddBody(dstvRequeryRequest);

                var Response = Client.Execute(Request);
                var res = JsonConvert.DeserializeObject<DSTVResponse>(Response.Content);
                for (int i = 0; i <= res.data.Length; i++)
                {
                    string price = res.data[i].availablePricingOptions[0].price.ToString();
                    string code = res.data[i].code;
                    string name = res.data[i].name;
                    string nameCheck = $"{res.data[i].name}(";
                    string dstvName = $"{name}({price})";
                    using (SqlConnection con = new SqlConnection(Configuration["ConnectionStrings:GTCNDbConection"]))
                    {

                        using (SqlCommand cmd = con.CreateCommand())
                        {
                            int resultFinal = 0;
                            try
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "UpdateDSTVProducts";
                                cmd.CommandTimeout = 120;
                                cmd.Parameters.AddWithValue("@lov_value", code);
                                cmd.Parameters.AddWithValue("@lov_text", dstvName);
                                cmd.Parameters.AddWithValue("@lov_data", price);
                                cmd.Parameters.AddWithValue("@lov_check", nameCheck);


                                if (con.State != ConnectionState.Open)
                                {
                                    con.Open();
                                }

                                resultFinal = cmd.ExecuteNonQuery();
                                if (resultFinal > 0)
                                {
                                    _logger.LogInformation($"Table updated for {name} with the code: {code}");
                                }
                                else
                                {
                                    _logger.LogInformation($"unable to update table for for {name} with the code: {code}");
                                }
                            }
                            catch (Exception ex)
                            {

                                _logger.LogInformation($"Error occured {ex.Message}: {ex.StackTrace}");

                            }
                            finally
                            {
                                if (con.State == ConnectionState.Open) con.Close();
                                con.Dispose();
                            }
                        }
                    }
                }


            }
            catch (Exception exception)
            {
                _logger.LogInformation($"Error occured: {exception.StackTrace} with message {exception.Message}");
                return null;
            }
            return null;
        }
    }
}
