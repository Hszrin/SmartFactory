## 프로젝트 개요

### 개발 목적

스마트팩토리 / MES 환경에서 자주 사용되는 생산관리 흐름을 직접 구현하기 위해 제작했습니다.

주요 업무 흐름은 다음과 같습니다.

```text
제품
  ↓
생산계획
  ↓
작업지시
  ↓
생산실적
  ↓
불량관리
  ↓
대시보드 집계

생산 데이터는 SQL Server에 저장하고,
WPF 클라이언트에서는 MVVM 구조를 기반으로 데이터를 조회 및 관리하도록 구현했습니다.
주요 기능
제품 관리
- 제품 등록
- 제품 수정
- 제품 삭제
- 제품 목록 조회
- 제품 코드 및 제품명 관리
생산라인 관리
- 생산라인 등록
- 생산라인 수정
- 생산라인 삭제
- 생산라인 상태 관리
- 생산 목표 관련 데이터 관리
설비 관리
- 설비 등록
- 설비 수정
- 설비 삭제
- 설비와 생산라인 연결
- 설비 상태 관리
생산계획 관리
- 제품별 생산계획 등록
- 생산라인 지정
- 목표 생산량 설정
- 생산 시작일 / 종료일 관리
- 계획 상태 관리
작업지시 관리
생산계획을 기준으로 작업지시를 생성하고 관리합니다.
주요 데이터:
- 작업지시 번호
- 생산계획
- 제품
- 생산라인
- 목표 생산량
- 작업 상태
- 시작 시간
- 종료 시간
작업 상태는 다음과 같이 관리합니다.
WAITING
RUNNING
COMPLETED
CANCELLED

생산실적 관리
작업지시와 설비를 선택하여 생산실적을 등록할 수 있습니다.
생산실적에는 다음 데이터가 포함됩니다.
생산량
양품 수량
불량 수량
생산 시간

다음 조건을 만족하도록 검증합니다.
양품 수량 + 불량 수량 = 생산 수량

작업지시 자동 완료 처리
생산실적이 등록될 때 해당 작업지시의 누적 생산량을 계산합니다.
누적 생산량 >= 작업지시 목표 생산량

조건을 만족하면 작업지시를 자동으로 완료 처리합니다.
RUNNING
   ↓
COMPLETED

완료 시점에는 EndTime도 함께 기록됩니다.
생산실적 등록과 작업지시 상태 변경은 하나의 Transaction으로 처리하여
중간 오류 발생 시 데이터 불일치를 방지했습니다.
불량 관리
생산실적에 연결된 불량 상세 데이터를 관리합니다.
- 불량 유형
- 불량 수량
- 설명
- 등록 시간
불량 상세 수량의 합계가 생산실적의 전체 불량 수량을 초과하지 않도록 검증했습니다.
불량 상세 합계
    <=
ProductionResult.DefectQuantity

Dashboard
생산 데이터를 한 화면에서 확인할 수 있도록 Dashboard를 구현했습니다.
요약 정보
- 전체 생산량
- 양품 수량
- 불량 수량
- 불량률
- 진행 중 작업지시 수
- 완료 작업지시 수
불량률은 다음과 같이 계산합니다.
불량률 = 불량 수량 / 전체 생산량 × 100

생산량이 0일 경우 Divide By Zero가 발생하지 않도록 처리했습니다.
기간별 조회
Dashboard에서는 다음 기간을 기준으로 데이터를 조회할 수 있습니다.
오늘
최근 7일
이번 달

조회 기간은 다음 형태로 계산합니다.
StartDate <= ProductionTime < EndDate

이를 통해 날짜 경계값을 명확하게 처리했습니다.
설비별 생산량
생산실적 데이터를 설비 기준으로 그룹화하여
설비별 누적 생산량을 표시합니다.
ProductionResult
    ↓
Group By Machine
    ↓
SUM(ProductionQuantity)

표와 Column Chart 두 가지 방식으로 확인할 수 있습니다.
제품별 생산량
생산실적에서 작업지시와 제품 정보를 연결하여
제품별 생산량을 집계합니다.
ProductionResult
    ↓
WorkOrder
    ↓
Product
    ↓
Group By Product

표와 Column Chart 형태로 제공합니다.
불량 유형별 집계
불량 데이터를 유형별로 그룹화하여
각 불량 유형의 발생 수량을 확인할 수 있습니다.
Defect
  ↓
Group By DefectType
  ↓
SUM(DefectQuantity)

Dashboard에서는 Pie Chart와 DataGrid를 통해 확인할 수 있습니다.
작업지시 진행률
작업지시의 목표 생산량과 실제 누적 생산량을 비교하여
진행률을 계산합니다.
진행률 =
현재 생산량 / 목표 생산량 × 100

ProgressBar와 퍼센트 값을 함께 표시하여
작업 진행 상황을 쉽게 확인할 수 있도록 구성했습니다.
생산실적이 아직 존재하지 않는 작업지시도 표시할 수 있도록
WorkOrder를 기준으로 조회한 뒤 생산실적 집계 데이터를 연결합니다.
Dashboard Chart
차트 라이브러리는 LiveCharts2를 사용했습니다.
사용 차트:
- 설비별 생산량 : Column Chart
- 제품별 생산량 : Column Chart
- 불량 유형별 수량 : Pie Chart
각 영역은 Chart / Table 모드를 전환할 수 있도록 구현했습니다.
Chart
 ↕
Table

데이터가 존재하지 않을 경우에는 빈 차트 대신
조회된 데이터가 없습니다.

메시지를 표시하도록 처리했습니다.
System Architecture
┌──────────────────────┐
│        View          │
│       (WPF)          │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│      ViewModel       │
│        MVVM          │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│       Service        │
│   Business Logic     │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│     Repository       │
│    Data Access       │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│   Entity Framework   │
│       Core           │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│     SQL Server       │
└──────────────────────┘

MVVM 구조
View에서는 UI 표시와 Binding만 담당하도록 하고,
화면 로직은 ViewModel에 분리했습니다.
View
  ↓ Binding
ViewModel
  ↓
Service
  ↓
Repository
  ↓
DbContext

CommunityToolkit.Mvvm을 사용하여 다음 기능을 활용했습니다.
- ObservableObject
- ObservableProperty
- RelayCommand
공통 ViewModel 구조
공통 기능을 재사용하기 위해 ViewModel 계층을 구성했습니다.
BaseViewModel
      │
      ├── BaseCrudViewModel<T, TKey>
      │       ├── ProductViewModel
      │       ├── MachineViewModel
      │       ├── ProductionPlanViewModel
      │       └── ...
      │
      └── DashboardViewModel

BaseCrudViewModel에서는 다음 기능을 공통 처리합니다.
- 데이터 조회
- Refresh
- Delete
- Collection 갱신
- 공통 CRUD 처리
Dependency Injection
Microsoft.Extensions.DependencyInjection을 사용하여
View, ViewModel, Service, Repository, DbContext를 DI로 관리했습니다.
화면 단위로 Scope를 생성하여
각 화면에서 독립적인 DbContext를 사용할 수 있도록 구성했습니다.
MainWindow
    ↓
CreateScope()
    ↓
View
    ↓
ViewModel
    ↓
Service
    ↓
Repository
    ↓
DbContext

화면 전환 및 Scope 관리
초기 구조에서는 화면을 전환할 때 기존 Scope를 먼저 Dispose했습니다.
빠르게 메뉴를 이동할 경우 이전 화면의 비동기 DB 조회가 끝나기 전에
DbContext가 Dispose되면서 프로그램이 종료되는 문제가 발생했습니다.
기존 구조
화면 A DB 조회 시작
      ↓
화면 B 클릭
      ↓
화면 A Scope Dispose
      ↓
화면 A의 DB 조회는 아직 실행 중
      ↓
Disposed DbContext 접근
      ↓
Exception

개선 구조
새 화면의 데이터를 먼저 로딩하고
로딩이 완료된 이후 화면을 교체하도록 변경했습니다.
현재 화면 유지
      ↓
새 메뉴 클릭
      ↓
새 Scope 생성
      ↓
새 ViewModel 데이터 조회
      ↓
조회 완료
      ↓
화면 교체
      ↓
이전 Scope Dispose

CancellationToken 기반 Navigation
사용자가 새로운 메뉴를 클릭했을 때
이전 화면의 데이터 로딩이 아직 진행 중이라면
CancellationToken을 이용해 기존 작업을 취소하도록 구현했습니다.
Machine 클릭
    ↓
Machine 조회 시작

Product 클릭
    ↓
Machine 조회 Cancel
    ↓
Product 조회 시작

Dashboard 클릭
    ↓
Product 조회 Cancel
    ↓
Dashboard 조회 시작

가장 마지막으로 선택한 화면만 최종적으로 표시됩니다.
이를 통해 빠른 메뉴 전환 시에도
프로그램이 안정적으로 동작하도록 개선했습니다.
Entity Framework Core
EF Core를 사용하여 SQL Server 데이터를 관리합니다.
주요 기능:
- LINQ Query
- Include / Navigation Property
- GroupBy
- Sum
- Count
- Transaction
- Async Query
- CancellationToken
Navigation Property 활용
Entity 간 관계는 Navigation Property를 이용하여 구성했습니다.
예:
ProductionResult
      ↓
WorkOrder
      ↓
Product

집계 쿼리에서는 별도의 Include() 없이
Navigation Property를 사용한 LINQ 표현식을 통해
EF Core가 SQL JOIN으로 변환하도록 구현했습니다.
Database Structure
주요 Entity:
Product
ProductionLine
Machine
ProductionPlan
WorkOrder
ProductionResult
Defect

관계:
Product ───────┐
               ├── ProductionPlan
ProductionLine ┘          │
                           ▼
                       WorkOrder
                           │
                ┌──────────┴──────────┐
                ▼                     ▼
        ProductionResult          Product
                │
                ▼
             Defect

또한 Machine은 ProductionLine과 연결됩니다.
ProductionLine
      │
      └── Machine

Seed Data
Dashboard 및 생산 데이터 조회 테스트를 위해
대량의 테스트 데이터를 생성하여 사용했습니다.
데이터 구성 예시:
Product              30
ProductionLine        6
Machine              36
ProductionPlan      240
WorkOrder           720
ProductionResult   7,000+
Defect             8,000+

Excel 형태로 Seed 데이터를 생성하고
Python 스크립트를 이용하여 SQL Server에 데이터를 입력했습니다.
Excel
  ↓
Python
  ↓
SQL Server
  ↓
WPF Application

Seed Import
Python에서 다음 라이브러리를 사용했습니다.
pandas
openpyxl
pyodbc

Foreign Key 관계를 유지하기 위해 다음 순서로 데이터를 입력합니다.
Product
↓
ProductionLine
↓
Machine
↓
ProductionPlan
↓
WorkOrder
↓
ProductionResult
↓
Defect

테스트 DB 초기화 후 Excel의 ID 값을 유지하여
관련 Entity 간 Foreign Key 관계가 일치하도록 구성했습니다.
Troubleshooting
1. 빠른 화면 전환 시 프로그램 종료
문제
빠르게 메뉴를 이동하면 비동기 조회 중인 화면의
Scoped DbContext가 먼저 Dispose되면서 프로그램이 종료되었습니다.
해결
- CancellationToken 적용
- 새 화면 로딩 완료 후 화면 교체
- 이전 Scope는 화면 교체 이후 Dispose
- 취소된 Navigation은 OperationCanceledException으로 별도 처리
이를 통해 빠른 화면 이동에서도 안정적으로 동작하도록 개선했습니다.
2. DbContext 동시 접근 문제
문제
하나의 Scoped DbContext를 사용하는 ViewModel에서
여러 비동기 DB Query를 동시에 실행할 경우
DbContext의 Thread-Safe 문제로 예외가 발생할 수 있었습니다.
해결
초기화 과정에서 Query를 병렬 실행하지 않고
순차적으로 await하도록 변경했습니다.
Summary 조회
    ↓
Machine 조회
    ↓
Product 조회
    ↓
Defect 조회
    ↓
WorkOrder 조회

3. 생산실적과 작업지시 상태 불일치
문제
생산실적 등록에는 성공했지만
작업지시 상태 변경 과정에서 오류가 발생하면
두 데이터가 서로 다른 상태가 될 가능성이 있었습니다.
해결
생산실적 등록과 작업지시 상태 변경을
하나의 Transaction으로 처리했습니다.
Transaction Start
       ↓
ProductionResult Insert
       ↓
누적 생산량 계산
       ↓
WorkOrder 상태 변경
       ↓
Commit

오류 발생 시 전체 작업을 Rollback합니다.
4. 불량 상세 수량 검증
문제
불량 상세 데이터를 여러 개 등록할 경우
ProductionResult에 기록된 전체 불량 수량보다
상세 불량 합계가 커질 수 있었습니다.
해결
등록 / 수정 시 기존 불량 상세 데이터의 합계를 계산하여
불량 상세 합계 <= 전체 불량 수량

조건을 만족할 때만 저장하도록 비즈니스 로직을 구현했습니다.
5. Dashboard 데이터가 없을 때 UI 처리
문제
조회 기간에 데이터가 없을 경우
빈 Chart가 표시되어 사용자가 오류로 오해할 수 있었습니다.
해결
데이터 존재 여부를 별도 Property로 관리했습니다.
HasMachineData
HasProductData
HasDefectData

데이터가 없을 경우 Chart 대신 안내 메시지를 표시하도록 개선했습니다.
Tech Stack
Language / Framework
- C#
- .NET 8
- WPF
Architecture
- MVVM
- Repository Pattern
- Service Layer
- Dependency Injection
Library
- CommunityToolkit.Mvvm
- LiveCharts2
Database
- SQL Server
- Entity Framework Core
Data / Utility
- Python
- pandas
- pyodbc
- Excel
UI
Dark Theme 기반으로 UI를 구성했습니다.
주요 화면:
- Dashboard
- Product
- Production Line
- Machine
- Production Plan
- Work Order
- Production Result
- Defect
Dashboard에서는 Chart와 DataGrid를 함께 활용하여
생산 현황을 시각적으로 확인할 수 있도록 구성했습니다.
실행 화면
Dashboard
![Dashboard](./Images/dashboard.png)

생산계획
![Production Plan](./Images/production-plan.png)

작업지시
![Work Order](./Images/work-order.png)

생산실적
![Production Result](./Images/production-result.png)

불량관리
![Defect](./Images/defect.png)

프로젝트를 통해 경험한 내용
- WPF 기반 데스크톱 애플리케이션 개발
- MVVM 구조 설계
- Dependency Injection 및 Scope 관리
- Entity Framework Core를 이용한 데이터 접근
- SQL Server 관계형 데이터 모델링
- Repository / Service 계층 분리
- 생산관리 비즈니스 로직 구현
- Transaction을 통한 데이터 정합성 관리
- LINQ 기반 데이터 집계
- Dashboard 및 Chart 구현
- Async / Await 기반 비동기 처리
- CancellationToken 기반 작업 취소
- DbContext Lifecycle 관리
- 대량 Seed 데이터 생성 및 테스트
프로젝트 구조
SmartFactory
│
├── Models
│
├── Views
│   └── Dashboard
│
├── ViewModels
│
├── Services
│
├── Repositories
│
├── Dtos
│
├── Enums
│
├── Converters
│
├── Data
│   └── SmartFactoryDbContext
│
└── Tools
    └── Seed
        ├── seed_import.py
        └── SmartFactory_BulkSeed.xlsx

핵심 구현 목표
이 프로젝트에서는 단순한 CRUD 프로그램 제작보다는
생산계획
    ↓
작업지시
    ↓
생산실적
    ↓
불량
    ↓
Dashboard

형태의 생산관리 업무 흐름을 구현하는 것에 중점을 두었습니다.
또한 프로그램의 기능 구현뿐만 아니라
- 비즈니스 로직 분리
- 데이터 정합성
- 비동기 처리
- 객체 수명 관리
- 화면 전환 안정성
- 대량 데이터 집계
등 실제 애플리케이션 개발에서 발생할 수 있는 문제를 직접 해결하는 것을 목표로 했습니다.