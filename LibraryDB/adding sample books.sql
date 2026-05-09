USE LibraryDB;
GO

-- =============================================
-- 1. CLEAN ALL LOOKUP TABLES
-- =============================================

DELETE FROM BOOK_AUTHOR;
DELETE FROM BOOK_COPIES;
DELETE FROM BORROWING;
DELETE FROM RESERVATION;
DELETE FROM FINES;
DELETE FROM PAYMENTS;
DELETE FROM NOTIFICATIONS;
DELETE FROM BOOKS;
DELETE FROM USERS;
DELETE FROM AUTHORS;
DELETE FROM CATEGORIES;
DELETE FROM PUBLISHERS;
DELETE FROM ROLES;

-- Reset Identity Seeds
DBCC CHECKIDENT ('ROLES', RESEED, 0);
DBCC CHECKIDENT ('PUBLISHERS', RESEED, 0);
DBCC CHECKIDENT ('CATEGORIES', RESEED, 0);
DBCC CHECKIDENT ('AUTHORS', RESEED, 0);
DBCC CHECKIDENT ('BOOKS', RESEED, 0);
DBCC CHECKIDENT ('BOOK_COPIES', RESEED, 0);
DBCC CHECKIDENT ('USERS', RESEED, 0);
GO

-- =============================================
-- 2. INSERT LOOKUP DATA
-- =============================================

-- Roles
INSERT INTO ROLES (RoleName) VALUES 
('Admin'), ('Librarian'), ('Member'), ('Guest');

-- Publishers
INSERT INTO PUBLISHERS (Name) VALUES 
('Penguin'), ('Addison-Wesley'), ('McGraw-Hill'), ('Bloomsbury'),
('Bantam Books'), ('HarperCollins'), ('OReilly Media'), ('Pearson');

-- Categories
INSERT INTO CATEGORIES (CategoryName) VALUES 
('Programming'), ('Database Systems'), ('Fiction'), ('Fantasy'),
('Science Fiction'), ('Mystery'), ('Biography'), ('Technology');

-- Authors
INSERT INTO AUTHORS (Name) VALUES 
('Robert C. Martin'), ('Abraham Silberschatz'), ('George Orwell'),
('J.K. Rowling'), ('George R.R. Martin'), ('J.R.R. Tolkien'),
('Agatha Christie');

-- =============================================
-- 3. INSERT MAIN DATA (BOOKS + USERS)
-- =============================================

-- Books
INSERT INTO BOOKS (Title, ISBN, PublisherID, CategoryID, PublicationYear)
VALUES 
('Clean Code', 'ISBN-0001', 2, 1, 2008),                    -- Addison-Wesley, Programming
('Introduction to Database Systems', 'ISBN-0002', 8, 2, 2004), -- Pearson, Database Systems
('1984', 'ISBN-0003', 1, 3, 1949),                          -- Penguin, Fiction
('Harry Potter and the Philosopher''s Stone', '9780747532699', 4, 4, 1997), -- Bloomsbury, Fantasy
('A Game of Thrones', '9780553103540', 5, 4, 1996),         -- Bantam Books, Fantasy
('The Lord of the Rings', '9780261103252', 1, 4, 1954),     -- Penguin, Fantasy
('Murder on the Orient Express', '9780062073488', 6, 6, 1934); -- HarperCollins, Mystery

-- Users
INSERT INTO USERS (Name, Email, Password, Phone, RoleID, Status, RegistrationDate)
VALUES 
('Admin User', 'admin@library.com', 'admin123', '0123456789', 1, 'Active', GETDATE()),
('Librarian Sara', 'sara.lib@library.com', 'lib123', '0112233445', 2, 'Active', GETDATE()),
('Mohamed Ahmed', 'mohamed@example.com', 'pass123', '0155566778', 3, 'Active', GETDATE()),
('Fatima Ali', 'fatima@example.com', 'pass123', '0100112233', 3, 'Active', GETDATE()),
('Omar Hassan', 'omar@example.com', 'pass123', '0122334455', 3, 'Active', GETDATE());
GO

-- =============================================
-- 4. INSERT BOOK_COPIES + BOOK_AUTHOR
-- =============================================

-- Book Copies (Multiple copies per book)
INSERT INTO BOOK_COPIES (BookID, Status, ShelfLocation)
VALUES 
(1, 'Available', 'A-101'), (1, 'Available', 'A-102'), (1, 'Available', 'A-103'), -- Clean Code
(2, 'Available', 'B-201'), (2, 'Available', 'B-202'),                             -- Database Systems
(3, 'Available', 'C-301'), (3, 'Available', 'C-302'),                             -- 1984
(4, 'Available', 'D-401'), (4, 'Available', 'D-402'), (4, 'Available', 'D-403'), -- Harry Potter
(5, 'Available', 'E-501'), (5, 'Available', 'E-502'),                             -- Game of Thrones
(6, 'Available', 'F-601'), (6, 'Available', 'F-602'),                             -- Lord of the Rings
(7, 'Available', 'G-701');                                                         -- Murder on the Orient Express

-- Book Authors (Many-to-Many)
INSERT INTO BOOK_AUTHOR (BookID, AuthorID) VALUES 
(1,1), (2,2), (3,3), (4,4), (5,5), (6,6), (7,7);
GO

