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
            Console.WriteLine("Enter your User ID:");
            string userId = Console.ReadLine();

            Console.WriteLine("Enter the book ID you want to return:");
            string bookId = Console.ReadLine();

            var record = BorrowingRecords.FirstOrDefault(r => r.UID.Equals(userId, StringComparison.OrdinalIgnoreCase) && r.BID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && !r.ISReturned);

            if (record.UID == null)
            {
                Console.WriteLine("No active borrowing record found for this book.");
                return;
            }

            // Update book copies
            Books = Books.Select(b => b.BID == bookId ? (b.BID, b.BName, b.BAuthor, b.Copies + 1, b.BorrowedCopies - 1, b.Price, b.Category, b.BorrowPeriod) : b).ToList();

            // Update borrowing record
            BorrowingRecords = BorrowingRecords.Select(r => r.UID == userId && r.BID == bookId ? (r.UID, r.BID, r.BorrowDate, r.ReturnDate, DateTime.Now, r.Rating, true) : r).ToList();

            Console.WriteLine($"You have successfully returned '{Books.FirstOrDefault(b => b.BID == bookId).BName}'.");
        }


        static void AddNewBook()
        {
            string bookId, name, author, category;
            int copies, borrowPeriod;
            decimal price;

            // Get Book ID
            Console.WriteLine("Enter Book ID:");
            bookId = Console.ReadLine();
            if (Books.Any(b => b.BID.Equals(bookId, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Book ID already exists.");
                return;
            }

            // Get Book Name
            Console.WriteLine("Enter Book Name:");
            name = Console.ReadLine();
            if (Books.Any(b => b.BName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Book name already exists.");
                return;
            }

            // Get Book Author
            Console.WriteLine("Enter Book Author:");
            author = Console.ReadLine();

            // Get Book Copies
            copies = GetIntegerInputWithException1("Enter number of copies (integer):");

            // Get Book Price
            price = GetDecimalInputWithException("Enter book price (decimal):");

            // Get Book Category
            Console.WriteLine("Enter Book Category:");
            category = Console.ReadLine();
            if (!Categories.Any(c => c.CName.Equals(category, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Invalid category. Please choose from the following:");
                foreach (var cat in Categories)
                {
                    Console.WriteLine($"- {cat.CName}");
                }
                return;
            }

            // Get Borrow Period
            borrowPeriod = GetIntegerInputWithException1("Enter borrow period (days):");

            // Add Book
            Books.Add((bookId, name, author, copies, 0, price, category, borrowPeriod));
            Console.WriteLine("Book Added Successfully");
        }
        // Helper function to get integer input
        static int GetIntegerInputWithException1(string prompt)
        {
            int result;
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        // Helper function to get decimal input
        static decimal GetDecimalInputWithException1(string prompt)
        {
            decimal result;
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();
                if (decimal.TryParse(input, out result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid decimal number.");
            }
        }
        static void EditBook()
        {
            Console.WriteLine("Enter the Book ID to edit:");
            string id = Console.ReadLine();

            // Find the index of the book to be edited
            int bookIndex = Books.FindIndex(b => b.BID == id);

            if (bookIndex == -1)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            var book = Books[bookIndex];

            Console.WriteLine("Enter new Book Name (or press Enter to keep current):");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName))
            {
                // Check for duplicate book names
                if (Books.Any(b => b.BName.Equals(newName, StringComparison.OrdinalIgnoreCase) && b.BID != id))
                {
                    Console.WriteLine("A book with this name already exists.");
                    return;
                }
                book.BName = newName;
            }

            Console.WriteLine("Enter new Book Author (or press Enter to keep current):");
            string newAuthor = Console.ReadLine();
            if (!string.IsNullOrEmpty(newAuthor)) book.BAuthor = newAuthor;

            Console.WriteLine("Enter new Book Copies (or press Enter to keep current):");
            string copiesInput = Console.ReadLine();
            if (int.TryParse(copiesInput, out int newCopies))
            {
                // Ensure the number of copies doesn't decrease
                if (newCopies < book.Copies)
                {
                    Console.WriteLine("The number of copies cannot be decreased.");
                    return;
                }
                book.Copies = newCopies;
            }

            // Update the book details in the list
            Books[bookIndex] = book;
            Console.WriteLine("Book details updated successfully.");
        }


        static void RemoveBook()
        {
            Console.WriteLine("Enter the Book ID to remove:");
            string id = Console.ReadLine();

            // Find the index of the book to be removed
            int bookIndex = Books.FindIndex(b => b.BID == id);

            if (bookIndex == -1)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            var book = Books[bookIndex];

            // Check if the book has any borrowed copies
            if (book.BorrowedCopies > 0)
            {
                Console.WriteLine($"The book cannot be removed because it has {book.BorrowedCopies} borrowed copies.");
                return;
            }

            Console.WriteLine("Do you want to remove this book? (yes/no)");
            string confirm = Console.ReadLine();

            if (confirm.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                Books.RemoveAt(bookIndex);
                Console.WriteLine("Book removed successfully.");
            }
            else
            {
                Console.WriteLine("Book removal cancelled.");
            }
        }



        static void ViewAllBooks()
        {
            // Calculate maximum widths for each column based on the book data
            int nameWidth = Books.Max(book => book.BName.Length) + 2; // Adding some padding
            int authorWidth = Books.Max(book => book.BAuthor.Length) + 2;
            int idWidth = Books.Max(book => book.BID.ToString().Length) + 2;
            int copiesWidth = Books.Max(book => book.Copies.ToString().Length) + 2;
            int borrowedCopiesWidth = Books.Max(book => book.BorrowedCopies.ToString().Length) + 2;
            int priceWidth = Books.Max(book => book.Price.ToString("F2").Length) + 2; // Assuming price is a decimal
            int categoryWidth = Books.Max(book => book.Category.Length) + 2;
            int borrowPeriodWidth = Books.Max(book => book.BorrowPeriod.ToString().Length) + 2;

            // Use StringBuilder to create formatted output
            StringBuilder sb = new StringBuilder();

            // Append column headers
            sb.AppendLine($"{"Book Number",10} {"Name".PadRight(nameWidth)} {"Author".PadRight(authorWidth)} {"ID".PadRight(idWidth)} {"Copies Available".PadRight(copiesWidth)} {"Borrowed Copies".PadRight(borrowedCopiesWidth)} {"Price".PadRight(priceWidth)} {"Category".PadRight(categoryWidth)} {"Borrow Period".PadRight(borrowPeriodWidth)}");
            sb.AppendLine(new string('-', 10 + nameWidth + authorWidth + idWidth + copiesWidth + borrowedCopiesWidth + priceWidth + categoryWidth + borrowPeriodWidth));

            // Append book data
            int bookNumber = 0;
            foreach (var book in Books)
            {
                bookNumber++;
                sb.AppendLine($"{bookNumber,10} {book.BName.PadRight(nameWidth)} {book.BAuthor.PadRight(authorWidth)} {book.BID.ToString().PadRight(idWidth)} {book.Copies.ToString().PadRight(copiesWidth)} {book.BorrowedCopies.ToString().PadRight(borrowedCopiesWidth)} {book.Price.ToString("F2").PadRight(priceWidth)} {book.Category.PadRight(categoryWidth)} {book.BorrowPeriod.ToString().PadRight(borrowPeriodWidth)}");
            }

            // Print the final formatted string
            Console.WriteLine(sb.ToString());
        }
        static void ViewUserProfile()
        {
            Console.WriteLine("Enter your User ID:");
            string userId = Console.ReadLine();

            var user = Users.FirstOrDefault(u => u.UID.Equals(userId, StringComparison.OrdinalIgnoreCase));

            if (user.UID != null)
            {
                Console.WriteLine($"User ID: {user.UID}");
                Console.WriteLine($"Name: {user.Uname}");
                Console.WriteLine($"Email: {user.Email}");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }
        static void AdminReports()
        {
            // Ensure Categories are loaded
            LoadCategories();

            Console.WriteLine("Books per Category Report:");

            // Ensure we have data
            if (Books.Count == 0)
            {
                Console.WriteLine("No books available to report.");
                return;
            }

            // Load Categories to ensure they are up-to-date
            LoadCategories();

            // Group books by category and count them
            var bookCountsByCategory = Books
                .GroupBy(b => b.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            // Print the report
            foreach (var category in Categories)
            {
                // Find the count of books for this category
                var count = bookCountsByCategory
                    .FirstOrDefault(b => b.Category.Equals(category.CName, StringComparison.OrdinalIgnoreCase))?.Count ?? 0;

                Console.WriteLine($"Category: {category.CName}, Number of Books: {count}");
            }
        }


        static void SearchForBook()
        {
            Console.WriteLine("Enter the book name you want to search:");
            string searchTerm = Console.ReadLine().Trim();

            bool found = false;

            foreach (var book in Books)
            {
                // Check if the book name contains the search term, ignoring case
                if (book.BName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Display all details of the matching book
                    Console.WriteLine($"Book ID: {book.BID}");
                    Console.WriteLine($"Book Name: {book.BName}");
                    Console.WriteLine($"Book Author: {book.BAuthor}");
                    Console.WriteLine($"Copies Available: {book.Copies}");
                    Console.WriteLine($"Borrowed Copies: {book.BorrowedCopies}");
                    Console.WriteLine($"Price: {book.Price}");
                    Console.WriteLine($"Category: {book.Category}");
                    Console.WriteLine($"Borrow Period (days): {book.BorrowPeriod}");
                    Console.WriteLine(new string('-', 50)); // Separator line for readability

                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No books found with the given search term.");
            }
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
        static decimal GetDecimalInputWithException(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                try
                {
                    return Convert.ToDecimal(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid decimal.");
                }
            }
        }

        static void LoadDataFromFiles()
        {
            LoadUsers();
            LoadAdmins();
            LoadCategories();
            LoadBooks();
            LoadBorrowingRecords();
        }
        static void SaveDataToFiles()
        {
            SaveToFile(usersFilePath, Users.Select(u => $"{u.UID}|{u.Uname}|{u.Email}|{u.Password}"));
            SaveToFile(adminsFilePath, Admins.Select(a => $"{a.AID}|{a.AName}|{a.Email}|{a.Password}"));
            SaveToFile(categoriesFilePath, Categories.Select(c => $"{c.CID}|{c.CName}|{c.NOFBooks}"));
            SaveToFile(booksFilePath, Books.Select(b => $"{b.BID}|{b.BName}|{b.BAuthor}|{b.Copies}|{b.BorrowedCopies}|{b.Price}|{b.Category}|{b.BorrowPeriod}"));
            SaveToFile(borrowingFilePath, BorrowingRecords.Select(r => $"{r.UID}|{r.BID}|{r.BorrowDate}|{r.ReturnDate}|{r.ActualReturnDate}|{r.Rating}|{r.ISReturned}"));
        }

        static void SaveToFile(string filePath, IEnumerable<string> lines)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

    }
}
