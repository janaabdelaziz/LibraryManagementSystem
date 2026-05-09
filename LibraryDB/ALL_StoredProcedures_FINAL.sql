-- ================================================================
-- LibraryDB - ALL STORED PROCEDURES
-- Module 5 (Librarian Features) + Module 8 (Reports)
-- Run this file ONCE in SSMS before running the app
-- ================================================================

USE LibraryDB;
GO

-- ================================================================
-- HELPER SPs (Dropdowns)
-- ================================================================

CREATE OR ALTER PROCEDURE sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryID, CategoryName FROM CATEGORIES ORDER BY CategoryName;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetAllPublishers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PublisherID, Name FROM PUBLISHERS ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetAllAuthors
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AuthorID, Name FROM AUTHORS ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetActiveMembers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, FullName, Username, Email
    FROM USERS
    WHERE IsActive = 1
    ORDER BY FullName;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetBookCopies
    @BookID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CopyID, ShelfLocation, Status
    FROM BOOK_COPIES
    WHERE BookID = @BookID
    ORDER BY CopyID;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetActiveBorrowings
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Br.BorrowID,
        U.FullName                                                  AS MemberName,
        U.Email,
        B.Title                                                     AS BookTitle,
        BC.CopyID,
        BC.ShelfLocation,
        Br.BorrowDate,
        Br.DueDate,
        CASE
            WHEN Br.DueDate < CAST(GETDATE() AS DATE)
            THEN DATEDIFF(DAY, Br.DueDate, GETDATE())
            ELSE 0
        END                                                         AS DaysOverdue
    FROM BORROWING Br
    JOIN USERS       U  ON Br.UserID  = U.UserID
    JOIN BOOK_COPIES BC ON Br.CopyID  = BC.CopyID
    JOIN BOOKS       B  ON BC.BookID  = B.BookID
    WHERE Br.ReturnDate IS NULL
    ORDER BY Br.DueDate ASC;
END;
GO

-- ================================================================
-- MODULE 5 - BOOK MANAGEMENT
-- ================================================================

CREATE OR ALTER PROCEDURE sp_AddBook
    @Title           VARCHAR(200),
    @ISBN            VARCHAR(20),
    @PublicationYear INT,
    @CategoryID      INT,
    @PublisherID     INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM BOOKS WHERE ISBN = @ISBN)
        BEGIN
            RAISERROR('A book with this ISBN already exists.', 16, 1);
            RETURN;
        END
        INSERT INTO BOOKS (Title, ISBN, PublisherID, CategoryID, PublicationYear)
        VALUES (@Title, @ISBN, @PublisherID, @CategoryID, @PublicationYear);
        SELECT SCOPE_IDENTITY() AS NewBookID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateBook
    @BookID          INT,
    @Title           VARCHAR(200),
    @ISBN            VARCHAR(20),
    @PublicationYear INT,
    @CategoryID      INT,
    @PublisherID     INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM BOOKS WHERE BookID = @BookID)
        BEGIN
            RAISERROR('Book not found.', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM BOOKS WHERE ISBN = @ISBN AND BookID <> @BookID)
        BEGIN
            RAISERROR('Another book with this ISBN already exists.', 16, 1);
            RETURN;
        END
        UPDATE BOOKS
        SET Title           = @Title,
            ISBN            = @ISBN,
            PublicationYear = @PublicationYear,
            CategoryID      = @CategoryID,
            PublisherID     = @PublisherID
        WHERE BookID = @BookID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteBook
    @BookID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM BOOKS WHERE BookID = @BookID)
        BEGIN
            RAISERROR('Book not found.', 16, 1);
            RETURN;
        END
        IF EXISTS (
            SELECT 1 FROM BORROWING Br
            JOIN BOOK_COPIES BC ON Br.CopyID = BC.CopyID
            WHERE BC.BookID = @BookID AND Br.ReturnDate IS NULL
        )
        BEGIN
            RAISERROR('Cannot delete: one or more copies are currently borrowed.', 16, 1);
            RETURN;
        END
        DELETE FROM BOOK_AUTHOR WHERE BookID = @BookID;
        DELETE FROM BOOK_COPIES  WHERE BookID = @BookID;
        DELETE FROM BOOKS        WHERE BookID = @BookID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_AddBookCopy
    @BookID        INT,
    @ShelfLocation VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM BOOKS WHERE BookID = @BookID)
        BEGIN
            RAISERROR('Book not found.', 16, 1);
            RETURN;
        END
        INSERT INTO BOOK_COPIES (BookID, Status, ShelfLocation)
        VALUES (@BookID, 'Available', @ShelfLocation);
        SELECT SCOPE_IDENTITY() AS NewCopyID;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteBookCopy
    @CopyID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM BOOK_COPIES WHERE CopyID = @CopyID)
        BEGIN
            RAISERROR('Copy not found.', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM BORROWING WHERE CopyID = @CopyID AND ReturnDate IS NULL)
        BEGIN
            RAISERROR('Cannot delete: this copy is currently borrowed.', 16, 1);
            RETURN;
        END
        DELETE FROM BOOK_COPIES WHERE CopyID = @CopyID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ================================================================
-- MODULE 5 - SEARCH BOOKS (COMPLEX SP - joins 5 tables)
-- ================================================================

CREATE OR ALTER PROCEDURE sp_SearchBooks
    @Title      VARCHAR(200) = NULL,
    @AuthorName VARCHAR(100) = NULL,
    @CategoryID INT          = NULL,
    @ISBN       VARCHAR(20)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        B.BookID,
        B.Title,
        B.ISBN,
        B.PublicationYear,
        C.CategoryName,
        P.Name                                                      AS PublisherName,
        STUFF((
            SELECT ', ' + A2.Name
            FROM BOOK_AUTHOR BA2
            JOIN AUTHORS A2 ON BA2.AuthorID = A2.AuthorID
            WHERE BA2.BookID = B.BookID
            FOR XML PATH(''), TYPE
        ).value('.','NVARCHAR(MAX)'), 1, 2, '')                     AS Authors,
        COUNT(DISTINCT BC.CopyID)                                   AS TotalCopies,
        SUM(CASE WHEN BC.Status = 'Available' THEN 1 ELSE 0 END)   AS AvailableCopies
    FROM BOOKS B
    LEFT JOIN CATEGORIES  C  ON B.CategoryID  = C.CategoryID
    LEFT JOIN PUBLISHERS  P  ON B.PublisherID = P.PublisherID
    LEFT JOIN BOOK_COPIES BC ON B.BookID      = BC.BookID
    LEFT JOIN BOOK_AUTHOR BA ON B.BookID      = BA.BookID
    LEFT JOIN AUTHORS     A  ON BA.AuthorID   = A.AuthorID
    WHERE
        (@Title      IS NULL OR B.Title LIKE '%' + @Title + '%')
        AND (@ISBN       IS NULL OR B.ISBN = @ISBN)
        AND (@CategoryID IS NULL OR B.CategoryID = @CategoryID)
        AND (@AuthorName IS NULL OR A.Name LIKE '%' + @AuthorName + '%')
    GROUP BY B.BookID, B.Title, B.ISBN, B.PublicationYear,
             C.CategoryName, P.Name
    ORDER BY B.Title;
END;
GO

-- ================================================================
-- MODULE 5 - ISSUE BOOK (COMPLEX SP - joins 4 tables)
-- ================================================================

CREATE OR ALTER PROCEDURE sp_IssueBook
    @UserID  INT,
    @CopyID  INT,
    @DueDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM USERS WHERE UserID = @UserID AND IsActive = 1)
        BEGIN
            RAISERROR('User not found or account is not active.', 16, 1);
            RETURN;
        END
        IF NOT EXISTS (SELECT 1 FROM BOOK_COPIES WHERE CopyID = @CopyID AND Status = 'Available')
        BEGIN
            RAISERROR('This copy is not available for borrowing.', 16, 1);
            RETURN;
        END
        IF EXISTS (
            SELECT 1 FROM FINES F
            JOIN BORROWING Br ON F.BorrowID = Br.BorrowID
            WHERE Br.UserID = @UserID AND F.Status = 'Unpaid'
        )
        BEGIN
            RAISERROR('User has unpaid fines. Please settle them before borrowing.', 16, 1);
            RETURN;
        END
        INSERT INTO BORROWING (UserID, CopyID, BorrowDate, DueDate, ReturnDate)
        VALUES (@UserID, @CopyID, GETDATE(), @DueDate, NULL);
        UPDATE BOOK_COPIES SET Status = 'Borrowed' WHERE CopyID = @CopyID;
        UPDATE RESERVATION
        SET Status = 'Fulfilled'
        WHERE UserID = @UserID AND CopyID = @CopyID AND Status = 'Pending';
        SELECT SCOPE_IDENTITY() AS NewBorrowID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ================================================================
-- MODULE 5 - RETURN BOOK (COMPLEX SP - auto fine calculation)
-- ================================================================

CREATE OR ALTER PROCEDURE sp_ReturnBook
    @BorrowID   INT,
    @FinePerDay DECIMAL(6,2) = 1.00
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @CopyID     INT;
        DECLARE @DueDate    DATE;
        DECLARE @ReturnDate DATE = CAST(GETDATE() AS DATE);
        DECLARE @DaysLate   INT;
        DECLARE @FineAmt    DECIMAL(6,2);
        SELECT @CopyID = CopyID, @DueDate = DueDate
        FROM BORROWING
        WHERE BorrowID = @BorrowID AND ReturnDate IS NULL;
        IF @CopyID IS NULL
        BEGIN
            RAISERROR('Borrowing record not found or already returned.', 16, 1);
            RETURN;
        END
        UPDATE BORROWING SET ReturnDate = @ReturnDate WHERE BorrowID = @BorrowID;
        UPDATE BOOK_COPIES SET Status = 'Available' WHERE CopyID = @CopyID;
        SET @DaysLate = DATEDIFF(DAY, @DueDate, @ReturnDate);
        IF @DaysLate > 0
        BEGIN
            SET @FineAmt = @DaysLate * @FinePerDay;
            INSERT INTO FINES (BorrowID, Amount, Status)
            VALUES (@BorrowID, @FineAmt, 'Unpaid');
            SELECT @DaysLate AS DaysLate, @FineAmt AS FineAmount, 'Fine generated' AS Message;
        END
        ELSE
        BEGIN
            SELECT 0 AS DaysLate, 0.00 AS FineAmount, 'Returned on time - no fine' AS Message;
        END
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ================================================================
-- MODULE 5 - RESERVATIONS
-- ================================================================

CREATE OR ALTER PROCEDURE sp_AddReservation
    @UserID INT,
    @CopyID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM BOOK_COPIES WHERE CopyID = @CopyID AND Status = 'Available')
        BEGIN
            RAISERROR('Copy is available - please borrow it directly.', 16, 1);
            RETURN;
        END
        IF EXISTS (
            SELECT 1 FROM RESERVATION
            WHERE UserID = @UserID AND CopyID = @CopyID AND Status = 'Pending'
        )
        BEGIN
            RAISERROR('User already has a pending reservation for this copy.', 16, 1);
            RETURN;
        END
        INSERT INTO RESERVATION (UserID, CopyID, ReservationDate, Status)
        VALUES (@UserID, @CopyID, GETDATE(), 'Pending');
        SELECT SCOPE_IDENTITY() AS NewReservationID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_CancelReservation
    @ReservationID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM RESERVATION WHERE ReservationID = @ReservationID AND Status = 'Pending')
        BEGIN
            RAISERROR('Reservation not found or already cancelled/fulfilled.', 16, 1);
            RETURN;
        END
        UPDATE RESERVATION SET Status = 'Cancelled' WHERE ReservationID = @ReservationID;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_RecordFine
    @BorrowID INT,
    @Amount   DECIMAL(6,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM BORROWING WHERE BorrowID = @BorrowID)
        BEGIN
            RAISERROR('Borrowing record not found.', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM FINES WHERE BorrowID = @BorrowID)
            UPDATE FINES SET Amount = @Amount WHERE BorrowID = @BorrowID;
        ELSE
            INSERT INTO FINES (BorrowID, Amount, Status) VALUES (@BorrowID, @Amount, 'Unpaid');
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- ================================================================
-- MODULE 8 - MANAGERIAL REPORTS
-- ================================================================

CREATE OR ALTER PROCEDURE sp_Report_LibraryStats
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM BOOKS)                                        AS TotalBooks,
        (SELECT COUNT(*) FROM BOOK_COPIES)                                  AS TotalCopies,
        (SELECT COUNT(*) FROM BOOK_COPIES WHERE Status = 'Available')       AS AvailableCopies,
        (SELECT COUNT(*) FROM BOOK_COPIES WHERE Status = 'Borrowed')        AS BorrowedCopies,
        (SELECT COUNT(*) FROM USERS WHERE IsActive = 1)                     AS ActiveMembers,
        (SELECT COUNT(*) FROM BORROWING WHERE ReturnDate IS NULL)           AS CurrentBorrowings,
        (SELECT COUNT(*) FROM BORROWING
         WHERE ReturnDate IS NULL
           AND DueDate < CAST(GETDATE() AS DATE))                           AS OverdueCount,
        (SELECT COUNT(*) FROM RESERVATION WHERE Status = 'Pending')         AS PendingReservations,
        (SELECT ISNULL(SUM(Amount), 0) FROM FINES WHERE Status = 'Unpaid') AS TotalUnpaidFines,
        (SELECT ISNULL(SUM(Amount), 0) FROM PAYMENTS)                       AS TotalRevenue;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_MostBorrowedBooks
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 10
        B.Title,
        C.CategoryName,
        STUFF((
            SELECT ', ' + A2.Name
            FROM BOOK_AUTHOR BA2
            JOIN AUTHORS A2 ON BA2.AuthorID = A2.AuthorID
            WHERE BA2.BookID = B.BookID
            FOR XML PATH(''), TYPE
        ).value('.','NVARCHAR(MAX)'), 1, 2, '')  AS Authors,
        COUNT(Br.BorrowID)                       AS TimesBorrowed
    FROM BOOKS B
    JOIN BOOK_COPIES BC ON B.BookID     = BC.BookID
    JOIN BORROWING   Br ON BC.CopyID    = Br.CopyID
    JOIN CATEGORIES  C  ON B.CategoryID = C.CategoryID
    GROUP BY B.BookID, B.Title, C.CategoryName
    ORDER BY TimesBorrowed DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_AvgBorrowDuration
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        C.CategoryName,
        COUNT(Br.BorrowID) AS TotalBorrowings,
        AVG(DATEDIFF(DAY, Br.BorrowDate, ISNULL(Br.ReturnDate, GETDATE()))) AS AvgDays,
        MIN(DATEDIFF(DAY, Br.BorrowDate, ISNULL(Br.ReturnDate, GETDATE()))) AS MinDays,
        MAX(DATEDIFF(DAY, Br.BorrowDate, ISNULL(Br.ReturnDate, GETDATE()))) AS MaxDays
    FROM BORROWING Br
    JOIN BOOK_COPIES BC ON Br.CopyID    = BC.CopyID
    JOIN BOOKS       B  ON BC.BookID    = B.BookID
    JOIN CATEGORIES  C  ON B.CategoryID = C.CategoryID
    GROUP BY C.CategoryID, C.CategoryName
    ORDER BY AvgDays DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_FineRevenue
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        YEAR(PaymentDate)            AS [Year],
        DATENAME(MONTH, PaymentDate) AS [Month],
        COUNT(PaymentID)             AS PaymentsCount,
        SUM(Amount)                  AS TotalCollected,
        AVG(Amount)                  AS AvgPayment,
        MAX(Amount)                  AS MaxPayment
    FROM PAYMENTS
    GROUP BY YEAR(PaymentDate), MONTH(PaymentDate), DATENAME(MONTH, PaymentDate)
    ORDER BY [Year] DESC, MONTH(PaymentDate) DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_OverdueStats
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(*) AS TotalBorrowings,
        SUM(CASE WHEN ReturnDate > DueDate
                 OR (ReturnDate IS NULL AND DueDate < CAST(GETDATE() AS DATE))
             THEN 1 ELSE 0 END) AS OverdueCount,
        CAST(100.0 *
            SUM(CASE WHEN ReturnDate > DueDate
                     OR (ReturnDate IS NULL AND DueDate < CAST(GETDATE() AS DATE))
                 THEN 1 ELSE 0 END)
            / NULLIF(COUNT(*),0) AS DECIMAL(5,2)) AS OverdueRatePct,
        AVG(CASE WHEN ReturnDate > DueDate
            THEN DATEDIFF(DAY, DueDate, ReturnDate) END) AS AvgDaysLate,
        MAX(CASE WHEN ReturnDate > DueDate
            THEN DATEDIFF(DAY, DueDate, ReturnDate) END) AS MaxDaysLate
    FROM BORROWING;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_BooksByCategory
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        C.CategoryName,
        COUNT(DISTINCT B.BookID) AS TotalTitles,
        COUNT(BC.CopyID)         AS TotalCopies,
        SUM(CASE WHEN BC.Status = 'Available' THEN 1 ELSE 0 END) AS AvailableCopies,
        SUM(CASE WHEN BC.Status = 'Borrowed'  THEN 1 ELSE 0 END) AS BorrowedCopies
    FROM CATEGORIES C
    LEFT JOIN BOOKS       B  ON C.CategoryID = B.CategoryID
    LEFT JOIN BOOK_COPIES BC ON B.BookID     = BC.BookID
    GROUP BY C.CategoryID, C.CategoryName
    ORDER BY TotalTitles DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_TopMembers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 10
        U.FullName,
        U.Email,
        COUNT(DISTINCT Br.BorrowID) AS TotalBorrowings,
        SUM(CASE WHEN Br.ReturnDate IS NULL THEN 1 ELSE 0 END) AS CurrentlyBorrowing,
        ISNULL(SUM(F.Amount), 0) AS TotalFines,
        ISNULL(SUM(CASE WHEN F.Status = 'Unpaid' THEN F.Amount ELSE 0 END), 0) AS UnpaidFines
    FROM USERS U
    LEFT JOIN BORROWING Br ON U.UserID    = Br.UserID
    LEFT JOIN FINES     F  ON Br.BorrowID = F.BorrowID
    WHERE U.IsActive = 1
    GROUP BY U.UserID, U.FullName, U.Email
    ORDER BY TotalBorrowings DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_AuthorPopularity
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        A.Name                      AS AuthorName,
        COUNT(DISTINCT B.BookID)    AS TotalBooks,
        COUNT(DISTINCT BC.CopyID)   AS TotalCopies,
        COUNT(DISTINCT Br.BorrowID) AS TotalBorrowings
    FROM AUTHORS A
    LEFT JOIN BOOK_AUTHOR BA ON A.AuthorID = BA.AuthorID
    LEFT JOIN BOOKS       B  ON BA.BookID  = B.BookID
    LEFT JOIN BOOK_COPIES BC ON B.BookID   = BC.BookID
    LEFT JOIN BORROWING   Br ON BC.CopyID  = Br.CopyID
    GROUP BY A.AuthorID, A.Name
    ORDER BY TotalBorrowings DESC;
END;
GO

-- ================================================================
-- MODULE 8 - DETAILED REPORTS
-- ================================================================

CREATE OR ALTER PROCEDURE sp_Report_OverdueBooks
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        U.FullName                                          AS MemberName,
        U.Email,
        B.Title                                             AS BookTitle,
        BC.CopyID,
        BC.ShelfLocation,
        Br.BorrowDate,
        Br.DueDate,
        DATEDIFF(DAY, Br.DueDate, GETDATE())                AS DaysOverdue,
        CAST(DATEDIFF(DAY, Br.DueDate, GETDATE()) * 1.00
             AS DECIMAL(8,2))                               AS EstimatedFine
    FROM BORROWING Br
    JOIN USERS       U  ON Br.UserID  = U.UserID
    JOIN BOOK_COPIES BC ON Br.CopyID  = BC.CopyID
    JOIN BOOKS       B  ON BC.BookID  = B.BookID
    WHERE Br.ReturnDate IS NULL
      AND Br.DueDate < CAST(GETDATE() AS DATE)
    ORDER BY DaysOverdue DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_BorrowingHistory
    @UserID       INT         = NULL,
    @StartDate    DATE        = NULL,
    @EndDate      DATE        = NULL,
    @StatusFilter VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Br.BorrowID,
        U.FullName                                                  AS MemberName,
        B.Title                                                     AS BookTitle,
        C.CategoryName,
        BC.CopyID,
        Br.BorrowDate,
        Br.DueDate,
        Br.ReturnDate,
        CASE
            WHEN Br.ReturnDate IS NULL AND Br.DueDate < CAST(GETDATE() AS DATE) THEN 'Overdue'
            WHEN Br.ReturnDate IS NULL THEN 'Active'
            ELSE 'Returned'
        END                                                         AS BorrowStatus,
        ISNULL(F.Amount, 0)                                         AS FineAmount,
        ISNULL(F.Status, 'N/A')                                     AS FineStatus
    FROM BORROWING Br
    JOIN USERS       U  ON Br.UserID    = U.UserID
    JOIN BOOK_COPIES BC ON Br.CopyID    = BC.CopyID
    JOIN BOOKS       B  ON BC.BookID    = B.BookID
    JOIN CATEGORIES  C  ON B.CategoryID = C.CategoryID
    LEFT JOIN FINES  F  ON Br.BorrowID  = F.BorrowID
    WHERE
        (@UserID    IS NULL OR Br.UserID    = @UserID)
        AND (@StartDate IS NULL OR Br.BorrowDate >= @StartDate)
        AND (@EndDate   IS NULL OR Br.BorrowDate <= @EndDate)
        AND (
            @StatusFilter IS NULL
            OR (@StatusFilter = 'Overdue'  AND Br.ReturnDate IS NULL AND Br.DueDate < CAST(GETDATE() AS DATE))
            OR (@StatusFilter = 'Active'   AND Br.ReturnDate IS NULL AND Br.DueDate >= CAST(GETDATE() AS DATE))
            OR (@StatusFilter = 'Returned' AND Br.ReturnDate IS NOT NULL)
        )
    ORDER BY Br.BorrowDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Report_ActiveReservations
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        R.ReservationID,
        U.FullName                                          AS MemberName,
        U.Email,
        B.Title                                             AS BookTitle,
        BC.CopyID,
        BC.ShelfLocation,
        BC.Status                                           AS CopyStatus,
        R.ReservationDate,
        DATEDIFF(DAY, R.ReservationDate, GETDATE())         AS DaysWaiting,
        R.Status
    FROM RESERVATION R
    JOIN USERS       U  ON R.UserID  = U.UserID
    JOIN BOOK_COPIES BC ON R.CopyID  = BC.CopyID
    JOIN BOOKS       B  ON BC.BookID = B.BookID
    WHERE R.Status = 'Pending'
    ORDER BY R.ReservationDate ASC;
END;
GO

PRINT '================================================';
PRINT 'ALL Stored Procedures created successfully!';
PRINT 'Module 5: 13 SPs | Module 8: 11 SPs';
PRINT 'Total: 24 Stored Procedures';
PRINT '================================================';
GO
