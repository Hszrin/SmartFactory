using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFactory.Models;
using SmartFactory.Repositories.Interface;
using SmartFactory.Services.Interface;
using System.Collections.ObjectModel;

namespace SmartFactory.ViewModels
{
    public partial class DefectViewModel
        : BaseCrudViewModel<Defect, int>
    {
        private readonly IDefectRepository _defectRepository;
        private readonly IDefectService _defectService;
        private readonly IProductionResultRepository _resultRepository;

        public ObservableCollection<Defect> Defects { get; } = new();
        public ObservableCollection<ProductionResult> ProductionResults { get; } = new();

        [ObservableProperty]
        private Defect? _selectedDefect;
        [ObservableProperty]
        private ProductionResult? _selectedResult;
        public ObservableCollection<string> DefectTypeList { get; } = new()
        {
            "외관 불량",
            "스크래치",
            "오염",
            "파손",
            "치수 불량",
            "조립 불량",
            "부품 누락",
            "접착 불량",
            "용접 불량",
            "인쇄 불량",
            "기능 불량",
            "전기 불량",
            "변형",
            "균열",
            "기타"
        };
        [ObservableProperty]
        private string _defectType = "";
        [ObservableProperty]
        private string _description = "";
        [ObservableProperty]
        private int _defectQuantity;
        public DefectViewModel(
            IDefectRepository defectRepository,
            IDefectService defectService,
            IProductionResultRepository resultRepository,
            ICommonRepository<Defect, int> commonRepository)
            : base(commonRepository)
        {
            _defectService = defectService;
            _defectRepository = defectRepository;
            _resultRepository = resultRepository;
        }

        protected override ObservableCollection<Defect> Items => Defects;
        protected override Defect? SelectedItem
        {
            get => SelectedDefect;
            set => SelectedDefect = value;
        }
        protected override int GetKey(Defect item)
        {
            return item.DefectId;
        }
        protected override Task<List<Defect>> GetAllAsync(CancellationToken token)
        {
            return _defectRepository.GetAllAsync(token);
        }
        public override async Task InitializeAsync(CancellationToken token)
        {
            await LoadCollectionAsync(
                () => _resultRepository.GetAllAsync(token),
                ProductionResults);

            await RefreshItemsAsync(token);
        }

        [RelayCommand]
        private async Task AddDefect()
        {
            if (!ValidateInput())
                return;

            var defect = new Defect
            {
                ResultId = SelectedResult!.ResultId,
                DefectQuantity = DefectQuantity,
                DefectType = DefectType,
                Description = Description,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _defectService.AddDefect(defect);
                await RefreshItemsAsync(CancellationToken.None);
                ClearInput();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        [RelayCommand]
        private async Task UpdateDefect()
        {
            if (!ValidateInput())
                return;

            if (SelectedDefect == null)
                return;

            SelectedDefect.ResultId = SelectedResult!.ResultId;
            SelectedDefect.DefectQuantity = DefectQuantity;
            SelectedDefect.DefectType = DefectType;
            SelectedDefect.Description = Description;

            try
            {
                await _defectService.UpdateDefect(SelectedDefect);
                await RefreshItemsAsync(CancellationToken.None);
                ClearInput();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        partial void OnSelectedDefectChanged(Defect? value)
        {
            if (value == null)
                return;

            SelectedResult = ProductionResults.FirstOrDefault(x => x.ResultId == value.ResultId);

            DefectQuantity = value.DefectQuantity;
            DefectType = value.DefectType;
            Description = value.Description;
        }
        private bool ValidateInput()
        {
            if (SelectedResult == null)
            {
                ShowError("생산 실적을 선택해 주세요.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(DefectType))
            {
                ShowError("불량 유형을 선택해 주세요.");
                return false;
            }

            if (DefectQuantity <= 0)
            {
                ShowError("불량 수량은 1 이상이어야 합니다.");
                return false;
            }

            return true;
        }
        protected override void ClearInput()
        {
            SelectedDefect = null;
            SelectedResult = null;

            DefectQuantity = 0;

            DefectType = string.Empty;
            Description = string.Empty;
        }
    }
}
