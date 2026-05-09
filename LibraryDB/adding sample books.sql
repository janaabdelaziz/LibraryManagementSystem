USE LibraryDB;
GO

INSERT INTO BOOKS (Title, ISBN, PublisherID, CategoryID, PublicationYear)
VALUES
(
 'Clean Code',
 'ISBN-0001',
 (SELECT PublisherID FROM PUBLISHERS WHERE Name = 'OReilly Media'),
 (SELECT CategoryID FROM CATEGORIES WHERE CategoryName = 'Programming'),
 2008
),
(
 'Introduction to Database Systems',
 'ISBN-0002',
 (SELECT PublisherID FROM PUBLISHERS WHERE Name = 'Pearson'),
 (SELECT CategoryID FROM CATEGORIES WHERE CategoryName = 'Databases'),
 2004
),
(
 '1984',
 'ISBN-0003',
 (SELECT PublisherID FROM PUBLISHERS WHERE Name = 'Penguin Books'),
 (SELECT CategoryID FROM CATEGORIES WHERE CategoryName = 'Fiction'),
 1949
),
(
 'Harry Potter and the Philosopher''s Stone',
 '9780747532699',
 (SELECT PublisherID FROM PUBLISHERS WHERE Name = 'Bloomsbury'),
 (SELECT CategoryID FROM CATEGORIES WHERE CategoryName = 'Fantasy'),
 1997
),
(
 'A Game of Thrones',
 '9780553103540',
 (SELECT PublisherID FROM PUBLISHERS WHERE Name = 'Bantam Books'),
 (SELECT CategoryID FROM CATEGORIES WHERE CategoryName = 'Fantasy'),
 1996
),
(
 'The Lord of the Rings',
 '9780261103252',
 (SELECT PublisherID FROM PUBLISHERS WHERE Name = 'Penguin Books'),
 (SELECT CategoryID FROM CATEGORIES WHERE CategoryName = 'Fantasy'),
 1954
);
GO

INSERT INTO BOOK_COPIES (BookID, Status, ShelfLocation)
VALUES
((SELECT BookID FROM BOOKS WHERE ISBN = 'ISBN-0001'), 'Available', 'F-1'),
((SELECT BookID FROM BOOKS WHERE ISBN = 'ISBN-0001'), 'Borrowed',  'F-2'),
((SELECT BookID FROM BOOKS WHERE ISBN = 'ISBN-0002'), 'Available', 'F-3'),
((SELECT BookID FROM BOOKS WHERE ISBN = 'ISBN-0003'), 'Available', 'F-4'),
((SELECT BookID FROM BOOKS WHERE ISBN = '9780747532699'), 'Available', 'F-5'),
((SELECT BookID FROM BOOKS WHERE ISBN = '9780553103540'), 'Available', 'F-6'),
((SELECT BookID FROM BOOKS WHERE ISBN = '9780261103252'), 'Available', 'F-7');
GO