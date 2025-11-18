using LoveYuri.Core.Sqlite;
using LoveYuri.Core.Sqlite.Attribute;

namespace NetToolkitExamples;

[TableInfo(DatabaseName, TableName)]
public class FullEoData {
    public const string TableName = "FullEoData";

    public const string DatabaseName =
        @"E:\love-yuri\NetToolkit\NetToolkitExamples\bin\Release\net8.0\image-capture.s3db";

    public int Wd { get; init; }
    public int Hv { get; init; }
    public int ApIndex { get; init; }
    public bool HighBeamCurrent { get; init; }
    public int GunAlignXLsv { get; init; }
    public int GunAlignYLsv { get; init; }
    public int ApAlignXLsv { get; init; }
    public int ApAlignYLsv { get; init; }
    public double Lens1Current { get; init; }
    public string Lens1CurrentText { get; init; } = string.Empty;
    public int Lens1Lsv { get; init; }
    public double Lens2Current { get; init; }
    public string Lens2CurrentText { get; init; } = string.Empty;
    public int Lens2Lsv { get; init; }
    public int StigX13Lsv { get; init; }
    public int StigX2Lsv { get; init; }
    public int StigX4Lsv { get; init; }
    public int StigY13Lsv { get; init; }
    public int StigY2Lsv { get; init; }
    public int StigY4Lsv { get; init; }
}
