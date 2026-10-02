using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using System.Collections.ObjectModel;

namespace SmartFactory.ViewModels
{
    public partial class ProductViewModel : BaseCrudViewModel<Product, int>
    {
        private readonly IProductRepository _productRepository;

        public ObservableCollection<Product> Products { get; } = new();

        [ObservableProperty] private Product? _selectedProduct;
        [ObservableProperty] private string _productCode = string.Empty;
        [ObservableProperty] private string _productName = string.Empty;
        [ObservableProperty] private string _unit = "EA";

        public ProductViewModel(
            IProductRepository productRepository,
            ICommonRepository<Product, int> commonRepository)
            : base(commonRepository)
        {
            _productRepository = productRepository;
        }

        protected override ObservableCollection<Product> Items => Products;

        protected override Product? SelectedItem
        {
            get => SelectedProduct;
            set => SelectedProduct = value;
        }

        protected override int GetKey(Product item) => item.ProductId;

        protected override Task<List<Product>> GetAllAsync(CancellationToken token)
        {
            return _productRepository.GetAllAsync(token);
        }

        public override Task InitializeAsync(CancellationToken token)
        {
            return RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            if (string.IsNullOrWhiteSpace(ProductCode) ||
                string.IsNullOrWhiteSpace(ProductName))
            {
                ShowError("제품 코드와 제품명을 입력해 주세요.");
                return;
            }

            var product = new Product
            {
                ProductCode = ProductCode,
                ProductName = ProductName,
                Unit = string.IsNullOrWhiteSpace(Unit) ? "EA" : Unit,
                CreatedAt = DateTime.Now
            };

            await _commonRepository.AddAsync(product);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        [RelayCommand]
        private async Task UpdateProduct()
        {
            if (SelectedProduct == null)
                return;

            SelectedProduct.ProductCode = ProductCode;
            SelectedProduct.ProductName = ProductName;
            SelectedProduct.Unit = string.IsNullOrWhiteSpace(Unit) ? "EA" : Unit;

            await _productRepository.UpdateAsync(SelectedProduct);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        partial void OnSelectedProductChanged(Product? value)
        {
            if (value == null)
                return;

            ProductCode = value.ProductCode;
            ProductName = value.ProductName;
            Unit = value.Unit;
        }

        protected override void ClearInput()
        {
            SelectedProduct = null;
            ProductCode = string.Empty;
            ProductName = string.Empty;
            Unit = "EA";
        }

        protected override string GetDeleteConfirmMessage(Product item)
        {
            return $"[{item.ProductName}] 제품을 삭제하시겠습니까?";
        }

        protected override string GetDeleteErrorMessage()
        {
            return "이 제품은 다른 데이터에서 사용 중이라 삭제할 수 없습니다.";
        }
    }
}
