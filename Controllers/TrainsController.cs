using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using TrainBookingAPI.Models;

namespace TrainBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TrainsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult GetAllTrains()
        {
            string query = @"
                              SELECT DISTINCT tr.Id as IdTrain, st.Id as IdSubTrain, tr.[Name] as TrainName, ori.[Value] as Initial, ori.[Code] as InitialCode, dest.[Value] as Destination, dest.[Code] as DestinationCode, convert(varchar, st.DateOfDeparture, 3) as DepartureDate, tr.DepartureTime, 
                                convert(varchar, st.DateOfArrival, 3) as ArrivalDate, tr.ArrivalTime, tr.JourneyTime, tf.Monday, tf.Tuesday, tf.Wednesday, tf.Thrusday, tf.Friday, tf.Saturday, tf.Sunday, tfa.VegFood, tfa.NonVegFood, tfa.Beddings
                                FROM Train as tr
                                LEFT JOIN SubTrain as st on st.IdTrain = tr.Id
                                LEFT JOIN TrainFrequency as tf on tf.IdTrain = tr.Id
                                LEFT JOIN TrainFacilities as tfa on tfa.IdTrain = tr.Id
                                LEFT JOIN Stations as ori on  tr.Initial = ori.Id
                                LEFT JOIN Stations as dest on tr.Destination = dest.Id";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, myConn))
                {
                    myReader = sqlCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("path/{trainId}")]
        public JsonResult GetTrainPathDetails(int trainId)
        {
            string query = @"
                            SELECT tp.Id as IdTrainPath, tr.Id as TrainId,tr.[Name],tr.TrainCode, ori.[Value] as Origin, dest.[Value] as Destination, sta.[Value] as Station, sta.Code as StationCode, 
							tp.ExpectedArrival, tp.ExpectedDeparture, tp.HaltTime, tp.PathRank
                            FROM TRAIN as tr
                            JOIN TrainPath as tp on tp.IdTrain = tr.Id                            
                            LEFT JOIN Stations as ori on  tr.Initial = ori.Id
                            LEFT JOIN Stations as dest on tr.Destination = dest.Id
                            LEFT JOIN Stations as sta on tp.Stations = sta.Id
                            WHERE tr.Id = @TrainID";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@TrainID", trainId);
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("coaches/{subTrainId}")]
        public JsonResult GetTrainCoachDetails(int subTrainId)
        {
            string query = @"
                            SELECT stc.Id as IdSubCoach, tr.[Name] as TrainName, tr.TrainCode, stc.IdSubTrain, coa.Id as IdCoach, coa.CoachName, stc.SeatsLeft, stc.Fare,stc.WaitingListCount 
                            FROM SubTrainCoaches stc
                            INNER JOIN SubTrain st on stc.IdSubTrain = st.Id
                            INNER JOIN Train tr on st.IdTrain = tr.Id
                            LEFT JOIN Coaches coa on stc.IdCoaches = coa.Id
                            WHERE st.Id = @IdSubTrain";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@IdSubTrain", subTrainId);
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
