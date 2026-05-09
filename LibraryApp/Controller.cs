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
        FROM Users U
        JOIN Roles R ON U.RoleID = R.RoleID
        WHERE U.Username = '" + username + @"'
        AND U.Password = '" + password + "'";

            return dbMan.ExecuteReader(query);
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

    }

}