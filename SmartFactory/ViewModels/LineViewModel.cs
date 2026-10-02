using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using System.Collections.ObjectModel;

namespace SmartFactory.ViewModels
{
    public partial class LineViewModel
        : BaseCrudViewModel<ProductionLine, int>
    {
        private readonly ILineRepository _lineRepository;

        public ObservableCollection<ProductionLine> Lines { get; } = new();

        public ObservableCollection<string> StatusList { get; } = new()
        {
            "STOP",
            "RUN"
        };

        [ObservableProperty] private ProductionLine? _selectedLine;
        [ObservableProperty] private string _lineCode = string.Empty;
        [ObservableProperty] private string _lineName = string.Empty;
        [ObservableProperty] private string _status = "STOP";

        public LineViewModel(
            ILineRepository lineRepository,
            ICommonRepository<ProductionLine, int> commonRepository)
            : base(commonRepository)
        {
            _lineRepository = lineRepository;
        }

        protected override ObservableCollection<ProductionLine> Items => Lines;

        protected override ProductionLine? SelectedItem
        {
            get => SelectedLine;
            set => SelectedLine = value;
        }

        protected override int GetKey(ProductionLine item)
        {
            return item.LineId;
        }

        protected override Task<List<ProductionLine>> GetAllAsync(CancellationToken token)
        {
            return _lineRepository.GetAllAsync(token);
        }

        public override async Task InitializeAsync(CancellationToken token)
        {
            await RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddLine()
        {
            if (!ValidateInput())
                return;

            var line = new ProductionLine
            {
                LineCode = LineCode,
                LineName = LineName,
                Status = Status,
                CreatedAt = DateTime.Now
            };

            await _commonRepository.AddAsync(line);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        [RelayCommand]
        private async Task UpdateLine()
        {
            if (SelectedLine == null)
                return;

            if (!ValidateInput())
                return;

            SelectedLine.LineCode = LineCode;
            SelectedLine.LineName = LineName;
            SelectedLine.Status = Status;

            await _lineRepository.UpdateAsync(SelectedLine);
            await RefreshItemsAsync(CancellationToken.None);
            ClearInput();
        }

        partial void OnSelectedLineChanged(ProductionLine? value)
        {
            if (value == null)
                return;

            LineCode = value.LineCode;
            LineName = value.LineName;
            Status = value.Status;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(LineCode))
            {
                ShowError("라인 코드를 입력해 주세요.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LineName))
            {
                ShowError("라인명을 입력해 주세요.");
                return false;
            }

            return true;
        }

        protected override void ClearInput()
        {
            SelectedLine = null;
            LineCode = string.Empty;
            LineName = string.Empty;
            Status = "STOP";
        }

        protected override string GetDeleteConfirmMessage(ProductionLine item)
        {
            return $"[{item.LineName}] 생산 라인을 삭제하시겠습니까?";
        }

        protected override string GetDeleteErrorMessage()
        {
            return "이 생산 라인은 설비 또는 생산 계획에서 사용 중이라 삭제할 수 없습니다.";
        }
    }
}