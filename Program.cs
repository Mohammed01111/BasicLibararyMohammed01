using System.Text;
using System.Collections.Generic;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(string UID, string Uname, string Email, string Password)> Users = new List<(string UID, string Uname, string Email, string Password)>();
        static List<(string AID, string AName, string Email, string Password)> Admins = new List<(string AID, string AName, string Email, string Password)>();
        static List<(string CID, string CName, int NOFBooks)> Categories = new List<(string CID, string CName, int NOFBooks)>();
        static List<(string BID, string BName, string BAuthor, int Copies, int BorrowedCopies, decimal Price, string Category, int BorrowPeriod)> Books = new List<(string BID, string BName, string BAuthor, int Copies, int BorrowedCopies, decimal Price, string Category, int BorrowPeriod)>();
        static List<(string UID, string BID, DateTime BorrowDate, DateTime ReturnDate, DateTime ActualReturnDate, int Rating, bool ISReturned)> BorrowingRecords = new List<(string UID, string BID, DateTime BorrowDate, DateTime ReturnDate, DateTime ActualReturnDate, int Rating, bool ISReturned)>();


        // File paths
        static string usersFilePath = "C:\\Users\\codeline user\\Documents\\test\\UsersFile.txt";
        static string adminsFilePath = "C:\\Users\\codeline user\\Documents\\test\\AdminsFile.txt";
        static string categoriesFilePath = "C:\\Users\\codeline user\\Documents\\test\\CategoriesFile.txt";
        static string booksFilePath = "C:\\Users\\codeline user\\Documents\\test\\BooksFile.txt";
        static string borrowingFilePath = "C:\\Users\\codeline user\\Documents\\test\\BorrowingFile.txt";



        static void Main(string[] args)
        //checkout
        {
            LoadDataFromFiles();

            bool exitFlag = false;

            do
            {
                Console.WriteLine("Choose 1 for admin Or 2 for user Or 3 for Save & Exit:");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        var adminId = Authenticate("admin");
                        if (adminId != null)
                        {
                            AdminMenu();
                        }
                        break;

                    case "2":
                        var userId = Authenticate("user");
                        if (userId != null)
                        {
                            UserMenu(userId); // Pass userId to UserMenu
                        }
                        break;

                    case "3":
                        SaveDataToFiles();
                        exitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Enter Correct Choice");
                        break;
                }
            } while (!exitFlag);
        }
        static string Authenticate(string userType)
        {
            Console.WriteLine($"Enter {userType} Name:");
            string name = Console.ReadLine().Trim();
            Console.WriteLine($"Enter {userType} Password:");
            string password = Console.ReadLine().Trim();

            if (userType == "admin")
            {
                var admin = Admins.FirstOrDefault(a => a.AName.Equals(name, StringComparison.OrdinalIgnoreCase) && a.Password.Equals(password));
                if (admin.AID != null)
                {
                    return admin.AID; // Return admin ID if authenticated
                }
            }
            else if (userType == "user")
            {
                var user = Users.FirstOrDefault(u => u.Uname.Equals(name, StringComparison.OrdinalIgnoreCase) && u.Password.Equals(password));
                if (user.UID != null)
                {
                    return user.UID; // Return user ID if authenticated
                }
            }

            Console.WriteLine("Invalid Name or Password.");
            return null;
        }
        static void AdminMenu()
        {
            bool exitFlag = false;

            do
            {
                Console.WriteLine("Welcome Admin");
                Console.WriteLine("\n Enter the char of operation you need :");
                Console.WriteLine("\n A- Add New Book");
                Console.WriteLine("\n B- Display All Books");
                Console.WriteLine("\n C- Edit Book");
                Console.WriteLine("\n D- Remove Book");
                Console.WriteLine("\n E- Search for Book by Name");
                Console.WriteLine("\n F- Show reports");
                Console.WriteLine("\n G- Save and Exit");

                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "A":
                        AddNewBook();
                        break;

                    case "B":
                        ViewAllBooks();
                        break;

                    case "C":
                        EditBook();
                        break;

                    case "D":
                        RemoveBook();
                        break;

                    case "E":
                        SearchForBook();
                        break;

                    case "F":
                        AdminReports();
                        break;

                    case "G":
                        SaveDataToFiles();
                        exitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
                Console.Clear();
            } while (!exitFlag);
        }


        static void UserMenu(string userId)
        {
            bool exitFlag = false;

            do
            {
                // Check for overdue books
                bool hasOverdueBooks = BorrowingRecords.Any(r => r.UID.Equals(userId, StringComparison.OrdinalIgnoreCase) && r.ReturnDate < DateTime.Now && !r.ISReturned);

                Console.WriteLine("Welcome User");
                Console.WriteLine("\nEnter the char of operation you need:");
                Console.WriteLine("\n A- Search for Book by Name");

                if (!hasOverdueBooks)
                {
                    Console.WriteLine("\n B- Borrow Book");
                }

                Console.WriteLine("\n C- Return Book ");
                Console.WriteLine("\n D- View User profile ");
                Console.WriteLine("\n F- Save and Exit");

                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "A":
                        SearchForBook();
                        break;

                    case "B":
                        if (!hasOverdueBooks)
                        {
                            BorrowBook();
                        }
                        else
                        {
                            Console.WriteLine("You have overdue books. You can only return books.");
                        }
                        break;

                    case "C":
                        ReturnBook();
                        break;

                    case "D":
                        ViewUserProfile();
                        break;

                    case "F":
                        exitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Sorry, your choice was wrong.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
                Console.Clear();
            } while (!exitFlag);
        }


        static void BorrowBook()
        {
            Console.WriteLine("Enter your User ID:");
            string userId = Console.ReadLine();

            Console.WriteLine("Enter the book ID you want to borrow:");
            string bookId = Console.ReadLine();

            var book = Books.FirstOrDefault(b => b.BID.Equals(bookId, StringComparison.OrdinalIgnoreCase));

            if (book.BID == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            if (book.Copies <= 0)
            {
                Console.WriteLine("Sorry, this book is currently not available.");
                return;
            }

            Console.WriteLine($"You have selected '{book.BName}' by {book.BAuthor}. Copies available: {book.Copies}");
            Console.WriteLine("Do you want to borrow this book? (yes/no)");
            string confirm = Console.ReadLine();

            if (confirm.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                // Update book copies
                Books = Books.Select(b => b.BID == bookId ? (b.BID, b.BName, b.BAuthor, b.Copies - 1, b.BorrowedCopies + 1, b.Price, b.Category, b.BorrowPeriod) : b).ToList();

                // Add borrowing record
                BorrowingRecords.Add((userId, bookId, DateTime.Now, DateTime.Now.AddDays(book.BorrowPeriod), DateTime.MinValue, 0, false));

                Console.WriteLine($"You have successfully borrowed '{book.BName}'.");
            }
            else
            {
                Console.WriteLine("Borrowing canceled.");
            }
        }


        static void ReturnBook()
        {
            Console.WriteLine("Enter the book name you want to return:");
            string name = Console.ReadLine();

            // Find the book in the Books list
            for (int i = 0; i < Books.Count; i++)
            {
                var book = Books[i];

                // Check if the book name matches
                if (book.BName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    // Update book quantity
                    Books[i] = (book.BName, book.BAuthor, book.ID, book.Qnt + 1);

                    // Update borrowed books list
                    var borrowedBook = BorrowedBooks.FirstOrDefault(bb => bb.BName.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (borrowedBook.BName != null)
                    {
                        if (borrowedBook.BorrowedCount > 1)
                        {
                            BorrowedBooks[BorrowedBooks.IndexOf(borrowedBook)] = (borrowedBook.BName, borrowedBook.BorrowedCount - 1);
                        }
                        else
                        {
                            BorrowedBooks.Remove(borrowedBook);
                        }
                    }

                    Console.WriteLine($"You have successfully returned '{book.BName}'.");
                    return;
                }
            }

            Console.WriteLine("Book not found.");
        }
        

        static void AddnNewBook()
        {
            Console.WriteLine("Enter Book Name");
            string name = Console.ReadLine();

            Console.WriteLine("Enter Book Author");
            string author = Console.ReadLine();

            
            int ID = GetIntegerInputWithException("Enter Book ID ");

            
            int qnt = GetIntegerInputWithException("Enter Book Quantity ");


            Books.Add((name, author, ID, qnt));
            Console.WriteLine("Book Added Succefully");

        }
        static void EditBook()
        {
            Console.WriteLine("Enter the Book ID to edit:");
            int id = GetIntegerInputWithException("");

            for (int i = 0; i < Books.Count; i++)
            {
                var book = Books[i];

                if (book.ID == id)
                {
                    Console.WriteLine("Enter new Book Name (or press Enter to keep current):");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrEmpty(name)) book.BName = name;

                    Console.WriteLine("Enter new Book Author (or press Enter to keep current):");
                    string author = Console.ReadLine();
                    if (!string.IsNullOrEmpty(author)) book.BAuthor = author;

                    Console.WriteLine("Enter new Book Quantity (or press Enter to keep current):");
                    string quantityInput = Console.ReadLine();
                    if (int.TryParse(quantityInput, out int quantity)) book.Qnt = quantity;

                    Books[i] = book;
                    Console.WriteLine("Book details updated successfully.");
                    return;
                }
            }

            Console.WriteLine("Book not found.");
        }
        static void RemoveBook()
        {
            Console.WriteLine("Enter the Book ID to remove:");
            int id = GetIntegerInputWithException("");

            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i].ID == id)
                {
                    Books.RemoveAt(i);
                    Console.WriteLine("Book removed successfully.");
                    return;
                }
            }

            Console.WriteLine("Book not found.");
        }

        static void ViewAllBooks()
        {
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;

            for (int i = 0; i < Books.Count; i++)
            {
                BookNumber = i + 1;
                sb.Append("Book ").Append(BookNumber).Append(" name : ").Append(Books[i].BName);
                sb.AppendLine();
                sb.Append("Book ").Append(BookNumber).Append(" Author : ").Append(Books[i].BAuthor);
                sb.AppendLine();
                sb.Append("Book ").Append(BookNumber).Append(" ID : ").Append(Books[i].ID);
                sb.AppendLine();
                sb.Append("Book ").Append(BookNumber).Append(" Quantity : ").Append(Books[i].Qnt);
                sb.AppendLine().AppendLine();
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
        }

        static void SearchForBook()
        {
            Console.WriteLine("Enter the book name you want");
            string name = Console.ReadLine();
            bool flag = false;

            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i].BName == name)
                {
                    Console.WriteLine("Book Author is : " + Books[i].BAuthor);
                    flag = true;
                    break;
                }
            }

            if (flag != true)
            { Console.WriteLine("book not found"); }
        }

        static int GetIntegerInputWithException(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                try
                {
                    return Convert.ToInt32(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
            }
        }

        
        static void LoadBooksFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                Books.Add((parts[0], parts[1], int.Parse(parts[2]), int.Parse(parts[3])));
                            }
                        }
                    }
                    Console.WriteLine("Books loaded from file successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }

        static void SaveBooksToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var book in Books)
                    {
                        writer.WriteLine($"{book.BName}|{book.BAuthor}|{book.ID}|{book.Qnt}");
                    }
                }
                Console.WriteLine("Books saved to file successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

    }
}
