using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;
class Program
{
    static void Main(string[] args)
        => CommandLineApplication.Execute<Program>(args);
    [Option(ShortName = "i")]
    public string inputFile { get; set; }
        //= @"C:\Users\golub\source\lessons\multiplat\pract1\pract1\INPUT.txt";
    [Option(ShortName = "o")]
    public string outputFile { get; set; }
        //= @"C:\Users\golub\source\lessons\multiplat\pract1\pract1\OUTPUT.txt";
    
    static string changeSymToNum(string scenario)
    {
        string toWorkWith = new String(scenario.Where(Char.IsDigit).ToArray());

        if (Regex.IsMatch(scenario, @"^D\d{1,2}$"))
        {
            if (int.Parse(toWorkWith) <= 10)
            {
                return (int.Parse(toWorkWith) * 2).ToString();
            }
        }
        else if (Regex.IsMatch(scenario, @"^T\d{1,2}$"))
        {
            if (int.Parse(toWorkWith) <= 6)
            {
                return (int.Parse(toWorkWith) * 3).ToString();
            }
        }
        return null; 
    }

    static List<List<string>> solveDnTPrioritisation(List<string> scenario)
    {
        List<List<string>> toFill = new List<List<string>>();
        toFill.Add(new List<string> (scenario));
        string toSave;
        int i = 1;
        if (scenario.Count == 1)
        {
            return toFill;
        }
            if (scenario.Count==2)
        {
            if (Regex.IsMatch(scenario[0], @"[DT]\d{1,2}"))
            {
                toFill.Add(new List<string>(scenario));
                toSave= changeSymToNum(scenario[0]);
                if (toSave != null) {
                    toFill[1][0] = toSave;
                }
                return toFill;
            }
            else { return toFill; }
        }
        else 
        {
            if (Regex.IsMatch(scenario[0], @"[DT]\d{1,2}"))
            {
               
                toSave = changeSymToNum(scenario[0]);
                if (toSave != null)
                {
                    toFill.Add(new List<string>(scenario));
                    toFill[1][0] = toSave;
                    i++;
                }
               
            }
            if (Regex.IsMatch(scenario[1], @"[DT]\d{1,2}"))
            {
                toSave = changeSymToNum(scenario[1]);
                if (toSave != null)
                {
                    toFill.Add(new List<string>(scenario));
                    toFill[i][1] = changeSymToNum(scenario[1]);
                    return toFill;
                }
            }
            return toFill;
        }

    }

    static void FindCombinations(int total, List<int> currentCombination, List<int> numbers, List<List<int>> combinations, int maxNumbers)
    {
        if (total == 0 && currentCombination.Count >= 1 && currentCombination.Count <= maxNumbers)
        {
            combinations.Add(new List<int>(currentCombination));
            return;
        }
        if (total < 0 || currentCombination.Count >= maxNumbers)
        {
            return;
        }

        foreach (int number in numbers)
        {
            currentCombination.Add(number);
            FindCombinations(total - number, currentCombination, numbers, combinations, maxNumbers);
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }

    void OnExecute()
    {
        int maxNumbers = 3;

        List<int> numbers = new List<int>(); // basic points
        List<int> doubleNumbers = new List<int>(numbers.Count); //double points
        List<int> tripleNumbers = new List<int>(numbers.Count); //triple points
        List<int> listOfPoints = new List<int>(); //points

        List<List<int>> combinations = new List<List<int>>();
        List<List<string>> additionalScenarios = new List<List<string>>();
        int n = int.Parse(File.ReadAllText(inputFile));

        //get target number
        //Console.Write("Введіть бажану кількість очок: ");


        //simple points
        for (int i = 1; i <= 20; i++)
        {
            numbers.Add(i);
        }
        for (int i = 1; i <= 20; i++)
        {
            listOfPoints.Add(i);
        }
        //double points
        foreach (int num in numbers)
        {
            if (!listOfPoints.Contains(num * 2))
            { 
            listOfPoints.Add(num*2);
            }
            doubleNumbers.Add(num * 2);
        }

        //triple points
        foreach (int num in numbers)
        {
            if (!listOfPoints.Contains(num * 3))
            {
                listOfPoints.Add(num * 3);
            }
            tripleNumbers.Add(num * 3);
        }

        listOfPoints.Add(25);
        listOfPoints.Add(50);

        FindCombinations(n, new List<int>(), listOfPoints, combinations, maxNumbers);

        using (StreamWriter w = new StreamWriter(outputFile))
        {
            foreach (List<int> combination in combinations)
            {
                List<string> formattedCombination = new List<string>();
                foreach (int num in combination)
                {
                    if (doubleNumbers.Contains(num))
                    {
                        formattedCombination.Add($"D{num / 2}");
                    }
                    else if (tripleNumbers.Contains(num))
                    {
                        formattedCombination.Add($"T{num / 3}");
                    }
                    else if (num == 50)
                    {
                        formattedCombination.Add($"Bull");
                    }
                    else
                    {
                        formattedCombination.Add(num.ToString());
                    }
                }

                //Console.WriteLine(string.Join(" ", formattedCombination)); 
                string lastVal = formattedCombination[formattedCombination.Count - 1];
                if (Regex.IsMatch(lastVal, @"^D\d{1,2}$"))
                {
                    additionalScenarios = solveDnTPrioritisation(formattedCombination);

                    //w.WriteLine(string.Join(" ", formattedCombination));
                    foreach (var a in additionalScenarios)
                    {
                        w.WriteLine(string.Join(" ", a));
                    }
                }
            }
        }
    }
}
