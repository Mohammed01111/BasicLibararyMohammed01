using System.Text;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(string BName, string BAuthor, int ID, int Qnt)> Books = new List<(string BName, string BAuthor, int ID, int Qnt)>();
        static string filePath = "C:\\Users\\codeline user\\Documents\\lib.txt";

        static void Main(string[] args)
        //checkout
        {// downloaded form ahmed device 
            bool ExitFlag = false;

            LoadBooksFromFile();

            do
            {
                Console.WriteLine("Choose 1 for admin Or 2 for user Or 3 for Save & Exit:");
                string choice = Console.ReadLine();
                switch (choice)
                {

                    case "1":
                        AdminMenu();
                        break;

                    case "2":
                        UserMenu();
                        break;

                    case "3":
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("enter correct choice");
                        break;
                }
            } while (ExitFlag != true);



        }

        static void AdminMenu()
        {
            bool ExitFlag = false;

            do
            {
                Console.WriteLine("Welcome Admin");
                Console.WriteLine("\n Enter the char of operation you need :");
                Console.WriteLine("\n A- Add New Book");
                Console.WriteLine("\n B- Display All Books");
                Console.WriteLine("\n C- Edit Book");
                Console.WriteLine("\n D- Remove Book");
                Console.WriteLine("\n E- Search for Book by Name");
                Console.WriteLine("\n F- Save and Exit");

                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "A":
                        AddnNewBook();
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
                        ExitFlag = true;
                        break;
                    

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;



                }

                Console.WriteLine("press any key to continue");
                string cont = Console.ReadLine();

                Console.Clear();

            } while (ExitFlag != true);

        }


        static void UserMenu()
        {

            bool ExitFlag = false;

            do
            {
                Console.WriteLine("Welcome User");
                Console.WriteLine("\n Enter the char of operation you need :");
                Console.WriteLine("\n A- Search for Book by Name");
                Console.WriteLine("\n B- Borrow Book");
                Console.WriteLine("\n C- Return Book ");
                Console.WriteLine("\n D- Save and Exit");

                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "A":
                        SearchForBook();
                        break;

                    case "B":
                        BorrowBook();
                        break;

                    case "C":
                        ReturnBook();
                        break;

                    case "D":
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;



                }

                Console.WriteLine("press any key to continue");
                string cont = Console.ReadLine();

                Console.Clear();

            } while (ExitFlag != true);


        }
        static void BorrowBook()
        {
            Console.WriteLine("Enter the book name you want to borrow:");
            string name = Console.ReadLine();

            
            for (int i = 0; i < Books.Count; i++)
            {
                var book = Books[i];

                if (book.BName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    if (book.Qnt <= 0)
                    {
                        Console.WriteLine("Sorry, this book is currently not available.");
                        return;
                    }

                    Console.WriteLine($"You have selected '{book.BName}' by {book.BAuthor}. Quantity available: {book.Qnt}");
                    Console.WriteLine("Do you want to borrow this book? (yes/no)");
                    string confirm = Console.ReadLine();

                    if (confirm.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        
                        Books[i] = (book.BName, book.BAuthor, book.ID, book.Qnt - 1);

                        Console.WriteLine($"You have successfully borrowed '{book.BName}'.");
                    }
                    else
                    {
                        Console.WriteLine("Borrowing canceled.");
                    }
                    return; 
                }
            }

            
            Console.WriteLine("Book not found.");
        }
       
        static void ReturnBook()
        {
            Console.WriteLine("Enter the book name you want to return:");
            string name = Console.ReadLine();

            // Find the book with the specified name
            for (int i = 0; i < Books.Count; i++)
            {
                var book = Books[i];

                if (book.BName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    // Update quantity
                    Books[i] = (book.BName, book.BAuthor, book.ID, book.Qnt + 1);

                    Console.WriteLine($"You have successfully returned '{book.BName}'.");
                    return; // Exit the method after processing
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
                        writer.WriteLine($"{book.BName}|{book.BAuthor}|{book.ID}");
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
