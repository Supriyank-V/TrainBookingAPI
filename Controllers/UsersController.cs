using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using TrainBookingAPI.Models;

namespace TrainBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public UsersController(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"SELECT usr.Id, usr.FirstName, usr.LastName, usr.[Login], acsl.[Value] as Rights, usr.Phone, usr.Email, usr.Nortifications, usr.[Address] 
                                FROM Users as usr LEFT JOIN AccessLevel as acsl on usr.IdAccessLevel = acsl.Id;";
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

        [HttpGet ("login={login}&password={password}")]
        public JsonResult GetSpecificUser(string login, string password) 
        {
            string query = @"SELECT usr.Id, usr.FirstName, usr.LastName, usr.[Login], acsl.[Value] as Rights, usr.Phone, usr.Email, usr.Nortifications, usr.[Address] 
                                FROM Users as usr LEFT JOIN AccessLevel as acsl on usr.IdAccessLevel = acsl.Id
                                LEFT JOIN UserCreds as usrc on usrc.IdUser = usr.Id
                                WHERE usrc.[Login] like @Login and usrc.[Password] like @Password;";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);
                    myReader = cmd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(User user)
        {
            try
            {
                string query = @"INSERT INTO Users VALUES ( @Rights,@FirstName, @LastName, @Login, @Email, @Phone, @Nortification, @Address)
                                INSERT INTO UserCreds VALUES ( (Select Id from Users WHERE [Login] = @Login),@Login,@Password)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
                SqlDataReader myReader;
                using (SqlConnection myConn = new SqlConnection(sqlDataSource))
                {
                    myConn.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(query, myConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@Rights", user.AccessType);
                        sqlCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        sqlCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        sqlCommand.Parameters.AddWithValue("@Login", user.Login);
                        sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@Phone", user.PhoneNumber);
                        sqlCommand.Parameters.AddWithValue("@Nortification", user.IsNortification);
                        sqlCommand.Parameters.AddWithValue("@Address", user.Address);
                        sqlCommand.Parameters.AddWithValue("@Password", user.Password);
                        myReader = sqlCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myConn.Close();
                    }
                }
                SendEmail(user, 1, null);
                return new JsonResult(1);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Violation of UNIQUE KEY constraint 'Login_Unique'."))
                {
                    return new JsonResult(-2);
                }
                else
                {
                    return new JsonResult(ex.Message);
                }
            }
        }

        [HttpPut]
        public JsonResult Put(User user)
        {
            try
            {
                string query = @"
                              UPDATE Users 
                                SET FirstName = @FirstName,
                                LastName = @LastName,
                                Email = @Email,
                                Phone = @Phone,
                                [Address] = @Address,
                                Nortifications = @Nortification
                                WHERE Id = @UserId;
                            ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
                SqlDataReader myReader;
                using (SqlConnection myConn = new SqlConnection(sqlDataSource))
                {
                    myConn.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(query, myConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        sqlCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@Phone", user.PhoneNumber);
                        sqlCommand.Parameters.AddWithValue("@Address", user.Address);
                        sqlCommand.Parameters.AddWithValue("@Nortification", user.IsNortification);
                        sqlCommand.Parameters.AddWithValue("@UserId", user.Id);
                        myReader = sqlCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myConn.Close();
                    }
                }
                SendEmail(user, 2, null);
                return new JsonResult("Updated Successfully");
            }
            catch(Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpDelete ("{id}")]
        public JsonResult Delete(int id) 
        {
            string query = @"DELETE FROM UserCreds WHERE IdUser = @UserId
                                DELETE FROM Users WHERE Id = @UserId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("TrainAppCon");
            SqlDataReader myReader;
            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, myConn))
                {
                    sqlCommand.Parameters.AddWithValue("@UserId", id);
                    myReader = sqlCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myConn.Close();
                }
            }
            return new JsonResult(1);
        }

        [HttpGet ("delete/email={email}")]
        public JsonResult SendDeleteMail(string email)
        {
            try
            {
                SendEmail(null, 3, email);
                return new JsonResult(1);
            }
            catch (Exception ex)
            {
                return new JsonResult(-2);
            }
        }

        public async Task<IActionResult> SendEmail(User user, int type, string emailId)
        {
            if (type == 1)
            {
                Email email = new Email();
                string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\RegisterTemplate.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[FirstName]", user.FirstName);
                MailText = MailText.Replace("[Login]", user.Login);
                MailText = MailText.Replace("[PhoneNumber]", user.PhoneNumber.ToString());
                MailText = MailText.Replace("[Address]", user.Address);
                MailText = MailText.Replace("[Password]", user.Password);

                email.Body = MailText;
                email.To = user.Email;
                email.Subject = "Welcome to Train Booking App";
                await _emailService.SendEmailAsync(email);
                return Ok();
            }
            else if (type == 2)
            {
                Email email = new Email();
                string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\AccountUpdateTemplate.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[FirstName]", user.FirstName);

                email.Body = MailText;
                email.To = user.Email;
                email.Subject = "Account Details Updated Successfully";
                await _emailService.SendEmailAsync(email);
                return Ok();
            }
            else
            {
                Email email = new Email();
                string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\AccountDeleteTemplate.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                email.Body = MailText;
                email.To = emailId;
                email.Subject = "Account Deleted Successfully";
                await _emailService.SendEmailAsync(email);
                return Ok();
            }
        }
    }
}
