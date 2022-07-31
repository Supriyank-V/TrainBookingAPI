using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using TrainBookingAPI.Models;

namespace TrainBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly IConfiguration _configuration;

        public SearchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet ("stations")]
        public JsonResult Get()
        {
            string query = @"select DISTINCT * from Stations where IsTerminalStation=1";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);

        }

        [HttpGet("coaches")]
        public JsonResult GetCoaches()
        {
            string query = @"select DISTINCT * from Coaches";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);

        }

        [HttpGet("initial={Initial}&destination={Destination}&travelDate={travelDate}")]
        public JsonResult GetAllFetchedTrain(string? Initial, string? Destination, string? travelDate)
        {
            string query = @"";

            if (Initial != "undefined" && Destination != "undefined" && travelDate != "undefined")
            {
                query = @"
                        SELECT DISTINCT tr.Id as IdTrain, st.Id as IdSubTrain, tr.[Name] as TrainName, ori.[Value] as Initial, ori.[Code] as InitialCode, dest.[Value] as Destination, dest.[Code] as DestinationCode, convert(varchar, st.DateOfDeparture, 3) as DepartureDate, tr.DepartureTime, 
                                convert(varchar, st.DateOfArrival, 3) as ArrivalDate, tr.ArrivalTime, tr.JourneyTime, tf.Monday, tf.Tuesday, tf.Wednesday, tf.Thrusday, tf.Friday, tf.Saturday, tf.Sunday, tfa.VegFood, tfa.NonVegFood, tfa.Beddings
                                FROM Train as tr
                                LEFT JOIN SubTrain as st on st.IdTrain = tr.Id
                                LEFT JOIN TrainFrequency as tf on tf.IdTrain = tr.Id
                                LEFT JOIN TrainFacilities as tfa on tfa.IdTrain = tr.Id
                                LEFT JOIN Stations as ori on  tr.Initial = ori.Id
                                LEFT JOIN Stations as dest on tr.Destination = dest.Id
								WHERE ori.[Value] = @Initial and dest.[Value] = @Destination and st.DateOfDeparture = @TravelDate";
            }
            else if (Initial != "undefined" && Destination != "undefined" & travelDate == "undefined")
            {
                query = @"
                        SELECT DISTINCT tr.Id as IdTrain, st.Id as IdSubTrain, tr.[Name] as TrainName, ori.[Value] as Initial, ori.[Code] as InitialCode, dest.[Value] as Destination, dest.[Code] as DestinationCode, convert(varchar, st.DateOfDeparture, 3) as DepartureDate, tr.DepartureTime, 
                                convert(varchar, st.DateOfArrival, 3) as ArrivalDate, tr.ArrivalTime, tr.JourneyTime, tf.Monday, tf.Tuesday, tf.Wednesday, tf.Thrusday, tf.Friday, tf.Saturday, tf.Sunday, tfa.VegFood, tfa.NonVegFood, tfa.Beddings
                                FROM Train as tr
                                LEFT JOIN SubTrain as st on st.IdTrain = tr.Id
                                LEFT JOIN TrainFrequency as tf on tf.IdTrain = tr.Id
                                LEFT JOIN TrainFacilities as tfa on tfa.IdTrain = tr.Id
                                LEFT JOIN Stations as ori on  tr.Initial = ori.Id
                                LEFT JOIN Stations as dest on tr.Destination = dest.Id
							WHERE ori.[Value] = @Initial and dest.[Value] = @Destination";
            }
            else if ( Initial == "undefined" && Destination == "undefined" & travelDate != "undefined")
            {
                query = @"
                        SELECT DISTINCT tr.Id as IdTrain, st.Id as IdSubTrain, tr.[Name] as TrainName, ori.[Value] as Initial, ori.[Code] as InitialCode, dest.[Value] as Destination, dest.[Code] as DestinationCode, convert(varchar, st.DateOfDeparture, 3) as DepartureDate, tr.DepartureTime, 
                                convert(varchar, st.DateOfArrival, 3) as ArrivalDate, tr.ArrivalTime, tr.JourneyTime, tf.Monday, tf.Tuesday, tf.Wednesday, tf.Thrusday, tf.Friday, tf.Saturday, tf.Sunday, tfa.VegFood, tfa.NonVegFood, tfa.Beddings
                                FROM Train as tr
                                LEFT JOIN SubTrain as st on st.IdTrain = tr.Id
                                LEFT JOIN TrainFrequency as tf on tf.IdTrain = tr.Id
                                LEFT JOIN TrainFacilities as tfa on tfa.IdTrain = tr.Id
                                LEFT JOIN Stations as ori on  tr.Initial = ori.Id
                                LEFT JOIN Stations as dest on tr.Destination = dest.Id
							WHERE st.DateOfDeparture = @TravelDate";
            }
            else
            {
                query = @"
                        SELECT DISTINCT tr.Id as IdTrain, st.Id as IdSubTrain, tr.[Name] as TrainName, ori.[Value] as Initial, ori.[Code] as InitialCode, dest.[Value] as Destination, dest.[Code] as DestinationCode, convert(varchar, st.DateOfDeparture, 3) as DepartureDate, tr.DepartureTime, 
                                convert(varchar, st.DateOfArrival, 3) as ArrivalDate, tr.ArrivalTime, tr.JourneyTime, tf.Monday, tf.Tuesday, tf.Wednesday, tf.Thrusday, tf.Friday, tf.Saturday, tf.Sunday, tfa.VegFood, tfa.NonVegFood, tfa.Beddings
                                FROM Train as tr
                                LEFT JOIN SubTrain as st on st.IdTrain = tr.Id
                                LEFT JOIN TrainFrequency as tf on tf.IdTrain = tr.Id
                                LEFT JOIN TrainFacilities as tfa on tfa.IdTrain = tr.Id
                                LEFT JOIN Stations as ori on  tr.Initial = ori.Id
                                LEFT JOIN Stations as dest on tr.Destination = dest.Id";   
            }

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    if(Initial != "undefined")
                    { 
                        cmd.Parameters.AddWithValue("@Initial", Initial);
                    }
                    if(Destination != "undefined")
                    {
                        cmd.Parameters.AddWithValue("@Destination", Destination);
                    }
                    if (travelDate != "undefined")
                    {
                        cmd.Parameters.AddWithValue("@TravelDate", travelDate);
                    }
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

    }
}
