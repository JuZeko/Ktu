// Žukauskaitė Domantė IFF-0/8
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace K1
{
    class Book
    {
        public string Distributor { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }

        public Book(string name)
        {
            this.Amount = 1;
            this.Price = 0;
            this.Distributor = "";
            this.Name = name;
        }
        public Book(string distributor, string name, int amount, decimal price)
        {
            this.Distributor = distributor;
            this.Name = name;
            this.Amount = amount;
            this.Price = price;
        }

        public override string ToString()
        {
            return String.Format("|{0,-12}|{1,-20}|{2,7:d}|{3,6:f2}|", this.Distributor, this.Name, this.Amount, this.Price);
        }

        public static bool operator >=(Book book1, Book book2)
        {
            return book1.Price >= book2.Price;
        }

        public static bool operator <=(Book book1, Book book2)
        {
            return book1.Price <= book2.Price;
        }
        public override bool Equals(Object obj)
        {
            return this.Name == ((Book)obj).Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }

    class BookStore
    {
        private List<Book> AllBooks;

        public BookStore()
        {
            AllBooks = new List<Book>();
        }
        public BookStore(List<Book> Books)
        {
            AllBooks = new List<Book>();
            foreach(Book book in Books)
            {
                this.AllBooks.Add(book);
            }
        }
        
        public int GetCount()
        {
            return this.AllBooks.Count;
        }

        public Book GetBook (int index)
        {
            return this.AllBooks[index];
        }

        public void AddBook(Book book)
        {
            this.AllBooks.Add(book);
        }

        public decimal Sum()
        {
            decimal sum = 0;
            foreach(Book book in this.AllBooks)
            {
                sum += book.Price*book.Amount;
            }
            return sum;
        }

        public int IndexMaxPrice(Book book)
        {
            int index = 0;
            foreach (Book book1 in this.AllBooks)
            {
                if(book.Equals(book1) && book1 >= book)
                {
                    book.Price = book1.Price;
                    index = this.AllBooks.IndexOf(book1);
                }
            }
            return index;
        }

        public void AddSalePrice(List<Book> Books)
        {
            foreach(Book book in Books)
            {
                if (this.AllBooks.Contains(book))
                {
                    book.Price = GetBook(IndexMaxPrice(book)).Price;
                }
            }
        }
    }

    static class InOut
    {
        public static BookStore InputBooks(string fileName)
        {
            List<Book> allBooks = new List<Book>();
            string[] lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1257));
            foreach(String line in lines)
            {
                string[] Values = line.Split(';');
                string distributor = Values[0];
                string name = Values[1];
                int amount = int.Parse(Values[2]);
                decimal price = decimal.Parse(Values[3]);
                Book book = new Book(distributor, name, amount, price);
                allBooks.Add(book);
            }
            BookStore bookStore = new BookStore(allBooks);
            return bookStore;
        }

        public static List <Book> InputSoldBooks(string fileName)
        {
            List<Book> soldBooks = new List<Book>();
            string[] lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1257));
            foreach(String line in lines)
            {
                Book book = new Book(line);
                soldBooks.Add(book);
            }
            return soldBooks;
        }

        public static void Print(BookStore books, string fileName, string header)
        {
            string[] lines = new string[books.GetCount() + 5];
            lines[0] = header;
            lines[1] = new string('-', 60);
            lines[2] = string.Format("|{0,-12}|{1,-20}|{2,7:d}|{3,6:f2}|", "Platintojas", "Pavadinimas", "Kiekis", "Kaina");
            lines[3] = new string('-', 60);
            for (int i = 0; i < books.GetCount(); i++)
            {
                Book book = books.GetBook(i);
                lines[i + 4] = book.ToString();
            }
            lines[lines.Length-1] = new string('-', 60);
            File.AppendAllLines(fileName, lines, Encoding.UTF8);

        }

        public static void Print(List<Book> books, string fileName, string header)
        {
            string[] lines = new string[books.Count + 5];
            lines[0] = header;
            lines[1] = new string('-', 60);
            lines[2] = string.Format("|{0,-12}|{1,-20}|{2,7:d}|{3,6:f2}|", "Platintojas", "Pavadinimas", "Kiekis", "Kaina");
            lines[3] = new string('-', 60);
            for (int i = 0; i < books.Count; i++)
            {
                Book book = books[i];
                lines[i + 4] = book.ToString();
            }
            lines[lines.Length - 1] = new string('-', 60);
            File.AppendAllLines(fileName, lines, Encoding.UTF8);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BookStore bookStore = InOut.InputBooks(@"Knyga.txt");
            List<Book> soldBooks = InOut.InputSoldBooks(@"Parduota.txt");
            File.Delete("Rezultatai.txt");
            InOut.Print(bookStore, "Rezultatai.txt", "Pradinė knygyno lentelė");
            InOut.Print(soldBooks, "Rezultatai.txt", "Pradinė knygų pardavimo lentelė");
            bookStore.AddSalePrice(soldBooks);
            BookStore soldBookRegister = new BookStore(soldBooks);
            InOut.Print(soldBookRegister, "Rezultatai.txt", "Papildyta pardavio lentelė");
        }
    }
}
