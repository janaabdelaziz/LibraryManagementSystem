Create db LibraryDB;
GO 

USE LibraryDB;
GO

-- =============================================
-- 1. Lookup Tables
-- =============================================

CREATE TABLE ROLES (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE PUBLISHERS (
    PublisherID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE CATEGORIES (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE AUTHORS (
    AuthorID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL
);

-- =============================================
-- 2. Main Tables
-- =============================================

CREATE TABLE USERS (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Phone VARCHAR(15),
    RoleID INT NOT NULL,
    RegistrationDate DATE DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Active' CHECK (Status IN ('Active', 'Inactive', 'Suspended')),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES ROLES(RoleID)
);

CREATE TABLE BOOKS (
    BookID INT PRIMARY KEY IDENTITY(1,1),
    Title VARCHAR(200) NOT NULL,
    ISBN VARCHAR(20) UNIQUE,
    PublisherID INT,
    CategoryID INT NOT NULL,
    PublicationYear INT,
    CONSTRAINT FK_Books_Publishers FOREIGN KEY (PublisherID) REFERENCES PUBLISHERS(PublisherID),
    CONSTRAINT FK_Books_Categories FOREIGN KEY (CategoryID) REFERENCES CATEGORIES(CategoryID)
);

CREATE TABLE BOOK_COPIES (
    CopyID INT PRIMARY KEY IDENTITY(1,1),
    BookID INT NOT NULL,
    Status VARCHAR(20) DEFAULT 'Available' CHECK (Status IN ('Available', 'Borrowed', 'Reserved', 'Damaged', 'Lost')),
    ShelfLocation VARCHAR(50),
    CONSTRAINT FK_BookCopies_Books FOREIGN KEY (BookID) REFERENCES BOOKS(BookID)
);

CREATE TABLE BOOK_AUTHOR (
    BookID INT NOT NULL,
    AuthorID INT NOT NULL,
    PRIMARY KEY (BookID, AuthorID),
    CONSTRAINT FK_BookAuthor_Books FOREIGN KEY (BookID) REFERENCES BOOKS(BookID),
    CONSTRAINT FK_BookAuthor_Authors FOREIGN KEY (AuthorID) REFERENCES AUTHORS(AuthorID)
);

-- =============================================
-- 3. Transaction Tables
-- =============================================

CREATE TABLE BORROWING (
    BorrowID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    CopyID INT NOT NULL,
    BorrowDate DATE DEFAULT GETDATE(),
    DueDate DATE NOT NULL,
    ReturnDate DATE NULL,
    CONSTRAINT FK_Borrowing_Users FOREIGN KEY (UserID) REFERENCES USERS(UserID),
    CONSTRAINT FK_Borrowing_BookCopies FOREIGN KEY (CopyID) REFERENCES BOOK_COPIES(CopyID)
);

CREATE TABLE RESERVATION (
    ReservationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    CopyID INT NOT NULL,
    ReservationDate DATE DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Fulfilled', 'Cancelled')),
    CONSTRAINT FK_Reservation_Users FOREIGN KEY (UserID) REFERENCES USERS(UserID),
    CONSTRAINT FK_Reservation_BookCopies FOREIGN KEY (CopyID) REFERENCES BOOK_COPIES(CopyID)
);

CREATE TABLE FINES (
    FineID INT PRIMARY KEY IDENTITY(1,1),
    BorrowID INT NOT NULL,
    Amount DECIMAL(6,2) NOT NULL CHECK (Amount >= 0),
    Status VARCHAR(20) DEFAULT 'Unpaid' CHECK (Status IN ('Unpaid', 'Paid')),
    CONSTRAINT FK_Fines_Borrowing FOREIGN KEY (BorrowID) REFERENCES BORROWING(BorrowID)
);

CREATE TABLE PAYMENTS (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    FineID INT NOT NULL,
    Amount DECIMAL(6,2) NOT NULL CHECK (Amount > 0),
    PaymentDate DATE DEFAULT GETDATE(),
    PaymentMethod VARCHAR(50),
    CONSTRAINT FK_Payments_Fines FOREIGN KEY (FineID) REFERENCES FINES(FineID)
);

CREATE TABLE NOTIFICATIONS (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Message VARCHAR(255) NOT NULL,
    Type VARCHAR(50),
    Date DATE DEFAULT GETDATE(),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserID) REFERENCES USERS(UserID)
);
