using ActorMapReduceWordCount.Actors;
using ActorMapReduceWordCount.Messages;
using Akka.Hosting;
using Microsoft.Extensions.Hosting;


var file = PrintInstructionsAndGetFile();
if (file.Length == 0)
{
    return;
}





var system = ActorSystem.Create("helloAkka");
var counter = system.ActorOf(CountSupervisor.Create(), "supervisor");
counter.Tell(new StartCount(file));
Console.ReadLine();





static String PrintInstructionsAndGetFile()
{
    Console.WriteLine("Word counter.  Select the document to count:");
    Console.WriteLine(" (1) Magna Carta");
    Console.WriteLine(" (2) Declaration of Independence");
    var choice = Console.ReadLine() ?? String.Empty;
    String path = @"C:\code\git\akka\ActorWordCounterMR1_5\src\ActorWordCounterMR1_5.App\Files\";

    if (choice.Equals("1"))
    {
        return $"{path}MagnaCarta.txt";
    }
    else if (choice.Equals("2"))
    {
        return $"{path}DeclarationOfIndependence.txt";
    }
    else if (choice.Equals("3"))
    {
        return $"{path}test.txt";
    }

    else
    {
        Console.WriteLine("Invalid -- bye!");
        return String.Empty;
    }
}