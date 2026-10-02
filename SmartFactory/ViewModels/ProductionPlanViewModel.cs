using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using System.Collections.ObjectModel;

namespace SmartFactory.ViewModels
{
    public partial class ProductionPlanViewModel
        : BaseCrudViewModel<ProductionPlan, int>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductionPlanRepository _planRepository;
        private readonly ILineRepository _lineRepository;

        public ObservableCollection<ProductionPlan> Plans { get; } = new();
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<ProductionLine> Lines { get; } = new();

        [ObservableProperty]
        private Product? _selectedProduct;

        [ObservableProperty]
        private ProductionPlan? _selectedPlan;

        [ObservableProperty]
        private ProductionLine? _selectedLine;

        [ObservableProperty]
        private int _targetQuantity;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private string _status = "PLANNED";

        public ObservableCollection<string> StatusList { get; } = new()
        {
            "PLANNED",
            "RUNNING",
            "COMPLETED",
            "CANCELLED"
        };
        public ProductionPlanViewModel(
            IProductionPlanRepository planRepository,
            IProductRepository productRepository,
            ILineRepository lineRepository,
            ICommonRepository<ProductionPlan, int> commonRepository)
            : base(commonRepository)
        {
            _planRepository = planRepository;
            _productRepository = productRepository;
            _lineRepository = lineRepository;
        }
        protected override ObservableCollection<ProductionPlan> Items
            => Plans;
        protected override ProductionPlan? SelectedItem
        {
            get => SelectedPlan;
            set => SelectedPlan = value;
        }
        protected override int GetKey(ProductionPlan item)
        {
            return item.PlanId;
        }

        protected override Task<List<ProductionPlan>> GetAllAsync(CancellationToken token)
        {
            return _planRepository.GetAllAsync(token);
        }

        public override async Task InitializeAsync(CancellationToken token)
        {
            await LoadCollectionAsync(
                () => _productRepository.GetAllAsync(token),
                Products);

            await LoadCollectionAsync(
                () => _lineRepository.GetAllAsync(token),
                Lines);

            await RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddPlan()
        {
            if (!ValidateInput())
                return;

            var plan = new ProductionPlan
            {
                ProductId = SelectedProduct!.ProductId,
                LineId = SelectedLine!.LineId,
                TargetQuantity = TargetQuantity,
                StartDate = StartDate,
                EndDate = EndDate,
                Status = "PLANNED",
                CreatedAt = DateTime.Now
            };

            await _commonRepository.AddAsync(plan);

            await RefreshItemsAsync(CancellationToken.None);

            ClearInput();
        }
        [RelayCommand]
        private async Task UpdatePlan()
        {
            if (SelectedPlan == null)
                return;

            if (!ValidateInput())
                return;

            SelectedPlan.ProductId =
                SelectedProduct!.ProductId;

            SelectedPlan.LineId =
                SelectedLine!.LineId;

            SelectedPlan.TargetQuantity =
                TargetQuantity;

            SelectedPlan.StartDate =
                StartDate;

            SelectedPlan.EndDate =
                EndDate;

            SelectedPlan.Status =
                Status;

            await _planRepository.UpdateAsync(
                SelectedPlan);

            await RefreshItemsAsync(CancellationToken.None);

            ClearInput();
        }
        partial void OnSelectedPlanChanged(
            ProductionPlan? value)
        {
            if (value == null)
                return;

            SelectedProduct =
                Products.FirstOrDefault(
                    x => x.ProductId == value.ProductId);

            SelectedLine =
                Lines.FirstOrDefault(
                    x => x.LineId == value.LineId);

            TargetQuantity =
                value.TargetQuantity;

            StartDate =
                value.StartDate;

            EndDate =
                value.EndDate;

            Status =
                value.Status;
        }
        private bool ValidateInput()
        {
            if (SelectedProduct == null ||
                SelectedLine == null)
            {
                ShowError(
                    "제품과 생산 라인을 선택해 주세요.");

                return false;
            }

            if (TargetQuantity <= 0)
            {
                ShowError(
                    "목표 생산량은 1 이상이어야 합니다.");

                return false;
            }

            if (EndDate < StartDate)
            {
                ShowError(
                    "종료일은 시작일보다 빠를 수 없습니다.");

                return false;
            }

            return true;
        }

        protected override void ClearInput()
        {
            SelectedPlan = null;
            SelectedProduct = null;
            SelectedLine = null;

            TargetQuantity = 0;

            StartDate = DateTime.Today;
            EndDate = DateTime.Today;

            Status = "PLANNED";
        }
        protected override string GetDeleteConfirmMessage(
            ProductionPlan item)
        {
            return $"[{item.PlanId}] 생산 계획을 삭제하시겠습니까?";
        }
        protected override string GetDeleteErrorMessage()
        {
            return "이 생산 계획은 다른 데이터에서 사용 중이라 삭제할 수 없습니다.";
        }
    }
}