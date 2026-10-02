from pathlib import Path
import pandas as pd
import pyodbc

# =========================================================
# Excel 경로
# =========================================================

BASE_DIR = Path(__file__).resolve().parent
EXCEL_PATH = BASE_DIR / "SmartFactory_BulkSeed.xlsx"

# =========================================================
# SQL Server 연결
# =========================================================

drivers = pyodbc.drivers()

if "ODBC Driver 18 for SQL Server" in drivers:
    DRIVER = "ODBC Driver 18 for SQL Server"
elif "ODBC Driver 17 for SQL Server" in drivers:
    DRIVER = "ODBC Driver 17 for SQL Server"
else:
    raise Exception(
        "SQL Server ODBC Driver 17 또는 18이 설치되어 있지 않습니다."
    )

CONNECTION_STRING = (
    f"DRIVER={{{DRIVER}}};"
    r"SERVER=.\SQLEXPRESS;"
    r"DATABASE=SmartFactoryDB;"
    r"Trusted_Connection=yes;"
    r"TrustServerCertificate=yes;"
)

# =========================================================
# pandas 값을 DB에 넣을 수 있는 값으로 변환
# =========================================================

def db_value(value):
    if pd.isna(value):
        return None

    if isinstance(value, pd.Timestamp):
        return value.to_pydatetime()

    return value

# =========================================================
# 기존 데이터 삭제
# =========================================================

def reset_database(cursor):
    print("기존 데이터 삭제 중...")

    # FK 때문에 자식 테이블부터 삭제
    cursor.execute("DELETE FROM Defect")
    cursor.execute("DELETE FROM ProductionResult")
    cursor.execute("DELETE FROM WorkOrder")
    cursor.execute("DELETE FROM ProductionPlan")
    cursor.execute("DELETE FROM Machine")
    cursor.execute("DELETE FROM ProductionLine")
    cursor.execute("DELETE FROM Product")

    # Identity 다시 1부터 시작하도록 초기화
    cursor.execute("DBCC CHECKIDENT ('Defect', RESEED, 0)")
    cursor.execute("DBCC CHECKIDENT ('ProductionResult', RESEED, 0)")
    cursor.execute("DBCC CHECKIDENT ('WorkOrder', RESEED, 0)")
    cursor.execute("DBCC CHECKIDENT ('ProductionPlan', RESEED, 0)")
    cursor.execute("DBCC CHECKIDENT ('Machine', RESEED, 0)")
    cursor.execute("DBCC CHECKIDENT ('ProductionLine', RESEED, 0)")
    cursor.execute("DBCC CHECKIDENT ('Product', RESEED, 0)")

    print("기존 데이터 삭제 완료")

# =========================================================
# Product
# =========================================================

def import_products(cursor):
    print("Product 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="Product"
    )

    cursor.execute(
        "SET IDENTITY_INSERT Product ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO Product
            (
                ProductId,
                ProductCode,
                ProductName,
                Unit,
                CreatedAt
            )
            VALUES (?, ?, ?, ?, ?)
        """,
        int(row["ProductId"]),
        str(row["ProductCode"]),
        str(row["ProductName"]),
        str(row["Unit"]),
        db_value(row["CreatedAt"]))

    cursor.execute(
        "SET IDENTITY_INSERT Product OFF"
    )

    print(f"Product 완료: {len(df)}건")

# =========================================================
# ProductionLine
# =========================================================

def import_lines(cursor):
    print("ProductionLine 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="ProductionLine"
    )

    cursor.execute(
        "SET IDENTITY_INSERT ProductionLine ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO ProductionLine
            (
                LineId,
                LineCode,
                LineName,
                Status,
                CreatedAt
            )
            VALUES (?, ?, ?, ?, ?)
        """,
        int(row["LineId"]),
        str(row["LineCode"]),
        str(row["LineName"]),
        str(row["Status"]),
        db_value(row["CreatedAt"]))

    cursor.execute(
        "SET IDENTITY_INSERT ProductionLine OFF"
    )

    print(f"ProductionLine 완료: {len(df)}건")

# =========================================================
# Machine
# =========================================================

def import_machines(cursor):
    print("Machine 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="Machine"
    )

    cursor.execute(
        "SET IDENTITY_INSERT Machine ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO Machine
            (
                MachineId,
                MachineCode,
                MachineName,
                LineId,
                Status,
                CreatedAt
            )
            VALUES (?, ?, ?, ?, ?, ?)
        """,
        int(row["MachineId"]),
        str(row["MachineCode"]),
        str(row["MachineName"]),
        int(row["LineId"]),
        str(row["Status"]),
        db_value(row["CreatedAt"]))

    cursor.execute(
        "SET IDENTITY_INSERT Machine OFF"
    )

    print(f"Machine 완료: {len(df)}건")

# =========================================================
# ProductionPlan
# =========================================================

def import_plans(cursor):
    print("ProductionPlan 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="ProductionPlan"
    )

    cursor.execute(
        "SET IDENTITY_INSERT ProductionPlan ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO ProductionPlan
            (
                PlanId,
                ProductId,
                LineId,
                TargetQuantity,
                StartDate,
                EndDate,
                Status
            )
            VALUES (?, ?, ?, ?, ?, ?, ?)
        """,
        int(row["PlanId"]),
        int(row["ProductId"]),
        int(row["LineId"]),
        int(row["TargetQuantity"]),
        db_value(row["StartDate"]),
        db_value(row["EndDate"]),
        str(row["Status"]))

    cursor.execute(
        "SET IDENTITY_INSERT ProductionPlan OFF"
    )

    print(f"ProductionPlan 완료: {len(df)}건")

# =========================================================
# WorkOrder
# =========================================================

def import_workorders(cursor):
    print("WorkOrder 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="WorkOrder"
    )

    cursor.execute(
        "SET IDENTITY_INSERT WorkOrder ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO WorkOrder
            (
                WorkOrderId,
                PlanId,
                ProductId,
                LineId,
                TargetQuantity,
                Status,
                StartTime,
                EndTime
            )
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """,
        int(row["WorkOrderId"]),
        int(row["PlanId"]),
        int(row["ProductId"]),
        int(row["LineId"]),
        int(row["TargetQuantity"]),
        str(row["Status"]),
        db_value(row["StartTime"]),
        db_value(row["EndTime"]))

    cursor.execute(
        "SET IDENTITY_INSERT WorkOrder OFF"
    )

    print(f"WorkOrder 완료: {len(df)}건")

# =========================================================
# ProductionResult
# =========================================================

def import_results(cursor):
    print("ProductionResult 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="ProductionResult"
    )

    cursor.execute(
        "SET IDENTITY_INSERT ProductionResult ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO ProductionResult
            (
                ResultId,
                WorkOrderId,
                MachineId,
                ProductionQuantity,
                GoodQuantity,
                DefectQuantity,
                ProductionTime
            )
            VALUES (?, ?, ?, ?, ?, ?, ?)
        """,
        int(row["ResultId"]),
        int(row["WorkOrderId"]),
        int(row["MachineId"]),
        int(row["ProductionQuantity"]),
        int(row["GoodQuantity"]),
        int(row["DefectQuantity"]),
        db_value(row["ProductionTime"]))

    cursor.execute(
        "SET IDENTITY_INSERT ProductionResult OFF"
    )

    print(
        f"ProductionResult 완료: {len(df)}건"
    )

# =========================================================
# Defect
# =========================================================

def import_defects(cursor):
    print("Defect 입력 중...")

    df = pd.read_excel(
        EXCEL_PATH,
        sheet_name="Defect"
    )

    cursor.execute(
        "SET IDENTITY_INSERT Defect ON"
    )

    for _, row in df.iterrows():
        cursor.execute("""
            INSERT INTO Defect
            (
                DefectId,
                ResultId,
                DefectType,
                DefectQuantity,
                Description,
                CreatedAt
            )
            VALUES (?, ?, ?, ?, ?, ?)
        """,
        int(row["DefectId"]),
        int(row["ResultId"]),
        str(row["DefectType"]),
        int(row["DefectQuantity"]),
        db_value(row["Description"]),
        db_value(row["CreatedAt"]))

    cursor.execute(
        "SET IDENTITY_INSERT Defect OFF"
    )

    print(f"Defect 완료: {len(df)}건")

# =========================================================
# 결과 확인
# =========================================================

def verify(cursor):
    print("\n========== 입력 결과 ==========")

    tables = [
        "Product",
        "ProductionLine",
        "Machine",
        "ProductionPlan",
        "WorkOrder",
        "ProductionResult",
        "Defect"
    ]

    for table in tables:
        cursor.execute(
            f"SELECT COUNT(*) FROM {table}"
        )

        count = cursor.fetchone()[0]

        print(
            f"{table:<20} : {count:,}건"
        )

# =========================================================
# Main
# =========================================================

def main():
    print("SmartFactory Seed Import 시작\n")

    print(
        "Excel 위치:",
        EXCEL_PATH
    )

    if not EXCEL_PATH.exists():
        print("\n엑셀 파일을 찾을 수 없습니다.")
        print(
            "seed_import.py와 "
            "SmartFactory_BulkSeed.xlsx를 "
            "같은 폴더에 넣어주세요."
        )
        return

    connection = None
    cursor = None

    try:
        connection = pyodbc.connect(
            CONNECTION_STRING
        )

        cursor = connection.cursor()

        print(
            f"SQL Server 연결 성공 "
            f"({DRIVER})\n"
        )

        reset_database(cursor)

        import_products(cursor)
        import_lines(cursor)
        import_machines(cursor)
        import_plans(cursor)
        import_workorders(cursor)
        import_results(cursor)
        import_defects(cursor)

        connection.commit()

        print("\nDB Commit 완료")

        verify(cursor)

        print(
            "\nSmartFactory Seed Import 성공"
        )

    except Exception as e:
        if connection is not None:
            connection.rollback()

        print("\n오류 발생")
        print(e)

        print(
            "\n전체 작업 Rollback 완료"
        )

    finally:
        if cursor is not None:
            cursor.close()

        if connection is not None:
            connection.close()

if __name__ == "__main__":
    main()