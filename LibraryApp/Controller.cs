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

        // You can add more methods here for other operations

    }

}