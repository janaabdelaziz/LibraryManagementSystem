USE LibraryDB;
GO

--only run these if CATEGORIES, PUBLISHERS, AUTHORS are empty

INSERT INTO CATEGORIES (CategoryName)
VALUES ('Fantasy'), ('Science Fiction'), ('Classic Literature');
GO

INSERT INTO PUBLISHERS (Name)
VALUES ('Bloomsbury'), ('Bantam Books'), ('Penguin Books');
GO

INSERT INTO AUTHORS (Name)
VALUES ('J.K. Rowling'), ('Fyodor Dostoveosky'), ('J.R.R. Tolkien');
GO
USE LibraryDB;
GO

-- Lookup data (only run once on an empty DB)

INSERT INTO CATEGORIES (CategoryName)
VALUES ('Programming'), ('Databases'), ('Fiction'),
       ('Fantasy'), ('Science Fiction'), ('Classic Literature');
GO

INSERT INTO PUBLISHERS (Name)
VALUES ('OReilly Media'), ('Pearson'), ('Penguin'),
       ('Bloomsbury'), ('Bantam Books'), ('Penguin Books');
GO

INSERT INTO AUTHORS (Name)
VALUES ('Robert Martin'), ('C.J. Date'), ('George Orwell'),
       ('J.K. Rowling'), ('Fyodor Dostoevsky'), ('J.R.R. Tolkien');
GO

-- Sample books

INSERT INTO BOOKS (Title, ISBN, PublisherID, CategoryID, PublicationYear)
VALUES
('Clean Code', 'ISBN-0001', 1, 1, 2008),
('Introduction to Database Systems', 'ISBN-0002', 2, 2, 2004),
('1984', 'ISBN-0003', 3, 3, 1949),
('Harry Potter and the Philosopher''s Stone', '9780747532699', 4, 4, 1997),
('A Game of Thrones', '9780553103540', 5, 4, 1996),
('The Lord of the Rings', '9780261103252', 6, 4, 1954);
GO

-- Sample book copies

INSERT INTO BOOK_COPIES (BookID, Status, ShelfLocation)
VALUES
(1, 'Available', 'F-1'),
(1, 'Borrowed',  'F-2'),
(2, 'Available', 'F-3'),
(3, 'Available', 'F-4'),
(4, 'Available', 'F-5'),
(5, 'Available', 'F-6'),
(6, 'Available', 'F-7');
GO

-- Test

SELECT * FROM BOOKS;
GO

SELECT * FROM BOOK_COPIES;
GO

--sample books

USE LibraryDB;
GO

INSERT INTO BOOKS (Title, ISBN, PublisherID, CategoryID, PublicationYear)
VALUES
('Clean Code', 'ISBN-0001', 1, 1, 2008),
('Introduction to Database Systems', 'ISBN-0002', 2, 2, 2004),
('1984', 'ISBN-0003', 3, 3, 1949),
('Harry Potter and the Philosopher''s Stone', '9780747532699', 1, 4, 1997),  -- Fantasy
('A Game of Thrones',                         '9780553103540', 2, 4, 1996),  -- Fantasy
('The Lord of the Rings',                     '9780261103252', 3, 4, 1954);  -- Fantasy

GO

--sample book copies

USE LibraryDB;
GO

-- Assume BookID 1,2,3 are the three books above
INSERT INTO BOOK_COPIES (BookID, Status, ShelfLocation)
VALUES
(1, 'Available', 'F-1'),
(1, 'Borrowed',  'F-2'),
(2, 'Available', 'F-3'),
(3, 'Available', 'F-4');
GO


--test
SELECT * FROM BOOKS;
GO

SELECT * FROM BOOK_COPIES;
GO