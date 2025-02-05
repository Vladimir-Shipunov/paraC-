using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static List<Book> allBooks = new List<Book>();
    static List<Person> allPeople = new List<Person>();

    static void Main(string[] args)
    {
        LoadStuff();

        Console.WriteLine("Выберите способ входа?");
        Console.WriteLine("1 - Библиотекарь");
        Console.WriteLine("2 - Пользователь");

        string choice = Console.ReadLine();
        Person currentPerson = null;

        if (choice == "1")
        {
            Console.Write("Как вас зовут (библиотекарь)? ");
            string name = Console.ReadLine();
            Librarian librarian = new Librarian(name);
            allPeople.Add(librarian);
            currentPerson = librarian;
        }
        else if (choice == "2")
        {
            Console.Write("Как вас зовут (пользователь)? ");
            string name = Console.ReadLine();
            User user = (User)allPeople.Find(p => p is User && ((User)p).Name == name);

            if (user == null)
            {
                user = new User(name);
                allPeople.Add(user);
            }

            currentPerson = user;
        }
        else
        {
            Console.WriteLine("Не понял.");
            return;
        }

        currentPerson.DoStuff();
        SaveStuff();
    }


    public abstract class Person
    {
        private string name;

        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        public Person(string name)
        {
            this.name = name;
        }

        public abstract void DoStuff(); 
    }

    public class Librarian : Person
    {
        public Librarian(string name) : base(name) { }

        public override void DoStuff()
        {
            while (true)
            {
                Console.WriteLine("\nЧто делать?");
                Console.WriteLine("1 - Добавить книгу");
                Console.WriteLine("2 - Удалить книгу");
                Console.WriteLine("3 - Новый пользователь");
                Console.WriteLine("4 - Список пользователей");
                Console.WriteLine("5 - Список книг");
                Console.WriteLine("6 - Выйти");

                string action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        RemoveBook();
                        break;
                    case "3":
                        NewUser();
                        break;
                    case "4":
                        ShowUsers();
                        break;
                    case "5":
                        ShowBooks();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Что-то не так.");
                        break;
                }
            }
        }
    }

    public class User : Person
    {
        private List<string> myBooks = new List<string>();

        public List<string> MyBooks
        {
            get { return myBooks; }
            private set { myBooks = value; }
        }

        public User(string name) : base(name) { }

        public override void DoStuff()
        {
            while (true)
            {
                Console.WriteLine("\nЧто делать?");
                Console.WriteLine("1 - Посмотреть книги");
                Console.WriteLine("2 - Взять книгу");
                Console.WriteLine("3 - Вернуть книгу");
                Console.WriteLine("4 - Моя коллекция");
                Console.WriteLine("5 - Выйти");

                string action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        ShowAvailableBooks();
                        break;
                    case "2":
                        GrabBook(this);
                        break;
                    case "3":
                        ReturnBook(this);
                        break;
                    case "4":
                        ShowMyBooks(this);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Что-то не так.");
                        break;
                }
            }
        }
    }

    public class Book
    {
        private string title;
        private string author;
        private bool available;

        public string Title
        {
            get { return title; }
            private set { title = value; }
        }

        public string Author
        {
            get { return author; }
            private set { author = value; }
        }

        public bool Available
        {
            get { return available; }
            set { available = value; }
        }

        public Book(string title, string author, bool available)
        {
            this.title = title;
            this.author = author;
            this.available = available;
        }
    }

    static void AddBook()
    {
        Console.Write("Название книги: ");
        string title = Console.ReadLine();
        Console.Write("Автор: ");
        string author = Console.ReadLine();

        allBooks.Add(new Book(title, author, true));
        Console.WriteLine("Добавлено!");
    }

    static void RemoveBook()
    {
        Console.Write("Какую книгу удалить? ");
        string title = Console.ReadLine();
        Book book = allBooks.Find(b => b.Title == title);

        if (book != null)
        {
            allBooks.Remove(book);
            Console.WriteLine("Удалено!");
        }
        else
        {
            Console.WriteLine("Не нашёл.");
        }
    }

    static void NewUser()
    {
        Console.Write("Имя нового пользователя: ");
        string name = Console.ReadLine();
        allPeople.Add(new User(name));
        Console.WriteLine("Готово!");
    }

    static void ShowUsers()
    {
        foreach (var person in allPeople)
        {
            if (person is User user)
            {
                Console.WriteLine(user.Name);
            }
        }
    }

    static void ShowBooks()
    {
        foreach (var book in allBooks)
        {
            Console.WriteLine($"{book.Title} ({book.Author}) - {(book.Available ? "Есть" : "Нет")}");
        }
    }

    static void ShowAvailableBooks()
    {
        foreach (var book in allBooks)
        {
            if (book.Available)
            {
                Console.WriteLine($"{book.Title} ({book.Author})");
            }
        }
    }

    static void GrabBook(User user)
    {
        Console.Write("Какую книгу взять? ");
        string title = Console.ReadLine();
        Book book = allBooks.Find(b => b.Title == title && b.Available);

        if (book != null)
        {
            book.Available = false;
            user.MyBooks.Add(book.Title);
            Console.WriteLine("Взято!");
        }
        else
        {
            Console.WriteLine("Нет такой книги или она занята.");
        }
    }

    static void ReturnBook(User user)
    {
        Console.Write("Какую книгу вернуть? ");
        string title = Console.ReadLine();
        Book book = allBooks.Find(b => b.Title == title && !b.Available);

        if (book != null && user.MyBooks.Contains(title))
        {
            book.Available = true;
            user.MyBooks.Remove(title);
            Console.WriteLine("Возвращено!");
        }
        else
        {
            Console.WriteLine("Это не ваша книга.");
        }
    }

    static void ShowMyBooks(User user)
    {
        if (user.MyBooks.Count == 0)
        {
            Console.WriteLine("У вас нет книг.");
        }
        else
        {
            foreach (var book in user.MyBooks)
            {
                Console.WriteLine(book);
            }
        }
    }

    static void SaveStuff()
    {
        string directoryPath = @"C:\Users\sipun\OneDrive\Desktop\pr1";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        try
        {
            File.WriteAllLines(Path.Combine(directoryPath, "books.txt"), allBooks.ConvertAll(b => $"{b.Title}|{b.Author}|{(b.Available ? "yes" : "no")}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения книг: {ex.Message}");
        }

        var peopleData = new List<string>();
        foreach (var person in allPeople)
        {
            if (person is User user)
            {
                peopleData.Add($"User|{user.Name}|{string.Join(",", user.MyBooks)}");
            }
            else if (person is Librarian librarian)
            {
                peopleData.Add($"Librarian|{librarian.Name}");
            }
        }

        try
        {
            File.WriteAllLines(Path.Combine(directoryPath, "people.txt"), peopleData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения людей: {ex.Message}");
        }
    }

    static void LoadStuff()
    {
        string directoryPath = @"C:\Users\sipun\OneDrive\Desktop\pr1";
        string booksFilePath = Path.Combine(directoryPath, "books.txt");
        string peopleFilePath = Path.Combine(directoryPath, "people.txt");

        if (File.Exists(booksFilePath))
        {
            try
            {
                foreach (var line in File.ReadAllLines(booksFilePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        allBooks.Add(new Book(parts[0], parts[1], parts[2] == "yes"));
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка загрузки книги: некорректный формат строки - {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения книг: {ex.Message}");
            }
        }

        if (File.Exists(peopleFilePath))
        {
            try
            {
                foreach (var line in File.ReadAllLines(peopleFilePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 2)
                    {
                        if (parts[0] == "User")
                        {
                            User user = new User(parts[1]);
                            if (parts.Length > 2 && !string.IsNullOrEmpty(parts[2]))
                            {
                                user.MyBooks.AddRange(parts[2].Split(','));
                            }
                            allPeople.Add(user);
                        }
                        else if (parts[0] == "Librarian")
                        {
                            allPeople.Add(new Librarian(parts[1]));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка загрузки человека: некорректный формат строки - {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения людей: {ex.Message}");
            }
        }
    }
}