using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

public class Program
{
    static string? ProgramPath
    {
        get
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exePath);
            return exeDir;
        }
    }
    static void Main(string[] args)
    {
        Console.Write("Enter the text that you want to convert: ");
        string input = Console.ReadLine();
        Console.WriteLine();
        SaveJson(new ConversionTable('A'));
        if (Directory.Exists(@"tables\"))
            Directory.CreateDirectory(@"tables\");

        DirectoryInfo d = new DirectoryInfo(@"tables\");
        FileInfo[] translationTables = d.GetFiles("*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < translationTables.Length; i++)
        {
            Console.WriteLine($"{i}. {translationTables[i].Name}");
        }
        Console.WriteLine();
        Console.Write("Which translation table do you want to use?");
        int x = int.Parse(Console.ReadLine());
        var table = LoadTable(translationTables[x].Name);

        string output = "";
        for (int a = 0; a < input.Length; a++)
        {
            bool match = false;
            //If current char is \ and index + 1 isnt over the list lenght (a.k.a check for newline)
            if (input[a] == '\\' && a + 1 <= input.Length)
            {
                if (input[a+1] == 'n')
                output += table.FindHexStringByName("newline");
                a++;
                continue;
            }
            //Check for letter in database
            for (int b = 0; b < table.letters.Count; b++)
            {
                if (input[a] == table.letters[b].letter.ToCharArray()[0])
                {
                    match = true;
                    output += table.letters[b].hexString;
                    output += " ";
                    break;
                }
            }
            if(match == false)
            {
                Console.WriteLine($"Could not find translation for [{input[a]}]");
            }
        }
        Console.WriteLine($"Translation: {output}");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();


    }
    private static void SaveJson(ConversionTable table)
    {
        using (FileStream fileStream = File.Create(Path.Combine(ProgramPath, "cpkfiles2.json")))
        {
            string jsonSerialized = JsonConvert.SerializeObject(table, Formatting.Indented);

            byte[] buffer = Encoding.Default.GetBytes(jsonSerialized);
            fileStream.Write(buffer, 0, buffer.Length);

            fileStream.Close();
        }

    }
    static ConversionTable LoadTable(string path)
    {
        string test = Path.Combine(ProgramPath, "tables", path);
        using (FileStream fileStream = File.OpenRead(test))
        {
            byte[] buffer = new byte[new FileInfo(test).Length];
            fileStream.Read(buffer, 0, buffer.Length);

            string json = Encoding.Default.GetString(buffer);
            var obj =  JsonConvert.DeserializeObject<ConversionTable>(json);
            //for (int i = 0; i < obj.letters.Count; i++)
            //{
            //    var temp = Regex.Replace(obj.letters[i].hexString.ToString(), @"(.{2})", "$1 ").TrimEnd();
            //    var temp2 = obj.letters[i];
            //    temp2.temporaryDoNotChange = temp;
            //    obj.letters[i] = temp2;
            //}
            return obj;
        }
    }
}

public class ConversionTable
{
    [JsonProperty("ConversionTable")]
    public List<LetterComparison> letters = new List<LetterComparison>();
    public ConversionTable(char letter)
    {
    }
    public string FindHexStringByName(string name)
    {
        for (int i = 0; i < letters.Count; i++)
        {
            if(letters[i].letter.ToString() == name)
            {
                return letters[i].hexString;
            }
        }
        return "Not Found";
    }
}
public struct LetterComparison
{
    public string letter;
    public string hexString;
    [JsonIgnore]
    public string temporaryDoNotChange;
    public LetterComparison(char l, string h, string n = "")
    {
        letter = l.ToString();
        hexString = h;
        temporaryDoNotChange = n;
    }
}
