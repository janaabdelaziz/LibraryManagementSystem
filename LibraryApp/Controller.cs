    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace LibraryManagementSystem
{
    public class Controller
    {
        private DBManager dbMan;

        public Controller()
        {
            dbMan = new DBManager();
        }

        // Example: count all users in USERS table
        public int CountUsers()
        {
            string query = "SELECT COUNT(UserID) FROM USERS;";
            object result = dbMan.ExecuteScalar(query);

            if (result == null)
                return 0;

            return (int)result;
        }
        public DataTable Login(string username, string password)
        {
            string query = @"
        SELECT U.UserID, U.FullName, U.Username, U.IsActive, R.RoleName
        FROM USERS U
        JOIN ROLES R ON U.RoleID = R.RoleID
        WHERE U.Username = '" + username + @"'
        AND U.Password = '" + password + "'";

            DataTable dt = dbMan.ExecuteReader(query);
            if (dt == null) dt = new DataTable();
            return dt;
        }

        // Example: select all users (for a DataGridView later)
        public DataTable SelectAllUsers()
        {
            string query = "SELECT * FROM USERS;";
            return dbMan.ExecuteReader(query);
        }

        public void TerminateConnection()
        {
            dbMan.CloseConnection();
        }

        public DataTable SearchBooksByTitle(string titlePart)
        {
            string query =
                "SELECT B.BookID, B.Title, B.ISBN, B.PublicationYear, " +
                "       C.CategoryName, P.Name AS PublisherName, " +
                "       BC.CopyID, BC.Status, BC.ShelfLocation " +
                "FROM BOOKS B " +
                "LEFT JOIN CATEGORIES C ON B.CategoryID = C.CategoryID " +
                "LEFT JOIN PUBLISHERS P ON B.PublisherID = P.PublisherID " +
                "LEFT JOIN BOOK_COPIES BC ON B.BookID = BC.BookID " +
                "WHERE B.Title LIKE '%" + titlePart + "%';";

            return dbMan.ExecuteReader(query);
        }

        public DataTable GetBorrowingHistoryForUser(int userId)
        {
            string query =
                "SELECT B.Title, BC.CopyID, Br.BorrowDate, Br.DueDate, Br.ReturnDate, BC.Status " +
                "FROM BORROWING Br " +
                "JOIN BOOK_COPIES BC ON Br.CopyID = BC.CopyID " +
                "JOIN BOOKS B ON BC.BookID = B.BookID " +
                "WHERE Br.UserID = " + userId + " " +
                "ORDER BY Br.BorrowDate DESC;";

            return dbMan.ExecuteReader(query);
        }


        public int BorrowBook(int userId, int copyId, DateTime dueDate)
        {
            // Insert into BORROWING
            string insertBorrow =
                "INSERT INTO BORROWING (UserID, CopyID, BorrowDate, DueDate, ReturnDate) " +
                "VALUES (" + userId + ", " + copyId + ", GETDATE(), '" +
                dueDate.ToString("yyyy-MM-dd") + "', NULL);";

            int rows1 = dbMan.ExecuteNonQuery(insertBorrow);

            // Update BOOK_COPIES status
            string updateCopy =
                "UPDATE BOOK_COPIES SET Status = 'Borrowed' WHERE CopyID = " + copyId + ";";

            int rows2 = dbMan.ExecuteNonQuery(updateCopy);

            return rows1 + rows2;   // > 0 means success
        }

        public int ReserveBook(int userId, int copyId)
        {
            string insertReservation =
                "INSERT INTO RESERVATION (UserID, CopyID, ReservationDate, Status) " +
                "VALUES (" + userId + ", " + copyId + ", GETDATE(), 'Pending');";

            int rows1 = dbMan.ExecuteNonQuery(insertReservation);

            // Optional: mark copy as Reserved if you want
            string updateCopy =
                "UPDATE BOOK_COPIES SET Status = 'Reserved' WHERE CopyID = " + copyId + ";";

            int rows2 = dbMan.ExecuteNonQuery(updateCopy);

            return rows1 + rows2;
        }


        ////////////////////////////////////////////////////////
        ////add more methods here:
        private DataTable ExecSP(string spName, System.Data.SqlClient.SqlParameter[] parms = null)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager
                    .ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(spName, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    if (parms != null) cmd.Parameters.AddRange(parms);
                    using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        var dt = new System.Data.DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Runs a stored procedure with no result set (INSERT/UPDATE/DELETE)
        private void ExecSPNonQuery(string spName, System.Data.SqlClient.SqlParameter[] parms = null)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager
                    .ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(spName, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    if (parms != null) cmd.Parameters.AddRange(parms);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ── DROPDOWN HELPERS ─────────────────────────────────────────────

        public System.Data.DataTable GetAllCategories()
        {
            return ExecSP("sp_GetAllCategories");
        }

        public System.Data.DataTable GetAllPublishers()
        {
            return ExecSP("sp_GetAllPublishers");
        }

        public System.Data.DataTable GetAllAuthors()
        {
            return ExecSP("sp_GetAllAuthors");
        }

        public System.Data.DataTable GetActiveMembers()
        {
            return ExecSP("sp_GetActiveMembers");
        }

        // ── MODULE 5 METHODS ─────────────────────────────────────────────

        public void AddBook(string title, string isbn, int year, int categoryID, int publisherID)
        {
            ExecSPNonQuery("sp_AddBook", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@Title",           title),
        new System.Data.SqlClient.SqlParameter("@ISBN",            isbn),
        new System.Data.SqlClient.SqlParameter("@PublicationYear", year),
        new System.Data.SqlClient.SqlParameter("@CategoryID",      categoryID),
        new System.Data.SqlClient.SqlParameter("@PublisherID",     publisherID)
            });
        }

        public void UpdateBook(int bookID, string title, string isbn, int year, int categoryID, int publisherID)
        {
            ExecSPNonQuery("sp_UpdateBook", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@BookID",          bookID),
        new System.Data.SqlClient.SqlParameter("@Title",           title),
        new System.Data.SqlClient.SqlParameter("@ISBN",            isbn),
        new System.Data.SqlClient.SqlParameter("@PublicationYear", year),
        new System.Data.SqlClient.SqlParameter("@CategoryID",      categoryID),
        new System.Data.SqlClient.SqlParameter("@PublisherID",     publisherID)
            });
        }

        public void DeleteBook(int bookID)
        {
            ExecSPNonQuery("sp_DeleteBook", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@BookID", bookID)
            });
        }

        public void AddBookCopy(int bookID, string shelfLocation)
        {
            ExecSPNonQuery("sp_AddBookCopy", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@BookID",        bookID),
        new System.Data.SqlClient.SqlParameter("@ShelfLocation", shelfLocation)
            });
        }

        public void DeleteBookCopy(int copyID)
        {
            ExecSPNonQuery("sp_DeleteBookCopy", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@CopyID", copyID)
            });
        }

        public System.Data.DataTable GetBookCopies(int bookID)
        {
            return ExecSP("sp_GetBookCopies", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@BookID", bookID)
            });
        }

        public System.Data.DataTable SearchBooksAdvanced(string title = null, string author = null, int? categoryID = null, string isbn = null)
        {
            return ExecSP("sp_SearchBooks", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@Title",      (object)title      ?? System.DBNull.Value),
        new System.Data.SqlClient.SqlParameter("@AuthorName", (object)author     ?? System.DBNull.Value),
        new System.Data.SqlClient.SqlParameter("@CategoryID", (object)categoryID ?? System.DBNull.Value),
        new System.Data.SqlClient.SqlParameter("@ISBN",       (object)isbn       ?? System.DBNull.Value)
            });
        }

        public System.Data.DataTable IssueBook(int userID, int copyID, DateTime dueDate)
        {
            return ExecSP("sp_IssueBook", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@UserID",  userID),
        new System.Data.SqlClient.SqlParameter("@CopyID",  copyID),
        new System.Data.SqlClient.SqlParameter("@DueDate", dueDate.Date)
            });
        }

        public System.Data.DataTable ReturnBook(int borrowID)
        {
            return ExecSP("sp_ReturnBook", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@BorrowID",   borrowID),
        new System.Data.SqlClient.SqlParameter("@FinePerDay", 1.00m)
            });
        }

        public System.Data.DataTable GetActiveBorrowings()
        {
            return ExecSP("sp_GetActiveBorrowings");
        }

        public void AddReservation(int userID, int copyID)
        {
            ExecSPNonQuery("sp_AddReservation", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@UserID", userID),
        new System.Data.SqlClient.SqlParameter("@CopyID", copyID)
            });
        }

        public void CancelReservation(int reservationID)
        {
            ExecSPNonQuery("sp_CancelReservation", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@ReservationID", reservationID)
            });
        }

        public void RecordFine(int borrowID, decimal amount)
        {
            ExecSPNonQuery("sp_RecordFine", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@BorrowID", borrowID),
        new System.Data.SqlClient.SqlParameter("@Amount",   amount)
            });
        }

        // ── MODULE 8 REPORT METHODS ───────────────────────────────────────

        public System.Data.DataTable Report_LibraryStats() => ExecSP("sp_Report_LibraryStats");
        public System.Data.DataTable Report_MostBorrowedBooks() => ExecSP("sp_Report_MostBorrowedBooks");
        public System.Data.DataTable Report_AvgBorrowDuration() => ExecSP("sp_Report_AvgBorrowDuration");
        public System.Data.DataTable Report_FineRevenue() => ExecSP("sp_Report_FineRevenue");
        public System.Data.DataTable Report_OverdueStats() => ExecSP("sp_Report_OverdueStats");
        public System.Data.DataTable Report_BooksByCategory() => ExecSP("sp_Report_BooksByCategory");
        public System.Data.DataTable Report_TopMembers() => ExecSP("sp_Report_TopMembers");
        public System.Data.DataTable Report_AuthorPopularity() => ExecSP("sp_Report_AuthorPopularity");
        public System.Data.DataTable Report_OverdueBooks() => ExecSP("sp_Report_OverdueBooks");
        public System.Data.DataTable Report_ActiveReservations() => ExecSP("sp_Report_ActiveReservations");

        public System.Data.DataTable Report_BorrowingHistory(
            int? userID = null, DateTime? startDate = null,
            DateTime? endDate = null, string statusFilter = null)
        {
            return ExecSP("sp_Report_BorrowingHistory", new System.Data.SqlClient.SqlParameter[]
            {
        new System.Data.SqlClient.SqlParameter("@UserID",       (object)userID       ?? System.DBNull.Value),
        new System.Data.SqlClient.SqlParameter("@StartDate",    (object)startDate    ?? System.DBNull.Value),
        new System.Data.SqlClient.SqlParameter("@EndDate",      (object)endDate      ?? System.DBNull.Value),
        new System.Data.SqlClient.SqlParameter("@StatusFilter", (object)statusFilter ?? System.DBNull.Value)
            });
        }

    }

}