using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using SmartFactory.Services.Interface;
using System.Collections.ObjectModel;

namespace SmartFactory.ViewModels
{
    public partial class ProductionResultViewModel
        : BaseCrudViewModel<ProductionResult, long>
    {
        private readonly IProductionResultRepository _resultRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IProductionService _productionService;
        public ObservableCollection<ProductionResult> Results { get; } = new();
        public ObservableCollection<WorkOrder> WorkOrders { get; } = new();
        public ObservableCollection<Machine> Machines { get; } = new();

        [ObservableProperty]
        private ProductionResult? _selectedResult;

        [ObservableProperty]
        private WorkOrder? _selectedWorkOrder;

        [ObservableProperty]
        private Machine? _selectedMachine;

        [ObservableProperty]
        private int _productionQuantity;

        [ObservableProperty]
        private int _goodQuantity;

        [ObservableProperty]
        private int _defectQuantity;

        [ObservableProperty]
        private DateTime _productionTime = DateTime.Now;
        public ProductionResultViewModel(
            ICommonRepository<ProductionResult, long> commonRepository,
            IProductionResultRepository resultRepository,
            IWorkOrderRepository workOrderRepository,
            IMachineRepository machineRepository,
            IProductionService productionService)
            : base(commonRepository)
        {
            _resultRepository = resultRepository;
            _workOrderRepository = workOrderRepository;
            _machineRepository = machineRepository;
            _productionService = productionService;
        }
        protected override ObservableCollection<ProductionResult> Items
            => Results;

        protected override ProductionResult? SelectedItem
        {
            get => SelectedResult;
            set => SelectedResult = value;
        }
        protected override long GetKey(
            ProductionResult item)
        {
            return item.ResultId;
        }
        protected override Task<List<ProductionResult>> GetAllAsync(CancellationToken token)
        {
            return _resultRepository.GetAllAsync(token);
        }

        public override async Task InitializeAsync(CancellationToken token)
        {
            await LoadCollectionAsync(
                () => _workOrderRepository.GetAllAsync(token),
                WorkOrders);

            await LoadCollectionAsync(
                () => _machineRepository.GetAllAsync(token),
                Machines);

            await RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddResult()
        {
            if (!ValidateInput())
                return;

            var result = new ProductionResult
            {
                WorkOrderId =
                    SelectedWorkOrder!.WorkOrderId,

                MachineId =
                    SelectedMachine!.MachineId,

                ProductionQuantity =
                    ProductionQuantity,

                GoodQuantity =
                    GoodQuantity,

                DefectQuantity =
                    DefectQuantity,

                ProductionTime =
                    DateTime.Now
            };

            try
            {
                await _productionService
                    .RegisterResultAsync(result);

                // 생산실적 등록으로 작업지시가 완료될 수 있으므로 선택 목록도 갱신한다.
                await LoadCollectionAsync(
                    () => _workOrderRepository
                .GetAllAsync(CancellationToken.None),
                    WorkOrders);

                await RefreshItemsAsync(CancellationToken.None);

                ClearInput();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        [RelayCommand]
        private async Task UpdateResult()
        {
            if (SelectedResult == null)
                return;

            if (!ValidateInput())
                return;

            SelectedResult.WorkOrderId =
                SelectedWorkOrder!.WorkOrderId;

            SelectedResult.MachineId =
                SelectedMachine!.MachineId;

            SelectedResult.ProductionQuantity =
                ProductionQuantity;

            SelectedResult.GoodQuantity =
                GoodQuantity;

            SelectedResult.DefectQuantity =
                DefectQuantity;

            SelectedResult.ProductionTime =
                ProductionTime;

            await _resultRepository.UpdateAsync(
                SelectedResult);

            await RefreshItemsAsync(CancellationToken.None);

            ClearInput();
        }
        partial void OnSelectedResultChanged(
            ProductionResult? value)
        {
            if (value == null)
                return;

            SelectedWorkOrder =
                WorkOrders.FirstOrDefault(
                    x => x.WorkOrderId ==
                         value.WorkOrderId);

            SelectedMachine =
                Machines.FirstOrDefault(
                    x => x.MachineId ==
                         value.MachineId);

            ProductionQuantity =
                value.ProductionQuantity;

            GoodQuantity =
                value.GoodQuantity;

            DefectQuantity =
                value.DefectQuantity;

            ProductionTime =
                value.ProductionTime;
        }

        private bool ValidateInput()
        {
            if (SelectedWorkOrder == null)
            {
                ShowError("작업지시를 선택해 주세요.");

                return false;
            }
            if (SelectedMachine == null)
            {
                ShowError(
                    "설비를 선택해 주세요.");

                return false;
            }
            if (ProductionQuantity <= 0)
            {
                ShowError("생산수량은 1 이상이어야 합니다.");

                return false;
            }
            if (GoodQuantity < 0 || DefectQuantity < 0)
            {
                ShowError("양품수량과 불량수량은 0 이상이어야 합니다.");

                return false;
            }
            if (GoodQuantity + DefectQuantity != ProductionQuantity)
            {
                ShowError("생산수량은 양품수량 + 불량수량과 같아야 합니다.");

                return false;
            }

            return true;
        }
        protected override void ClearInput()
        {
            SelectedResult = null;
            SelectedWorkOrder = null;
            SelectedMachine = null;

            ProductionQuantity = 0;
            GoodQuantity = 0;
            DefectQuantity = 0;

            ProductionTime = DateTime.Now;
        }
        protected override string GetDeleteConfirmMessage(
            ProductionResult item)
        {
            return $"[{item.ResultId}] 생산 실적을 삭제하시겠습니까?";
        }
    }
}