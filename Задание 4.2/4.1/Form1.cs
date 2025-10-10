using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StoreInventoryApp
{
    // ��������� �����
    public interface IProduct
    {
        string Name { get; set; }
        string ProductCode { get; set; }
        decimal CalculatePrice();
        int GetStockQuantity();
        string GetProductInfo();
    }

    // ������� ����� ������
    public abstract class BaseProduct : IProduct
    {
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public decimal BasePrice { get; set; }
        public int StockQuantity { get; set; }

        public BaseProduct(string name, string code, decimal basePrice, int stock)
        {
            Name = name;
            ProductCode = code;
            BasePrice = basePrice;
            StockQuantity = stock;
        }

        public abstract decimal CalculatePrice();

        public int GetStockQuantity()
        {
            return StockQuantity;
        }

        public virtual string GetProductInfo()
        {
            return $"{Name} (���: {ProductCode}) - {CalculatePrice():C} - � �������: {StockQuantity} ��.";
        }
    }

    // ����� �������� �������
    public class FoodProduct : BaseProduct
    {
        public DateTime ExpiryDate { get; set; }
        public bool IsPerishable { get; set; }

        public FoodProduct(string name, string code, decimal basePrice, int stock,
                         DateTime expiryDate, bool isPerishable)
            : base(name, code, basePrice, stock)
        {
            ExpiryDate = expiryDate;
            IsPerishable = isPerishable;
        }

        public override decimal CalculatePrice()
        {
            decimal price = BasePrice;

            if (IsPerishable)
            {
                price *= 1.1m;
            }

            if ((ExpiryDate - DateTime.Now).TotalDays <= 3)
            {
                price *= 0.7m;
            }
            else if ((ExpiryDate - DateTime.Now).TotalDays <= 7)
            {
                price *= 0.9m;
            }

            return Math.Round(price, 2);
        }

        public override string GetProductInfo()
        {
            string perishableInfo = IsPerishable ? "���������������" : "����������� ��������";
            string daysLeft = $"{(ExpiryDate - DateTime.Now).Days} ��.";
            return $"{base.GetProductInfo()} | ����: {ExpiryDate:dd.MM.yyyy} ({daysLeft}) | {perishableInfo}";
        }
    }

    // ����� �����������
    public class ElectronicsProduct : BaseProduct
    {
        public int WarrantyMonths { get; set; }
        public string Brand { get; set; }

        public ElectronicsProduct(string name, string code, decimal basePrice, int stock,
                                int warrantyMonths, string brand)
            : base(name, code, basePrice, stock)
        {
            WarrantyMonths = warrantyMonths;
            Brand = brand;
        }

        public override decimal CalculatePrice()
        {
            decimal price = BasePrice;

            if (WarrantyMonths > 12)
            {
                price *= 1.15m;
            }

            if (IsPremiumBrand(Brand))
            {
                price *= 1.2m;
            }

            return Math.Round(price, 2);
        }

        private bool IsPremiumBrand(string brand)
        {
            string[] premiumBrands = { "Apple", "Samsung", "Sony", "LG", "Bose" };
            return Array.Exists(premiumBrands, b => b.Equals(brand, StringComparison.OrdinalIgnoreCase));
        }

        public override string GetProductInfo()
        {
            return $"{base.GetProductInfo()} | �����: {Brand} | ��������: {WarrantyMonths} ���.";
        }
    }

    // ����� ������
    public class ClothingProduct : BaseProduct
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }

        public ClothingProduct(string name, string code, decimal basePrice, int stock,
                             string size, string color, string material)
            : base(name, code, basePrice, stock)
        {
            Size = size;
            Color = color;
            Material = material;
        }

        public override decimal CalculatePrice()
        {
            decimal price = BasePrice;

            if (IsPremiumMaterial(Material))
            {
                price *= 1.25m;
            }

            if (IsLargeSize(Size))
            {
                price *= 0.9m;
            }

            return Math.Round(price, 2);
        }

        private bool IsPremiumMaterial(string material)
        {
            string[] premiumMaterials = { "����", "�������", "����", "Merino", "�������" };
            return Array.Exists(premiumMaterials, m => m.Equals(material, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsLargeSize(string size)
        {
            string[] largeSizes = { "XL", "XXL", "XXXL", "52", "54", "56" };
            return Array.Exists(largeSizes, s => s.Equals(size, StringComparison.OrdinalIgnoreCase));
        }

        public override string GetProductInfo()
        {
            return $"{base.GetProductInfo()} | ������: {Size} | ����: {Color} | ��������: {Material}";
        }
    }

    // ����� ����� ������ ���� �������� ����� ����������� � ������� ���������
    public partial class Form1 : Form
    {
        private List<IProduct> products;

        public Form1()
        {
            InitializeComponent();
            products = new List<IProduct>();
            InitializeProductComboBox();
            LoadSampleData();
            RefreshProductList();
        }

        private void InitializeProductComboBox()
        {
            cmbProductType.Items.Add("�������� �������");
            cmbProductType.Items.Add("�����������");
            cmbProductType.Items.Add("������");
            cmbProductType.SelectedIndex = 0;
        }

        private void LoadSampleData()
        {
            products.Add(new FoodProduct("������", "F001", 80, 50, DateTime.Now.AddDays(5), true));
            products.Add(new ElectronicsProduct("��������", "E001", 50000, 10, 24, "Samsung"));
            products.Add(new ClothingProduct("��������", "C001", 1500, 100, "M", "�����", "������"));
            products.Add(new FoodProduct("����", "F002", 40, 30, DateTime.Now.AddDays(2), true));
            products.Add(new ElectronicsProduct("��������", "E002", 3000, 25, 12, "Sony"));
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInputFields();
        }

        private void UpdateInputFields()
        {
            lblExtra1.Visible = false;
            txtExtra1.Visible = false;
            lblExtra2.Visible = false;
            txtExtra2.Visible = false;
            lblExtra3.Visible = false;
            txtExtra3.Visible = false;
            dtpExpiryDate.Visible = false;
            chkPerishable.Visible = false;

            string productType = cmbProductType.SelectedItem.ToString();

            switch (productType)
            {
                case "�������� �������":
                    lblExtra1.Text = "���� ��������:";
                    dtpExpiryDate.Visible = true;
                    chkPerishable.Text = "���������������";
                    chkPerishable.Visible = true;
                    break;

                case "�����������":
                    lblExtra1.Text = "�����:";
                    lblExtra2.Text = "�������� (���):";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    txtExtra2.Visible = true;
                    break;

                case "������":
                    lblExtra1.Text = "������:";
                    lblExtra2.Text = "����:";
                    lblExtra3.Text = "��������:";
                    lblExtra1.Visible = true;
                    txtExtra1.Visible = true;
                    lblExtra2.Visible = true;
                    txtExtra2.Visible = true;
                    lblExtra3.Visible = true;
                    txtExtra3.Visible = true;
                    break;
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                IProduct product = CreateProductFromInput();
                if (product != null)
                {
                    products.Add(product);
                    RefreshProductList();
                    ClearInputFields();
                    MessageBox.Show("����� ������� ��������!", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IProduct CreateProductFromInput()
        {
            string name = txtName.Text;
            string code = txtCode.Text;
            decimal basePrice = decimal.Parse(txtPrice.Text);
            int stock = int.Parse(txtStock.Text);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("�������� � ��� ������ �� ����� ���� �������");

            string productType = cmbProductType.SelectedItem.ToString();

            switch (productType)
            {
                case "�������� �������":
                    return new FoodProduct(name, code, basePrice, stock, dtpExpiryDate.Value, chkPerishable.Checked);

                case "�����������":
                    string brand = txtExtra1.Text;
                    int warranty = int.Parse(txtExtra2.Text);
                    return new ElectronicsProduct(name, code, basePrice, stock, warranty, brand);

                case "������":
                    string size = txtExtra1.Text;
                    string color = txtExtra2.Text;
                    string material = txtExtra3.Text;
                    return new ClothingProduct(name, code, basePrice, stock, size, color, material);

                default:
                    throw new ArgumentException("����������� ��� ������");
            }
        }

        private void RefreshProductList()
        {
            lstProducts.Items.Clear();
            decimal totalInventoryValue = 0;
            int totalItems = 0;

            foreach (var product in products)
            {
                lstProducts.Items.Add(product.GetProductInfo());
                totalInventoryValue += product.CalculatePrice() * product.GetStockQuantity();
                totalItems += product.GetStockQuantity();
            }

            lblTotalProducts.Text = $"�������: {products.Count}";
            lblTotalItems.Text = $"������: {totalItems}";
            lblTotalValue.Text = $"����� ���������: {totalInventoryValue:C}";
        }

        private void ClearInputFields()
        {
            txtName.Clear();
            txtCode.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            txtExtra1.Clear();
            txtExtra2.Clear();
            txtExtra3.Clear();
            dtpExpiryDate.Value = DateTime.Now.AddDays(7);
            chkPerishable.Checked = false;
        }

        private void btnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (lstProducts.SelectedIndex >= 0 && lstProducts.SelectedIndex < products.Count)
            {
                products.RemoveAt(lstProducts.SelectedIndex);
                RefreshProductList();
            }
            else
            {
                MessageBox.Show("�������� ����� ��� ��������", "��������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                RefreshProductList();
                return;
            }

            lstProducts.Items.Clear();
            foreach (var product in products)
            {
                if (product.Name.ToLower().Contains(searchTerm) ||
                    product.ProductCode.ToLower().Contains(searchTerm))
                {
                    lstProducts.Items.Add(product.GetProductInfo());
                }
            }
        }

        private void btnShowLowStock_Click(object sender, EventArgs e)
        {
            lstProducts.Items.Clear();
            foreach (var product in products)
            {
                if (product.GetStockQuantity() <= 10)
                {
                    lstProducts.Items.Add($"{product.GetProductInfo()} - ������ �����!");
                }
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            RefreshProductList();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                RefreshProductList();
            }
        }
    }
}