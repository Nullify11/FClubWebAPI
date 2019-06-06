using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AnalysisServices.AdomdClient;
using System.Text;

namespace FClubWebAPI.Controllers
{
    public class SalesController : ApiController
    {
        AdomdConnection connection;
        public IHttpActionResult GetSales()
        {   
            return Ok(RequestSales("SELECT {[Measures].[Sale Count]} ON COLUMNS, " +
                                   "NONEMPTY({[Date].[Year].[Year]}) ON ROWS " +
                                   "FROM  [Fclub DW]"));
        }

        public IHttpActionResult GetSales(int year)
        {
            return Ok(RequestSales("SELECT {[Measures].[Sale Count] }ON COLUMNS, " +
                                   "NONEMPTY({[Date].[Month Name].[Month Name]}) ON ROWS " +
                                   "FROM   [Fclub DW] " +
                                   "WHERE  [Date].[Year].&[" + year + "]"));
        }

        public IHttpActionResult GetSales(int year, string month)
        {
            return Ok(RequestSales("SELECT {[Measures].[Sale Count]} ON COLUMNS, " +
                                   "{[Date].[Day Number Of Month].[Day Number Of Month]} ON ROWS " +
                                   "FROM[Fclub DW] " +
                                   "WHERE {([Date].[Year].&[" + year + "], [Date].[Month Name].&[" + month + "])}"));
        }



        private Dictionary<string, int> RequestSales(string MDXQuery)
        {
            Dictionary<string, int> responseMessage = new Dictionary<string, int>();
            try
            {
                OpenConnection();

                AdomdCommand command = new AdomdCommand(MDXQuery, connection);
                AdomdDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    responseMessage.Add(Convert.ToString(dataReader[0]), Convert.ToInt32(dataReader[1]));
                }

                dataReader.Close();
                connection.Close();

                return responseMessage;

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occured while retriving data:\n" +
                    "Error message: " + e.Message);
                return null;
            }

        }

        private void OpenConnection()
        {
            connection = new AdomdConnection(
                "Data Source=localhost;Catalog=Analysis Services Tutorial");
            connection.Open();
        }
    }
}
