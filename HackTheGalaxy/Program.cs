// See https://aka.ms/new-console-template for more information
using HackTheGalaxy;
using Newtonsoft.Json;
using System.Diagnostics;

Console.WriteLine("Enter the puzzle: ");
var json = Console.ReadLine();
var input = JsonConvert.DeserializeObject<string[]>(json ?? String.Empty);

if (input == null || input.Length != 64)
{
	Console.WriteLine("Invalid input.");
}
else
{
	Console.WriteLine();

	var graph = new Graph();
	graph.Load(input);

	Stopwatch stopwatch = new Stopwatch();

	Console.WriteLine("Solving...");

    stopwatch.Start();
    graph.Solve();
	stopwatch.Stop();

	Console.WriteLine($"Elapsed Time for Solve is {stopwatch.ElapsedMilliseconds} ms");
	Console.ReadLine();
}

/*
string[] input = new string[64] {
				"1DR",	"D",	"D",	"DR",	"L",	"L",	"D",	"DL",
				"UR",	"D",	"R",	"17DR", "52D",	"D",	"D",	"L",
				"R",	"43L",	"U",	"14DR", "DL",	"UR",	"10D",	"45L",
				"D",	"R",	"UR",	"UL",	"15UL", "28R",	"D",	"L",
				"60R",	"L",	"26UL",	"R",	"DR",	"L",	"D",	"61L",
				"D",	"R",	"D",	"54L",	"DR",	"R",	"UL",	"D",
				"UR",	"DR",	"56R",	"UR",	"UL",	"L",	"U",	"U",
				"R",	"UL",	"U",	"33L",	"R",	"U",	"L",	"64"};

["1DR","D","D","DR","L","L","D","DL","UR","D","R","17DR","52D","D","D","L","R","43L","U","14DR","DL","UR","10D","45L","D","R","UR","UL","15UL","28R","D","L","60R","L","26UL","R","DR","L","D","61L","D","R","D","54L","DR","R","UL","D","UR","DR","56R","UR","UL","L","U","U","R","UL","U","33L","R","U","L","64"]

 */