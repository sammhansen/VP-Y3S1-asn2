namespace Groceries
{
    class Grocery
    {
        public string? ItemId { get; set; }
        public string? Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Qty * Price;
    }

    class Program
    {
        const decimal VAT_RATE = 0.16m;

        static void Main()
        {
            Console.Write("Enter input file path: ");
            string? inputPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                Console.WriteLine("Invalid input. File path cannot be empty.");
                return;
            }

            if (!File.Exists(inputPath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            List<Grocery> groceries = ReadGroceriesFromFile(inputPath);
            PrintReceipt(groceries);
            WriteReceiptToFile(groceries, "output/receipt.txt");
        }

        static List<Grocery> ReadGroceriesFromFile(string path)
        {
            var lines = File.ReadAllLines(path).Skip(1); // skip header
            var groceries = new List<Grocery>();

            foreach (var line in lines)
            {
                var cols = line.Split(',');
                groceries.Add(new Grocery
                {
                    ItemId = cols[0],
                    Name = cols[1],
                    Qty = int.Parse(cols[2]),
                    Price = decimal.Parse(cols[3])
                });
            }

            return groceries;
        }

        static void PrintReceipt(List<Grocery> groceries)
        {
            Console.WriteLine("\n--- Shopping Receipt ---\n");
            Console.WriteLine("{0,-8} {1,-10} {2,3} {3,8} {4,10}", "ItemId", "Name",
                              "Qty", "Price", "Total");

            decimal subtotal = 0;
            foreach (var item in groceries)
            {
                Console.WriteLine("{0,-8} {1,-10} {2,3} {3,8:F2} {4,10:F2}", item.ItemId,
                                  item.Name, item.Qty, item.Price, item.Total);
                subtotal += item.Total;
            }

            decimal tax = subtotal * VAT_RATE;
            decimal grandTotal = subtotal + tax;

            Console.WriteLine("\nSubtotal:   {0:F2}", subtotal);
            Console.WriteLine("VAT (16%):  {0:F2}", tax);
            Console.WriteLine("Grand Total:{0:F2}", grandTotal);
        }

        static void WriteReceiptToFile(List<Grocery> groceries, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var writer = new StreamWriter(path);
            writer.WriteLine("--- Shopping Receipt ---");
            writer.WriteLine("{0,-8} {1,-10} {2,3} {3,8} {4,10}", "ItemId", "Name",
                             "Qty", "Price", "Total");

            decimal subtotal = 0;
            foreach (var item in groceries)
            {
                writer.WriteLine("{0,-8} {1,-10} {2,3} {3,8:F2} {4,10:F2}", item.ItemId,
                                 item.Name, item.Qty, item.Price, item.Total);
                subtotal += item.Total;
            }

            decimal tax = subtotal * VAT_RATE;
            decimal grandTotal = subtotal + tax;

            writer.WriteLine();
            writer.WriteLine("Subtotal:   {0:F2}", subtotal);
            writer.WriteLine("VAT (16%):  {0:F2}", tax);
            writer.WriteLine("Grand Total:{0:F2}", grandTotal);
            Console.WriteLine("\n--- Your were served by BSE-01-0012/2024 ---\n");

            Console.WriteLine($"\nReceipt saved to: {path}");
        }
    }
}
