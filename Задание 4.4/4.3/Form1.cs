using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    // ��������� �����
    public interface IBook
    {
        string Title { get; set; }
        string Author { get; set; }
        string ISBN { get; set; }
        int Year { get; set; }
        bool IsAvailable { get; }
        string Borrower { get; set; }
        DateTime DueDate { get; set; }
        bool CheckAvailability();
        bool BorrowBook(string borrower, int days);
        bool ReturnBook();
        string GetBookInfo();
        string GetStatus();
    }

    // ������� ����� �����
    public abstract class BaseBook : IBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; protected set; }
        public string Borrower { get; set; }
        public DateTime DueDate { get; set; }

        public BaseBook(string title, string author, string isbn, int year)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            Year = year;
            IsAvailable = true;
            Borrower = "";
        }

        public virtual bool CheckAvailability()
        {
            return IsAvailable;
        }

        public virtual bool BorrowBook(string borrower, int days)
        {
            if (!IsAvailable) return false;

            Borrower = borrower;
            DueDate = DateTime.Now.AddDays(days);
            IsAvailable = false;
            return true;
        }

        public virtual bool ReturnBook()
        {
            if (IsAvailable) return false;

            Borrower = "";
            DueDate = DateTime.MinValue;
            IsAvailable = true;
            return true;
        }

        public virtual string GetBookInfo()
        {
            return $"{Title} - {Author} ({Year}) ISBN: {ISBN}";
        }

        public virtual string GetStatus()
        {
            if (IsAvailable)
                return "��������";
            else
                return $"������: {Borrower} (�� {DueDate:dd.MM.yyyy})";
        }
    }

    // ������� �����
    public class RegularBook : BaseBook
    {
        public int MaxBorrowDays { get; set; }

        public RegularBook(string title, string author, string isbn, int year, int maxBorrowDays = 14)
            : base(title, author, isbn, year)
        {
            MaxBorrowDays = maxBorrowDays;
        }

        public override bool BorrowBook(string borrower, int days)
        {
            if (days > MaxBorrowDays)
            {
                MessageBox.Show($"������������ ���� ������: {MaxBorrowDays} ����", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return base.BorrowBook(borrower, days);
        }

        public override string GetBookInfo()
        {
            return $"{base.GetBookInfo()} [������� �����] - ����. ����: {MaxBorrowDays} ��.";
        }
    }

    // �������
    public class Textbook : BaseBook
    {
        public string Subject { get; set; }
        public string GradeLevel { get; set; }
        public bool IsReference { get; set; }

        public Textbook(string title, string author, string isbn, int year, string subject, string gradeLevel, bool isReference = false)
            : base(title, author, isbn, year)
        {
            Subject = subject;
            GradeLevel = gradeLevel;
            IsReference = isReference;
        }

        public override bool BorrowBook(string borrower, int days)
        {
            if (IsReference)
            {
                MessageBox.Show("���������� ������� ������ ����� �� ���", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (days > 30)
            {
                MessageBox.Show("������������ ���� ������ ��������: 30 ����", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return base.BorrowBook(borrower, days);
        }

        public override string GetBookInfo()
        {
            string referenceInfo = IsReference ? "����������" : "��� ������";
            return $"{base.GetBookInfo()} [�������] - {Subject}, {GradeLevel} ({referenceInfo})";
        }
    }

    // ������� ����������
    public class ScientificBook : BaseBook
    {
        public string Field { get; set; }
        public bool IsRare { get; set; }
        public int CopyNumber { get; set; }

        public ScientificBook(string title, string author, string isbn, int year, string field, bool isRare = false, int copyNumber = 1)
            : base(title, author, isbn, year)
        {
            Field = field;
            IsRare = isRare;
            CopyNumber = copyNumber;
        }

        public override bool BorrowBook(string borrower, int days)
        {
            if (IsRare)
            {
                MessageBox.Show("������ ������� ���������� ������ ����� �� ���", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (days > 7)
            {
                MessageBox.Show("������������ ���� ������ ������� ����������: 7 ����", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return base.BorrowBook(borrower, days);
        }

        public override string GetBookInfo()
        {
            string rareInfo = IsRare ? "������" : "�������";
            return $"{base.GetBookInfo()} [������� ����������] - {Field}, {rareInfo}, ���. �{CopyNumber}";
        }
    }

    // �������������� ����������
    public class FictionBook : BaseBook
    {
        public string Genre { get; set; }
        public int PageCount { get; set; }
        public bool IsBestseller { get; set; }

        public FictionBook(string title, string author, string isbn, int year, string genre, int pageCount, bool isBestseller = false)
            : base(title, author, isbn, year)
        {
            Genre = genre;
            PageCount = pageCount;
            IsBestseller = isBestseller;
        }

        public override bool BorrowBook(string borrower, int days)
        {
            int maxDays = IsBestseller ? 10 : 21;

            if (days > maxDays)
            {
                MessageBox.Show($"������������ ���� ������: {maxDays} ����", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return base.BorrowBook(borrower, days);
        }

        public override string GetBookInfo()
        {
            string bestsellerInfo = IsBestseller ? "����������" : "�������";
            return $"{base.GetBookInfo()} [��������������] - {Genre}, {PageCount} ���. ({bestsellerInfo})";
        }
    }

    // �������� ����� ����������
    public partial class Form1 : Form
    {
        private List<IBook> books;
        private IBook selectedBook;

        public Form1()
        {
            InitializeComponent();
            books = new List<IBook>();
            InitializeBookTypeComboBox();
            LoadSampleData();
            RefreshBooksList();
        }

        private void InitializeBookTypeComboBox()
        {
            cmbBookType.Items.Add("������� �����");
            cmbBookType.Items.Add("�������");
            cmbBookType.Items.Add("������� ����������");
            cmbBookType.Items.Add("�������������� ����������");
            cmbBookType.SelectedIndex = 0;
        }

        private void LoadSampleData()
        {
            // ������� ����
            books.Add(new RegularBook("�������� � C#", "���� ����", "978-0-123456-78-9", 2023, 14));
            books.Add(new Textbook("���������� ��� ����������", "������ �.�.", "978-1-234567-89-0", 2022, "����������", "1 ����", false));
            books.Add(new ScientificBook("������������� ���������", "������ �.�.", "978-2-345678-90-1", 2021, "Computer Science", false, 1));
            books.Add(new FictionBook("������ � ���������", "�������� �.�.", "978-3-456789-01-2", 1967, "�����", 384, true));

            // ������ ���� ����� ��� �������
            books[0].BorrowBook("������ ����", 14);
        }

        private void cmbBookType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInputFields();
        }

        private void UpdateInputFields()
        {
            // �������� ��� �������������� ����
            lblExtra1.Visible = false;
            txtExtra1.Visible = false;
            lblExtra2.Visible = false;
            txtExtra2.Visible = false;
            lblExtra3.Visible = false;
            txtExtra3.Visible = false;
            chkExtra1.Visible = false;

            string bookType = cmbBookType.SelectedItem.ToString();

            switch (bookType)
            {
                case "������� �����":
                    lblExtra1.Text = "����. ���� ������:";
                    txtExtra1.Text = "14";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    break;

                case "�������":
                    lblExtra1.Text = "�������:";
                    lblExtra2.Text = "�����/����:";
                    lblExtra3.Text = "����������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    txtExtra2.Visible = true;
                    lblExtra3.Visible = true;
                    chkExtra1.Visible = true;
                    break;

                case "������� ����������":
                    lblExtra1.Text = "������� �����:";
                    lblExtra2.Text = "������:";
                    lblExtra3.Text = "����� ����������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    chkExtra1.Visible = true;
                    lblExtra3.Visible = true;
                    txtExtra3.Visible = true;
                    txtExtra3.Text = "1";
                    break;

                case "�������������� ����������":
                    lblExtra1.Text = "����:";
                    lblExtra2.Text = "���������� �������:";
                    lblExtra3.Text = "����������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    txtExtra2.Visible = true;
                    lblExtra3.Visible = true;
                    chkExtra1.Visible = true;
                    break;
            }
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            try
            {
                IBook book = CreateBookFromInput();
                if (book != null)
                {
                    books.Add(book);
                    RefreshBooksList();
                    ClearInputFields();
                    MessageBox.Show("����� ������� ���������!", "�����",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IBook CreateBookFromInput()
        {
            string title = txtTitle.Text;
            string author = txtAuthor.Text;
            string isbn = txtISBN.Text;
            int year = int.Parse(txtYear.Text);

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("��� �������� ���� ������ ���� ���������");

            string bookType = cmbBookType.SelectedItem.ToString();

            switch (bookType)
            {
                case "������� �����":
                    int maxDays = int.Parse(txtExtra1.Text);
                    return new RegularBook(title, author, isbn, year, maxDays);

                case "�������":
                    string subject = txtExtra1.Text;
                    string gradeLevel = txtExtra2.Text;
                    bool isReference = chkExtra1.Checked;
                    return new Textbook(title, author, isbn, year, subject, gradeLevel, isReference);

                case "������� ����������":
                    string field = txtExtra1.Text;
                    bool isRare = chkExtra1.Checked;
                    int copyNumber = int.Parse(txtExtra3.Text);
                    return new ScientificBook(title, author, isbn, year, field, isRare, copyNumber);

                case "�������������� ����������":
                    string genre = txtExtra1.Text;
                    int pageCount = int.Parse(txtExtra2.Text);
                    bool isBestseller = chkExtra1.Checked;
                    return new FictionBook(title, author, isbn, year, genre, pageCount, isBestseller);

                default:
                    throw new ArgumentException("����������� ��� �����");
            }
        }

        private void RefreshBooksList()
        {
            lstBooks.Items.Clear();
            int availableCount = 0;
            int borrowedCount = 0;

            foreach (var book in books)
            {
                lstBooks.Items.Add($"{book.GetBookInfo()} - {book.GetStatus()}");
                if (book.IsAvailable)
                    availableCount++;
                else
                    borrowedCount++;
            }

            lblTotalBooks.Text = $"����� ����: {books.Count}";
            lblAvailableBooks.Text = $"��������: {availableCount}";
            lblBorrowedBooks.Text = $"������: {borrowedCount}";
        }

        private void ClearInputFields()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtISBN.Clear();
            txtYear.Clear();
            txtExtra1.Clear();
            txtExtra2.Clear();
            txtExtra3.Clear();
            chkExtra1.Checked = false;
        }

        private void btnRemoveBook_Click(object sender, EventArgs e)
        {
            if (lstBooks.SelectedIndex >= 0 && lstBooks.SelectedIndex < books.Count)
            {
                books.RemoveAt(lstBooks.SelectedIndex);
                RefreshBooksList();
            }
            else
            {
                MessageBox.Show("�������� ����� ��� ��������", "��������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lstBooks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBooks.SelectedIndex >= 0 && lstBooks.SelectedIndex < books.Count)
            {
                selectedBook = books[lstBooks.SelectedIndex];
                DisplayBookDetails(selectedBook);
                UpdateBorrowReturnButtons();
            }
        }

        private void DisplayBookDetails(IBook book)
        {
            txtDetails.Clear();
            txtDetails.AppendText($"=== ���������� � ����� ===\r\n");
            txtDetails.AppendText($"��������: {book.Title}\r\n");
            txtDetails.AppendText($"�����: {book.Author}\r\n");
            txtDetails.AppendText($"ISBN: {book.ISBN}\r\n");
            txtDetails.AppendText($"��� �������: {book.Year}\r\n");
            txtDetails.AppendText($"������: {book.GetStatus()}\r\n");
            txtDetails.AppendText($"�����������: {(book.IsAvailable ? "��������" : "������")}\r\n");

            if (!book.IsAvailable)
            {
                txtDetails.AppendText($"��������: {book.Borrower}\r\n");
                txtDetails.AppendText($"���� �����: {book.DueDate:dd.MM.yyyy}\r\n");

                // �������� ���������
                if (book.DueDate < DateTime.Now)
                {
                    txtDetails.AppendText($"!!! ���������� �� {(DateTime.Now - book.DueDate).Days} ���� !!!\r\n");
                }
            }
        }

        private void UpdateBorrowReturnButtons()
        {
            if (selectedBook != null)
            {
                btnBorrowBook.Enabled = selectedBook.IsAvailable;
                btnReturnBook.Enabled = !selectedBook.IsAvailable;
            }
            else
            {
                btnBorrowBook.Enabled = false;
                btnReturnBook.Enabled = false;
            }
        }

        private void btnBorrowBook_Click(object sender, EventArgs e)
        {
            if (selectedBook == null)
            {
                MessageBox.Show("�������� ����� ��� ������", "��������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string borrower = txtBorrower.Text;
            if (string.IsNullOrWhiteSpace(borrower))
            {
                MessageBox.Show("������� ��� ��������", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int days = int.Parse(txtBorrowDays.Text);
                if (days <= 0)
                {
                    MessageBox.Show("���������� ���� ������ ���� ������������� ������", "������",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (selectedBook.BorrowBook(borrower, days))
                {
                    RefreshBooksList();
                    DisplayBookDetails(selectedBook);
                    UpdateBorrowReturnButtons();
                    txtBorrower.Clear();
                    txtBorrowDays.Clear();
                    MessageBox.Show("����� ������� ������!", "�����",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ������ �����: {ex.Message}", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            if (selectedBook == null)
            {
                MessageBox.Show("�������� ����� ��� ��������", "��������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedBook.ReturnBook())
            {
                RefreshBooksList();
                DisplayBookDetails(selectedBook);
                UpdateBorrowReturnButtons();
                MessageBox.Show("����� ������� ����������!", "�����",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                RefreshBooksList();
                return;
            }

            lstBooks.Items.Clear();
            foreach (var book in books)
            {
                if (book.Title.ToLower().Contains(searchTerm) ||
                    book.Author.ToLower().Contains(searchTerm) ||
                    book.ISBN.ToLower().Contains(searchTerm))
                {
                    lstBooks.Items.Add($"{book.GetBookInfo()} - {book.GetStatus()}");
                }
            }
        }

        private void btnShowAvailable_Click(object sender, EventArgs e)
        {
            lstBooks.Items.Clear();
            foreach (var book in books)
            {
                if (book.IsAvailable)
                {
                    lstBooks.Items.Add($"{book.GetBookInfo()} - {book.GetStatus()}");
                }
            }
        }

        private void btnShowBorrowed_Click(object sender, EventArgs e)
        {
            lstBooks.Items.Clear();
            foreach (var book in books)
            {
                if (!book.IsAvailable)
                {
                    lstBooks.Items.Add($"{book.GetBookInfo()} - {book.GetStatus()}");
                }
            }
        }

        private void btnShowOverdue_Click(object sender, EventArgs e)
        {
            lstBooks.Items.Clear();
            foreach (var book in books)
            {
                if (!book.IsAvailable && book.DueDate < DateTime.Now)
                {
                    int overdueDays = (DateTime.Now - book.DueDate).Days;
                    lstBooks.Items.Add($"{book.GetBookInfo()} - ���������� �� {overdueDays} ���� - {book.Borrower}");
                }
            }
        }
    }
}