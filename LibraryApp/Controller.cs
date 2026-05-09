using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class Controller
    {
        private DBManager dbMan;

        public Controller()
        {
            dbMan = new DBManager();
        }

        // Example: count all users in USERS table (safe cast)
        public int CountUsers()
        {
            string query = "SELECT COUNT(UserID) FROM USERS;";
            object result = dbMan.ExecuteScalar(query);

            if (result == null || result == DBNull.Value)
                return 0;

            return Convert.ToInt32(result);
        }

        public int Signup(string name, string email, string phone, string password)
        {
            string query = @"
        INSERT INTO USERS (Name, Email, Password, Phone, RoleID, RegistrationDate, Status)
        VALUES (
            '" + name + @"',
            '" + email + @"',
            '" + password + @"',
            '" + phone + @"',
            (SELECT RoleID FROM ROLES WHERE RoleName = 'Member'),
            GETDATE(),
            'Active'
        )";

            return dbMan.ExecuteNonQuery(query);
        }

        public int ChangePassword(string email, string oldPassword, string newPassword)
        {
            string query = @"
        UPDATE USERS
        SET Password = '" + newPassword + @"'
        WHERE Email = '" + email + @"'
        AND Password = '" + oldPassword + @"'
        AND Status = 'Active'";

            return dbMan.ExecuteNonQuery(query);
        }

        public DataTable Login(string email, string password)
        {
            // NOTE: store hashed + salted password in DB instead of plain text. This example keeps the existing shape,
            // but uses parameterized command to avoid injection.
            string query = @"
                SELECT U.UserID, U.Name, U.Email, U.Password, U.Phone, R.RoleID, U.Status, U.RegistrationDate
                FROM USERS U
                JOIN ROLES R ON U.RoleID = R.RoleID
                WHERE U.Email = @email AND U.Password = @password;";

            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = (object)email ?? DBNull.Value;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = (object)password ?? DBNull.Value;
                return dbMan.ExecuteReader(cmd);
            }
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
                "WHERE B.Title LIKE @titlePart;";

            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.Add("@titlePart", SqlDbType.NVarChar).Value = "%" + (titlePart ?? string.Empty) + "%";
                return dbMan.ExecuteReader(cmd);
            }
        }

        public DataTable GetBorrowingHistoryForUser(int userId)
        {
            string query =
                "SELECT B.Title, BC.CopyID, Br.BorrowDate, Br.DueDate, Br.ReturnDate, BC.Status " +
                "FROM BORROWING Br " +
                "JOIN BOOK_COPIES BC ON Br.CopyID = BC.CopyID " +
                "JOIN BOOKS B ON BC.BookID = B.BookID " +
                "WHERE Br.UserID = @userId " +
                "ORDER BY Br.BorrowDate DESC;";

            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                return dbMan.ExecuteReader(cmd);
            }
        }


        public int BorrowBook(int userId, int copyId, DateTime dueDate)
        {
            // Use a transaction to ensure both insert and update succeed together.
            using (SqlCommand insertBorrow = new SqlCommand(
                "INSERT INTO BORROWING (UserID, CopyID, BorrowDate, DueDate, ReturnDate) " +
                "VALUES (@userId, @copyId, GETDATE(), @dueDate, NULL);"))
            using (SqlCommand updateCopy = new SqlCommand(
                "UPDATE BOOK_COPIES SET Status = 'Borrowed' WHERE CopyID = @copyId;"))
            {
                insertBorrow.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                insertBorrow.Parameters.Add("@copyId", SqlDbType.Int).Value = copyId;
                insertBorrow.Parameters.Add("@dueDate", SqlDbType.Date).Value = dueDate.Date;

                updateCopy.Parameters.Add("@copyId", SqlDbType.Int).Value = copyId;

                var commands = new[] { insertBorrow, updateCopy };
                return dbMan.ExecuteTransaction(commands);
            }
        }

        public int ReserveBook(int userId, int copyId)
        {
            using (SqlCommand insertReservation = new SqlCommand(
                "INSERT INTO RESERVATION (UserID, CopyID, ReservationDate, Status) " +
                "VALUES (@userId, @copyId, GETDATE(), 'Pending');"))
            using (SqlCommand updateCopy = new SqlCommand(
                "UPDATE BOOK_COPIES SET Status = 'Reserved' WHERE CopyID = @copyId;"))
            {
                insertReservation.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                insertReservation.Parameters.Add("@copyId", SqlDbType.Int).Value = copyId;

                updateCopy.Parameters.Add("@copyId", SqlDbType.Int).Value = copyId;

                var commands = new[] { insertReservation, updateCopy };
                return dbMan.ExecuteTransaction(commands);
            }
        }


        ////////////////////////////////////////////////////////
        ////add more methods here:

    }

}