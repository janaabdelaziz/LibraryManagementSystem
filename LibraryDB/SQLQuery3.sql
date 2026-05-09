--BORROWING

USE LibraryDB;
GO

SELECT 
    B.Title,
    BC.CopyID,
    Br.BorrowDate,
    Br.DueDate,
    Br.ReturnDate,
    BC.Status
FROM BORROWING Br
JOIN BOOK_COPIES BC ON Br.CopyID = BC.CopyID
JOIN BOOKS B ON BC.BookID = B.BookID
WHERE Br.UserID = 2
ORDER BY Br.BorrowDate DESC;
GO

USE LibraryDB;
GO

SELECT * FROM BORROWING;
GO

SELECT * FROM BOOK_COPIES;
GO
-----------------------------
USE LibraryDB;
GO

SELECT * FROM RESERVATION;
GO

SELECT * FROM BOOK_COPIES;
GO