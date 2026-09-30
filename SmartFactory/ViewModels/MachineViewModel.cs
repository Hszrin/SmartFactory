using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using System.Collections.ObjectModel;
using System.Windows;

namespace SmartFactory.ViewModels
{
    public partial class MachineViewModel
        : BaseCrudViewModel<Machine, int>
    {
        private readonly IMachineRepository _machineRepository;
        private readonly ILineRepository _lineRepository;

        public ObservableCollection<Machine> Machines { get; } = new();
        public ObservableCollection<ProductionLine> Lines { get; } = new();

        public ObservableCollection<string> StatusList { get; } = new()
        {
            "STOP",
            "RUN",
            "ERROR"
        };

        [ObservableProperty] private Machine? _selectedMachine;
        [ObservableProperty] private ProductionLine? _selectedLine;
        [ObservableProperty] private string _machineCode = string.Empty;
        [ObservableProperty] private string _machineName = string.Empty;
        [ObservableProperty] private string _status = "STOP";

        public MachineViewModel(
            IMachineRepository machineRepository,
            ILineRepository lineRepository,
            ICommonRepository<Machine, int> commonRepository)
            : base(commonRepository)
        {
            _machineRepository = machineRepository;
            _lineRepository = lineRepository;
        }

        protected override ObservableCollection<Machine> Items => Machines;

        protected override Machine? SelectedItem
        {
            get => SelectedMachine;
            set => SelectedMachine = value;
        }

        protected override int GetKey(Machine item)
        {
            return item.MachineId;
        }

        protected override Task<List<Machine>> GetAllAsync(CancellationToken token)
        {
            return _machineRepository.GetAllAsync(token);
        }

        public override async Task InitializeAsync(CancellationToken token)
        {
            await LoadCollectionAsync(
                ()=>_lineRepository.GetAllAsync(token),
                Lines);

            await RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddMachine()
        {
            if (!ValidateInput())
                return;

            var machine = new Machine
            {
                MachineCode = MachineCode,
                MachineName = MachineName,
                LineId = SelectedLine!.LineId,
                Status = Status,
                CreatedAt = DateTime.Now
            };

            await _commonRepository.AddAsync(machine);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        [RelayCommand]
        private async Task UpdateMachine()
        {
            if (SelectedMachine == null)
                return;

            if (!ValidateInput())
                return;

            SelectedMachine.MachineCode = MachineCode;
            SelectedMachine.MachineName = MachineName;
            SelectedMachine.LineId = SelectedLine!.LineId;
            SelectedMachine.Status = Status;

            await _machineRepository.UpdateAsync(SelectedMachine);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        partial void OnSelectedMachineChanged(Machine? value)
        {
            if (value == null)
                return;

            MachineCode = value.MachineCode;
            MachineName = value.MachineName;
            Status = value.Status;

            SelectedLine = Lines.FirstOrDefault(
                x => x.LineId == value.LineId);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(MachineCode))
            {
                ShowError("설비 코드를 입력해 주세요.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(MachineName))
            {
                ShowError("설비명을 입력해 주세요.");
                return false;
            }

            if (SelectedLine == null)
            {
                ShowError("생산 라인을 선택해 주세요.");
                return false;
            }

            return true;
        }

        protected override void ClearInput()
        {
            SelectedMachine = null;
            SelectedLine = null;
            MachineCode = string.Empty;
            MachineName = string.Empty;
            Status = "STOP";
        }

        protected override string GetDeleteConfirmMessage(Machine item)
        {
            return $"[{item.MachineName}] 설비를 삭제하시겠습니까?";
        }

        protected override string GetDeleteErrorMessage()
        {
            return "이 설비는 생산 실적에서 사용 중이라 삭제할 수 없습니다.";
        }
    }
}