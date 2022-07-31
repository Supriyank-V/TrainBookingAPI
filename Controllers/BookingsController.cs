using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using TrainBookingAPI.Models;

namespace TrainBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public BookingsController(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet ("{userId}")]
        public JsonResult GetCurrentBookings(int userId)
        {
            string query = @"SELECT cb.Id as BookingId, st.Id as SubTrainId,tr.ID as TrainId, tr.[Name] as TrainName, 
                            convert(varchar, cb.BookedOn, 3) as BookedOn, convert(varchar, st.DateOfDeparture, 3) as DateOfDeparture , convert(varchar, st.DateOfArrival, 3) as DateOfArrival ,tr.DepartureTime, tr.ArrivalTime, 
                            stao.[Value] as Initial, stad.[Value] as Destination, cb.NoOfSeats,cb.SeatsCoach, cb.[Status],cb.Comments 
                            FROM CurrentBookings as cb
                            LEFT JOIN SubTrain as st on cb.IdSubTrain = st.Id
                            LEFT JOIN Train as tr on st.IdTrain = tr.Id
                            LEFT JOIN Users as usr on cb.IdUser = usr.Id
                            LEFT JOIN Stations as stao on cb.Initial = stao.Id
                            LEFT JOIN Stations as stad on cb.Destination = stad.Id
                            WHERE usr.Id = @UserId ORDER BY cb.BookedOn DESC";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("bookingId={bookingId}&userId={userId}")]
        public JsonResult GetSpecificBookingDetails(int bookingId, int userId)
        {
            string query = @"SELECT * FROM BookingDetails as bd 
                                LEFT JOIN CurrentBookings as cb on bd.IdBooking = cb.Id
                                WHERE cb.Id = @BookingId and cb.IdUser = @UserId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("previous/{userId}")]
        public JsonResult GetPreviousBookings(int userId)

        {
            string query = @"SELECT cb.Id as BookingId, st.Id as SubTrainId,tr.ID as TrainId, tr.[Name] as TrainName, 
                            convert(varchar, cb.BookedOn, 3) as BookedOn, convert(varchar, st.DateOfDeparture, 3) as DateOfDeparture , convert(varchar, st.DateOfArrival, 3) as DateOfArrival ,tr.DepartureTime, tr.ArrivalTime, 
                            stao.[Value] as Initial, stad.[Value] as Destination, cb.NoOfSeats,cb.SeatsCoach, cb.[Status],cb.Comments 
                            FROM PreviousBookings as cb
                            LEFT JOIN SubTrain as st on cb.IdSubTrain = st.Id
                            LEFT JOIN Train as tr on st.IdTrain = tr.Id
                            LEFT JOIN Users as usr on cb.IdUser = usr.Id
                            LEFT JOIN Stations as stao on cb.Initial = stao.Id
                            LEFT JOIN Stations as stad on cb.Destination = stad.Id
                            WHERE usr.Id = @UserId ORDER BY cb.BookedOn DESC";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("previous/bookingId={bookingId}&userId={userId}")]
        public JsonResult GetPreviousBookingDetails(int bookingId, int userId)
        {
            string query = @"SELECT * FROM BookingDetails as bd 
                                        LEFT JOIN PreviousBookings as cb on bd.IdBooking = cb.Id
                                        WHERE cb.Id = @BookingId and cb.IdUser = @UserId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult BookTicket (Booking bookTicket)
        {
            try
            {
                int idBooking = 0;
                DataTable passengerDetails = new DataTable();
                //Name
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = "Name";
                dataColumn.DataType = typeof(string);
                passengerDetails.Columns.Add(dataColumn);
                //Age
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Age";
                dataColumn.DataType = typeof(int);
                passengerDetails.Columns.Add(dataColumn);
                //Gender
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Gender";
                dataColumn.DataType = typeof(string);
                passengerDetails.Columns.Add(dataColumn);
                //PhoneNumber
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "PhoneNumber";
                dataColumn.DataType = typeof(string);
                passengerDetails.Columns.Add(dataColumn);
                //Email
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Email";
                dataColumn.DataType = typeof(string);
                passengerDetails.Columns.Add(dataColumn);

                foreach (Passengers item in bookTicket.Passengers)
                {
                    DataRow row = passengerDetails.NewRow();
                    row[0] = item.Name;
                    row[1] = item.Age;
                    row[2] = item.Gender;
                    row[3] = item.PhoneNumber;
                    row[4] = item.Email;
                    passengerDetails.Rows.Add(row);
                }

                string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
                using (SqlConnection myConn = new SqlConnection(sqlDataSource))
                {
                    myConn.Open();
                    using (var sqlCommand = myConn.CreateCommand())
                    {
                        sqlCommand.CommandText = "[dbo].[AddBooking]";
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add(new SqlParameter("@IdTrain", SqlDbType.Int) { Value = bookTicket.IdTrain });
                        sqlCommand.Parameters.Add(new SqlParameter("@IdSubTrain", SqlDbType.Int) { Value = bookTicket.IdSubTrain });
                        sqlCommand.Parameters.Add(new SqlParameter("@IdUser", SqlDbType.Int) { Value = bookTicket.IdUser });
                        sqlCommand.Parameters.Add(new SqlParameter("@BookingFor", SqlDbType.Date) { Value = bookTicket.BookingFor });
                        sqlCommand.Parameters.Add(new SqlParameter("@Initial", SqlDbType.NVarChar) { Value = bookTicket.Initial });
                        sqlCommand.Parameters.Add(new SqlParameter("@Destination", SqlDbType.NVarChar) { Value = bookTicket.Destination });
                        sqlCommand.Parameters.Add(new SqlParameter("@NoOfSeats", SqlDbType.Int) { Value = bookTicket.NoOfSeats });
                        sqlCommand.Parameters.Add(new SqlParameter("@IdSubTrainCoach", SqlDbType.Int) { Value = bookTicket.IdSubTrainCoach });
                        sqlCommand.Parameters.Add(new SqlParameter("@Fare", SqlDbType.Int) { Value = bookTicket.Fare });
                        sqlCommand.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.NVarChar) { Value = bookTicket.PaymentMethod });
                        sqlCommand.Parameters.Add(new SqlParameter("@MealPreference", SqlDbType.NVarChar) { Value = bookTicket.MealPreference });
                        sqlCommand.Parameters.Add(new SqlParameter("@Passengers", SqlDbType.Structured) { Value = passengerDetails });
                        sqlCommand.Parameters.Add(new SqlParameter("@IdBooked", SqlDbType.Int) { Value = 500 });
                        sqlCommand.Parameters["@IdBooked"].Direction = ParameterDirection.Output;
                        
                        sqlCommand.ExecuteNonQuery();
                        idBooking = (int)sqlCommand.Parameters["@IdBooked"].Value;
                        myConn.Close();
                    }
                    SendEmail(bookTicket, idBooking , null, 1, 0, null);
                }
                return new JsonResult("Ticket Booked");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }

        [HttpDelete ("delete/{bookingId}")]
        public JsonResult DeleteCurrentBooking(int bookingId)
        {
            try
            {
                string query = @"UPDATE SubTrainCoaches 
                                SET SeatsLeft = SeatsLeft + (Select COUNT(*) from BookingDetails where IdBooking = @BookingId)
                                WHERE Id = (Select SeatsCoach from CurrentBookings where Id = @BookingId);
                                DELETE FROM BookingDetails WHERE IdBooking = @BookingId
                                DELETE FROM CurrentBookings WHERE Id = @BookingId";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
                SqlDataReader myReader;
                using (SqlConnection myConn = new SqlConnection(sqlDataSource))
                {
                    myConn.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(query, myConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@BookingId", bookingId);
                        myReader = sqlCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myConn.Close();
                    }
                }
                return new JsonResult(1);
            }
            catch(Exception ex)
            {
                return new JsonResult(-2);
            }
        }

        [HttpGet("delete/email={email}&bookingId={bookingId}&userName={userName}")]
        public JsonResult SendDeleteMail(string email, int bookingId, string userName)
        {
            try
            {
                SendEmail(null, 3, email, 2 , bookingId, userName);
                return new JsonResult(1);
            }
            catch (Exception ex)
            {
                return new JsonResult(-2);
            }
        }


        [HttpGet]
        public async Task<IActionResult> SendEmail(Booking booking , int BookingId, string emailId, int type, int IdDeletedBooking, string userName)
        {
            try
            {
                if (type == 1)
                {
                    Email email = new Email();
                    string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\BookTrainTemplate.html";
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    string IdTrainName = booking.IdTrain + "-" + booking.TrainName;
                    MailText = MailText.Replace("[FirstName]", booking.FirstName);
                    MailText = MailText.Replace("[TrainName]", booking.TrainName);
                    MailText = MailText.Replace("[IdTrainName]", IdTrainName);
                    MailText = MailText.Replace("[BookingID]", BookingId.ToString());
                    MailText = MailText.Replace("[BookingOn]", DateTime.Now.ToString());
                    MailText = MailText.Replace("[DepartureDate]", booking.BookingFor);
                    MailText = MailText.Replace("[Initial]", booking.Initial);
                    MailText = MailText.Replace("[Destination]", booking.Destination);
                    MailText = MailText.Replace("[PassengerCount]", booking.NoOfSeats.ToString());
                    MailText = MailText.Replace("[Meal]", booking.MealPreference);
                    MailText = MailText.Replace("[PaymentMode]", booking.PaymentMethod);
                    MailText = MailText.Replace("[Fare]", booking.Fare.ToString());

                    email.Body = MailText;
                    email.To = booking.Email;
                    email.Subject = booking.TrainName +  " - Booked for - " + booking.BookingFor +  " - from - " + booking.Initial;
                    await _emailService.SendEmailAsync(email);
                    return Ok();
                }
                else
                {
                    Email email = new Email();
                    string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\BookingDeleteTemplate.html";
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();
                    MailText = MailText.Replace("[bookingId]", IdDeletedBooking.ToString());
                    MailText = MailText.Replace("[User]", userName.ToString());
                    email.Body = MailText;
                    email.To = emailId;
                    email.Subject = "Ticket Cancelled for Booking Id : " + IdDeletedBooking;
                    await _emailService.SendEmailAsync(email);
                    return Ok();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

