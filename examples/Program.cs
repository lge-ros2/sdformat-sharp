// SDFormat-Sharp Examples
// Run all examples or pick one by number

using Examples;

Console.WriteLine("╔═══════════════════════════════════════════════╗");
Console.WriteLine("║         SDFormat-Sharp Examples               ║");
Console.WriteLine("╠═══════════════════════════════════════════════╣");
Console.WriteLine("║  1. Parse World   – Load & print world.sdf   ║");
Console.WriteLine("║  2. Build Model   – Create model from code   ║");
Console.WriteLine("║  3. Inspect/Modify– Load, edit, re-serialize ║");
Console.WriteLine("║  4. Sensors       – Create sensor-rich model  ║");
Console.WriteLine("║  5. Full Parse    – Verify all SDF elements   ║");
Console.WriteLine("║  6. Gazebosim     – Parse gazebosim SDF files  ║");
Console.WriteLine("║  0. Run all                                   ║");
Console.WriteLine("╚═══════════════════════════════════════════════╝");
Console.Write("\nSelect example [0-6]: ");

var input = args.Length > 0 ? args[0] : Console.ReadLine()?.Trim();
var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
var worldSdf = Path.Combine(dataDir, "world.sdf");

switch (input)
{
    case "1": Example1_ParseWorld.Run(worldSdf); break;
    case "2": Example2_BuildModel.Run(); break;
    case "3": Example3_InspectAndModify.Run(); break;
    case "4": Example4_Sensors.Run(); break;
    case "5": Example5_ComprehensiveParse.Run(); break;
    case "6": Example6_ParseGazebosimSdf.Run(); break;
    default:
        Example1_ParseWorld.Run(worldSdf);
        Console.WriteLine();
        Example2_BuildModel.Run();
        Console.WriteLine();
        Example3_InspectAndModify.Run();
        Console.WriteLine();
        Example4_Sensors.Run();
        Console.WriteLine();
        Example5_ComprehensiveParse.Run();
        Console.WriteLine();
        Example6_ParseGazebosimSdf.Run();
        break;
}
