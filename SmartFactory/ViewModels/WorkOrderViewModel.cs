using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using System.Collections.ObjectModel;

namespace SmartFactory.ViewModels
{
    public partial class WorkOrderViewModel : BaseCrudViewModel<WorkOrder, int>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IProductionPlanRepository _planRepository;

        public ObservableCollection<WorkOrder> WorkOrders { get; } = new();
        public ObservableCollection<ProductionPlan> Plans { get; } = new();

        public ObservableCollection<string> StatusList { get; } = new()
        {
            "WAITING",
            "RUNNING",
            "COMPLETED",
            "CANCELLED"
        };

        [ObservableProperty] private WorkOrder? _selectedOrder;
        [ObservableProperty] private ProductionPlan? _selectedPlan;
        [ObservableProperty] private int _targetQuantity;
        [ObservableProperty] private DateTime _startTime = DateTime.Today;
        [ObservableProperty] private DateTime _endTime = DateTime.Today;
        [ObservableProperty] private string _status = "WAITING";

        public WorkOrderViewModel(
            ICommonRepository<WorkOrder, int> commonRepository,
            IWorkOrderRepository repository,
            IProductionPlanRepository planRepository)
            : base(commonRepository)
        {
            _workOrderRepository = repository;
            _planRepository = planRepository;
        }

        protected override ObservableCollection<WorkOrder> Items => WorkOrders;

        protected override WorkOrder? SelectedItem
        {
            get => SelectedOrder;
            set => SelectedOrder = value;
        }

        protected override int GetKey(WorkOrder item)
        {
            return item.WorkOrderId;
        }

        protected override Task<List<WorkOrder>> GetAllAsync(CancellationToken token)
        {
            return _workOrderRepository.GetAllAsync(token);
        }

        public override async Task InitializeAsync(CancellationToken token)
        {
            await LoadCollectionAsync(
                () => _planRepository.GetAllAsync(token),
                Plans);

            await RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddWorkOrder()
        {
            if (SelectedPlan == null)
            {
                ShowError("생산 계획을 선택해 주세요.");
                return;
            }

            if (TargetQuantity <= 0)
            {
                ShowError("목표 생산량은 1 이상이어야 합니다.");
                return;
            }

            var order = new WorkOrder
            {
                PlanId = SelectedPlan.PlanId,
                ProductId = SelectedPlan.ProductId,
                LineId = SelectedPlan.LineId,
                TargetQuantity = TargetQuantity,
                Status = "WAITING",
                StartTime = null,
                EndTime = null,
                CreatedAt = DateTime.Now
            };

            await _commonRepository.AddAsync(order);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        [RelayCommand]
        private async Task UpdateWorkOrder()
        {
            if (SelectedOrder == null || SelectedPlan == null)
                return;

            SelectedOrder.PlanId = SelectedPlan.PlanId;
            SelectedOrder.ProductId = SelectedPlan.ProductId;
            SelectedOrder.LineId = SelectedPlan.LineId;
            SelectedOrder.TargetQuantity = TargetQuantity;

            if (SelectedOrder.Status != Status)
            {
                if (Status == "RUNNING" &&
                    SelectedOrder.StartTime == null)
                {
                    SelectedOrder.StartTime = DateTime.Now;
                }

                if (Status == "COMPLETED" &&
                    SelectedOrder.EndTime == null)
                {
                    SelectedOrder.EndTime = DateTime.Now;
                }
            }

            SelectedOrder.Status = Status;

            await _workOrderRepository.UpdateAsync(SelectedOrder);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        partial void OnSelectedPlanChanged(ProductionPlan? value)
        {
            if (value == null)
                return;

            TargetQuantity = value.TargetQuantity;

            if (SelectedOrder == null)
                Status = "WAITING";
        }

        partial void OnSelectedOrderChanged(WorkOrder? value)
        {
            if (value == null)
                return;

            SelectedPlan = Plans.FirstOrDefault(
                x => x.PlanId == value.PlanId);

            TargetQuantity = value.TargetQuantity;
            Status = value.Status;

            StartTime = value.StartTime ?? DateTime.Today;
            EndTime = value.EndTime ?? DateTime.Today;
        }

        protected override void ClearInput()
        {
            SelectedOrder = null;
            SelectedPlan = null;
            TargetQuantity = 0;
            StartTime = DateTime.Today;
            EndTime = DateTime.Today;
            Status = "WAITING";
        }

        protected override string GetDeleteConfirmMessage(WorkOrder item)
        {
            return $"[{item.WorkOrderId}] 작업지시를 삭제하시겠습니까?";
        }

        protected override string GetDeleteErrorMessage()
        {
            return "이 작업지시는 생산 실적에서 사용 중이라 삭제할 수 없습니다.";
        }
    }
}